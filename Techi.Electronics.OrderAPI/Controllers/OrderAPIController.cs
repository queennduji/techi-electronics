using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techi.Electronics.OrderAPI.Models.Dto;
using Techi.Electronics.OrderAPI.Service.IService;
using Techi.Electronics.OrderAPI.Utility;

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
        [HttpGet("GetOrders")]
        public async Task<ResponseDto> GetOrders(
            string? userId = "",
            CancellationToken cancellationToken = default)
        {
            bool isAdmin = User.IsInRole(SD.RoleAdmin);
            return await _orderService.GetOrdersAsync(userId, isAdmin, cancellationToken);
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<ResponseDto> GetOrder(
           int id,
           CancellationToken cancellationToken = default)
        {
            return await _orderService.GetOrderAsync(id, cancellationToken);
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto, CancellationToken cancellationToken)
        {
            return await _orderService.CreateOrderAsync(cartDto, cancellationToken);
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto, CancellationToken cancellationToken)
        {
            return await _orderService.CreateStripeSessionAsync(stripeRequestDto, cancellationToken);
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId, CancellationToken cancellationToken)
        {
            return await _orderService.ValidateStripeSessionAsync(orderHeaderId, cancellationToken);
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(
            int orderId,
            [FromBody] string newStatus,
            CancellationToken cancellationToken)
        {
            return await _orderService.UpdateOrderStatusAsync(orderId, newStatus, cancellationToken);
        }
    }
}