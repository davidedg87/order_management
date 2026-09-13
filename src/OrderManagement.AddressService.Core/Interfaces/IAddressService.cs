using OrderManagement.AddressService.Core.Dtos;
using OrderManagement.AddressService.Core.Models;
using OrderManagement.Common.Interfaces;

namespace OrderManagement.AddressService.Core.Interfaces
{
    public interface IAddressService : IBaseService<Address, AddressDto, AddressEditDto>
    {
        Task<IEnumerable<AddressDto>> GetByIdsAsync(List<int> addressIds);

        Task<bool> IsDuplicateAddressAsync(AddressEditDto addressDto);
    }
}