using Microsoft.AspNetCore.Mvc;
using PhotoSi.AddressService.Core.Dtos;
using PhotoSi.OrderService.Core.Interfaces;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSiTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IOrderService _orderService;
        public ProductController(IProductService productService, IProductCategoryService productCategoryService, IOrderService orderService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _orderService = orderService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpPost("paginate")]
        public async Task<IActionResult> GetPaginatedProducts([FromBody] PagedFilter filter)
        {
            // Valida i parametri del filtro
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
            {
                return BadRequest("PageNumber and PageSize must be greater than zero.");
            }

            var paginatedResult = await _productService.PaginateAsync(filter);

            // Restituisci il risultato della paginazione
            return Ok(paginatedResult);

        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductEditDto productDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if (await _productService.IsDuplicateProductAsync(productDto))
            {
                return BadRequest("A product with the same values already exists.");
            }

          

            // Controllo: La categoria indicata deve esistere
            var categoryExists = await _productCategoryService.GetByIdAsync(productDto.ProductCategoryId);
            if (categoryExists == null)
            {
                return BadRequest($"Category with ID {productDto.ProductCategoryId} does not exist.");
            }

            // Controllo: Il prezzo non può essere inferiore a 0
            if (productDto.Price < 0)
            {
                return BadRequest("The product price cannot be less than 0.");
            }

            //Sbianco l'ID per evitare che venga passato dall'esterno
            productDto.Id = null;
            productDto.Id = await _productService.CreateAsync(productDto);
            return CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductEditDto productDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if (id != productDto.Id)
                return BadRequest("ID mismatch between route and body.");

            // Controllo: Il prodotto con l'ID fornito deve esistere
            var existingProduct = await _productService.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            // Controllo: La categoria indicata deve esistere
            var categoryExists = await _productCategoryService.GetByIdAsync(productDto.ProductCategoryId);
            if (categoryExists == null)
            {
                return BadRequest($"Category with ID {productDto.ProductCategoryId} does not exist.");
            }

            // Controllo: Il prezzo non può essere inferiore a 0
            if (productDto.Price < 0)
            {
                return BadRequest("The product price cannot be less than 0.");
            }

            await _productService.UpdateAsync(productDto);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Controllo: Il prodotto con l'ID fornito deve esistere
            var existingProduct = await _productService.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            // Controllo: Verifica che il prodotto non sia associato ad alcun ordine
            var isProductAssociatedWithOrders = await _orderService.HasPendingOrProcessingOrdersWithProductAsync(id);
            if (isProductAssociatedWithOrders)
            {
                return BadRequest($"Product with ID {id} is associated with one or more orders and cannot be deleted.");
            }

            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}