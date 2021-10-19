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
            await _userData.InitRoleAsync();
            var model = _userData.GetUserList();
            return View(model);
        }

        public IActionResult UserRole()
        {
            return View(_userData.GetUserRoleList());
        }


        public IActionResult SeedingRole(string name)
        {
            var model = _userData.GetUserRole(name);
            return PartialView("SeedingRole", model);
        }

        [HttpPost]
        public IActionResult SeedingRole(UserRoleVm userRoleVM)
        {

            return View();
        }
    }
}
