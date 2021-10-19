using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class ExcelRespone
    {
        public string fileName { set; get; }
        public int processed { set; get; }
        public int updated { set; get; }
        public int imported { set; get; }
        public string error { set; get; }
    }
}
