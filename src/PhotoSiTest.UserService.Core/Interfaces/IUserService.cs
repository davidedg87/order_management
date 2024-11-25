
using PhotoSi.UserService.Core.Dtos;
using PhotoSiTest.Common.Interfaces;
using PhotoSiTest.UserService.Core.Models;
using System.Threading.Tasks;

namespace PhotoSi.UserService.Core.Interfaces
{
    public interface IUserService : IBaseService<User, UserDto, UserEditDto>
    {
        Task<IEnumerable<UserDto>> GetByIdsAsync(List<int> userIds);

        Task<bool> IsDuplicateUserAsync(UserEditDto userDto);
    }
}