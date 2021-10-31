using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class MfgIssueNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IsNId { set; get; }

        public string RqId { set; get; }

        public DateTime? IssuedOn { set; get; }

        public string IssuedBy { set; get; }
    }

    public class MfgIssueNoteDet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        public int IsNId { set; get; }

        public string ItemNo { set; get; }

        public string ItemName { set; get; }

        public double Quantity { set; get; }

        public int PtId { set; get; }

    }
}
