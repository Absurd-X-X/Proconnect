using Application.Common.Repositories;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWork(ProConnectDbContext context) : IUnitOfWork
    {
        public async Task<int> SaveAsync()
        {
                return await context.SaveChangesAsync();
            
        }
    }
}
