﻿using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IRequestService
    {
        public List<InvRequestVm> GetSoRequests(string wp, bool closed);

        public List<InvRequestVm> GetMfgRequests(string wp, bool closed);

        public RequestDetailVm GetRequestDetail(string id);

        public List<ItemAvailVm> GetItemAvails(string id, string wp);

        public List<ItemMasterVm> GetItemMasterVms(RequestDet rq);

        public List<IssueNoteVm> GetListIssueNote();
        public IssueNoteSoDetail GetIssueNoteDetail(int id, int sot);
        
        public IssueNoteShipperVm GetVehicleNoteDetail(int id);

        public Task<CommonResponse<UploadReport>> ImportList(string filepath, string fileName, string user, string wp);
        public Task<CommonResponse<int>> CloseShipperNote(MfgIssueNote issueNote, string user);

        //public Task<CommonResponse<UploadReport>> ImportList(string filepath, string fileName, string user);
    }
}
