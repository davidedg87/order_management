using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoSi.UserService.Core.Dtos;
using PhotoSi.UserService.Core.Interfaces;
using PhotoSiTest.Common.Services;
using PhotoSiTest.UserService.Core.Models;

namespace PhotoSi.UserService.Services
{
    public class UserService : BaseService<User, UserDto, UserEditDto>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger) : base(userRepository, logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetByIdsAsync(List<int> userIds)
        {
            _logger.LogTrace("Fetching users for IDs: {UserIds}", string.Join(", ", userIds));

            var users = await _userRepository.Query()
                                              .Where(a => userIds.Contains(a.Id))
                                              .Select(p => p.Adapt<UserDto>())
                                              .ToListAsync();

            if (users == null || users.Count == 0)
            {
                _logger.LogWarning("No users found for the provided IDs.");
            }
            else
            {
                _logger.LogTrace("Found {UserCount} users for the provided IDs.", users.Count);
            }

            return users!;
        }

        public async Task<bool> IsDuplicateUserAsync(UserEditDto userDto)
        {
            _logger.LogTrace("Checking if user exists with Email: {Email}", userDto.Email);

            // Verifica se esiste già un utente con la stessa email
            var existingUser = await _userRepository.Query()
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            return existingUser != null; // Restituisce true se esiste un duplicato
        }
    }
}
