using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class DeleteProject
    {
        public record DeleteProjectCommand(Guid Id, bool DeletePermanently) : IRequest<Result<string>>;

        public class DeleteProjectHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<DeleteProjectCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
            {
                var project = await projectRepository.GetByIdAsync(request.Id);

                if (project is null)
                    return Result<string>.Failure("Project not found");

                if (request.DeletePermanently)
                {
                    projectRepository.Delete(project);
                }
                else
                {
                    project.IsDeleted = true;

                    projectRepository.Update(project);
                }

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Project deleted successfully", "deleted");
            }
        }
    }
}