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

        public IActionResult Qc()
        {
            return View();
        }

        public IActionResult Allocate()
        {
            return View();
        }

        public IActionResult Movement()
        {
            return View();
        }

        public IActionResult RequestInv()
        {
            return View();
        }

        public IActionResult DownloadTemplate(string id)
        {
            var filePath = "";
            var template = "";
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Error", "Home", "An error occurred");
            }
            if (id == "itemdata")
            {
                template = "Item data upload template.xlsx";
                filePath = "template/IDL01_Item data list.xlsx";
            }
            else if (id == "supplier")
            {
                template = "Supplier upload template.xlsx";
                filePath = "template/SPL01_Supplier List.xlsx";
            }else if(id == "customer")
            {
                template = "Customer upload template.xlsx";
                filePath = "template/CTM01_Customer list.xlsx";
            }
            else if (id == "incoming")
            {
                template = "Imcoming list upload template.xlsx";
                filePath = "template/ICLS01_Incoming List (Supplier).xlsx";
            }


            var fileExists = System.IO.File.Exists(filePath);
            string error = "";
            if (fileExists)
            {
                try
                {
                    var fs = System.IO.File.OpenRead(filePath);
                    return File(fs, "application /vnd.ms-excel", template);
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
