using Techi.Electronics.CouponAPI.Models.Dto;

namespace Techi.Electronics.CouponAPI.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto> GetAllCouponsAsync(CancellationToken cancellationToken);
        Task<ResponseDto> GetCouponByIdAsync(int id, CancellationToken cancellationToken);
        Task<ResponseDto> GetCouponByCodeAsync(string code, CancellationToken cancellationToken);
        Task<ResponseDto> CreateCouponAsync(CouponDto couponDto, CancellationToken cancellationToken);
        Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteCouponAsync(int id, CancellationToken cancellationToken);
    }
}
