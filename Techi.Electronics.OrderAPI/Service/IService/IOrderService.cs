using Techi.Electronics.OrderAPI.Models.Dto;

namespace Techi.Electronics.OrderAPI.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);
        Task<ResponseDto> ValidateStripeSessionAsync(int orderHeaderId);
    }
}
