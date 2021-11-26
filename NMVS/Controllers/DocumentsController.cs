using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class DocumentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RoleManager()
        {
            return View();
        }

        public IActionResult Site()
        {
            return View();
        }

        public IActionResult Warehouse()
        {
            return View();
        }

        public IActionResult Location()
        {
            return View();
        }

        public IActionResult ItemData()
        {
            return View();
        }

        public IActionResult IncomingList()
        {
            return View();
        }

        public IActionResult DownloadItemDataForm()
        {
            var filePath = "template/IDL01_Item data list.xlsx";
            var fileExists = System.IO.File.Exists(filePath);
            string error = "";
            if (fileExists)
            {
                try
                {
                    var fs = System.IO.File.OpenRead(filePath);
                    return File(fs, "application /vnd.ms-excel", "Item data upload template.xlsx");
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
            }

            return RedirectToAction("Error", "Home", error);


        }
        public IActionResult DownloadCustomerForm()
        {
            var filePath = "template/CTM01_Customer list.xlsx";
            var fileExists = System.IO.File.Exists(filePath);
            string error = "";
            if (fileExists)
            {
                try
                {
                    var fs = System.IO.File.OpenRead(filePath);
                    return File(fs, "application /vnd.ms-excel", "Customer upload template.xlsx");
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
            }

            return RedirectToAction("Error", "Home", error);


        }
        public IActionResult DownloadSupplierForm()
        {
            var filePath = "template/SPL01_Supplier List.xlsx";
            var fileExists = System.IO.File.Exists(filePath);
            string error = "";
            if (fileExists)
            {
                try
                {
                    var fs = System.IO.File.OpenRead(filePath);
                    return File(fs, "application /vnd.ms-excel", "Supplier upload template.xlsx");
                }
                catch (Exception e)
                {
                    error = e.Message;
                }
            }

            return RedirectToAction("Error", "Home", error);


        }
    }
}
