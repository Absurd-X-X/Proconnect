using Application.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Contract.Settings
{
    public interface IFileStorage
    {
        Task<FileUploadResult> UploadAsync(
            IFormFile file,
            string folder,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string publicId,
            CancellationToken cancellationToken = default);
    }
}