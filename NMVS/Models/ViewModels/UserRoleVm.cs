using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class UserRoleVm
    {
        public string UserName { set; get; }

        public bool UserManagement { set; get; }

        public bool RequestInv { set; get; }

        public bool HandleRequest { set; get; }

        public bool ReceiveInv { set; get; }

        public bool QC { set; get; }

        public bool CreateSO { set; get; }

        public bool AppSO { set; get; }

        public bool RegVehicle { set; get; }

        public bool Guard { set; get; }

        public bool Active { set; get; }

        public bool ArrangeInventory { set; get; }
        public bool MoveInv { set; get; }
    }
}
