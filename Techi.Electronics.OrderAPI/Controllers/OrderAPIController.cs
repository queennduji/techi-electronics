using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techi.Electronics.OrderAPI.Models.Dto;
using Techi.Electronics.OrderAPI.Service.IService;

namespace Techi.Electronics.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderAPIController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            return await _orderService.CreateOrderAsync(cartDto);
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            return await _orderService.CreateStripeSessionAsync(stripeRequestDto);
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            return await _orderService.ValidateStripeSessionAsync(orderHeaderId);
        }
    }
}