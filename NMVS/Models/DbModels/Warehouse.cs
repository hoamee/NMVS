using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class Warehouse
    {
        [Key]
        [Required(ErrorMessage = "Warehouse code must be 2 ~ 10 char or digit")]
        [StringLength(50)]
        [Display(Name = "Warehouse code")]
        public string WhCode { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100)]
        [Display(Name = "Description")]
        public string WhDesc { get; set; }

        [Display(Name = "Active")]
        public bool WhStatus { get; set; }

        [StringLength(200)]
        [Display(Name = "Comment")]
        public string WhCmmt { get; set; }

        //Foreign key for Site
        [Display(Name = "Site")]
        public string SiCode { get; set; }
        public virtual Site Site { get; set; }

        //Foreign key for Site
        [Display(Name = "Type")]
        public string Type { get; set; }
        //public virtual GeneralizedCode Generalized { get; set; }
        //public virtual ICollection<WarehouseLoc> WarehouseLocs { set; get; }
    }
}
