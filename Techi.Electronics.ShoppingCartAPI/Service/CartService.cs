using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Techi.Electronics.MessageBus;
using Techi.Electronics.ShoppingCartAPI.Data;
using Techi.Electronics.ShoppingCartAPI.Models;
using Techi.Electronics.ShoppingCartAPI.Models.Dto;
using Techi.Electronics.ShoppingCartAPI.Service.IService;

namespace Techi.Electronics.ShoppingCartAPI.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public CartService(
            AppDbContext db,
            IMapper mapper,
            ICouponService couponService,
            IProductService productService,
            IMessageBus messageBus,
            IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _couponService = couponService;
            _productService = productService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        public async Task<ResponseDto> GetCartAsync(string userId, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var cartHeader = await _db.CartHeaders
                    .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

                if (cartHeader == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cart not found";
                    return response;
                }

                var cart = new CartDto
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(cartHeader)
                };

                var cartDetails = await _db.CartDetails
                    .Where(u => u.CartHeaderId == cartHeader.CartHeaderId)
                    .ToListAsync(cancellationToken);

                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(cartDetails);

                var productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);

                    if (item.Product != null)
                    {
                        cart.CartHeader.CartTotal += item.Count * item.Product.Price;
                    }
                }

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);

                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                response.Result = cart;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> ApplyCouponAsync(CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var cartFromDb = await _db.CartHeaders
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId, cancellationToken);

                if (cartFromDb == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cart not found";
                    return response;
                }

                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                await _db.SaveChangesAsync(cancellationToken);

                response.Result = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.ToString();
            }

            return response;
        }

        public async Task<ResponseDto> RemoveCouponAsync(CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var cartFromDb = await _db.CartHeaders
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId, cancellationToken);

                if (cartFromDb == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cart not found";
                    return response;
                }

                cartFromDb.CouponCode = string.Empty;
                await _db.SaveChangesAsync(cancellationToken);

                response.Result = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.ToString();
            }

            return response;
        }

        public async Task<ResponseDto> CartUpsertAsync(CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var cartHeaderFromDb = await _db.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId, cancellationToken);

                var cartDetailToProcess = cartDto.CartDetails.First();

                if (cartHeaderFromDb == null)
                {
                    var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);

                    await _db.CartHeaders.AddAsync(cartHeader, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);

                    cartDetailToProcess.CartHeaderId = cartHeader.CartHeaderId;

                    await _db.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDetailToProcess), cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    var cartDetailsFromDb = await _db.CartDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            u => u.ProductId == cartDetailToProcess.ProductId &&
                                 u.CartHeaderId == cartHeaderFromDb.CartHeaderId,
                            cancellationToken);

                    if (cartDetailsFromDb == null)
                    {
                        cartDetailToProcess.CartHeaderId = cartHeaderFromDb.CartHeaderId;

                        await _db.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDetailToProcess), cancellationToken);
                        await _db.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {
                        cartDetailToProcess.Count += cartDetailsFromDb.Count;
                        cartDetailToProcess.CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDetailToProcess.CartDetailsId = cartDetailsFromDb.CartDetailsId;

                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDetailToProcess));
                        await _db.SaveChangesAsync(cancellationToken);
                    }
                }

                response.Result = cartDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> RemoveCartAsync(int cartDetailsId, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var cartDetails = await _db.CartDetails
                    .FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId, cancellationToken);

                if (cartDetails == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Cart item not found";
                    return response;
                }

                var totalCountOfCartItem = await _db.CartDetails
                    .CountAsync(u => u.CartHeaderId == cartDetails.CartHeaderId, cancellationToken);

                _db.CartDetails.Remove(cartDetails);

                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId, cancellationToken);

                    if (cartHeaderToRemove != null)
                    {
                        _db.CartHeaders.Remove(cartHeaderToRemove);
                    }
                }

                await _db.SaveChangesAsync(cancellationToken);

                response.Result = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> EmailCartRequestAsync(CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            if (cartDto == null || cartDto.CartHeader == null)
            {
                response.IsSuccess = false;
                response.Message = "Invalid cart request";
                return response;
            }

            try
            {
                var queueName = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

                if (string.IsNullOrWhiteSpace(queueName))
                {
                    throw new InvalidOperationException($"EmailShoppingCart {queueName} is not configured.");
                }

                await _messageBus.PublishMessage(cartDto, queueName);

                response.Result = true;
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