using HouseHelp.Domain.Enums;

namespace HouseHelp.Contracts.Residents;

public record CreateBookingRequestDto(Guid HelperId, Guid FlatId, string ServiceType, DateTimeOffset StartAt, DateTimeOffset EndAt, string PaymentMethod, string? Notes);

public record BookingResponseDto(Guid Id, Guid ResidentId, Guid HelperId, Guid FlatId, string ServiceType, DateTimeOffset StartAt, DateTimeOffset EndAt, decimal PriceEstimate, BookingState State, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt, string? Notes, uint RowVersion);

public record CancelBookingRequestDto(string Reason);

public record BookingReviewRequestDto(int Rating, string? Comment);
