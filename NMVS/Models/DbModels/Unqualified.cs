using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class Unqualified
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UqId { set; get; }

        public string ItemNo { set; get; }

        public int PtId { set; get; }
        public string SoNbr { set; get; }

        public double Quantity { set; get; }

        public double RecycleQty { set; get; }

        public double DisposedQty { set; get; }

        public string Description { set; get; }

        public string Note { set; get; }

    }
}
