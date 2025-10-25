namespace HouseHelp.Domain.Services;

public interface IOtpProvider
{
    Task<string> RequestAsync(string phone, CancellationToken cancellationToken = default);
    Task<bool> VerifyAsync(string requestId, string code, CancellationToken cancellationToken = default);
}
