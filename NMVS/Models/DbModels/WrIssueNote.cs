using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class WrIssueNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InId { set; get; }
        public string SoNbr { set; get; }
        public DateTime IssuedOn { set; get; }
        public string IssuedBy { set; get; }
        public string SoldTo { set; get; }
        public string ShipTo { set; get; }
        public int Shipper { set; get; }

    }

    public class WrIssueNoteDet
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IndID { set; get; }
        public string ItemNo { set; get; }
        public int PtId { set; get; }
        public int PackCount { set; get; }
        public double Quantity { set; get; }
        public int InId { set; get; }
    }
}
