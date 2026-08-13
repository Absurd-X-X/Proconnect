using Application.Common.Repositories;
using Application.Common.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Professional
{
    public class AddEducation
    {
        public record AddEducationCommand(
            Guid ProfessionalProfileId,
            string Institution,
            string Degree,
            string FieldOfStudy,
            DateTime StartDate,
            DateTime? EndDate,
            string Grade,
            string? Description,
            string CreatedBy
            ) : IRequest<Result<AddEducationResponse>>;

        public class AddEducationHandler(
            IProfessionalProfileRepository professionalProfileRepository,
            IEducationRepository educationRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<AddEducationCommand, Result<AddEducationResponse>>
        {
            public async Task<Result<AddEducationResponse>> Handle(AddEducationCommand request, CancellationToken cancellationToken)
            {
                var profile = await professionalProfileRepository.GetByIdAsync(request.ProfessionalProfileId);

                if (profile is null)
                    return Result<AddEducationResponse>.Failure("Professional profile not found");

                var education = new Education
                {
                    ProfessionalProfileId = profile.Id,
                    Institution = request.Institution,
                    Degree = request.Degree,
                    FieldOfStudy = request.FieldOfStudy,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Grade = request.Grade,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy,
                    DateModified = DateTime.UtcNow
                };

                await educationRepository.CreateAsync(education);

                await unitOfWork.SaveAsync();

                return Result<AddEducationResponse>.Success(
                    new AddEducationResponse(education.Id, education.Institution, education.Degree),
                    "Education added successfully");
            }
        }
    }

    public record AddEducationResponse(Guid Id, string Institution, string Degree);
}