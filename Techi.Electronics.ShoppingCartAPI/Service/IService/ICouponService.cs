using Techi.Electronics.ShoppingCartAPI.Models.Dto;

namespace Techi.Electronics.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
