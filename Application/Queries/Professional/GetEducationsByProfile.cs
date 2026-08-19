using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries.Professional
{
    public class GetEducationsByProfile
    {
        public record GetEducationsByProfileQuery(
            Guid ProfessionalProfileId,
            PageRequest PageRequest,
            bool UsePaging
            ) : IRequest<Result<PageResponse<GetEducationsByProfileResponse>>>;

        public class GetEducationsByProfileHandler(
            IEducationRepository educationRepository) : IRequestHandler<GetEducationsByProfileQuery, Result<PageResponse<GetEducationsByProfileResponse>>>
        {
            public async Task<Result<PageResponse<GetEducationsByProfileResponse>>> Handle(GetEducationsByProfileQuery request, CancellationToken cancellationToken)
            {
                var educations = await educationRepository.GetByProfessionalProfileIdAsync(
                    request.PageRequest,
                    request.UsePaging,
                    request.ProfessionalProfileId);

                var items = educations.Items.Select(e => new GetEducationsByProfileResponse(
                    e.Id,
                    e.Institution,
                    e.Degree,
                    e.FieldOfStudy,
                    e.StartDate,
                    e.EndDate,
                    e.Grade)).ToList();

                var response = new PageResponse<GetEducationsByProfileResponse>
                {
                    Items = items,
                    TotalCount = educations.TotalCount,
                    PageNumber = educations.PageNumber,
                    PageSize = educations.PageSize
                };

                return Result<PageResponse<GetEducationsByProfileResponse>>.Success(response, "Educations retrieved successfully");
            }
        }
    }

    public record GetEducationsByProfileResponse(
        Guid Id,
        string Institution,
        string Degree,
        string FieldOfStudy,
        DateTime StartDate,
        DateTime? EndDate,
        string Grade);
}