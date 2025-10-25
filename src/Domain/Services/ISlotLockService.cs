namespace HouseHelp.Domain.Services;

public interface ISlotLockService
{
    Task<bool> AcquireAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task ReleaseAsync(string key, CancellationToken cancellationToken = default);
}
