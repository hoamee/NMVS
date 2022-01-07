using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NMVS.Models;
using NMVS.Models.ConfigModels;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;

namespace NMVS.Controllers
{
    [Authorize(Roles = "Receive inventory")]
    public class IncomingListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IIncomingService _service;
        private readonly IExcelService _excelService;
        public IncomingListsController(ApplicationDbContext context, IIncomingService service, IExcelService excelService)
        {
            _service = service;
            _context = context;
            _excelService = excelService;
        }


        // GET: IncomingLists
        public IActionResult Browse()
        {

            return View(_service.BrowseIncomingList(false));
        }

        [Authorize(Roles = "Receive inventory")]
        // GET: IncomingLists/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _service.GetListDetail(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: IncomingLists/Create
        public IActionResult CreateListHeader()
        {
            ViewBag.Suppliers = _service.GetSupplier();
            return View();
        }

        // POST: IncomingLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Receive inventory")]
        public async Task<IActionResult> CreateListHeader([Bind("IcId,SupCode,Po,PoDate,Vehicle,Driver,DeliveryDate,IsWarranty,Closed")] IncomingList incomingList)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    incomingList.LastModifiedBy = User.Identity.Name;
                    _context.Add(incomingList);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occured. Please check whether the ID is null or used before" );
            }
            ViewBag.Suppliers = _service.GetSupplier();
            return View(incomingList);
        }

        // GET: IncomingLists/Edit/5
        [Authorize(Roles = "Receive inventory")]
        public async Task<IActionResult> UpdateList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomingList = await _context.IncomingLists.FindAsync(id);
            if (incomingList == null)
            {
                return NotFound();
            }
            ViewBag.Suppliers = _service.GetSupplier();
            return View(incomingList);
        }

        // POST: IncomingLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Receive inventory")]
        public async Task<IActionResult> UpdateList(int id, [Bind("IcId,SupCode,Po,PoDate,Vehicle,Driver,DeliveryDate,IsWarranty,Closed,ItemCount")] IncomingList incomingList)
        {
            if (id != incomingList.IcId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    incomingList.LastModifiedBy = User.Identity.Name;
                    _context.Update(incomingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncomingListExists(incomingList.IcId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Suppliers = _service.GetSupplier();
            return View(incomingList);
        }

        // GET: IncomingLists/Delete/5
        [Authorize(Roles = "Receive inventory")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incomingList = await _context.IncomingLists
                .FirstOrDefaultAsync(m => m.IcId == id);
            if (incomingList == null)
            {
                return NotFound();
            }

            return View(incomingList);
        }

        // POST: IncomingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Receive inventory")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incomingList = await _context.IncomingLists.FindAsync(id);
            _context.IncomingLists.Remove(incomingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncomingListExists(int id)
        {
            return _context.IncomingLists.Any(e => e.IcId == id);
        }


        [Authorize(Roles = "Receive inventory")]
        public async Task<JsonResult> UploadList(IList<IFormFile> files)
        {
            var common = new CommonResponse<UploadReport>();
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                filename = EnsureCorrectFilename(filename);

                using (FileStream output = System.IO.File.Create("uploads/" + filename))
                    await source.CopyToAsync(output);

                common = await _service.ImportList("uploads/" + filename, filename, User.Identity.Name);

            }


            return Json(common);
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }


        [Authorize(Roles = "Receive inventory")]
        public async Task<IActionResult> DownloadList(int id)
        {
            var common = await _excelService.GetReceiptNote(id, User.Identity.Name);
            if (common.status == 1)
            {
                var filePath = common.dataenum;
                var fileExists = System.IO.File.Exists(filePath);
                var fs = System.IO.File.OpenRead(filePath);
                return File(fs, "application /vnd.ms-excel", common.message);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { common.message });
            }

        }
    }
}
