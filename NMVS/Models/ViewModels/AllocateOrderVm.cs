using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class AllocateOrderVm
    {
        public int RequestId { set; get; }
        public int OrderId { set; get; }
        public string ItemNo { set; get; }
        public int PtId { set; get; }
        public double Qty { set; get; }
        public double Moved { set; get; }
        public string AlcFrom { set; get; }
        public string AlcToCode { set; get; }
        public string AlcToDesc { set; get; }
        public DateTime MvmTime { set; get; }
        public bool? Confirmed { set; get; }
        public string ConfirmBy { set; get; }
    }
}
