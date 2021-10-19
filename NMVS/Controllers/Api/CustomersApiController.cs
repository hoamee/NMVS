using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NMVS.Common;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NMVS.Controllers.Api
{
    [Route("api/Customers")]
    [ApiController]
    public class CustomersApiController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomersApiController(ICustomerService customerService, IHttpContextAccessor httpContextAccessor)
        {

            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;

        }

        //[HttpPost]
        //[Route("NewCustomer")]
        //public async Task<IActionResult> NewCustomer(CustomerVm cust)
        //{
        //    var role = _httpContextAccessor.HttpContext.User.IsInRole(Helper.UserManagement);
        //    CommonResponse<int> commonResponse;
        //    if (role)
        //    {
        //        CustomerVm ctm = cust;
        //        commonResponse = await _customerService.AddCustomer(ctm);
        //    }
        //    else
        //    {
        //        commonResponse = new();
        //        commonResponse.status = 0;
        //        commonResponse.message = "Access denied!";
        //    }

        //    return Ok(commonResponse);

        //}
    }
}
