using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface ILocService
    {
        public List<TypeVm> GetLocTypes();

        public List<TypeVm> GetWhList();
    }
}
