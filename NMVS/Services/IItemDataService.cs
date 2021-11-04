using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IItemDataService
    {
        public Task<CommonResponse<UploadReport>> ImportItemData(string filepath, string fileName, string user);

        public List<ItemData> GetItemList();
    }
}
