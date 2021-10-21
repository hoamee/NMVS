using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class ItemMasterVm
    {
        public int Ptid { set; get; }
        public string No { set; get; }
        public string Name { set; get; }
        public DateTime DateIn { set; get; }
        public string Loc { set; get; }
        public double Qty { set; get; }
        public double Booked { set; get; }
    }
}
