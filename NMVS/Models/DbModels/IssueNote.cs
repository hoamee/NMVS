using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class IssueNote
    {
        public int Id { set; get; }
        public string Item { set; get; }
        public string Loc { set; get; }
        public DateTime? DateIn { set; get; }
        public double Quantity { set; get; }
    }
}
