using NMVS.Models;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class LocService : ILocService
    {
        ApplicationDbContext _db;

        public LocService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<TypeVm> GetLocTypes()
        {
            var locTypes = (from gen in _db.GeneralizedCodes
                            where gen.CodeFldName == "LocType"
                            select new TypeVm
                            {
                                Code = gen.CodeValue,
                                Desc = gen.CodeDesc
                            }).ToList();

            return locTypes;
        }

        public List<TypeVm> GetWhList(string wp)
        {
            var whs = (from wh in _db.Warehouses
                       where wh.WhStatus == true
                       && wh.SiCode == wp
                            select new TypeVm
                            {
                                Code = wh.WhCode,
                                Desc = wh.WhDesc
                            }).ToList();

            return whs;
        }
    }
}
