using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class FileUploadRepository(ProConnectDbContext context) : IFileUploadRepository
    {
        public async Task AddAsync(FileUpload file)
        {
            await context.FileUploads.AddAsync(file);
        }

        public async Task<FileUpload?> GetByIdAsync(Guid id)
        {
            return await context.FileUploads
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);
        }

        public async Task<List<FileUpload>> GetByPostIdAsync(Guid postId)
        {
            return await context.FileUploads
                .AsNoTracking()
                .Where(f => f.PostId == postId && !f.IsDeleted)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public void Update(FileUpload file)
        {
            context.FileUploads.Update(file);
        }

        public async Task<List<FileUpload>> GetByMessageIdAsync(Guid messageId)
        {
            return await context.FileUploads
                .AsNoTracking()
                .Where(f => f.MessageId == messageId && !f.IsDeleted)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }
    }
}