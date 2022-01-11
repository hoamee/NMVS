using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class AllocateRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AlcId { set; get; }

        //Foreign key to item master
        [Display(Name = "Item master No.")]
        public int PtId { set; get; }
        public virtual ItemMaster ItemMaster { set; get; }

        [Display(Name = "Quantity")]
        public double AlcQty { set; get; }

        [Display(Name = "Reported quantity")]
        public double Reported { set; get; }

        [StringLength(50)]
        [Display(Name = "Allocate From")]
        public string AlcFrom { get; set; }
        public string AlcFromDesc { get; set; }

        [StringLength(50)]
        [Display(Name = "Allocate to")]
        public string LocCode { get; set; }
        public virtual Loc Loc { get; set; }

        [Display(Name = "Report")]
        public string AlcCmmt { get; set; }

        [Display(Name = "Status")]
        public bool? IsClosed { set; get; }
        [Display(Name = "Movement time")]
        public DateTime MovementTime { set; get; }
        [Display(Name = "Completed time")]
        public DateTime? CompletedTime { set; get; }
        public string Site { set; get; }
    }
}
