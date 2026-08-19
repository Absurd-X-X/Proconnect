using Application.Common.Repositories;
using Application.Common.Dtos;
using MediatR;

namespace Application.Queries.Professional
{
    public class GetProjectById
    {
        public record GetProjectByIdQuery(Guid Id) : IRequest<Result<GetProjectByIdResponse>>;

        public class GetProjectByIdHandler(
            IProjectRepository projectRepository) : IRequestHandler<GetProjectByIdQuery, Result<GetProjectByIdResponse>>
        {
            public async Task<Result<GetProjectByIdResponse>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
            {
                var project = await projectRepository.GetByIdAsync(request.Id);

                if (project is null)
                    return Result<GetProjectByIdResponse>.Failure("Project not found");

                var response = new GetProjectByIdResponse(
                    project.Id,
                    project.ProfessionalProfileId,
                    project.Title,
                    project.Description,
                    project.ProjectUrl);

                return Result<GetProjectByIdResponse>.Success(response, "Project retrieved successfully");
            }
        }
    }

    public record GetProjectByIdResponse(
        Guid Id,
        Guid ProfessionalProfileId,
        string Title,
        string Description,
        string ProjectUrl);
}