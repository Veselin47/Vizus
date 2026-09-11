using Microsoft.EntityFrameworkCore;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;
using Vizus.Infrastructure.Persistence;

namespace Vizus.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<Order?> GetByStripeSessionIdAsync(string sessionId, CancellationToken ct)
        => await _context.Orders.Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.StripeSessionId == sessionId, ct);

    public async Task AddAsync(Order order, CancellationToken ct)
        => await _context.Orders.AddAsync(order, ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}