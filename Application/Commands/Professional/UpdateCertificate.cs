using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class UpdateCertificate
    {
        public record UpdateCertificateCommand(
            Guid Id,
            string Name,
            string IssuingOrganization,
            DateTime IssueDate,
            DateTime? ExpireDate,
            string CredentialId,
            string CredentialUrl
            ) : IRequest<Result<string>>;

        public class UpdateCertificateHandler(
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdateCertificateCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdateCertificateCommand request, CancellationToken cancellationToken)
            {
                var certificate = await certificateRepository.GetByIdAsync(request.Id);

                if (certificate is null)
                    return Result<string>.Failure("Certificate not found");

                certificate.Name = request.Name;

                certificate.IssuingOrganization = request.IssuingOrganization;

                certificate.IssueDate = request.IssueDate;

                certificate.ExpireDate = request.ExpireDate;

                certificate.CredentialId = request.CredentialId;

                certificate.CredentialUrl = request.CredentialUrl;

                certificateRepository.UpdateAsync(certificate);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Certificate updated successfully", "updated");
            }
        }
    }
}