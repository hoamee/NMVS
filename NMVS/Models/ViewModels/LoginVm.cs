using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class LoginVm
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { set; get; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { set; get; }

        [Display(Name = "Work space")]
        public string Site { set; get; }
    }
}
