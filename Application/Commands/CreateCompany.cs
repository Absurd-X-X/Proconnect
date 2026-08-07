using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public class CreateCompany
    {
        public record CreateCompanyCommand(
            string Name,
            string Industry,
            string Description,
            string Website,
            string Email,
            string PhoneNumber,
            string Logo
        ) : IRequest<Result<Guid>>;



        public class CreateCompanyHandler(
            ICompanyRepository companyRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CreateCompanyCommand, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(
                CreateCompanyCommand request,
                CancellationToken cancellationToken)
            {
                var company = new Company
                {
                    Name = request.Name,

                    Industry = request.Industry,

                    Description = request.Description,

                    Website = request.Website,

                    Email = request.Email,

                    PhoneNumber = request.PhoneNumber,

                    Logo = request.Logo,

                    DateCreated = DateTime.UtcNow
                };


                await companyRepository.AddAsync(company);


                await unitOfWork.SaveAsync();


                return Result<Guid>.Success(
                    company.Id, "Company created successfully");
            }
        }
    }
}