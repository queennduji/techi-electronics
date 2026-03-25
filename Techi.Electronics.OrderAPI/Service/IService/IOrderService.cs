using Techi.Electronics.OrderAPI.Models.Dto;

namespace Techi.Electronics.OrderAPI.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto> CreateOrderAsync(CartDto cartDto, CancellationToken cancellationToken);
        Task<ResponseDto> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto, CancellationToken cancellationToken);
        Task<ResponseDto> ValidateStripeSessionAsync(int orderHeaderId, CancellationToken cancellationToken);
    }
}
