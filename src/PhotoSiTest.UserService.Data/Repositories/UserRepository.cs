using PhotoSi.UserService.Core.Interfaces;
using PhotoSiTest.Common.Repositories;
using PhotoSiTest.UserService.Core.Models;

namespace PhotoSiTest.UserService.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {

        public UserRepository(UserContext context) : base(context)
        {
        }


    }
}
