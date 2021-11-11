using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class WarehouseVm
    {
        public string WhCode { set; get; }
        public string WhName { set; get; }
        public string Site { set; get; }
        public double Remain { set; get; }
        public double Cap { set; get; }
        public double Used { set; get; }
        public double Hold { set; get; }
        public double OutGo { set; get; }
        public bool Status { set; get; }
        
    }


    public class WarehouseDetailVm
    {
        public WarehouseVm Wh { set; get; }

        public List<WarehouseVm> Locs { set; get; }
    }

    public class LocDetailVm
    {
        public WarehouseVm Loc { set; get; }

        public List<ItemMasterVm> Inv { set; get; }
    }
}
