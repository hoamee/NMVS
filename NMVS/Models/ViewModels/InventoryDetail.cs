using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class InventoryDetail
    {
        public ItemMasterVm ItemMasterVm { set; get; }
        public List<InventoryTransac> Transacs { set; get; }
    }
}
