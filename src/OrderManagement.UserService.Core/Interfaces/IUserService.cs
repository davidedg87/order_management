
using OrderManagement.UserService.Core.Dtos;
using OrderManagement.Common.Interfaces;
using OrderManagement.UserService.Core.Models;
using System.Threading.Tasks;

namespace OrderManagement.UserService.Core.Interfaces
{
    public interface IUserService : IBaseService<User, UserDto, UserEditDto>
    {
        Task<IEnumerable<UserDto>> GetByIdsAsync(List<int> userIds);

        Task<bool> IsDuplicateUserAsync(UserEditDto userDto);
    }
}