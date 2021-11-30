using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public interface IUserDataService
    {
        public List<UserVm> GetUserList();

        public Task InitRoleAsync();

        public List<UserRoleVm> GetUserRoleList();

        public UserRoleVm GetUserRole(string userName);

        public Task SeedingRole(UserRoleVm usr);

        public Task<CommonResponse<string>> ResetPassword(string usrName);
    }
}
