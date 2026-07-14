using Unamora.Application.Common.Interfaces;
using Unamora.Infrastructure.Persistence;

namespace Unamora.Infrastructure.Persistence;

public class UnitOfWork(UnamoraDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
