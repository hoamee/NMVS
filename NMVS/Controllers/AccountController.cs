using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<ApplicationRole> _roleManager;
        private readonly IUserDataService _userData;

        public AccountController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IUserDataService userDataService)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userData = userDataService;
        }

        public async Task<IActionResult> Login()
        {
            if (await _userManager.FindByNameAsync("nmvadmin") == null)
            {
                var newUser = new ApplicationUser
                {
                    FullName = "NMV Admin",
                    UserName = "nmvadmin",
                    Email = "le.thanh.hieu@netmarks.com.vn",
                    Active = false
                };

                var result = await _userManager.CreateAsync(newUser, "NetmarksHN#2021");
                ModelState.AddModelError("", "Not existed");

                var usr = new UserRoleVm
                {
                    Active = true,
                    AppSO = true,
                    ArrangeInventory = true,
                    CreateSO = true,
                    Guard = true,
                    HandleRequest = true,
                    MoveInv = true,
                    QC = true,
                    ReceiveInv = true,
                    RegVehicle = true,
                    RequestInv = true,
                    UserManagement = true,
                    UserName = "nmvadmin"
                };

                await _userData.SeedingRole(usr);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm model)
        {
            try {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User not existed");
                    }
                    else
                    {
                        if (user.Active)
                        {

                            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                            if (result.Succeeded)
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else if (result.IsLockedOut)
                            {
                                ModelState.AddModelError("", "This account has been locked out");
                            }
                            else
                            {
                                var err = "";
                                try
                                {
                                    var test = _db.Users.First(x => x.UserName == User.Identity.Name);
                                    if (test != null)
                                    {
                                        err = test.UserName;
                                    }
                                }
                                catch (Exception e)
                                {
                                    err = e.ToString();
                                }
                                ModelState.AddModelError("", err);
                                return View(model);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "This account hasn't been activated");
                        }
                    }

                }
            }catch(Exception e)
            {
                ModelState.AddModelError("", e.ToString());
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userCount = _userManager.Users.Count();
                if (userCount >= 20)
                {
                    ModelState.AddModelError("", "The number of users has reached the limit. Please update the licence!");
                }
                else
                {
                    try
                    {
                        var newUser = new ApplicationUser
                        {
                            FullName = model.FullName,
                            UserName = model.Account,
                            Email = model.Email,
                            Active = false
                        };

                        var result = await _userManager.CreateAsync(newUser, model.Password);

                        if (result.Succeeded)
                        {
                            ModelState.AddModelError("", "Your registration information has been saved. Please inform your administrator to activate your account!");

                            return View();
                        }
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "An error has occurred.");
                    }
                }

                

            }
            return View();
        }
    }
}
