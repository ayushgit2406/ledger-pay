using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LedgerPayDbContext _db;

        public OrderRepository(LedgerPayDbContext db)
        {
            _db = db;
        }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        }
        public async Task AddAsync(Order order, CancellationToken ct)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync(ct);
        }
        public async Task UpdateAsync(Order order, CancellationToken ct)
        {
            _db.Orders.Update(order);
            await _db.SaveChangesAsync(ct);
        }
    }
}