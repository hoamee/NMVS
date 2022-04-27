using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Common
{
    public class Helper
    {
        public const string UserManagement = "Manage user";

        public const string RequestInv = "Request inventory";

        public const string HandleRequest = "Handle request";

        public const string ReceiveInv = "Receive inventory";

        public const string QC = "QC";

        public const string CreateSO = "Create sales order";

        public const string AppSO = "Approve SO";

        public const string RegVehicle = "Register vehicle";

        public const string Guard = "Guard";

        public const string MoveInventory = "Move inventory";

        public const string ArrangeInventory = "Arrange inventory";

        public const string SupperUser = "SupperUser";

        public const string WoCreation = "WoCreation";

        public const string WoReporter = "WoReporter";

        public const string IssueUnplanned = "IssueUnplanned";

        public string GetItemHistory(string from, string to, int lastId, int newId, bool isAllocate, int orderNo)
        {
            string s = " **" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Moved from " + from + " to " + to + "; " +
                "Last ID - New ID: " + lastId + " - " + 
                (isAllocate ? newId + "; " + "Allocate order No.:" : "Issued; Issue order No.: ") + orderNo;

            return s;
        }

        public string GetItemHistory(string from, int newId, bool isAllocate, int orderNo)
        {
            string s = " **" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Moved from " + from + " to UQ location; " +
                "Unqualified Id: " + newId + "; " +
                (isAllocate ? "Allocate order No.:" : "Issue order No.: ") + orderNo;

            return s;
        }
    }
}
