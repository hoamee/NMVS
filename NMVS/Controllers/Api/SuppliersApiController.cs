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
    [Route("api/Suppliers")]
    [ApiController]
    public class SuppliersApiController : Controller
    {
        private readonly IIncomingService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SuppliersApiController(IIncomingService service, IHttpContextAccessor httpContextAccessor)
        {

            _service = service;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpGet]
        [Route("GetSupplierList")]
        public IActionResult GetSupplierList()
        {
            var role = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            CommonResponse<List<TypeVm>> commonResponse = new();

            if (!role)
            {
                commonResponse.status = 0;
                commonResponse.message = "Access denied!";
                return Ok(commonResponse);
            }

            try
            {
                commonResponse.dataenum = _service.GetSupplier();
                commonResponse.status = 1;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = -1;
            }
            return Ok(commonResponse);

        }

        
    }
}
