using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly LedgerPayDbContext _db;
        public PaymentRepository(LedgerPayDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Payment payment, CancellationToken ct)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync(ct);
        }
    }
}