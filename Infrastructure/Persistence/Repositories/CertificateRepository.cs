using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories
{
    public class CertificateRepository(
        ProConnectDbContext proConnectDb)
        : ICertificateRepository
    {
        public async Task AddAsync(Certificate certificate)
        {
            await proConnectDb.Certificates.AddAsync(certificate);
        }
    }
}