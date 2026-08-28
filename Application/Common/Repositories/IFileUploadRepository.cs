using Domain.Entities;

namespace Application.Common.Repositories
{
    public interface IFileUploadRepository
    {
        Task AddAsync(FileUpload file);

        Task<FileUpload?> GetByIdAsync(Guid id);

        Task<List<FileUpload>> GetByPostIdAsync(Guid postId);

        Task<List<FileUpload>> GetByMessageIdAsync(Guid messageId);

        void Update(FileUpload file);
    }
}