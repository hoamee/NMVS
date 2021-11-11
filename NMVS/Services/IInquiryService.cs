using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IInquiryService
    {
        public List<WarehouseVm> GetWarehouseData();

        public WarehouseDetailVm GetWarehouseDetail(string id);

        public LocDetailVm GetItemByLoc(string id);

        public List<MovementVm> GetMovementReport();
    }
}
