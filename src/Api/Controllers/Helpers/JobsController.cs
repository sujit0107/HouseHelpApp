using HouseHelp.Api.Extensions;
using HouseHelp.Application.Bookings;
using HouseHelp.Contracts.Helpers;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Helpers;

[Authorize(Policy = Policies.RequireHelper)]
[Route("jobs")]
public class JobsController : ApiControllerBase
{
    private readonly BookingService _bookingService;
    private readonly IBookingRepository _bookings;

    public JobsController(BookingService bookingService, IBookingRepository bookings)
    {
        _bookingService = bookingService;
        _bookings = bookings;
    }

    [HttpGet]
    public async Task<ActionResult<HelperJobsResponseDto>> Get([FromQuery] string state, CancellationToken cancellationToken)
    {
        var helperId = User.GetUserId();
        var parsed = Enum.TryParse<BookingState>(state, true, out var bookingState) ? bookingState : BookingState.Requested;
        var jobs = await _bookings.GetByHelperAndStateAsync(helperId, parsed, cancellationToken);
        var response = new HelperJobsResponseDto(jobs.Select(j => new HelperJobSummaryDto(j.Id, j.State, j.StartAt, j.EndAt, j.ServiceType, j.PriceEstimate)).ToList());
        return Ok(response);
    }

    [HttpPost("{id:guid}/accept")]
    public async Task<IActionResult> Accept(Guid id, CancellationToken cancellationToken)
    {
        await _bookingService.AcceptAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/decline")]
    public async Task<IActionResult> Decline(Guid id, CancellationToken cancellationToken)
    {
        await _bookingService.DeclineAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/arrived")]
    public Task<IActionResult> Arrived(Guid id, CancellationToken cancellationToken)
        => Transition(id, BookingState.Accepted, BookingState.Arrived, cancellationToken);

    [HttpPost("{id:guid}/start")]
    public Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
        => Transition(id, BookingState.Arrived, BookingState.InProgress, cancellationToken);

    [HttpPost("{id:guid}/complete")]
    public Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
        => Transition(id, BookingState.InProgress, BookingState.Completed, cancellationToken);

    private async Task<IActionResult> Transition(Guid id, BookingState from, BookingState to, CancellationToken cancellationToken)
    {
        await _bookingService.TransitionAsync(id, User.GetUserId(), from, to, cancellationToken);
        return NoContent();
    }
}
