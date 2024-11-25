using Microsoft.AspNetCore.Mvc;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.AddressService.Core.Interfaces;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSiTest.Common.BaseTypes;

namespace PhotoSiTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;

        public AddressController(IAddressService addressService, IOrderService orderService)
        {
            _addressService = addressService;
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _addressService.GetByIdAsync(id);
            if (address == null)
                return NotFound();
            return Ok(address);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await _addressService.GetAllAsync();
            return Ok(addresses);
        }


        [HttpPost("paginate")]
        public async Task<IActionResult> GetPaginatedAddresses([FromBody] PagedFilter filter)
        {
            // Valida i parametri del filtro
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
            {
                return BadRequest("PageNumber and PageSize must be greater than zero.");
            }

            var paginatedResult = await _addressService.PaginateAsync(filter);

            // Restituisci il risultato della paginazione
            return Ok(paginatedResult);
           
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress([FromBody] AddressEditDto addressDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if(await _addressService.IsDuplicateAddressAsync(addressDto))
            {
                return BadRequest("An address with the same values already exists.");
            }

            //Sbianca eventuale id proveneinte dal client
            addressDto.Id = null;

            addressDto.Id = await _addressService.CreateAsync(addressDto);
            return CreatedAtAction(nameof(GetAddressById), new { id = addressDto.Id }, addressDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressEditDto addressDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if (id != addressDto.Id)
                return BadRequest("ID mismatch between route and body.");

            // Controllo: L'indirizzo con l'ID fornito deve esistere
            var existingAddress = await _addressService.GetByIdAsync(id);
            if (existingAddress == null)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            await _addressService.UpdateAsync(addressDto);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {

            // Controllo: L'indirizzo con l'ID fornito deve esistere
            var existingAddress = await _addressService.GetByIdAsync(id);
            if (existingAddress == null)
            {
                return NotFound($"Address with ID {id} not found.");
            }

            // Controllo: Verifica che non ci siano ordini in stato Pending o Processing associati all'indirizzo
            var hasPendingOrProcessingOrders = await _orderService.HasPendingOrProcessingOrdersWithAddressAsync(id);
            if (hasPendingOrProcessingOrders)
            {
                return BadRequest($"Address with ID {id} is associated with orders that are in 'Pending' or 'Processing' state and cannot be deleted.");
            }

            await _addressService.DeleteAsync(id);
            return NoContent();
        }
    }
}