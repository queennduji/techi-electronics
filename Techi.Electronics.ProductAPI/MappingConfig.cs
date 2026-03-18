using AutoMapper;
using Techi.Electronics.ProductAPI.Models;
using Techi.Electronics.ProductAPI.Models.Dto;

namespace Techi.Electronics.ProductAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}