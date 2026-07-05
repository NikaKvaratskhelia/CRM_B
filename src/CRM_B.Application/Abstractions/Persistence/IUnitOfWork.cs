using System.Threading;
using System.Threading.Tasks;

namespace CRM_B.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<int> SaveChangesOnFailureAsync(CancellationToken ct = default);
}