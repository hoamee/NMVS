using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class UnqualifiedTransac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        public int UnqId { set; get; }

        public double Qty { set; get; }

        public bool IsDisposed { set; get; }

        public string Note { set; get; }

        public DateTime TransantionTime { set; get; }

        public string ByUser { set; get; }
    }
}
