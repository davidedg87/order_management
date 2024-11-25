using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.AddressService.Core.Interfaces;
using PhotoSi.AddressService.Core.Models;
using PhotoSiTest.Common.Services;
using Microsoft.Extensions.Logging;

namespace PhotoSi.AddressService.Services
{
    public class AddressService : BaseService<Address, AddressDto, AddressEditDto>, IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger<AddressService> _logger;

        public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger) : base(addressRepository, logger)
        {
            _addressRepository = addressRepository;
            _logger = logger;
        }

        public async Task<bool> IsDuplicateAddressAsync(AddressEditDto addressDto)
        {
            _logger.LogTrace("Checking if address exists with street: {Street}, city: {City}, postalCode: {PostalCode}, country: {Country}",
                addressDto.Street, addressDto.City, addressDto.PostalCode, addressDto.Country);

            // Verifica se esiste già un indirizzo con gli stessi valori
            var existingAddress = await _addressRepository.Query()
                .FirstOrDefaultAsync(a => a.Street == addressDto.Street &&
                                          a.City == addressDto.City &&
                                          a.PostalCode == addressDto.PostalCode &&
                                          a.Country == addressDto.Country);

            return existingAddress != null; // Restituisce true se esiste un duplicato
        }

        public async Task<IEnumerable<AddressDto>> GetByIdsAsync(List<int> addressIds)
        {
            _logger.LogTrace("Fetching addresses for IDs: {AddressIds}", string.Join(",", addressIds));

            var addresses = await _addressRepository.Query()
                                                     .Where(a => addressIds.Contains(a.Id))
                                                     .Select(p => p.Adapt<AddressDto>())
                                                     .ToListAsync();

            _logger.LogTrace("Fetched {Count} addresses for IDs: {AddressIds}", addresses.Count, string.Join(",", addressIds));

            return addresses;
        }
    }
}
