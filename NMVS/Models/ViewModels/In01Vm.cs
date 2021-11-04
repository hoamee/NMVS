using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class In01Vm
    {
        public int SpDetId { set; get; }

        public int ShpId { set; get; }

        public int DetId { set; get; }

        public string RqId { set; get; }

        public string ShipToId { set; get; }

        public string ShipToName { set; get; }

        public string ShipToAddr { set; get; }
        public string ShipToTax { set; get; }

        public string SoldTo { set; get; }
        public string SoldToName { set; get; }
        public string SoltToAddr { set; get; }
        public string SoldToTax { set; get; }

        public string ItemNo { set; get; }

        public string ItemName { set; get; }

        public string ItemUnit { set; get; }
        public string PkgType { set; get; }
        public double PkgQty { set; get; }
        public string Um { set; get; }
        public double Quantity { set; get; }
        public string BatchNo { set; get; }
        public int InventoryId { set; get; }
    }
}
