using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class Shipper
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Shipper ID")]
        public int ShpId { set; get; }

        [Required]
        [Display(Name = "Description")]
        public string ShpDesc { set; get; }

        [Required]
        [Display(Name = "Driver")]
        public string Driver { set; get; }

        [Display(Name = "Driver contact")]
        public string DrContact { set; get; }


        [Required]
        [Display(Name = "Ship To")]
        public string ShpTo { set; get; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public string ShpVia { set; get; }

        [Required]
        [Display(Name = "Date In")]
        [DataType(DataType.Date)]
        public DateTime DateIn { set; get; }

        [Display(Name = "Checked In")]
        public DateTime? ActualIn { set; get; }

        [Display(Name = "Checked out")]
        public DateTime? ActualOut { set; get; }

        [Display(Name = "Checked in by")]
        public string CheckInBy { set; get; }

        [Display(Name = "Checked out by")]
        public string CheckOutBy { set; get; }

        [Display(Name = "Loading port")]
        [Required]
        public string Loc { set; get; }

        [Display(Name = "Registered by")]
        public string RegisteredBy { set; get; }
    }

    public class ShipperDet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SpDetId { set; get; }

        [Display(Name = "Shipper")]
        public int ShpId { set; get; }

        public int DetId { set; get; }

        public string RqId { set; get; }

        [Display(Name = "Item No.")]
        public string ItemNo { set; get; }

        [Display(Name = "Item")]
        public string ItemName { set; get; }

        [Display(Name = "Quantity")]
        public double Quantity { set; get; }

        public int InventoryId { set; get; }
    }
}
