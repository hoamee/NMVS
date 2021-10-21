using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class InvRequestsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InvRequestsController(ApplicationDbContext db)
        {
            _db = db;
        }
        

        public IActionResult History()
        {
            
            return View();
        }
    }
}
