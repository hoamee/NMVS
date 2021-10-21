using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class IssueOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExpOrdId { set; get; }

        [Display(Name = "Issue Type")]
        public string IssueType { set; get; }

        [Display(Name = "Quantity")]
        public double ExpOrdQty { set; get; }

        [Display(Name = "Item Location")]
        public string WhlCode { get; set; }
        public virtual Loc Loc { get; set; }

        public int PtId { set; get; }

        [Display(Name = "To Location")]
        public string ToLoc { get; set; }

        [Display(Name = "To Location")]
        public string ToLocDesc { get; set; }

        [Display(Name = "Issued")]
        public bool? Confirm { set; get; }

        [Display(Name = "Confirmed by")]
        public string ConfirmedBy { set; get; }

        [Display(Name = "Request ID")]
        public string RqID { set; get; }
        public virtual InvRequest InvRequest { set; get; }

        [Display(Name = "Ordered by")]
        public string OrderBy { set; get; }

        [Required]
        [Display(Name = "Movement time")]
        public DateTime? MovementTime { get; set; }
    }
}
