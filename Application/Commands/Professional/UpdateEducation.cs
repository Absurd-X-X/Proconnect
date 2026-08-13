using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class UpdateEducation
    {
        public record UpdateEducationCommand(
            Guid Id,
            string Institution,
            string Degree,
            string FieldOfStudy,
            DateTime StartDate,
            DateTime? EndDate,
            string Grade,
            string? Description
            ) : IRequest<Result<string>>;

        public class UpdateEducationHandler(
            IEducationRepository educationRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdateEducationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdateEducationCommand request, CancellationToken cancellationToken)
            {
                var education = await educationRepository.GetByIdAsync(request.Id);

                if (education is null)
                    return Result<string>.Failure("Education record not found");

                education.Institution = request.Institution;

                education.Degree = request.Degree;

                education.FieldOfStudy = request.FieldOfStudy;

                education.StartDate = request.StartDate;

                education.EndDate = request.EndDate;

                education.Grade = request.Grade;

                education.Description = request.Description;

                education.DateModified = DateTime.UtcNow;

                educationRepository.UpdateAsync(education);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Education updated successfully", "updated");
            }
        }
    }
}