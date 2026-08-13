using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IPortfolioLinkRepository
    {
        Task CreateAsync(PortfolioLink link);

        Task<PortfolioLink?> GetByIdAsync(Guid id);

        Task<PageResponse<PortfolioLink>> GetByProfessionalProfileIdAsync(PageRequest request, bool usePaging, Guid professionalProfileId);

        void UpdateAsync(PortfolioLink link);

        void Delete(PortfolioLink link);
    }
}