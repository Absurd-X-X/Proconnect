using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface ICompanyReviewRepository
    {
        Task AddAsync(CompanyReview review);
        Task<CompanyReview?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid companyId, Guid reviewerId);
        Task<PageResponse<CompanyReview>> GetByCompanyIdAsync(Guid companyId, PageRequest request, bool usePaging);
        Task<List<CompanyReview>> GetAllByCompanyIdAsync(Guid companyId);
        void IncrementHelpful(CompanyReview review);
    }
}