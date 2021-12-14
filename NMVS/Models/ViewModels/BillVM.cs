using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class BillVM
    {
        public string Product{ set; get; }
        public int WoBillNbr { set; get; }

        public string WoNbr { set; get; }
        
        public double OrdQty { set; get; }

        
        public double ComQty { set; get; }

        
        public string Assignee { set; get; }

        
        public string Reporter { set; get; }

        
        public DateTime? LastUpdate { set; get; }

        [DataType(DataType.Date)]
        public DateTime OrdDate { set; get; }

        [DataType(DataType.Date)]
        public DateTime DueDate { set; get; }

        
        public bool IsClosed { set; get; }

        public bool? WoClosed { set; get; }
    }
}
