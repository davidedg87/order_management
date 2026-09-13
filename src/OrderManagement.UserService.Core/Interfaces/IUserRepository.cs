using OrderManagement.Common.Interfaces;
using OrderManagement.UserService.Core.Models;

namespace OrderManagement.UserService.Core.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
    }
}