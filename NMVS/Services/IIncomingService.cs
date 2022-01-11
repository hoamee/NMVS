
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IIncomingService
    {
        public List<IncomingListVm> BrowseIncomingList(bool closed, string wp);

        public IcmListVm GetListDetail(int? id);

        public List<TypeVm> GetSupplier();

        public Task<CommonResponse<UploadReport>> ImportList(string filepath, string fileName, string user, string wp);

        public Task<CommonResponse<UploadReport>> ImportWarranty(string filepath, string fileName, string user, string wp);
    }
}
