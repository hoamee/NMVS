using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserDataService _userData;
        RoleManager<ApplicationRole> _roleManager;
        public UsersController(IUserDataService userData, RoleManager<ApplicationRole> roleManager)
        {
            _userData = userData;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> AllUsers()
        {
            if (User.IsInRole("Manage user"))
            {
                await _userData.InitRoleAsync();
                var model = _userData.GetUserList();
                return View(model);
            }
            else
            {
                return View("_Error403");
            }
        }

        public IActionResult UserRole()
        {
            if (User.IsInRole("Manage user"))
            {
                return View(_userData.GetUserRoleList());
            }
            else
            {
                return View("_Error403");
            }
        }


        public IActionResult SeedingRole(string name)
        {

            if (User.IsInRole("Manage user"))
            {
                var model = _userData.GetUserRole(name);
                return PartialView("SeedingRole", model);
            }
            else
            {
                return View("_Error403");
            }
        }

        [HttpPost]
        public IActionResult SeedingRole(UserRoleVm userRoleVM)
        {
            if (User.IsInRole("Manage user"))
            {
                return View();

            }
            else
            {
                return View("_Error403");
            }

        }
    }
}
