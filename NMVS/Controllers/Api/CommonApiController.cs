using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
namespace NMVS.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonApiController : Controller
    {
        ApplicationDbContext _context;
        public CommonApiController(ApplicationDbContext context){
            _context = context;

        }

        [HttpGet]
        [Route("GetItemData")]
        public IActionResult GetItemData()
        {
            var model = (from d in _context.ItemDatas.Where(x=>x.Active == true) select new ItemDataJson{
                itemNo = d.ItemNo,
                itemName = d.ItemName
            }).ToList();

            
            return Ok(model);
        }
    }

    internal class ItemDataJson{
        public string itemNo {set;get;}
        public string itemName {set;get;}
    }
}