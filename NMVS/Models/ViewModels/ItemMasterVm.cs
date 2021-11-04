using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class ItemMasterVm
    {
        public int Ptid { set; get; }
        public string No { set; get; }
        public string Name { set; get; }
        [DataType(DataType.Date)]
        public DateTime DateIn { set; get; }
        public string Loc { set; get; }
        public double Qty { set; get; }
        public double Booked { set; get; }
        public string PackingType { set; get; }
        public string Sup { set; get; }
        public string Lot { set; get; }
        public string RcvBy { set; get; }
        public string Um { set; get; }
        public string Ref { set; get; }
        public DateTime? RefDate { set; get; }
        public string Note { set; get; }

        public string Po { set; get; }

    }
}
