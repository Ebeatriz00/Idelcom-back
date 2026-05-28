namespace Core.Interfaces.Services
{
    public record LoginAttemptResult(
        bool IsNowLocked,
        TimeSpan? LockoutDuration,
        int? AttemptsBeforeNextLock
    );

    public interface ILoginAttemptService
    {
        Task<TimeSpan?> IsLockedOutAsync(string key, CancellationToken ct = default);
        Task<LoginAttemptResult> RegisterFailureAsync(string key, CancellationToken ct = default);
        Task ResetAsync(string key, CancellationToken ct = default);
    }
}
