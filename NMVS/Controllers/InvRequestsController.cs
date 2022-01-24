using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    [Authorize(Roles = "Request inventory, Handle request")]
    public class InvRequestsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IRequestService _service;
        private readonly IExcelService _excelService;

        public InvRequestsController(ApplicationDbContext db, IRequestService service, IExcelService excelService)
        {
            _service = service;
            _db = db;
            _excelService = excelService;
        }

        public IActionResult SoRequests(){
            var workSpace = HttpContext.Session.GetString("susersite");

            return View(_service.GetSoRequests(workSpace, false));
        }

        public IActionResult History()
        {
            var workSpace = HttpContext.Session.GetString("susersite");

            return View(_service.GetMfgRequests(workSpace, false));
        }

        public IActionResult NewRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewRequest(InvRequest invRequest)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            if (invRequest is null)
            {
                ModelState.AddModelError("", "An error occurred");
            }
            else
            {
                var checkExist = _db.InvRequests.FirstOrDefault(x => x.Ref == invRequest.Ref && x.Site == workSpace);
                if (checkExist is null)
                {
                    invRequest.RqID = invRequest.Ref;
                    invRequest.Site = workSpace;
                    _db.Add(invRequest);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(History));
                }
                else
                {
                    ModelState.AddModelError("", "This batch is already exist");
                }
            }

            return View();
        }

        public IActionResult UpdateRequest(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var request = _db.InvRequests.Find(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        [HttpPost]
        public IActionResult UpdateRequest(InvRequest invRequest)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            if (invRequest is null)
            {
                ModelState.AddModelError("", "An error occurred");
            }
            else
            {

                try
                {
                    invRequest.Site = workSpace;
                    _db.Update(invRequest);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(History));
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.ToString());
                }

            }

            return View();
        }

        public IActionResult RequestDetail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _service.GetRequestDetail(id);

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public IActionResult RequestDetCreate(string id)
        {
            ViewBag.RqId = id;
            return View();
        }

        [HttpPost]
        public IActionResult RequestDetCreate(RequestDet det)
        {
            if (ModelState.IsValid)
            {
                det.Arranged = 0;
                det.Issued = 0;
                det.Picked = 0;
                _db.RequestDets.Add(det);
                _db.SaveChanges();
                return RedirectToAction("RequestDetail", new { id = det.RqID });
            }
            ViewBag.RqId = det.RqID;

            return View();
        }

        public IActionResult UpdateDet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var det = _db.RequestDets.Find(id);
            if (det == null)
            {
                return NotFound();
            }

            ViewBag.RqId = det.RqID;

            return View(det);
        }

        [HttpPost]
        public IActionResult UpdateDet(RequestDet det)
        {
            if (ModelState.IsValid)
            {

                det.Arranged = 0;
                det.Issued = 0;
                det.Picked = 0;
                _db.RequestDets.Update(det);
                _db.SaveChanges();
                return RedirectToAction("RequestDetail", new { id = det.RqID });

            }



            ViewBag.RqId = det.RqID;

            return View(det);
        }

        public IActionResult GetItemMaster(string id)
        {
            CommonResponse<List<ItemAvailVm>> commonResponse = new();
            var workSpace = HttpContext.Session.GetString("susersite");
            commonResponse.dataenum = _service.GetItemAvails(id, workSpace);
            if (commonResponse.dataenum.Any())
            {
                commonResponse.status = 1;
                commonResponse.message = "OK";
            }
            else
            {
                commonResponse.status = -1;
                commonResponse.message = "No available items in warehouse";
            }

            return Ok(commonResponse);

        }

        public IActionResult PickListSO(int id)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            var rq = _db.RequestDets.Find(id);

            ViewBag.DetId = id;
            ViewBag.RqId = rq.RqID;
            ViewBag.qty = rq.Quantity - rq.Picked;
            ViewBag.History = _db.IssueOrders.Where(x => x.DetId == id).ToList();

            ViewBag.LocList = new SelectList(_db.Shippers
                .Where(x => !string.IsNullOrEmpty(x.CheckInBy)
                    && string.IsNullOrEmpty(x.CheckOutBy)
                    && x.IssueConfirmed != true
                    && x.Site == workSpace)
                .ToList(), "ShpId", "ShpDesc");


            return View(_service.GetItemMasterVms(rq));

        }

        public IActionResult PickListMFG(int id)
        {
            var rq = _db.RequestDets.Find(id);
            var workSpace = HttpContext.Session.GetString("susersite");
            ViewBag.DetId = id;
            ViewBag.RqId = rq.RqID;
            ViewBag.qty = rq.Quantity - rq.Picked;
            ViewBag.History = _db.IssueOrders.Where(x => x.DetId == id).ToList();
            var whs = _db.Warehouses.Where(x => x.SiCode == workSpace).Select(s => s.WhCode);

            ViewBag.LocList = new SelectList(_db.Locs
                .Where(x => x.LocType == "MFG"
                    && whs.Contains(x.WhCode))
                .ToList(), "LocCode", "LocDesc");

            if (rq.SpecDate == null)
            {
                return View(_service.GetItemMasterVms(rq));
            }
            else
            {
                return View(_service.GetItemMasterVms(rq).Where(x => x.DateIn.Date == rq.SpecDate));
            }
        }

        public IActionResult IssueNoteMfg()
        {
            var model = _db.MfgIssueNotes.ToList();
            return View(model);
        }

        public IActionResult MfgDetail(int id)
        {
            var isn = _db.MfgIssueNotes.Find(id);

            var noteDet = (from d in _db.IssueNoteDets
                           join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                           from a in all.DefaultIfEmpty()
                           select new MfgIssueNoteDet
                           {
                               ItemNo = d.ItemNo,
                               Id = d.Id,
                               ItemName = a.ItemName,
                               IsNId = d.IsNId,
                               PtId = d.PtId,
                               Quantity = d.Quantity
                           }).ToList();

            var model = new MfgNoteVm
            {
                Det = noteDet,
                MfgIssueNote = isn
            };

            return View(model);
        }

        public IActionResult IssueNoteList()
        {
            var model = _service.GetListIssueNote();
            return View(model);
        }

        public IActionResult IssueNoteDetails(int id, int so)
        {
            return View(_service.GetIssueNoteDetail(id, so));
        }

        public IActionResult IssueNoteVehicle()
        {
            var model = _db.Shippers.ToList();
            foreach (var item in model)
            {
                try
                {
                    var ldp = string.Join(", ", _db.ShipperDets.Where(x => x.ShpId == item.ShpId).Select(x => x.RqId).ToArray());
                    item.Loc = ldp;
                }
                catch
                {
                    continue;
                }
            }
            return View(model);
        }

        public IActionResult IssueNoteVehicleDetails(int id)
        {
            return View(_service.GetVehicleNoteDetail(id));
        }

        public async Task<IActionResult> DownloadShipperNote(int id)
        {

            var common = await _excelService.GetshipperNote(id, User.Identity.Name);
            if (common.status == 1)
            {
                var filePath = common.dataenum;
                var fs = System.IO.File.OpenRead(filePath);
                return File(fs, "application /vnd.ms-excel", common.message);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { common.message });
            }
        }

        public async Task<IActionResult> DownloadPreShipperNote(int id)
        {

            var common = await _excelService.GetPreShipperNote(id, User.Identity.Name);
            if (common.status == 1)
            {
                var filePath = common.dataenum;
                var fs = System.IO.File.OpenRead(filePath);
                return File(fs, "application /vnd.ms-excel", common.message);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { common.message });
            }
        }

        public async Task<IActionResult> DownloadIssueNoteByShipper(int id, int so)
        {

            var common = await _excelService.GetIssueNoteSo(id, User.Identity.Name, 0, so);
            if (common.status == 1)
            {
                var filePath = common.dataenum;
                var fs = System.IO.File.OpenRead(filePath);
                return File(fs, "application /vnd.ms-excel", common.message);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { common.message });
            }
        }

        public async Task<IActionResult> GetIssueNote(int id)
        {

            var common = await _excelService.GetIssueNoteMFG(id, User.Identity.Name);
            if (common.status == 1)
            {
                var filePath = common.dataenum;
                var fs = System.IO.File.OpenRead(filePath);
                return File(fs, "application /vnd.ms-excel", common.message);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { common.message });
            }
        }

        public async Task<IActionResult> DownloadIssueNoteSO(int id, int so)
        {

            var common = await _excelService.GetIssueNoteSo(0, User.Identity.Name, id, so);
            if (common.status == 1)
            {
                var filePath = common.dataenum;
                var fs = System.IO.File.OpenRead(filePath);
                return File(fs, "application /vnd.ms-excel", common.message);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { common.message });
            }
        }
        public async Task<JsonResult> UploadList(IList<IFormFile> files)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            var common = new CommonResponse<UploadReport>();
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                filename = EnsureCorrectFilename(filename);

                using (FileStream output = System.IO.File.Create("uploads/" + filename))
                    await source.CopyToAsync(output);

                common = await _service.ImportList("uploads/" + filename, filename, User.Identity.Name, workSpace);

            }


            return Json(common);
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }
    }
}
