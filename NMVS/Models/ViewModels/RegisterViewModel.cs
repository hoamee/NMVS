using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Full name")]
        [StringLength(100, ErrorMessage = "Max length for full name is 100 char or digit")]
        public string FullName { set; get; }

        [Required]
        [Display(Name = "Account")]
        [RegularExpression(@"^\S*$", ErrorMessage = "No space allowed")]
        public string Account { set; get; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(50)]
        public string Password { set; get; }

        [Required]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [StringLength(50)]
        [Compare("Password", ErrorMessage = "Password is not match")]
        public string ConfirmPassword { set; get; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Email { set; get; }
    }
}
