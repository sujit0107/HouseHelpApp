using System.Net.Http.Headers;
using System.Net.Http.Json;
using HouseHelp.Api;
using HouseHelp.ApiTests.Fixtures;
using HouseHelp.Contracts.Auth;
using HouseHelp.Contracts.Residents;
using HouseHelp.Domain.Enums;
using Xunit;

namespace HouseHelp.ApiTests;

public class BookingFlowTests : IClassFixture<ApiDatabaseFixture>
{
    private readonly ApiDatabaseFixture _fixture;

    public BookingFlowTests(ApiDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ResidentDashboardEndpoints_BasicFlow()
    {
        var client = _fixture.Factory.CreateClient();

        // Step 1: request OTP for the seeded resident
        var otpRequest = await client.PostAsJsonAsync("/auth/otp/request", new OtpRequestDto("+910000000001"));
        otpRequest.EnsureSuccessStatusCode();
        var otpPayload = await otpRequest.Content.ReadFromJsonAsync<OtpRequestResponseDto>();
        Assert.NotNull(otpPayload);
        Assert.False(string.IsNullOrWhiteSpace(otpPayload!.RequestId));

        // Step 2: verify OTP to obtain JWTs
        var verifyResponse = await client.PostAsJsonAsync("/auth/otp/verify", new OtpVerifyDto(otpPayload.RequestId, "123456", "+910000000001", "Resident"));
        verifyResponse.EnsureSuccessStatusCode();
        var tokens = await verifyResponse.Content.ReadFromJsonAsync<TokenResponseDto>();
        Assert.NotNull(tokens);
        Assert.False(string.IsNullOrWhiteSpace(tokens!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        // Step 3: fetch flats for resident UI dropdown
        var flatsResponse = await client.GetFromJsonAsync<ResidentFlatsResponseDto>("/residents/flats");
        Assert.NotNull(flatsResponse);
        var flat = Assert.Single(flatsResponse!.Flats);

        // Step 4: search helpers via same endpoint UI consumes
        var helpersResponse = await client.GetFromJsonAsync<HelperSearchResponseDto>("/helpers/search?skill=Cleaning");
        Assert.NotNull(helpersResponse);
        var helper = Assert.Single(helpersResponse!.Helpers);

        // Step 5: book helper and ensure state is Requested
        var start = DateTimeOffset.UtcNow.AddDays(1);
        var createRequest = new CreateBookingRequestDto(helper.HelperId, flat.FlatId, "Cleaning", start, start.AddHours(2), "Razorpay", "integration-test");
        var bookingResponse = await client.PostAsJsonAsync("/bookings", createRequest);
        bookingResponse.EnsureSuccessStatusCode();
        var booking = await bookingResponse.Content.ReadFromJsonAsync<BookingResponseDto>();
        Assert.NotNull(booking);
        Assert.Equal(BookingState.Requested, booking!.State);
        Assert.Equal(helper.HelperId, booking.HelperId);
        Assert.Equal(flat.FlatId, booking.FlatId);
    }
}
