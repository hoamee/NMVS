using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IAllocateService
    {
        public List<AllocateRequestVm> GetRequestList(string wp);

        public List<TypeVm> GetItemDistinc();

        public List<ItemMasterVm> GetAvailItem(string locCode, string wp);

        public List<AllocateOrderVm> GetAllocateOrders(string wp);

        public List<AllocateRequestVm> GetItemAllocateHistory(int ptId);

        public List<ItemMasterVm> GetUnAllocated(string wp);

        public CommonResponse<int> ConfirmSelectLoc(List<JsPickingData> jsArr, string wp);
    }
}
