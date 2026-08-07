using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ICompanyRepository
    {
        Task AddAsync(Company company);

        Task<Company?> GetByIdAsync(Guid id);
    }
}