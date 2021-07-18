using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altamira.Entities;

namespace Altamira.Repository
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        public Task<User> GetDetail(int id);

        public Task<User> GetByUserName(string userName);
    }
}