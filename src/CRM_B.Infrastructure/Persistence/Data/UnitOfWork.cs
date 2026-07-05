using System.Threading;
using System.Threading.Tasks;
using CRM_B.Application.Abstractions.Persistence;

namespace CRM_B.Infrastructure.Persistence.Data;

internal sealed class UnitOfWork(DataContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);

    public Task<int> SaveChangesOnFailureAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}