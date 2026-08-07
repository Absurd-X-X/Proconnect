using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class AddProject
    {
        public record AddProjectCommand(
            Guid ProfessionalProfileId,
            string Title,
            string Description,
            string ProjectUrl
        ) : IRequest<Result<string>>;

        public class AddProjectHandler(
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<
                AddProjectCommand,
                Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddProjectCommand request,
                CancellationToken cancellationToken)
            {
                var project = new Project
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,

                    Title = request.Title,

                    Description = request.Description,

                    ProjectUrl = request.ProjectUrl,


                    DateCreated = DateTime.UtcNow
                };


                await projectRepository.AddAsync(project);

                await unitOfWork.SaveAsync();


                return Result<string>.Success(
                    "Project added successfully",
                    "created");
            }
        }
    }
}