using NMVS.Models;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class WarehouseService : IWarehouseService
    {
        ApplicationDbContext _db;

        public WarehouseService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<SiteVm> GetSite()
        {
            var siteList = (from site in _db.Sites
                            where site.Active == true
                            select new SiteVm
                            {
                                SiCode = site.SiCode,
                                SiDesc = site.SiName
                            }).ToList();

            return siteList;
        }

        public List<TypeVm> GetWhType()
        {
            var whTypes = (from gen in _db.GeneralizedCodes
                           where gen.CodeFldName == "WarehouseType"
                           select new TypeVm
                           {
                               Code = gen.CodeValue,
                               Desc = gen.CodeDesc
                           }).ToList();

            return whTypes;
        }
    }
}
