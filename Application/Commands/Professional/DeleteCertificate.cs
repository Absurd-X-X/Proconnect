using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class DeleteCertificate
    {
        public record DeleteCertificateCommand(Guid Id, bool DeletePermanently) : IRequest<Result<string>>;

        public class DeleteCertificateHandler(
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<DeleteCertificateCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeleteCertificateCommand request, CancellationToken cancellationToken)
            {
                var certificate = await certificateRepository.GetByIdAsync(request.Id);

                if (certificate is null)
                    return Result<string>.Failure("Certificate not found");

                if (request.DeletePermanently)
                {
                    certificateRepository.Delete(certificate);
                }
                else
                {
                    certificate.IsDeleted = true;
                    certificateRepository.UpdateAsync(certificate);
                }

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Certificate deleted successfully", "deleted");
            }
        }
    }
}