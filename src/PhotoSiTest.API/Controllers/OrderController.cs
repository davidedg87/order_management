using Microsoft.AspNetCore.Mvc;
using PhotoSi.AddressService.Core.Interfaces;
using PhotoSi.OrderService.Core.Dtos;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSi.UserService.Core.Interfaces;
using PhotoSiTest.Common.BaseTypes;

namespace PhotoSiTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IProductService _productService;

        public OrderController(IOrderService orderService, IUserService userService, IAddressService addressService, IProductService productService)
        {
            _orderService = orderService;
            _userService = userService;
            _addressService = addressService;
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var user = await _userService.GetByIdAsync(order.UserId);
            if (user != null)
                order.UserName = user.Name;

            var address = await _addressService.GetByIdAsync(order.AddressId);
            if (address != null)
                order.AddressFull = address.FullAddress;

            // Recuperiamo i codici dei prodotti associati
            var productCodes = await _productService.GetProductCodesByIdsAsync(order.ProductIds);
            order.ProductCodes = productCodes.Select(x => x.Code).ToList();

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            // Recupera tutti gli ordini
            var orders = await _orderService.GetAllAsync();

            // Se non ci sono ordini, ritorna una lista vuota
            if (orders == null || !orders.Any())
                return Ok(new List<OrderDto>());  // Ritorna una lista vuota

            // Estrai tutti gli ID degli indirizzi dagli ordini
            var addressIds = orders.Select(o => o.AddressId).Distinct().ToList();

            // Recupera tutti gli indirizzi in una sola chiamata
            var addresses = await _addressService.GetByIdsAsync(addressIds);

            // Estrai tutti gli ID dei prodotti dagli ordini
            var productIds = orders.SelectMany(o => o.ProductIds).Distinct().ToList();

            // Recupera i codici dei prodotti in una sola chiamata
            var productCodes = await _productService.GetProductCodesByIdsAsync(productIds);

            // Estrai tutti gli ID degli utenti dagli ordini
            var userIds = orders.Select(o => o.UserId).Distinct().ToList();

            // Recupera i dettagli degli utenti in una sola chiamata
            var users = await _userService.GetByIdsAsync(userIds);

            // Associa ogni indirizzo, codice prodotto e nome utente all'ordine corrispondente
            foreach (var order in orders)
            {
                // Associa l'indirizzo all'ordine
                var address = addresses.FirstOrDefault(a => a.Id == order.AddressId);
                if (address != null)
                {
                    order.AddressFull = address.FullAddress;
                }

                // Associa i codici dei prodotti all'ordine
                order.ProductCodes = productCodes
                    .Where(pc => order.ProductIds.Contains(pc.ProductId))
                    .Select(pc => pc.Code)
                    .ToList();

                // Associa il nome utente all'ordine
                var user = users.FirstOrDefault(u => u.Id == order.UserId);
                if (user != null)
                {
                    order.UserName = user.Name;
                }
            }

            return Ok(orders);  // Restituisci la lista degli ordini con indirizzi, codici dei prodotti e nome utente
        }


        [HttpPost("paginate")]
        public async Task<IActionResult> GetPaginatedOrders([FromBody] PagedFilter filter)
        {
            // Controlla che il filtro sia valido
            if (filter.PageNumber <= 0)
                return BadRequest("PageNumber must be greater than zero.");

            if (filter.PageSize <= 0)
                return BadRequest("PageSize must be greater than zero.");

            // Recupera gli ordini paginati
            var ordersResult = await _orderService.PaginateAsync(filter);

            // Se non ci sono ordini, ritorna una lista vuota
            if (ordersResult == null || !ordersResult.Items.Any())
                return Ok(new PaginatedResult<OrderDto>
                {
                    Items = new List<OrderDto>(),
                    TotalCount = 0,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                });

            // Estrai tutti gli ID degli indirizzi dagli ordini
            var addressIds = ordersResult.Items.Select(o => o.AddressId).Distinct().ToList();

            // Recupera tutti gli indirizzi in una sola chiamata
            var addresses = await _addressService.GetByIdsAsync(addressIds);

            // Estrai tutti gli ID dei prodotti dagli ordini
            var productIds = ordersResult.Items.SelectMany(o => o.ProductIds).Distinct().ToList();

            // Recupera i codici dei prodotti in una sola chiamata
            var productCodes = await _productService.GetProductCodesByIdsAsync(productIds);

            // Estrai tutti gli ID degli utenti dagli ordini
            var userIds = ordersResult.Items.Select(o => o.UserId).Distinct().ToList();

            // Recupera i dettagli degli utenti in una sola chiamata
            var users = await _userService.GetByIdsAsync(userIds);

            // Associa ogni indirizzo, codice prodotto e nome utente all'ordine corrispondente
            foreach (var order in ordersResult.Items)
            {
                // Associa l'indirizzo all'ordine
                var address = addresses.FirstOrDefault(a => a.Id == order.AddressId);
                if (address != null)
                {
                    order.AddressFull = address.FullAddress;
                }

                // Associa i codici dei prodotti all'ordine
                order.ProductCodes = productCodes
                    .Where(pc => order.ProductIds.Contains(pc.ProductId))
                    .Select(pc => pc.Code)
                    .ToList();

                // Associa il nome utente all'ordine
                var user = users.FirstOrDefault(u => u.Id == order.UserId);
                if (user != null)
                {
                    order.UserName = user.Name;
                }
            }

            // Restituisci i risultati con la paginazione
            return Ok(new PaginatedResult<OrderDto>
            {
                Items = ordersResult.Items,
                TotalCount = ordersResult.TotalCount,
                PageNumber = ordersResult.PageNumber,
                PageSize = ordersResult.PageSize
            });
        }



        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderEditDto orderDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if (await _orderService.IsDuplicateOrderAsync(orderDto))
            {
                return BadRequest("An order with the same values already exists.");
            }


            // Verifica l'indirizzo
            var addressExists = await _addressService.GetByIdAsync(orderDto.AddressId);
            if (addressExists == null)
            {
                return BadRequest($"Address with ID {orderDto.AddressId} does not exist.");
            }

            // Verifica che l'utente esista
            var userExists = await _userService.GetByIdAsync(orderDto.UserId);
            if (userExists == null)
            {
                return BadRequest($"User with ID {orderDto.UserId} does not exist.");
            }

            // Verifica che i prodotti esistano (qui supponiamo che siano identificati da ProductIds)
            var productsExist = await _productService.GetByIdsAsync(orderDto.ProductIds);
            if (productsExist.Count() != orderDto.ProductIds.Count)
            {
                return BadRequest("Some of the products do not exist.");
            }

            orderDto.TotalAmount = await _productService.GetSumAmount(orderDto.ProductIds);

            // Sbianca l'ID proveniente dal client
            orderDto.Id = null;
            // Creiamo l'ordine
            orderDto.Id = await _orderService.CreateAsync(orderDto);
            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.Id }, orderDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderEditDto orderDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if (id != orderDto.Id)
                return BadRequest("ID mismatch between route and body.");


            var existingOrder = await _orderService.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            // Verifica l'esistenza dell'utente
            var userExists = await _userService.GetByIdAsync(orderDto.UserId);
            if (userExists == null)
            {
                return BadRequest($"User with ID {orderDto.UserId} does not exist.");
            }

            // Verifica l'indirizzo
            var addressExists = await _addressService.GetByIdAsync(orderDto.AddressId);
            if (addressExists == null)
            {
                return BadRequest($"Address with ID {orderDto.AddressId} does not exist.");
            }

            // Verifica che i prodotti esistano
            var productsExist = await _productService.GetByIdsAsync(orderDto.ProductIds);
            if (productsExist.Count() != orderDto.ProductIds.Count)
            {
                return BadRequest("Some of the products do not exist.");
            }

            orderDto.TotalAmount = await _productService.GetSumAmount(orderDto.ProductIds);

            // Aggiorniamo l'ordine
            await _orderService.UpdateAsync(orderDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var existingOrder = await _orderService.GetByIdAsync(id);
            if (existingOrder == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            await _orderService.DeleteAsync(id);
            return NoContent();
        }

    }
}