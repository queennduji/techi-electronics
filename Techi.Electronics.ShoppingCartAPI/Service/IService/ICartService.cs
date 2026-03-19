using Techi.Electronics.ShoppingCartAPI.Models.Dto;

namespace Techi.Electronics.ShoppingCartAPI.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto> GetCartAsync(string userId, CancellationToken cancellationToken);
        Task<ResponseDto> ApplyCouponAsync(CartDto cartDto, CancellationToken cancellationToken);
        Task<ResponseDto> RemoveCouponAsync(CartDto cartDto, CancellationToken cancellationToken);
        Task<ResponseDto> CartUpsertAsync(CartDto cartDto, CancellationToken cancellationToken);
        Task<ResponseDto> RemoveCartAsync(int cartDetailsId, CancellationToken cancellationToken);
        Task<ResponseDto> EmailCartRequestAsync(CartDto cartDto, CancellationToken cancellationToken);
    }
}
