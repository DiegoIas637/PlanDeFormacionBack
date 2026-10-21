using Application.Common.Security;
using Application.Products;
using Application.Products.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlanDeFormacionBack.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ITokenGenerator _tokenGenerator;

        public ProductsController(IProductService productService, ITokenGenerator tokenGenerator)
        {
            _productService = productService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync(cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetByIdAsync(id, cancellationToken);
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var ownerId = _tokenGenerator.GetUserId(User);
            var product = await _productService.CreateAsync(request, ownerId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var product = await _productService.UpdateAsync(id, request, cancellationToken);
            return Ok(product);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _productService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
