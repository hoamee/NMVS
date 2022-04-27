using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class SalesOrder
    {
        [Display(Name = "SO Number")]
        [Required]
        [Key]
        public string SoNbr { get; set; }

        public int SoType { set; get; }

        [Required]
        [StringLength(50)]
        public string CustCode { set; get; }
        public virtual Customer Customer { set; get; }

        [Required]
        [Display(Name = "Ship to")]
        public string ShipTo { get; set; }

        [Required]
        [Display(Name = "Order date")]
        [DataType(DataType.Date)]
        public DateTime OrdDate { set; get; }

        [Display(Name = "Require date")]
        [DataType(DataType.Date)]
        public DateTime? ReqDate { set; get; }

        [Required]
        [Display(Name = "Due date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { set; get; }

        [Required]
        [Display(Name = "Pricing date")]
        [DataType(DataType.Date)]
        public DateTime PriceDate { set; get; }

        [Required]
        [Display(Name = "Currency")]
        public string SoCurr { set; get; }

        [Required]
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

        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { set; get; }

        public bool Warning { set; get; }
        public bool ReqReported { set; get; }
        public string ReqReportedNote { set; get; }

        public bool? RequestConfirmed { set; get; }
        public string ConfirmationNote { set; get; }
        public string RequestConfirmedBy { set; get; }
        public string ApprovalNote { set; get; }
        public bool Closed { set; get; }
        public bool Completed { get; set; }
        public ICollection<SoDetail> SoDets { set; get; }
    }

    public class SoDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SodId { set; get; }

        public string SoNbr { get; set; }
        public SalesOrder SalesOrder { set; get; }

        [Required]
        [Display(Name = "Item No.")]
        public string ItemNo { set; get; }


        [DataType(DataType.Date)]
        [Display(Name = "Date in")]
        public DateTime? SpecDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Required date")]
        public DateTime? RequiredDate { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public double Quantity { set; get; }

        [Required]
        [Display(Name = "Discount (%)")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Net Price")]
        public double NetPrice { get; set; }

        [Required]
        [Display(Name = "Tax (%)")]
        public double Tax { get; set; }

        [Display(Name = "Shipped")]
        public double Shipped { get; set; }

        public int? RqDetId { set; get; }
    }

}
