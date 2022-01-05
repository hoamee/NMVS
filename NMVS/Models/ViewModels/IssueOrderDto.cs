using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class IssueOrderDto
    {
        public string Item { get; set; }
        public int InventoryId { get; set; }
        public string From { get; set; }
        public string FromCode { get; set; }
        public string To { get; set; }
        public string ToCode { get; set; }
        public string Time { get; set; }
        public double Quantity { get; set; }
        public double Moved { get; set; }
        public double Reported { get; set; }
        public string RequestNo { get; set; }
        public string OrderBy { get; set; }
        public int IssueOrderId { get; set; }
        public bool? Confirmed { get; set; }
        public string ConfirmedBy { get; set; }
    }
}