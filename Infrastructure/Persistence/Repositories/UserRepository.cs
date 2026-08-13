using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository(ProConnectDbContext proConnectDb) : IUserRepository
    {
        public async Task<int> CountAsync()
        {
            return await proConnectDb.Users.CountAsync(x => !x.IsDeleted);
        }

        public async Task CreateAsync(User user)
        {
            await proConnectDb.Users.AddAsync(user);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await proConnectDb.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> ExistsByUserNameAsync(string userName)
        {
            return await proConnectDb.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<PageResponse<User>> GetAllAsync(PageRequest pageRequest, bool usePaging)
        {
            var query = proConnectDb.Users
                .Include(x => x.RecruiterProfile)
                .Include(x => x.ProfessionalProfile)
                .AsQueryable();

            if (usePaging)
            {
                var offset = query
                    .Skip((pageRequest.PageNumber -1 ) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize);

                return new PageResponse<User>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<User>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && !u.IsDeleted);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted);
        }

        public async Task<User?> GetByVerificationTokenAsync(string token)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .FirstOrDefaultAsync(u => u.VerificationToken == token && !u.IsDeleted);
        }

        public async Task<IEnumerable<User>> SearchAsync(string keyword)
        {
            return await proConnectDb.Users

                .Include(x => x.RecruiterProfile)

                .Include(x => x.ProfessionalProfile)

                .Where(u => u.FirstName.Contains(keyword) || u.LastName.Contains(keyword) || u.UserName.Contains(keyword) || u.Email.Contains(keyword) && !u.IsDeleted)

                .ToListAsync();
        }

        public void UpdateAsync(User user)
        {
            proConnectDb.Users.Update(user);
        }
    }
}
