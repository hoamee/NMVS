using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class AlcReportVm
    {
        public bool Unqualified { set; get; }

        public string Note { set; get; }

        public int OrdId { set; get; }

        public double Qty { set; get; }
    }
}
