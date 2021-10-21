using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class PickingData
    {
        public List<JsPickingData> jsArr { set; get; }
        public string loc { set; get; }
        public DateTime reqTime { set; get; }
    }


    public class JsPickingData
    {
        public string whcd { set; get; }
        public double qty { set; get; }
        public int id { set; get; }
        public string loc { set; get; }
        public DateTime reqTime { set; get; }
    }
}
