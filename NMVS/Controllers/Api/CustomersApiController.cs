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

        [HttpGet]
        [Route("GetCustomer")]
        public IActionResult GetCustomer(){
            return Ok(_customerService.GetCustomerList());
        }

        [HttpGet]
        [Route("GetCustomerList")]
        public IActionResult GetCustomerList(){
            var custList = _customerService.GetCustomerList().Select(p => new TypeVm{
                Code = p.CustCode,
                Desc = p.CustName,
                ShortName = p.ShortName
            }).ToList();
            return Ok(custList);
        }
    }
}
