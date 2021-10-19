using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class Site
    {
        [Key]
        [Required(ErrorMessage = "Site code must be 2 ~ 10 char or digit")]
        [StringLength(50)]
        [Display(Name = "Site code")]
        public string SiCode { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100)]
        [Display(Name = "Site name")]
        public string SiName { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [StringLength(250)]
        [Display(Name = "Comment")]
        public string SiCmmt { get; set; }

        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}
