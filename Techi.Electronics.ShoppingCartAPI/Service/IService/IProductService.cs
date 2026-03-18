using Techi.Electronics.ShoppingCartAPI.Models.Dto;

namespace Techi.Electronics.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
