using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NMVS.Common;
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
    public class SuppliersController : Controller
    {

        private readonly ISupplierService _supplierService;
        public SuppliersController(ISupplierService custService)
        {
            _supplierService = custService;
        }
        public IActionResult Browse()
        {
            return View(_supplierService.GetSupplierList());
        }

        public IActionResult NewSupplier()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewSupplier(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var commonResponse = await _supplierService.AddSupplier(supplier);
                if (!User.IsInRole(Helper.UserManagement))
                {
                    commonResponse.status = 0;
                }

                switch (commonResponse.status)
                {
                    case 1:
                        return RedirectToAction("Browse");
                    case 0:
                        ModelState.AddModelError("", "Access Denied");
                        return View();
                    case 2:
                        return View(); //Incomplete: return to edit if exist
                    case -1:
                        ModelState.AddModelError("", "An error occured, please try again later.");
                        return View();
                }
            }
            return View();
        }

        public IActionResult UpdateSupplier(string code)
        {
            var supplier = _supplierService.GetSupplier(code);
            if (supplier != null)
            {
                return View(supplier);
            }
            else
            {
                return RedirectToAction("Browse");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSupplier(Supplier vm)
        {
            if (ModelState.IsValid)
            {
                var common = await _supplierService.UpdateSupplier(vm);
                switch (common.status)
                {
                    case 1:
                        return RedirectToAction("Browse");
                    case -1:
                        ModelState.AddModelError("", common.message);
                        return View(vm);
                }
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<JsonResult> UploadList(IList<IFormFile> files)
        {
            var common = new CommonResponse<UploadReport>();
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                filename = EnsureCorrectFilename(filename);

                using (FileStream output = System.IO.File.Create("uploads/" + filename))
                    await source.CopyToAsync(output);

                common = await _supplierService.ImportSupplier("uploads/" + filename, filename, User.Identity.Name);
                
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
