using Microsoft.EntityFrameworkCore;
using PhotoSi.AddressService.Core.Interfaces;
using PhotoSi.AddressService.Core.Models;
using PhotoSi.AddressService.Data;
using PhotoSiTest.Common.Repositories;

namespace PhotoSiTest.AddressService.Data.Repositories
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {

        public AddressRepository(AddressContext context) : base(context)
        {
        }

           
    }
}
