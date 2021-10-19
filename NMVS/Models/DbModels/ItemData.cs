using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class ItemData
    {
        [Key]
        [Required(ErrorMessage = "Item Number should be 2 ~ 50 char or digit")]
        [StringLength(50)]
        [Display(Name = "Item No.")]
        public string ItemNo { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Description")]
        public string ItemName { get; set; }


        [StringLength(20)]
        [Display(Name = "Item type")]
        public string ItemType { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "UM")]
        public string ItemUm { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Packing")]
        public string ItemPkg { get; set; }

        [Required]
        [Display(Name = "Quantity/Pack")]
        public double ItemPkgQty { get; set; }

        [Display(Name = "Flammable")]
        public bool Flammable { set; get; }

        
        [Display(Name = "Warehouse Unit (per 1 pack)")]
        [Required]
        public double ItemWhUnit { set; get; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

    }
}
