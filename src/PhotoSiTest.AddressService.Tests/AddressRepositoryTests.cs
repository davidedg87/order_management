using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PhotoSi.AddressService.Data;
using PhotoSi.AddressService.Core.Models;
using PhotoSiTest.Common.Repositories;

namespace PhotoSiTest.AddressService.Data.Repositories
{
    public class AddressRepositoryTests
    {
        private readonly DbContextOptions<AddressContext> _options;

        public AddressRepositoryTests()
        {
            // Configura in memoria il contesto del database
            _options = new DbContextOptionsBuilder<AddressContext>()
                .UseInMemoryDatabase(databaseName: "AddressServiceTestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddAddress_ShouldSaveAddressCorrectly()
        {
            var addressRepository = new AddressRepository(new AddressContext(_options));

            // Crea un nuovo indirizzo da salvare
            var address = new Address
            {
                Street = "Via Roma",
                City = "Roma",
                PostalCode = "00100",
                Country = "Italia"
            };

            // Aggiungi l'indirizzo al repository
            await addressRepository.AddAsync(address);

            // Verifica che l'indirizzo sia stato salvato correttamente
            using (var context = new AddressContext(_options))
            {
                var savedAddress = await context.Addresses.FindAsync(address.Id);

                // Asserzioni con FluentAssertions
                savedAddress.Should().NotBeNull(); // Verifica che l'indirizzo non sia null
                savedAddress.Street.Should().Be(address.Street); // Confronta via
                savedAddress.City.Should().Be(address.City); // Confronta città
                savedAddress.PostalCode.Should().Be(address.PostalCode); // Confronta codice postale
                savedAddress.Country.Should().Be(address.Country); // Confronta paese
            }
        }
    }
}
