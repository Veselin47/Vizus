namespace Vizus.Application.Common.Interfaces;

public interface IUserLookupService
{
    Task<Dictionary<string, string>> GetEmailsByIdsAsync(List<string> userIds, CancellationToken ct);
    Task<string?> GetEmailByIdAsync(string userId, CancellationToken ct);
}