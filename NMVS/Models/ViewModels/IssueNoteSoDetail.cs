using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class IssueNoteSoDetail
    {
        public IssueNoteVm Isn { set; get; }
        public List<ShipperDet> Dets { set; get; }
    }
}
