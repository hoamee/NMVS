using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class SoVm
    {
        [Display(Name = "SO Number")]
        public string SoNbr { get; set; }

        public string SoType { set; get; }

        public bool Closed { set; get; }

        public string CustCode { set; get; }

        [Display(Name = "Ship to")]
        public string ShipTo { get; set; }

        [Display(Name = "Order date")]
        [DataType(DataType.Date)]
        public DateTime OrdDate { set; get; }

        [Display(Name = "Require date")]
        [DataType(DataType.Date)]
        public DateTime? ReqDate { set; get; }

        [Display(Name = "Due date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { set; get; }

        [Display(Name = "Pricing date")]
        [DataType(DataType.Date)]
        public DateTime PriceDate { set; get; }

        [Display(Name = "Currency")]
        public string SoCurr { set; get; }

        [Display(Name = "Ship via")]
        public string ShipVia { set; get; }

        [Display(Name = "Note")]
        public string Comment { set; get; }

        [Display(Name = "Confirmed")]
        public bool? Confirm { set; get; }

        [Display(Name = "Confirmed by")]
        public string ConfirmBy { set; get; }

        [Display(Name = "Updated by")]
        public string UpdatedBy { set; get; }

        public bool? WhConfirmed { set; get; }
        public string WhConfirmedBy { set; get; }
        public string ConfirmationNote { set; get; }

        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { set; get; }

    }
}