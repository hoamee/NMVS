using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class SoDetailVm
    {
        public SoVm SoVm { set; get; }

        public List<SoDetail> SoDets { set; get; }
    }
}
