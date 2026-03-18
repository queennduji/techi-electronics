using Techi.Electronics.ProductAPI.Models.Dto;

namespace Techi.Electronics.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDto> GetAllProductsAsync(CancellationToken cancellationToken);
        Task<ResponseDto> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<ResponseDto> CreateProductAsync(ProductDto productDto, CancellationToken cancellationToken);
        Task<ResponseDto> UpdateProductAsync(ProductDto productDto, CancellationToken cancellationToken);
        Task<ResponseDto> DeleteProductAsync(int id, CancellationToken cancellationToken);
    }
}
