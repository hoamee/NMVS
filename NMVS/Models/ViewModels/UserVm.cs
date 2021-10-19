using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class UserVm
    {
        public string UserName { set; get; }
        public string FullName { set; get; }
        public string UserEmail { set; get; }
        public bool Active { set; get; }
    }
}
