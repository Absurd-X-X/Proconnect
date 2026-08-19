using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Queries.Recruiter
{
    public class GetCompanyByInvitationCode
    {
        public record InvitationPreviewResponse(
            Guid CompanyId,
            string Name,
            string Industry,
            string? LogoUrl,
            string CompanySize
        );

        public record GetCompanyByInvitationCodeQuery(string InvitationCode) : IRequest<Result<InvitationPreviewResponse>>;

        public class GetCompanyByInvitationCodeHandler(
            ICompanyRepository companyRepository)
            : IRequestHandler<GetCompanyByInvitationCodeQuery, Result<InvitationPreviewResponse>>
        {
            public async Task<Result<InvitationPreviewResponse>> Handle(
                GetCompanyByInvitationCodeQuery request,
                CancellationToken cancellationToken)
            {
                var company = await companyRepository.GetByInvitationCodeAsync(request.InvitationCode);

                if (company is null)
                {
                    return Result<InvitationPreviewResponse>.Failure("Invalid invitation code");
                }

                if (company.InvitationCodeExpiry is null || company.InvitationCodeExpiry < DateTime.UtcNow)
                {
                    return Result<InvitationPreviewResponse>.Failure("This invitation code has expired");
                }

                var result = new InvitationPreviewResponse(
                    company.Id,
                    company.Name,
                    company.Industry,
                    company.LogoUrl,
                    company.CompanySize);

                return Result<InvitationPreviewResponse>.Success(result, "Invitation code verified");
            }
        }
    }
}