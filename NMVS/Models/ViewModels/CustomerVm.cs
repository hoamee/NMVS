using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class CustomerVm
    {
        [StringLength(50)]
        [Required]
        [Display(Name = "Customer code")]
        public string CustCode { set; get; }

        [StringLength(50)]
        [Required]
        [Display(Name = "Company code AP")]
        public string ApCode { set; get; }

        [StringLength(3)]
        [Required]
        [Display(Name = "Agent No.")]
        public string AgentNo { set; get; }

        [StringLength(100)]
        [Required]
        [Display(Name = "Customer name")]
        public string CustName { set; get; }

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
