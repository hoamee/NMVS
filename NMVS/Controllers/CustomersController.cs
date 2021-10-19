using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService custService)
        {
            _customerService = custService;
        }

        public IActionResult Browse()
        {
            return View(_customerService.GetCustomerList());
        }

        public IActionResult NewCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewCustomer(CustomerVm customerVm)
        {
            if (ModelState.IsValid)
            {
                var commonResponse = await _customerService.AddCustomer(customerVm);
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

        public IActionResult UpdateCustomer(string code)
        {
            var customer = _customerService.GetCustomer(code);
            if (customer != null)
            {
                return View(customer);
            }
            else
            {
                return RedirectToAction("Browse");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(Customer vm)
        {
            if (ModelState.IsValid)
            {
                var common = await _customerService.UpdateCustomer(vm);
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
            ExcelRespone excelRespose = new();
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                filename = this.EnsureCorrectFilename(filename);

                using (FileStream output = System.IO.File.Create("uploads/" + filename))
                    await source.CopyToAsync(output);

                excelRespose = await _customerService.ImportCustomer("uploads/" + filename);
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
