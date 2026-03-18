using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techi.Electronics.ProductAPI.Models.Dto;
using Techi.Electronics.ProductAPI.Service.IService;

namespace Techi.Electronics.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductAPIController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> Get(CancellationToken cancellationToken)
        {
            var response = await _productService.GetAllProductsAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> Get(int id, CancellationToken cancellationToken)
        {
            var response = await _productService.GetProductByIdAsync(id, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] ProductDto productDto, CancellationToken cancellationToken)
        {
            var response = await _productService.CreateProductAsync(productDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Put([FromBody] ProductDto productDto, CancellationToken cancellationToken)
        {
            var response = await _productService.UpdateProductAsync(productDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Delete(int id, CancellationToken cancellationToken)
        {
            var response = await _productService.DeleteProductAsync(id, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}