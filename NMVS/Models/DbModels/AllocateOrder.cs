using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class AllocateOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlcOrdId { set; get; }

        //Foreign key to item master
        [Display(Name = "Item No.")]
        public int PtId { set; get; }

        [Display(Name = "Quantity")]
        public double AlcOrdQty { set; get; }

        [Display(Name = "Moved quantity")]
        public double MovedQty { set; get; }

        [Display(Name = "Reported quantity")]
        public double Reported { set; get; }

        [Display(Name = "Allocate From")]
        public string AlcOrdFrom { get; set; }

        public string AlcOrdFDesc { get; set; }

        [Display(Name = "Allocate to")]
        public string LocCode { get; set; }
        public virtual Loc Loc { get; set; }

        [Display(Name = "Allocated")]
        public bool? Confirm { set; get; }

        [Display(Name = "Confirmed by")]
        public string ConfirmedBy { set; get; }

        [Display(Name = "Request ID")]
        public int RequestID { set; get; }

        [Display(Name = "Ordered by")]
        public string OrderBy { set; get; }


        [Display(Name = "Movement time")]
        public DateTime MovementTime { set; get; }

        [Display(Name = "Completed time")]
        public DateTime? CompletedTime { get; set; }

        public string MovementNote { set; get; }
    }
}
