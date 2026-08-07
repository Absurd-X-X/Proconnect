using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<AuditLog?> GetByIdAsync(Guid id);
    }
}
