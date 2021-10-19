using System;
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
    public class ItemDatasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IItemDataService _service;

        public ItemDatasController(ApplicationDbContext context, IItemDataService service)
        {
            _service = service;
            _context = context;
        }



        // GET: ItemDatas
        public IActionResult Browse()
        {
            
            return View(_service.GetItemList());
        }

        // GET: ItemDatas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemData = await _context.ItemDatas
                .FirstOrDefaultAsync(m => m.ItemNo == id);
            if (itemData == null)
            {
                return NotFound();
            }

            return View(itemData);
        }

        // GET: ItemDatas/Create
        public IActionResult AddItem()
        {
            ViewBag.ItemType = _context.GeneralizedCodes.Where(x => x.CodeFldName == "Itemtype").ToList();
            return View();
        }

        // POST: ItemDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem([Bind("ItemNo,ItemName,ItemType,ItemUm,ItemPkg,ItemPkgQty,Flammable,ItemWhUnit,Active")] ItemData itemData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ItemType = _context.GeneralizedCodes.Where(x => x.CodeFldName == "Itemtype").ToList();
            return View(itemData);
        }

        // GET: ItemDatas/Edit/5
        public async Task<IActionResult> UpdateItem(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemData = await _context.ItemDatas.FindAsync(id);
            if (itemData == null)
            {
                return NotFound();
            }
            ViewBag.ItemType = _context.GeneralizedCodes.Where(x => x.CodeFldName == "Itemtype").ToList();
            return View(itemData);
        }

        // POST: ItemDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateItem(string id, [Bind("ItemNo,ItemName,ItemType,ItemUm,ItemPkg,ItemPkgQty,Flammable,ItemWhUnit,Active")] ItemData itemData)
        {
            if (id != itemData.ItemNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemDataExists(itemData.ItemNo))
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
            ViewBag.ItemType = _context.GeneralizedCodes.Where(x => x.CodeFldName == "Itemtype").ToList();
            return View(itemData);
        }

        // GET: ItemDatas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemData = await _context.ItemDatas
                .FirstOrDefaultAsync(m => m.ItemNo == id);
            if (itemData == null)
            {
                return NotFound();
            }

            return View(itemData);
        }

        // POST: ItemDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var itemData = await _context.ItemDatas.FindAsync(id);
            _context.ItemDatas.Remove(itemData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemDataExists(string id)
        {
            return _context.ItemDatas.Any(e => e.ItemNo == id);
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

                excelRespose = await _service.ImportItemData("uploads/" + filename);
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
