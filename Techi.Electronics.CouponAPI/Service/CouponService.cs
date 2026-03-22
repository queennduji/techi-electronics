using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Techi.Electronics.CouponAPI.Data;
using Techi.Electronics.CouponAPI.Models;
using Techi.Electronics.CouponAPI.Models.Dto;
using Techi.Electronics.CouponAPI.Service.IService;

namespace Techi.Electronics.CouponAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CouponService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ResponseDto> GetAllCouponsAsync(CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var coupons = await _db.Coupons.ToListAsync(cancellationToken);
                response.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetCouponByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var coupon = await _db.Coupons
                    .FirstOrDefaultAsync(x => x.CouponId == id, cancellationToken);

                if (coupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Coupon not found";
                    return response;
                }

                response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetCouponByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var normalizedCode = code.Trim().ToLower();

                var coupon = await _db.Coupons
                    .FirstOrDefaultAsync(x => x.CouponCode.ToLower() == normalizedCode, cancellationToken);

                if (coupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Coupon not found";
                    return response;
                }

                response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> CreateCouponAsync(CouponDto couponDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var coupon = _mapper.Map<Coupon>(couponDto);

                await _db.Coupons.AddAsync(coupon, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDto.DiscountAmount * 100),
                    Name = couponDto.CouponCode,
                    Currency = "usd",
                    Id = couponDto.CouponCode
                };

                var service = new Stripe.CouponService();
                await service.CreateAsync(options);

                response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> UpdateCouponAsync(CouponDto couponDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var coupon = await _db.Coupons
                    .FirstOrDefaultAsync(x => x.CouponId == couponDto.CouponId, cancellationToken);

                if (coupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Coupon not found";
                    return response;
                }

                _mapper.Map(couponDto, coupon);

                await _db.SaveChangesAsync(cancellationToken);

                response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> DeleteCouponAsync(int id, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var coupon = await _db.Coupons
                    .FirstOrDefaultAsync(x => x.CouponId == id, cancellationToken);

                if (coupon == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Coupon not found";
                    return response;
                }

                _db.Coupons.Remove(coupon);
                await _db.SaveChangesAsync(cancellationToken);

                var service = new Stripe.CouponService();
                await service.DeleteAsync(coupon.CouponCode);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}