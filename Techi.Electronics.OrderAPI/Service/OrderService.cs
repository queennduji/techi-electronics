using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Techi.Electronics.MessageBus;
using Techi.Electronics.OrderAPI.Data;
using Techi.Electronics.OrderAPI.Models;
using Techi.Electronics.OrderAPI.Models.Dto;
using Techi.Electronics.OrderAPI.Service.IService;
using Techi.Electronics.OrderAPI.Utility;

namespace Techi.Electronics.OrderAPI.Service
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        public OrderService(AppDbContext db, IMapper mapper, IConfiguration configuration, IMessageBus messageBus)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
            _messageBus = messageBus;
        }

        public async Task<ResponseDto> GetOrdersAsync(
            string? userId,
            bool isAdmin,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                IQueryable<OrderHeader> query = _db.OrderHeaders
                    .Include(u => u.OrderDetails)
                    .OrderByDescending(u => u.OrderHeaderId);

                if (!isAdmin)
                {
                    query = query.Where(u => u.UserId == userId);
                }

                var orders = await query.ToListAsync(cancellationToken);
                response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(orders);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetOrderAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var orderHeader = await _db.OrderHeaders
                    .Include(u => u.OrderDetails)
                    .FirstOrDefaultAsync(u => u.OrderHeaderId == id, cancellationToken);

                if (orderHeader == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                    return response;
                }

                response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> CreateOrderAsync(CartDto cartDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.UtcNow;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);
                orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);

                OrderHeader order = _mapper.Map<OrderHeader>(orderHeaderDto);

                _db.OrderHeaders.Add(order);
                await _db.SaveChangesAsync(cancellationToken);

                orderHeaderDto.OrderHeaderId = order.OrderHeaderId;
                response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    });
                }

                if (stripeRequestDto.OrderHeader.Discount > 0 &&
                    !string.IsNullOrWhiteSpace(stripeRequestDto.OrderHeader.CouponCode))
                {
                    options.Discounts = new List<SessionDiscountOptions>
                    {
                        new SessionDiscountOptions
                        {
                            Coupon = stripeRequestDto.OrderHeader.CouponCode
                        }
                    };
                }

                var sessionService = new SessionService();
                Session session = sessionService.Create(options);

                stripeRequestDto.StripeSessionUrl = session.Url;

                OrderHeader? orderHeader = await _db.OrderHeaders
                    .FirstOrDefaultAsync(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);

                if (orderHeader == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                    return response;
                }

                orderHeader.StripeSessionId = session.Id;
                await _db.SaveChangesAsync(cancellationToken);

                response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> ValidateStripeSessionAsync(int orderHeaderId, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                OrderHeader? orderHeader = await _db.OrderHeaders
                    .FirstOrDefaultAsync(u => u.OrderHeaderId == orderHeaderId);

                if (orderHeader == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(orderHeader.StripeSessionId))
                {
                    response.IsSuccess = false;
                    response.Message = "Stripe session was not found for this order.";
                    return response;
                }

                var sessionService = new SessionService();
                Session session = sessionService.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;

                    await _db.SaveChangesAsync(cancellationToken);

                    RewardsDto rewardsDto = new()
                    {
                        OrderId = orderHeader.OrderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };

                    string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardsDto, topicName);

                    response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"Payment not completed. Stripe status: {paymentIntent.Status}";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> UpdateOrderStatusAsync(
           int orderId,
           string newStatus,
           CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            try
            {
                var orderHeader = await _db.OrderHeaders
                    .FirstOrDefaultAsync(u => u.OrderHeaderId == orderId, cancellationToken);

                if (orderHeader == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                    return response;
                }

                if (newStatus == SD.Status_Cancelled &&
                    !string.IsNullOrWhiteSpace(orderHeader.PaymentIntentId))
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentId
                    };

                    var refundService = new RefundService();
                    Refund refund = refundService.Create(options);
                }

                orderHeader.Status = newStatus;
                await _db.SaveChangesAsync(cancellationToken);

                response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
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