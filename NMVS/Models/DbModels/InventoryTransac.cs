using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class InventoryTransac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }
        public DateTime MovementTime { set; get; }
        public string From { set; get; }
        public string To { set; get; }
        public int LastId { set; get; }
        public int? NewId { set; get; }
        public bool IsDisposed { set; get; }
        public int OrderNo { set; get; }
        public bool IsAllocate { set; get; }
    }
}
