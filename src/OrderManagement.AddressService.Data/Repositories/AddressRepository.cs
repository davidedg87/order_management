using Microsoft.EntityFrameworkCore;
using OrderManagement.AddressService.Core.Interfaces;
using OrderManagement.AddressService.Core.Models;
using OrderManagement.AddressService.Data;
using OrderManagement.Common.Repositories;

namespace OrderManagement.AddressService.Data.Repositories
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {

        public AddressRepository(AddressContext context) : base(context)
        {
        }

           
    }
}
