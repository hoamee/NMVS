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
    [Route("api/Users")]
    [ApiController]
    public class UsersApiController : Controller
    {
        private readonly IUserDataService _userData;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersApiController(IUserDataService userData, IHttpContextAccessor httpContextAccessor)
        {

            _userData = userData;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpGet]
        [Route("GetUserRole/{usr}")]
        public IActionResult GetUserRole(string usr)
        {
            var role = _httpContextAccessor.HttpContext.User.IsInRole(Helper.UserManagement);
            CommonResponse<UserRoleVm> commonResponse = new();

            if (!role)
            {
                commonResponse.status = 0;
                commonResponse.message = "Access denied!";
                return Ok(commonResponse);
            }

            try
            {
                commonResponse.dataenum = _userData.GetUserRole(usr);
                commonResponse.status = 1;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = -1;
            }
            return Ok(commonResponse);

        }


        [HttpPost]
        [Route("SeedUserRole")]
        public async Task<IActionResult> SeedUserRoleAsync(UserRoleVm usr)
        {
            var role = _httpContextAccessor.HttpContext.User.IsInRole(Helper.UserManagement);
            CommonResponse<int> commonResponse = new();
            if (role)
            {
                var user = usr;
                await _userData.SeedingRole(user);
                commonResponse.status = 1;
            }
            else
            {
                commonResponse.status = 0;
                commonResponse.message = "Access denied!";
            }

            return Ok(commonResponse);

        }
    }
}
