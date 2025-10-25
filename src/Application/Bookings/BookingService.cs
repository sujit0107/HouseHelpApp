using HouseHelp.Application.Common;
using HouseHelp.Contracts.Residents;
using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using HouseHelp.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HouseHelp.Application.Bookings;

public class BookingService
{
    private readonly IBookingRepository _bookings;
    private readonly IUserRepository _users;
    private readonly IPaymentRepository _payments;
    private readonly IReviewRepository _reviews;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISlotLockService _slotLockService;
    private readonly IRealtimeNotifier _notifier;
    private readonly IPolicyService _policyService;
    private readonly BookingOptions _options;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookings,
        IUserRepository users,
        IPaymentRepository payments,
        IReviewRepository reviews,
        IUnitOfWork unitOfWork,
        ISlotLockService slotLockService,
        IRealtimeNotifier notifier,
        IPolicyService policyService,
        IOptions<BookingOptions> options,
        ILogger<BookingService> logger)
    {
        _bookings = bookings;
        _users = users;
        _payments = payments;
        _reviews = reviews;
        _unitOfWork = unitOfWork;
        _slotLockService = slotLockService;
        _notifier = notifier;
        _policyService = policyService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Booking> CreateBookingAsync(Guid residentId, CreateBookingRequestDto request, CancellationToken cancellationToken)
    {
        var lockKey = $"slot:{request.HelperId}:{request.StartAt:O}-{request.EndAt:O}";
        if (!await _slotLockService.AcquireAsync(lockKey, TimeSpan.FromSeconds(_options.RequestExpirySeconds), cancellationToken))
        {
            throw new InvalidOperationException("Slot temporarily locked");
        }

        try
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                ResidentId = residentId,
                HelperId = request.HelperId,
                FlatId = request.FlatId,
                ServiceType = request.ServiceType,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                PriceEstimate = 500, // TODO: pricing engine integration
                State = BookingState.Requested,
                CreatedAt = DateTimeOffset.UtcNow,
                Notes = request.Notes,
                RowVersion = 0
            };

            await _bookings.AddAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _notifier.NotifyBookingAsync(booking.Id, BookingState.Requested, cancellationToken);
            return booking;
        }
        finally
        {
            await _slotLockService.ReleaseAsync(lockKey, cancellationToken);
        }
    }

    public Task<Booking?> GetAsync(Guid bookingId, CancellationToken cancellationToken)
        => _bookings.GetByIdAsync(bookingId, cancellationToken);

    public async Task CancelAsync(Guid bookingId, Guid actorId, string reason, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        if (booking.State is BookingState.Completed or BookingState.Paid)
        {
            throw new InvalidOperationException("Cannot cancel completed booking");
        }

        var previous = booking.State;
        booking.State = BookingState.Cancelled;
        booking.UpdatedAt = DateTimeOffset.UtcNow;
        booking.Events.Add(new BookingEvent
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            From = previous,
            To = BookingState.Cancelled,
            At = booking.UpdatedAt.Value,
            ActorId = actorId,
            Reason = reason
        });
        await _bookings.UpdateAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notifier.NotifyBookingAsync(booking.Id, BookingState.Cancelled, cancellationToken);
    }

    public async Task AddReviewAsync(Guid bookingId, Guid residentId, BookingReviewRequestDto request, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        if (booking.ResidentId != residentId)
        {
            throw new InvalidOperationException("Not authorized");
        }

        if (booking.State is not BookingState.Completed and not BookingState.Paid)
        {
            throw new InvalidOperationException("Booking not completed");
        }

        var existing = await _reviews.GetByBookingIdAsync(booking.Id, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Review already submitted");
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            RaterId = booking.ResidentId,
            RateeId = booking.HelperId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _reviews.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public decimal CalculateRefundAmount(Booking booking, DateTimeOffset now)
    {
        var percentage = _policyService.CalculateRefundPercentage(booking.State, now, booking.StartAt);
        return booking.PriceEstimate * (percentage / 100m);
    }

    public async Task AcceptAsync(Guid bookingId, Guid helperId, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        if (booking.HelperId != helperId)
        {
            throw new InvalidOperationException("Not assigned to helper");
        }

        if (booking.State != BookingState.Requested)
        {
            throw new InvalidOperationException("Booking already processed");
        }

        booking.State = BookingState.Accepted;
        booking.UpdatedAt = DateTimeOffset.UtcNow;
        booking.Events.Add(new BookingEvent
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            From = BookingState.Requested,
            To = BookingState.Accepted,
            At = booking.UpdatedAt.Value,
            ActorId = helperId
        });

        await _bookings.UpdateAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notifier.NotifyBookingAsync(booking.Id, BookingState.Accepted, cancellationToken);
    }

    public async Task DeclineAsync(Guid bookingId, Guid helperId, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        if (booking.State != BookingState.Requested)
        {
            return;
        }

        booking.State = BookingState.Cancelled;
        booking.UpdatedAt = DateTimeOffset.UtcNow;
        booking.Events.Add(new BookingEvent
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            From = BookingState.Requested,
            To = BookingState.Cancelled,
            At = booking.UpdatedAt.Value,
            ActorId = helperId,
            Reason = "Declined"
        });

        await _bookings.UpdateAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notifier.NotifyBookingAsync(booking.Id, BookingState.Cancelled, cancellationToken);
    }

    public async Task TransitionAsync(Guid bookingId, Guid helperId, BookingState from, BookingState to, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(bookingId, cancellationToken) ?? throw new InvalidOperationException("Booking not found");
        if (booking.HelperId != helperId)
        {
            throw new InvalidOperationException("Not assigned to helper");
        }

        if (booking.State != from)
        {
            throw new InvalidOperationException("Invalid state transition");
        }

        booking.State = to;
        booking.UpdatedAt = DateTimeOffset.UtcNow;
        booking.Events.Add(new BookingEvent
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            From = from,
            To = to,
            At = booking.UpdatedAt.Value,
            ActorId = helperId
        });

        await _bookings.UpdateAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notifier.NotifyBookingAsync(booking.Id, to, cancellationToken);
    }
}
