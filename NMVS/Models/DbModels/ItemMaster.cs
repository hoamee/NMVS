using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class ItemMaster
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PtId { set; get; }


        [StringLength(50)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }

        [StringLength(100)]
        [Display(Name = "Lot No.")]
        public string PtLotNo { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public double PtQty { get; set; }

        [Display(Name = "QC accepted")]
        public double Accepted { get; set; }

        [Display(Name = "Checked by")]
        public string Qc { get; set; }

        [Display(Name = "Holding")]
        public double PtHold { get; set; }

        [Display(Name = "Loc.")]
        public string LocCode { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date in")]
        public DateTime PtDateIn { get; set; }

        [Display(Name = "Supplier")]
        public string SupCode { get; set; }
        public virtual Supplier Supplier { set; get; }

        [Display(Name = "Received quantity")]
        public double RecQty { get; set; }

        [Display(Name = "Received by")]
        public string RecBy { set; get; }

        [Display(Name = "Supplier Ref No")]
        public string RefNo { set; get; }

        [Display(Name = "Supplier Ref Date")]
        public DateTime? RefDate { set; get; }

        public int IcId { set; get; }
        public virtual IncomingList Ic { set; get; }

        public string BatchNo { set; get; }

        [Display(Name = "Comment")]
        public string PtCmt { get; set; }

        public bool? IsRecycled { set; get; }

        public bool? Passed { set; get; }

        public DateTime? RecycleDate { set; get; }

        public int? UnqualifiedId { set; get; }

        public int ParentId { set; get; }

        public string MovementNote { set; get; }
    }
}
