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

        public IActionResult SalesOrder ()
        {
            return View();
        }

        public IActionResult ProcessRequest() => View();

        public IActionResult Vehicle() => View();

        public IActionResult DownloadTemplate(string id)
        {
            var filePath = "null";
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
            } else if (id == "request")
            {
                template = "Inventory request upload template.xlsx";
                filePath = "template/IRQM01_Inventory request (MFG).xlsx";
            }
            else if (id == "so")
            {
                template = "Sales order upload template.xlsx";
                filePath = "template/SO01_Sales order.xlsx";
            }


            var fileExists = System.IO.File.Exists(filePath);
            string error;
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
            else
            {
                error = "Incorrect access";
            }

            return RedirectToAction("Error", "Home", error);


        }
    }
}
