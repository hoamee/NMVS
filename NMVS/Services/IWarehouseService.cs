using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IWarehouseService
    {
        public List<SiteVm> GetSite();

        public List<TypeVm> GetWhType();
    }
}
