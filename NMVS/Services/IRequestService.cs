using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IRequestService
    {
        public List<InvRequestVm> GetRequestList();

        public RequestDetailVm GetRequestDetail(string id);

        public List<ItemAvailVm> GetItemAvails(string id);

        public List<ItemMasterVm> GetItemMasterVms(RequestDet rq);

        public Task<CommonResponse<int>> CloseShipperNote(MfgIssueNote issueNote, string user);

        //public Task<CommonResponse<UploadReport>> ImportList(string filepath, string fileName, string user);
    }
}
