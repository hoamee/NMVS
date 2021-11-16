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

}
