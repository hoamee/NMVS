using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IAllocateService
    {
        public List<AllocateRequestVm> GetRequestList();

        public List<TypeVm> GetItemDistinc();

        public List<ItemMasterVm> GetAvailItem(string itemNo);
    }
}
