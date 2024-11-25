using Microsoft.AspNetCore.Mvc;
using PhotoSi.OrderService.Core.Dtos;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSi.UserService.Core.Dtos;
using PhotoSi.UserService.Core.Interfaces;
using PhotoSiTest.Common.BaseTypes;

namespace PhotoSiTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public UserController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();

            return Ok(users);
        }

        [HttpPost("paginate")]
        public async Task<IActionResult> GetPaginatedUsers([FromBody] PagedFilter filter)
        {
            // Valida i parametri del filtro
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
            {
                return BadRequest("PageNumber and PageSize must be greater than zero.");
            }

            var paginatedResult = await _userService.PaginateAsync(filter);

            // Restituisci il risultato della paginazione
            return Ok(paginatedResult);

        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserEditDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userService.IsDuplicateUserAsync(userDto))
            {
                return BadRequest("An user with the same values already exists.");
            }

            //Sbianco l'eventuale ID passato nel body
            userDto.Id = null;

            userDto.Id = await _userService.CreateAsync(userDto);
            return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserEditDto userDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if (id != userDto.Id)
                return BadRequest("ID mismatch between route and body.");

          

            // Controllo: L'utente con l'ID fornito deve esistere
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Controllo: Il nome dell'utente non può essere vuoto
            if (string.IsNullOrEmpty(userDto.Name))
            {
                return BadRequest("User name cannot be empty.");
            }

            // Controllo: L'email deve essere valida
            if (!IsValidEmail(userDto.Email))
            {
                return BadRequest("Invalid email address.");
            }

            await _userService.UpdateAsync(userDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {

            // Controllo: L'utente con l'ID fornito deve esistere
            var existingUser = await _userService.GetByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Controllo: Verifica che l'utente non sia associato ad alcun ordine
            var isUserAssociatedWithOrders = await _orderService.HasPendingOrProcessingOrdersWithUserAsync(id);
            if (isUserAssociatedWithOrders)
            {
                return BadRequest($"User with ID {id} is associated with one or more orders and cannot be deleted.");
            }

            await _userService.DeleteAsync(id);
            return NoContent();
        }

        // Metodo di validazione dell'email
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}