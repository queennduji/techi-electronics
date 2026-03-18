using AutoMapper;
using Techi.Electronics.ShoppingCartAPI.Models;
using Techi.Electronics.ShoppingCartAPI.Models.Dto;

namespace Techi.Electronics.ShoppingCartAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
        }
    }
}