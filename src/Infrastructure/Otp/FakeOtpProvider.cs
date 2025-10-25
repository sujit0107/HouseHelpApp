using System.Collections.Concurrent;
using HouseHelp.Domain.Services;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Infrastructure.Otp;

public class FakeOtpProvider : IOtpProvider
{
    private readonly ILogger<FakeOtpProvider> _logger;
    private readonly ConcurrentDictionary<string, string> _codes = new();

    public FakeOtpProvider(ILogger<FakeOtpProvider> logger)
    {
        _logger = logger;
    }

    public Task<string> RequestAsync(string phone, CancellationToken cancellationToken = default)
    {
        var code = "123456"; // deterministic for dev
        var requestId = Guid.NewGuid().ToString();
        _codes[requestId] = code;
        _logger.LogInformation("Issuing fake OTP {Code} for {Phone}", code, phone);
        return Task.FromResult(requestId);
    }

    public Task<bool> VerifyAsync(string requestId, string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_codes.TryGetValue(requestId, out var expected) && expected == code);
    }
}
