using Techi.Electronics.OrderAPI.Models.Dto;

namespace Techi.Electronics.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
