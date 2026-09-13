using OrderManagement.UserService.Core.Interfaces;
using OrderManagement.Common.Repositories;
using OrderManagement.UserService.Core.Models;

namespace OrderManagement.UserService.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {

        public UserRepository(UserContext context) : base(context)
        {
        }


    }
}
