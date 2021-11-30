using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IUserDataService userDataService)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userData = userDataService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]        
        public IActionResult AccessDenied(string returnUrl = null)
        {
            return View("_Error403");

        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.Status = -1;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVm passwordVm)
        {
            ViewBag.Status = -1;
            ViewBag.Message = "";
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, passwordVm.Password, passwordVm.ConfirmPassword);

                    
                    if (result.Succeeded)
                    {
                        ViewBag.Status = 1;
                    }
                    else 
                    {
                        ViewBag.Status = 0;
                        var mess = "";
                        foreach (var item in result.Errors)
                        {
                            mess += item.Description + ". ";
                        }
                        ViewBag.Message = mess;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Can not validate user");
                }
            }
            return View(passwordVm);

        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await _userData.InitRoleAsync();
            if (_userManager.Users.FirstOrDefault(x=>x.UserName == "nmvadmin") == null)
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
                    WoCreation= true,
                    WoReporter = true,
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
                                var usrHost = Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName;
                                var usr = _db.Users.First(x => x.UserName == user.UserName);
                                //if (!string.IsNullOrEmpty(usr.ActiveHostName))
                                //{
                                //    if (usr.ActiveHostName != usrHost)
                                //    {
                                //        ModelState.AddModelError("", "Your account is logged in at device " + usr.ActiveHostName + ", please sign out first");
                                //        return View();
                                //    }
                                //}                                
                                usr.ActiveHostName = Dns.GetHostEntry(HttpContext.Connection.RemoteIpAddress).HostName;
                                _db.Update(usr);
                                HttpContext.Session.SetString("sUserName", user.UserName);
                                HttpContext.Session.SetString("sUserHost", usrHost);
                                await _db.SaveChangesAsync();
                                
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
                                    var test = _db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
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
            }
            catch(Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> LogOffConflict(string message)
        {
            await _signInManager.SignOutAsync();
            ViewBag.mess = message;
            return View();
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
