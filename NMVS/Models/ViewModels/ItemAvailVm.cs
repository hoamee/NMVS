using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class ItemAvailVm
    {
        public string ItemNo { set; get; }
        public string Desc { set; get; }
        public double Quantity { set; get; }
        public DateTime DateIn { set; get; }
        public string sDateIn { set; get; }
    }
}
