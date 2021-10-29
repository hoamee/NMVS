using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface ISoService
    {
        public SoDetailVm GetSoDetail(string soNbr);

        public List<SoVm> GetBrowseData();

        public List<ItemAvailVm> GetItemNAvail();
    }
}
