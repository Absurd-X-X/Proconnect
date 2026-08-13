using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries
{
    public class GetCertificateById
    {
        public record GetCertificateByIdQuery(Guid Id) : IRequest<Result<GetCertificateByIdResponse>>;

        public class GetCertificateByIdHandler(
            ICertificateRepository certificateRepository) : IRequestHandler<GetCertificateByIdQuery, Result<GetCertificateByIdResponse>>
        {
            public async Task<Result<GetCertificateByIdResponse>> Handle(GetCertificateByIdQuery request, CancellationToken cancellationToken)
            {
                var certificate = await certificateRepository.GetByIdAsync(request.Id);

                if (certificate is null)
                    return Result<GetCertificateByIdResponse>.Failure("Certificate not found");

                var response = new GetCertificateByIdResponse(
                    certificate.Id,
                    certificate.ProfessionalProfileId,
                    certificate.Name,
                    certificate.IssuingOrganization,
                    certificate.IssueDate,
                    certificate.ExpireDate,
                    certificate.CredentialId,
                    certificate.CredentialUrl);

                return Result<GetCertificateByIdResponse>.Success(response, "Certificate retrieved successfully");
            }
        }
    }

    public record GetCertificateByIdResponse(
        Guid Id,
        Guid ProfessionalProfileId,
        string Name,
        string IssuingOrganization,
        DateTime IssueDate,
        DateTime? ExpireDate,
        string CredentialId,
        string CredentialUrl);
}