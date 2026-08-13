using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries
{
    public class GetEducationById
    {
        public record GetEducationByIdQuery(Guid Id) : IRequest<Result<GetEducationByIdResponse>>;

        public class GetEducationByIdHandler(
            IEducationRepository educationRepository) : IRequestHandler<GetEducationByIdQuery, Result<GetEducationByIdResponse>>
        {
            public async Task<Result<GetEducationByIdResponse>> Handle(GetEducationByIdQuery request, CancellationToken cancellationToken)
            {
                var education = await educationRepository.GetByIdAsync(request.Id);

                if (education is null)
                    return Result<GetEducationByIdResponse>.Failure("Education record not found");

                var response = new GetEducationByIdResponse(
                    education.Id,
                    education.ProfessionalProfileId,
                    education.Institution,
                    education.Degree,
                    education.FieldOfStudy,
                    education.StartDate,
                    education.EndDate,
                    education.Grade,
                    education.Description);

                return Result<GetEducationByIdResponse>.Success(response, "Education retrieved successfully");
            }
        }
    }

    public record GetEducationByIdResponse(
        Guid Id,
        Guid ProfessionalProfileId,
        string Institution,
        string Degree,
        string FieldOfStudy,
        DateTime StartDate,
        DateTime? EndDate,
        string Grade,
        string? Description);
}