using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Commands.Professional
{
    public class UpdateProject
    {
        public record UpdateProjectCommand(
            Guid Id,
            string Title,
            string Description,
            string ProjectUrl
            ) : IRequest<Result<string>>;

        public class UpdateProjectHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork) : IRequestHandler<UpdateProjectCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
            {
                var project = await projectRepository.GetByIdAsync(request.Id);

                if (project is null)
                    return Result<string>.Failure("Project not found");

                project.Title = request.Title;

                project.Description = request.Description;

                project.ProjectUrl = request.ProjectUrl;

                projectRepository.Update(project);

                await unitOfWork.SaveAsync();

                return Result<string>.Success("Project updated successfully", "updated");
            }
        }
    }
}