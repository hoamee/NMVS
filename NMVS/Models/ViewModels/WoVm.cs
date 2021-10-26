using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class WoVm
    {
        public string WoNbr { set; get; }
        public string ItemNo { set; get; }
        public string ItemName { set; get; }
        public string PrdLine { set; get; }
        public double QtyOrd { set; get; }
        public double QtyCom { set; get; }
        public DateTime OrdDate { set; get; }
        public DateTime ExpDate { set; get; }
        public string OrdBy { set; get; }
    }
}
