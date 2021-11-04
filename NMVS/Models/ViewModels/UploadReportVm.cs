using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class UploadReportVm

    {
        public UploadReport UploadReport { set; get; }
        public List<UploadError> UploadErrors { set; get; }
    }
}
