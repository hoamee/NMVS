using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class IssueNoteShipperVm
    {
        public Shipper Shp { set; get; }
        public List<ShipperDet> Det { set; get; }
    }
}
