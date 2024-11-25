using Microsoft.AspNetCore.Mvc;
using PhotoSi.OrderService.Core.Dtos;
using PhotoSi.ProductService.Core.Interfaces;
using PhotoSiTest.Common.BaseTypes;
using PhotoSiTest.ProductService.Core.Dtos;

namespace PhotoSiTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductService _productService;

        public ProductCategoryController(IProductCategoryService productCategoryService, IProductService productService)
        {
            _productCategoryService = productCategoryService;
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategoryById(int id)
        {
            var category = await _productCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductCategories()
        {
            var categories = await _productCategoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpPost("paginate")]
        public async Task<IActionResult> GetPaginatedProductCategories([FromBody] PagedFilter filter)
        {
            // Valida i parametri del filtro
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
            {
                return BadRequest("PageNumber and PageSize must be greater than zero.");
            }

            var paginatedResult = await _productCategoryService.PaginateAsync(filter);

            // Restituisci il risultato della paginazione
            return Ok(paginatedResult);

        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategory([FromBody] ProductCategoryEditDto categoryDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            if (await _productCategoryService.IsDuplicateProductCategoryAsync(categoryDto))
            {
                return BadRequest("A product category with the same values already exists.");
            }

            //Imposta l'ID a null per evitare che l'utente possa impostare un valore
            categoryDto.Id = null;

            categoryDto.Id = await _productCategoryService.CreateAsync(categoryDto);
            return CreatedAtAction(nameof(GetProductCategoryById), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductCategory(int id, [FromBody] ProductCategoryEditDto categoryDto)
        {
            // Validazione del modello
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Restituisce i messaggi di errore se il modello non è valido
            }

            // Controllo se l'ID della categoria nel DTO corrisponde all'ID nella route
            if (id != categoryDto.Id)
                return BadRequest("ID mismatch between route and body.");


            // Controllo se la categoria esiste nel sistema
            var existingCategory = await _productCategoryService.GetByIdAsync(id);
            if (existingCategory == null)
                return NotFound($"Category with ID {id} not found.");

            await _productCategoryService.UpdateAsync(categoryDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            // Controllo se la categoria esiste nel sistema
            var existingCategory = await _productCategoryService.GetByIdAsync(id);
            if (existingCategory == null)
                return NotFound($"Category with ID {id} not found.");

            // Controllo: Verifica se ci sono prodotti associati a questa categoria
            var productsInCategory = await _productService.GetByCategoryIdAsync(id);
            if (productsInCategory.Any())
            {
                return BadRequest("Cannot delete category because there are products associated with it.");
            }

            await _productCategoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}