using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Altamira.Entities;
using Altamira.Repository;
using Microsoft.EntityFrameworkCore;

namespace Altamira.EfRepository
{
    public class UserRepository : EfRepositoryBase<User>, IUserRepository
    {
        public UserRepository(DbContextOptions<EfContext> options) : base(options)
        {
        }

        public async Task<User> GetDetail(int id)
        {
            return await EfContext.Users
                .Include(x => x.Address)
                .Include(x => x.Address.Geo)
                .Include(x => x.Company)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetByUserName(string userName)
        {
            return await EfContext.Users.FirstOrDefaultAsync(x => x.Username == userName);
        }
    }
}