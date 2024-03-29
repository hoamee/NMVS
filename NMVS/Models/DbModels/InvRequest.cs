﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class InvRequest
    {
        [Key]
        [Display(Name = "Request No.")]
        public string RqID { get; set; }

        [Display(Name = "Request Type")]
        public string RqType { get; set; }


        [Display(Name = "Request by")]
        public string RqBy { get; set; }

        [Display(Name = "Date request")]
        public DateTime RqDate { get; set; }

        [Required]
        public string Ref { set; get; }

        public bool SoConfirm { set; get; }

        [Display(Name = "Note")]
        public string RqCmt { get; set; }

        public bool? Confirmed { set; get; }

        public string ConfirmedBy { set; get; }

        public string ConfirmationNote { set; get; }

        public bool Reported { set; get; }
        public string ReportedNote { set; get; }

        public bool Closed { set; get; }

        public string Site { set; get; }
    }

    public class RequestDet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetId { set; get; }

        [Required(ErrorMessage = "Item Number should be 2 ~ 10 char or digit")]
        [StringLength(50)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }

        public string ItemName { get; set; }

        [Display(Name = "Quantity")]
        public double Quantity { get; set; }

        [Display(Name = "in Pick list")]
        public double Picked { set; get; }

        [Display(Name = "Ready")]
        public double Ready { set; get; }

        [Display(Name = "Closed")]
        public bool? Closed { set; get; }

        [Display(Name = "Issued")]
        public double? Issued { get; set; }

        [Required]
        [Display(Name = "Date required")]
        public DateTime? RequireDate { get; set; }

        public double Arranged { set; get; }

        public bool? Shipped { set; get; }

        [Display(Name = "Report")]
        public string Report { set; get; }

        [Display(Name = "Issue Date")] public DateTime? IssueOn { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date in")]
        public DateTime? SpecDate { set; get; }

        public string MovementNote { set; get; }

        [Display(Name = "Request No.")]
        public string RqID { get; set; }

        public int? SodId { set; get; }

        public string Site { set; get; }

        
    }
}
