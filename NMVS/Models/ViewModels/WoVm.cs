using System;
using System.ComponentModel.DataAnnotations;
using NMVS.Models.DbModels;

namespace NMVS.Models.ViewModels
{
    public class WoVm
    {
        [Display(Name = "Work Order")]
        public string WoNbr { set; get; }

        [Display(Name = "Item No.")]
        public string ItemNo { set; get; }

        [Display(Name = "Item Name")]
        public string ItemName { set; get; }
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
    }
}