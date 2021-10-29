using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class IssueOrderVm
    {
        
        
        public int ExpOrdId { set; get; }

        [Display(Name = "Issue Type")]
        public string IssueType { set; get; }

        [Display(Name = "Quantity")]
        public double ExpOrdQty { set; get; }

        [Display(Name = "Moved quantity")]
        public double MovedQty { set; get; }

        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }

        [Display(Name = "Item Location")]
        public string LocCode { get; set; }

        public int PtId { set; get; }

        [Display(Name = "To Location")]
        public string ToLoc { get; set; }

        [Display(Name = "To vehiclde")]
        public int? ToVehicle { set; get; }

        [Display(Name = "Issue To")]
        public string IssueToDesc { get; set; }

        [Display(Name = "Issued")]
        public bool? Confirm { set; get; }

        [Display(Name = "Confirmed by")]
        public string ConfirmedBy { set; get; }

        [Display(Name = "Request ID")]
        public string RqID { set; get; }

        [Display(Name = "Request ID")]
        public int DetId { set; get; }


        [Display(Name = "Ordered by")]
        public string OrderBy { set; get; }

        
        [Display(Name = "Movement time")]
        public DateTime? MovementTime { get; set; }
    }
}
