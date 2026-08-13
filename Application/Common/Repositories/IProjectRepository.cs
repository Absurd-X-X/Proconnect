using Application.Common.Pagenation;
using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IProjectRepository
    {
        Task AddAsync(Project project);

        Task<Project?> GetByIdAsync(Guid id);

        Task<PageResponse<Project>> GetByProfessionalProfileIdAsync(Guid professionalProfileId, PageRequest request, bool usePaging);

        Task<PageResponse<Project>> GetAllAsync(PageRequest request, bool usePaging);

        void Update(Project project);

        void Delete(Project project);
    }
}