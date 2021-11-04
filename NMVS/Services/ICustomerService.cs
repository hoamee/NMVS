using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface ICustomerService
    {
        public List<CustomerVm> GetCustomerList();

        public Task<CommonResponse<Customer>> AddCustomer (CustomerVm customer);

        public Task<CommonResponse<Customer>> UpdateCustomer(Customer customer);

        public Customer GetCustomer(string customer);

        public Task<CommonResponse<UploadReport>> ImportCustomer(string filepath, string fileName, string user);

    }
}
