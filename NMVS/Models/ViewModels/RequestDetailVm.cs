using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class RequestDetailVm
    {
        public InvRequestVm Rq { set; get; }

        public List<RequestDet> Dets { set; get; }
    }
}
