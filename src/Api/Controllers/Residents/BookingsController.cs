using HouseHelp.Api.Extensions;
using HouseHelp.Application.Bookings;
using HouseHelp.Contracts.Residents;
using HouseHelp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Residents;

[Authorize(Policy = Policies.RequireResident)]
[Route("bookings")]
public class BookingsController : ApiControllerBase
{
    private readonly BookingService _bookingService;

    public BookingsController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponseDto>> Create([FromBody] CreateBookingRequestDto request, CancellationToken cancellationToken)
    {
        var residentId = User.GetUserId();
        var booking = await _bookingService.CreateBookingAsync(residentId, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = booking.Id }, ToDto(booking));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookingResponseDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingService.GetAsync(id, cancellationToken);
        if (booking is null)
        {
            return NotFound();
        }

        return Ok(ToDto(booking));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelBookingRequestDto request, CancellationToken cancellationToken)
    {
        await _bookingService.CancelAsync(id, User.GetUserId(), request.Reason, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/review")]
    public async Task<IActionResult> Review(Guid id, [FromBody] BookingReviewRequestDto request, CancellationToken cancellationToken)
    {
        await _bookingService.AddReviewAsync(id, User.GetUserId(), request, cancellationToken);
        return NoContent();
    }

    private static BookingResponseDto ToDto(HouseHelp.Domain.Entities.Booking booking)
        => new(booking.Id, booking.ResidentId, booking.HelperId, booking.FlatId, booking.ServiceType, booking.StartAt, booking.EndAt, booking.PriceEstimate, booking.State, booking.CreatedAt, booking.UpdatedAt, booking.Notes, booking.RowVersion);
}
