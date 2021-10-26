using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class InvRequestVm
    {
        public string Id { set; get; }

        public string RqType { set; get; }

        public DateTime Date { set; get; }

        public string Ref { set; get; }

        public string Note { set; get; }

        public string RqBy { set; get; }

        public bool SoConfirm { set; get; }
    }
}
