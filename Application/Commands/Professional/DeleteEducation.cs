using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class DeleteEducation
    {
        public record DeleteEducationCommand(Guid Id, bool DeletePermanently) : IRequest<Result<string>>;

        public class DeleteEducationHandler(
            IEducationRepository educationRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<DeleteEducationCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeleteEducationCommand request, CancellationToken cancellationToken)
            {
                var education = await educationRepository.GetByIdAsync(request.Id);

                if (education is null)
                    return Result<string>.Failure("Education record not found");

                if (request.DeletePermanently)
                {
                    educationRepository.Delete(education);
                }

                else
                {
                    education.IsDeleted = true;
                    educationRepository.UpdateAsync(education);
                }


                await unitOfWork.SaveAsync();

                return Result<string>.Success("Education deleted successfully", "deleted");
            }
        }
    }
}