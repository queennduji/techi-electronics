using AutoMapper;
using Techi.Electronics.OrderAPI.Models;
using Techi.Electronics.OrderAPI.Models.Dto;


namespace Techi.Electronics.OrderAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<OrderHeaderDto, CartHeaderDto>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

            CreateMap<CartDetailsDto, OrderDetailsDto>()
            .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

            CreateMap<OrderDetailsDto, CartDetailsDto>();

            CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
        }
    }
}