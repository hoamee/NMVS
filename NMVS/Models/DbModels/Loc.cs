using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class Loc
    {
        [Key]
        [Required(ErrorMessage = "Location code must be 2 ~ 10 char or digit")]
        [Display(Name = "Location code")]
        public string LocCode { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100)]
        [Display(Name = "Description")]
        public string LocDesc { get; set; }

        [Display(Name = "Active")]
        public bool LocStatus { get; set; }

        [StringLength(200)]
        [Display(Name = "Comment")]
        public string LocCmmt { get; set; }

        [Display(Name = "Location type")]
        public string LocType { get; set; }

        [Display(Name = "Location Capacity")]
        [Required]
        public double LocCap { get; set; }

        [Display(Name = "Remaining Capacity")]
        public double LocRemain { get; set; }

        [Display(Name = "Holding Capacity")]
        public double LocHolding { get; set; }

        [Display(Name = "Outgo")]
        public double LocOutgo { get; set; }

        [Display(Name = "Direct Load")]
        public bool Direct { set; get; }

        //Foreign key for Site
        [Display(Name = "Warehouse")]
        public string WhCode { get; set; }
        [Display(Name = "Flammable")]
        public bool Flammable { set; get; }
        
        public virtual Warehouse Warehouse { get; set; }
        //public virtual ICollection<Receipt> Receipts { set; get; }
    }
}
