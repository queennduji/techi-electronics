using AutoMapper;
using Techi.Electronics.CouponAPI.Models;
using Techi.Electronics.CouponAPI.Models.Dto;

namespace Techi.Electronics.CouponAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CouponDto, Coupon>().ReverseMap();
        }
    }
}