using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class CommonResponse<T>
    {
        public int status { set; get; }
        public string message { set; get; }
        public T dataenum { set; get; }
    }
}
