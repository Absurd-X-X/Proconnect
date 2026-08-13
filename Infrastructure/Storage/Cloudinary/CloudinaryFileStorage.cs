using Application.Common.Dtos;
using Application.Contract.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.Cloudinary
{
    public class CloudinaryFileStorage : IFileStorage
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryFileStorage(
            IOptions<CloudinarySettings> settings)
        {
            var account = new Account(
                settings.Value.CloudName,
                settings.Value.ApiKey,
                settings.Value.ApiSecret);

            _cloudinary = new CloudinaryDotNet.Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

        public async Task<FileUploadResult> UploadAsync(
            IFormFile file,
            string folder,
            CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            await using var stream = file.OpenReadStream();

            var uploadParams = new AutoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new InvalidOperationException(
                    result.Error.Message);

            return new FileUploadResult(
                result.SecureUrl?.ToString() ?? string.Empty,
                result.PublicId ?? string.Empty,
                result.ResourceType ?? string.Empty);
        }

        public async Task DeleteAsync(
            string publicId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return;

            var deleteParams = new DeletionParams(publicId);

            var result =
                await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
                throw new InvalidOperationException(
                    result.Error.Message);
        }
    }
}