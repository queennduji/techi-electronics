using Techi.Electronics.OrderAPI.Models.Dto;

namespace Techi.Electronics.OrderAPI.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto> GetOrdersAsync(string? userId, bool isAdmin, CancellationToken cancellationToken);
        Task<ResponseDto> GetOrderAsync(int id, CancellationToken cancellationToken);
        Task<ResponseDto> CreateOrderAsync(CartDto cartDto, CancellationToken cancellationToken);
        Task<ResponseDto> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto, CancellationToken cancellationToken);
        Task<ResponseDto> ValidateStripeSessionAsync(int orderHeaderId, CancellationToken cancellationToken);
        Task<ResponseDto> UpdateOrderStatusAsync(int orderId, string newStatus, CancellationToken cancellationToken);
    }
}
