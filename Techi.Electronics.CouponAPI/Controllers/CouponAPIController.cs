using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Techi.Electronics.CouponAPI.Models.Dto;
using Techi.Electronics.CouponAPI.Service.IService;

namespace Techi.Electronics.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponAPIController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> Get(CancellationToken cancellationToken)
        {
            var response = await _couponService.GetAllCouponsAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> Get(int id, CancellationToken cancellationToken)
        {
            var response = await _couponService.GetCouponByIdAsync(id, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("GetByCode/{code}")]
        public async Task<ActionResult<ResponseDto>> GetByCode(string code, CancellationToken cancellationToken)
        {
            var response = await _couponService.GetCouponByCodeAsync(code, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] CouponDto couponDto, CancellationToken cancellationToken)
        {
            var response = await _couponService.CreateCouponAsync(couponDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Put([FromBody] CouponDto couponDto, CancellationToken cancellationToken)
        {
            var response = await _couponService.UpdateCouponAsync(couponDto, cancellationToken);

            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Delete(int id, CancellationToken cancellationToken)
        {
            var response = await _couponService.DeleteCouponAsync(id, cancellationToken);

            if (!response.IsSuccess)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}