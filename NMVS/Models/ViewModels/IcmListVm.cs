using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class IcmListVm
    {
        public IncomingListVm Icm { set; get; }

        public List<PtVm> Pt { set; get; }
    }

    public class PtVm
    {
        public int PtId { set; get; }
        public string ItemNo { set; get; }
        public string ItemName { set; get; }
        public double RcvQty { set; get; }
        public double AcceptQty { set; get; }
        public bool? IsChecked { set; get; }
        public string CheckedBy { set; get; }
        public string Note { set; get; }
    }
}
