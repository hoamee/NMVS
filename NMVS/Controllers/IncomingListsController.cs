﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;

namespace NMVS.Controllers
{
    public class IncomingListsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IIncomingService _service;

        public IncomingListsController(ApplicationDbContext context, IIncomingService service)
        {
            _service = service;
            _context = context;
        }

        // GET: IncomingLists
        public IActionResult Browse()
        {
            
            return View(_service.BrowseIncomingList(false));
        }

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
        public async Task<IActionResult> CreateListHeader([Bind("IcId,SupCode,Po,PoDate,Vehicle,Driver,DeliveryDate,IsWarranty,Closed")] IncomingList incomingList)
        {
            if (ModelState.IsValid)
            {
                incomingList.LastModifiedBy = User.Identity.Name;
                _context.Add(incomingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Suppliers = _service.GetSupplier();
            return View(incomingList);
        }

        // GET: IncomingLists/Edit/5
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
        public async Task<IActionResult> UpdateList(int id, [Bind("IcId,SupCode,Po,PoDate,Vehicle,Driver,DeliveryDate,IsWarranty,Closed")] IncomingList incomingList)
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

        public JsonResult AddItem(ItemMaster item)
        {
            ItemMaster iItem = item;
            CommonResponse<int> common = new();
            try
            {
                _context.Add(new ItemMaster
                {
                    ItemNo = iItem.ItemNo,
                    IcId = iItem.IcId,
                    RecQty = iItem.RecQty,
                    RefNo = iItem.RefNo,
                    RefDate = iItem.RefDate,
                    PtCmt = iItem.PtCmt
                });
                _context.SaveChanges();
                common.status = 1;
            }
            catch(Exception e)
            {
                common.status = -1;
                common.message = e.ToString();
            }
            return Json(common);
        }

        public async Task<JsonResult> UploadList(IList<IFormFile> files)
        {
            ExcelRespone excelRespose = new();
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                filename = this.EnsureCorrectFilename(filename);

                using (FileStream output = System.IO.File.Create("uploads/" + filename))
                    await source.CopyToAsync(output);

                excelRespose = await _service.ImportList("uploads/" + filename, User.Identity.Name);
                excelRespose.fileName = filename;

            }
            return Json(excelRespose);
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }
    }
}