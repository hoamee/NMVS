using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Models.ViewModels;

namespace NMVS.Controllers.Api
{
    [ApiController]
    [Route("api/Movement")]
    public class MovementApiController : Controller
    {
        private readonly ApplicationDbContext _db;
        public MovementApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("GetIssueOrderList")]
        public IActionResult GetIssueOrderList()
        {
            
            return Ok();
        }
    }
}