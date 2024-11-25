using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.AddressService.Core.Models;
using PhotoSiTest.Common.Interfaces;

namespace PhotoSi.AddressService.Core.Interfaces
{
    public interface IAddressService : IBaseService<Address, AddressDto, AddressEditDto>
    {
        Task<IEnumerable<AddressDto>> GetByIdsAsync(List<int> addressIds);

        Task<bool> IsDuplicateAddressAsync(AddressEditDto addressDto);
    }
}