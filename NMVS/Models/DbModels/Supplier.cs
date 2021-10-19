using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class Supplier
    {
        [Key]
        [StringLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Supplier code")]
        public string SupCode { set; get; }

        [StringLength(100)]
        [Required]
        [Display(Name = "Supplier description")]
        public string SupDesc { set; get; }

        [StringLength(250)]
        [Required]
        [Display(Name = "Address")]
        public string Addr { set; get; }

        [StringLength(50)]
        [Display(Name = "City")]
        public string City { set; get; }

        [StringLength(50)]
        [Display(Name = "Country")]
        public string Ctry { set; get; }

        [StringLength(50)]
        [Display(Name = "Tax code")]
        public string TaxCode { set; get; }

        [StringLength(100)]
        [Display(Name = "Email 1")]
        [EmailAddress]
        public string Email1 { set; get; }

        [StringLength(100)]
        [Display(Name = "Email 2")]
        [EmailAddress]
        public string Email2 { set; get; }

        [StringLength(20)]
        [Display(Name = "Phone 1")]
        [Phone]
        public string Phone1 { set; get; }

        [StringLength(20)]
        [Display(Name = "Phone 2")]
        [Phone]
        public string Phone2 { set; get; }

        [Display(Name = "Note")]
        public string Note { set; get; }

        [Display(Name = "Active")]
        public bool Active { set; get; }
    }
}
