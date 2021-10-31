using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class MfgNoteVm
    {
        public MfgIssueNote MfgIssueNote { set; get; }

        public List<MfgIssueNoteDet> Det { set; get; }
    }
}
