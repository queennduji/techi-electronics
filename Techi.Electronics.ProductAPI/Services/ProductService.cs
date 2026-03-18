using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Techi.Electronics.ProductAPI.Data;
using Techi.Electronics.ProductAPI.Models;
using Techi.Electronics.ProductAPI.Models.Dto;
using Techi.Electronics.ProductAPI.Services.IService;

namespace Techi.Electronics.ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ResponseDto> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var products = await _db.Products.ToListAsync(cancellationToken);
                response.Result = _mapper.Map<IEnumerable<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var product = await _db.Products
                    .FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken);

                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product not found";
                    return response;
                }

                response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> CreateProductAsync(ProductDto productDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _db.Products.AddAsync(product, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> UpdateProductAsync(ProductDto productDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var product = _mapper.Map<Product>(productDto);
                _db.Products.Update(product);
                await _db.SaveChangesAsync(cancellationToken);

                response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var product = await _db.Products
                    .FirstOrDefaultAsync(x => x.ProductId == id, cancellationToken);

                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product not found";
                    return response;
                }

                _db.Products.Remove(product);
                await _db.SaveChangesAsync(cancellationToken);
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