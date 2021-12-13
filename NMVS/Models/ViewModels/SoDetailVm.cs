using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class SoDetailVm
    {
        public SoVm SoVm { set; get; }

        public List<SoDetVm> SoDets { set; get; }
    }

    public class SoDetVm{
        public int SodId { set; get; }
        public string SoNbr { get; set; }
        public SalesOrder SalesOrder { set; get; }
        public string ItemNo { set; get; }

        public string ItemName { set; get; }

        public DateTime? SpecDate { get; set; }
        public DateTime? RequiredDate { get; set; }

        public double Quantity { set; get; }

        public double Discount { get; set; }

        public double NetPrice { get; set; }

        public double Tax { get; set; }

        public double Shipped { get; set; }

        public int? RqDetId { set; get; }
    }
}
