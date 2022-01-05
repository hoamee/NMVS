using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Models.ViewModels;
using NMVS.Services;

namespace NMVS.Controllers.Api
{
    [Route("api/ItemData")]
    [ApiController]
    public class ItemDataApiController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IItemDataService service;

        public ItemDataApiController(ApplicationDbContext db, IItemDataService service)
        {
            _db = db;
            this.service = service;
        }

        [HttpGet]
        [Route("GetItemDataList")]
        public IActionResult GetItemDataList()
        {

            return Ok(service.GetItemList());
        }


        [HttpGet]
        [Route("ItemDataBrowse")]
        public IActionResult ItemDataBrowse()
        {
            var itemBrowse = service.GetItemList().Select(i => new TypeVm
            {
                Code = i.ItemNo,
                Desc = i.ItemName
            }).ToList();
            return Ok(itemBrowse);
        }

    }
}