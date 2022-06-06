using Mango.Services.CouponApi.Model.Dto;
using Mango.Services.CouponApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.Services.CouponApi.Controllers
{
    [ApiController]
    [Route("api/coupon")]
    public class CouponController : Controller
    {
        private readonly ICouponRepository repository;
        protected ResponseDto response;

        public CouponController(ICouponRepository repository)
        {
            this.repository = repository;
            this.response = new ResponseDto();
        }

        [HttpGet("{code}")]
        public async Task<ResponseDto> GetDiscountForCode(string code)
        {
            try
            {
                CouponDto coupon = await repository.GetCouponByCode(code);
                response.Result = coupon;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
            }

            return response;
        }

    }
}
