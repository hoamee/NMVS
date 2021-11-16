using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IExcelService
    {
        public Task<CommonResponse<string>> GetReceiptNote(int icId, string user);

        public Task<CommonResponse<string>> GetshipperNote(int shpId, string user);

        public Task<CommonResponse<string>> GetIssueNoteSo(int shpId, string user, int noteId, int sot);
        public Task<CommonResponse<string>> GetIssueNoteMFG(int shpId, string user);

    }
}
