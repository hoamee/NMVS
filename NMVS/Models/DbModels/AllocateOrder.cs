﻿using System;
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
        public virtual ItemMaster ItemMaster { set; get; }

        [Display(Name = "Quantity")]
        public double AlcOrdQty { set; get; }

        [Display(Name = "Allocate From")]
        public string AlcOrdFrom { get; set; }

        public string AlcOrdFDesc { get; set; }

        [Display(Name = "Allocate to")]
        public string LocCode { get; set; }
        public virtual Loc Loc { get; set; }

        [Display(Name = "Allocated")]
        public bool Confirm { set; get; }

        [Display(Name = "Confirmed by")]
        public string ConfirmedBy { set; get; }

        [Display(Name = "Request ID")]
        public int RequestID { set; get; }
        public virtual AllocateRequest AllocateRequest { set; get; }

        [Display(Name = "Ordered by")]
        public string OrderBy { set; get; }

        [Display(Name = "Order type")]
        public string CodeValue { get; set; }
        public virtual GeneralizedCode GeneralizedCode { get; set; }

        [Display(Name = "Movement time")]
        public DateTime MovementTime { set; get; }
    }
}
