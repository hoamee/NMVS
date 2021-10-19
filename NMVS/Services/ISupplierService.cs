using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface ISupplierService
    {
        public List<Supplier> GetSupplierList();

        public Task<CommonResponse<Supplier>> AddSupplier(Supplier supplier);

        public Task<CommonResponse<Supplier>> UpdateSupplier(Supplier supplier);

        public Supplier GetSupplier(string supplier);

        public Task<ExcelRespone> ImportSupplier(string fileName);

    }
}
