using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries
{
    public class GetCertificatesByProfile
    {
        public record GetCertificatesByProfileQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
            ) : IRequest<Result<PageResponse<GetCertificatesByProfileResponse>>>;

        public class GetCertificatesByProfileHandler(
            ICertificateRepository certificateRepository) : IRequestHandler<GetCertificatesByProfileQuery, Result<PageResponse<GetCertificatesByProfileResponse>>>
        {
            public async Task<Result<PageResponse<GetCertificatesByProfileResponse>>> Handle(GetCertificatesByProfileQuery request, CancellationToken cancellationToken)
            {
                var certificates = await certificateRepository.GetByProfessionalProfileIdAsync(
                    request.PageRequest,
                    request.UsePaging,
                    request.ProfessionalProfileId);

                var items = certificates.Items.Select(c => new GetCertificatesByProfileResponse(
                    c.Id,
                    c.Name,
                    c.IssuingOrganization,
                    c.IssueDate,
                    c.ExpireDate)).ToList();

                var response = new PageResponse<GetCertificatesByProfileResponse>
                {
                    Items = items,
                    TotalCount = certificates.TotalCount,
                    PageNumber = certificates.PageNumber,
                    PageSize = certificates.PageSize
                };

                return Result<PageResponse<GetCertificatesByProfileResponse>>.Success(response, "Certificates retrieved successfully");
            }
        }
    }

    public record GetCertificatesByProfileResponse(
        Guid Id,
        string Name,
        string IssuingOrganization,
        DateTime IssueDate,
        DateTime? ExpireDate);
}