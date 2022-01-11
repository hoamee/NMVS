using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class WorkOrder
    {
        [Key]
        [Display(Name = "Work Order")]
        public string WoNbr { set; get; }

        [Display(Name = "Item No.")]
        public string ItemNo { set; get; }
        public virtual ItemData ItemData { set; get; }

        [Display(Name = "Quantity ordered")]
        public double QtyOrd { set; get; }

        [Display(Name = "Quantity completed")]
        public double QtyCom { set; get; }

        [Display(Name = "Sales Order")]
        public string SoNbr { set; get; }

        [Display(Name = "Ordered By")]
        public string OrdBy { set; get; }

        [Display(Name = "Ordered date")]
        [DataType(DataType.Date)]
        public DateTime OrdDate { set; get; }

        [Display(Name = "Expire date")]
        [DataType(DataType.Date)]
        public DateTime ExpDate { set; get; }

        [Display(Name = "Prod Line")]
        public string PrLnId { set; get; }
        public ProdLine ProdLine { set; get; }

        public bool? Closed { set; get; }

        public string Site { set; get; }

    }

    public class WoBill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WoBillNbr { set; get; }

        public string WoNbr { set; get; }
        public WorkOrder WorkOrder { set; get; }

        [Display(Name = "Quantity ordered")]
        public double OrdQty { set; get; }

        [Display(Name = "Quantity completed")]
        public double ComQty { set; get; }

        [Display(Name = "Assigned to")]
        public string Assignee { set; get; }

        [Display(Name = "Update by")]
        public string Reporter { set; get; }

        [Display(Name = "Last Update")]
        public DateTime? LastUpdate { set; get; }

        [Display(Name = "Ordered date")]
        [DataType(DataType.Date)]
        public DateTime OrdDate { set; get; }

        [Display(Name = "Expire date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { set; get; }

        [Display(Name = "Closed")]
        public bool IsClosed { set; get; }
    }

    public class ProdLine
    {
        [Key]
        [Display(Name = "Product line")]
        public string PrLnId { set; get; }

        [Display(Name = "Site")]
        public string SiCode { set; get; }
        public Site Site { set; get; }

        [Display(Name = "Active")]
        public bool Active { set; get; }

    }
}
