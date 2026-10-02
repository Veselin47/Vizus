using Microsoft.EntityFrameworkCore;
using Vizus.Application.Common.Interfaces;

namespace Vizus.Infrastructure.Identity;

public class UserLookupService : IUserLookupService
{
    private readonly Persistence.ApplicationDbContext _context;

    public UserLookupService(Persistence.ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<string, string>> GetEmailsByIdsAsync(List<string> userIds, CancellationToken ct)
    {
        return await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Email ?? "—", ct);
    }
}