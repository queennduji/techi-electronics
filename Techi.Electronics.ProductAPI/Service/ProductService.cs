using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Techi.Electronics.ProductAPI.Data;
using Techi.Electronics.ProductAPI.Models;
using Techi.Electronics.ProductAPI.Models.Dto;
using Techi.Electronics.ProductAPI.Service.IService;

namespace Techi.Electronics.ProductAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(
            AppDbContext db,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
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

                if (productDto.Image != null)
                {
                    await SaveImageAsync(product, productDto.Image, cancellationToken);
                    _db.Products.Update(product);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/604x404";
                    _db.Products.Update(product);
                    await _db.SaveChangesAsync(cancellationToken);
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

        public async Task<ResponseDto> UpdateProductAsync(ProductDto productDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var existingProduct = await _db.Products
                    .FirstOrDefaultAsync(x => x.ProductId == productDto.ProductId, cancellationToken);

                if (existingProduct == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Product not found";
                    return response;
                }

                existingProduct.Name = productDto.Name;
                existingProduct.Price = productDto.Price;
                existingProduct.Description = productDto.Description;
                existingProduct.CategoryName = productDto.CategoryName;

                if (productDto.Image != null)
                {
                    DeleteImageFile(existingProduct.ImageLocalPath);
                    await SaveImageAsync(existingProduct, productDto.Image, cancellationToken);
                }

                _db.Products.Update(existingProduct);
                await _db.SaveChangesAsync(cancellationToken);

                response.Result = _mapper.Map<ProductDto>(existingProduct);
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

                DeleteImageFile(product.ImageLocalPath);

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

        private async Task SaveImageAsync(Product product, IFormFile image, CancellationToken cancellationToken)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "ProductImages");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string extension = Path.GetExtension(image.FileName);
            string fileName = $"{product.ProductId}{extension}";
            string fullFilePath = Path.Combine(uploadsFolder, fileName);

            await using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await image.CopyToAsync(stream, cancellationToken);
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            string baseUrl = $"{request?.Scheme}://{request?.Host}{request?.PathBase}";

            product.ImageUrl = $"{baseUrl}/ProductImages/{fileName}";
            product.ImageLocalPath = Path.Combine("wwwroot", "ProductImages", fileName);
        }

        private void DeleteImageFile(string? imageLocalPath)
        {
            if (string.IsNullOrWhiteSpace(imageLocalPath))
            {
                return;
            }

            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), imageLocalPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

    }
}