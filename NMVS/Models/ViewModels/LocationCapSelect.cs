using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class LocationCapSelect
    {

        public string LcpId { set; get; }

        public int Ptid { set; get; }

        public string LcName { set; get; }
        public Double? Holding { get; set; }
        public Double? RemainCapacity { get; set; }
        public string Packing { get; set; }

        public bool Framable { set; get; }
    }
}
