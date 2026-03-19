using Microsoft.AspNetCore.Mvc;
using Techi.Electronics.ShoppingCartAPI.Models.Dto;
using Techi.Electronics.ShoppingCartAPI.Service.IService;

namespace Techi.Electronics.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartAPIController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ActionResult<ResponseDto>> GetCart(string userId, CancellationToken cancellationToken)
        {
            var response = await _cartService.GetCartAsync(userId, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ActionResult<ResponseDto>> ApplyCoupon([FromBody] CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = await _cartService.ApplyCouponAsync(cartDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("RemoveCoupon")]
        public async Task<ActionResult<ResponseDto>> RemoveCoupon([FromBody] CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = await _cartService.RemoveCouponAsync(cartDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("CartUpsert")]
        public async Task<ActionResult<ResponseDto>> CartUpsert([FromBody] CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = await _cartService.CartUpsertAsync(cartDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("RemoveCart")]
        public async Task<ActionResult<ResponseDto>> RemoveCart([FromBody] int cartDetailsId, CancellationToken cancellationToken)
        {
            var response = await _cartService.RemoveCartAsync(cartDetailsId, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("EmailCartRequest")]
        public async Task<ActionResult<ResponseDto>> EmailCartRequest(
     [FromBody] CartDto cartDto,
     CancellationToken cancellationToken)
        {
            var response = await _cartService.EmailCartRequestAsync(cartDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}