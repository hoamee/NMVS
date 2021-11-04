using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class Spn01Vm
    {
        public string CustomerName { set; get; }
        public string ItemName { set; get; }
        public double PkgQty { set; get; }
        public string PkgType { set; get; }
        public double Qty { set; get; }
        public double Total { set; get; }
        public string Batch { set; get; }
        public string Note { set; get; }

        public string So { set; get; }
    }
}
