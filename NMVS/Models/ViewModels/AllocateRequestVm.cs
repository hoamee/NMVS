using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class AllocateRequestVm
    {
        public int AlcId { set; get; }
        public double AlcQty { set; get; }
        public string AlcFrom { set; get; }
        public string AlcTo { set; get; }
        public DateTime MovementTime { set; get; }
        public string Note { set; get; }
        public bool? IsClosed { set; get; }
        public string Item { set; get; }
        public int PtId { set; get; }
    }
}
