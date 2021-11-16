using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class IssueNoteVm
    {
        public int Id { set; get; }
        public int NoteType { set; get; }
        public string SearchId { set; get; }
        public string SoNbr { set; get; }
        public string SoldTo { set; get; }
        public string ShipTo { set; get; }
        public string IssuedBy { set; get; }
        public DateTime IssuedOn { set; get; }
        public string Vehicle { set; get; }
        public string DriverInfo { set; get; }
        public int ShipperId { set; get; }
    }
}
