using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class MovementVm
    {
        public int Id { set; get; }
        public int PtId { set; get; }
        public string ItemNo { set; get; }
        public string ItemName { set; get; }
        public string From { set; get; }
        public string To { set; get; }
        public double OrderQty { set; get; }
        public string OrderBy { set; get; }
        public double CompletedQty { set; get; }
        public double ReportedQty { set; get; }
        public string MoveBy { set; get; }
        public string RequestNo { set; get; }
        public string OrderType { set; get; }
        public DateTime MovementTime { set; get; }
        public DateTime? CompletedTime { set; get; }
        public string Note { set; get; }
        public bool? Completed { set; get; }
    }
}
