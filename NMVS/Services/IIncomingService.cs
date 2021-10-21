
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IIncomingService
    {
        public List<IncomingListVm> BrowseIncomingList(bool closed);

        public IcmListVm GetListDetail(int? id);

        public List<TypeVm> GetSupplier();

        public Task<ExcelRespone> ImportList(string filepath, string user);
    }
}
