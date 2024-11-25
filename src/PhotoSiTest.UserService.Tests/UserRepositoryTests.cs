using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoSiTest.UserService.Core.Models;

namespace PhotoSiTest.UserService.Data.Repositories
{
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<UserContext> _options;

        public UserRepositoryTests()
        {
            // Configura in memoria il contesto del database
            _options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "UserServiceTestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddUser_ShouldSaveUserCorrectly()
        {
            var userRepository = new UserRepository(new UserContext(_options));

            // Crea un nuovo utente da salvare
            var user = new User
            {
                Name = "John Doe",
                Email = "johndoe@example.com"
            };

            // Aggiungi l'utente al repository
            await userRepository.AddAsync(user);

            // Verifica che l'utente sia stato salvato correttamente
            using (var context = new UserContext(_options))
            {
                var savedUser = await context.Users.FindAsync(user.Id);

                // Asserzioni con FluentAssertions
                savedUser.Should().NotBeNull(); // Verifica che l'utente non sia null
                savedUser.Name.Should().Be(user.Name); // Confronta nome
                savedUser.Email.Should().Be(user.Email); // Confronta email
            }
        }
    }
}
