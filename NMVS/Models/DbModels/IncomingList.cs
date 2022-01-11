using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class IncomingList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IcId { set; get; }

        [Display(Name = "Supplier")]
        public string SupCode { set; get; }
        public virtual Supplier Supplier { set; get; }

        [Display(Name = "PO no.")]
        public string Po { set; get; }

        [DataType(DataType.Date)]
        [Display(Name = "PO date")]
        public DateTime? PoDate { set; get; }

        [Display(Name = "Vehicle")]
        public string Vehicle { set; get; }

        [Display(Name = "Driver")]
        public string Driver { set; get; }

        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { set; get; }

        [Display(Name = "Warranty return")]
        public bool IsWarranty { set; get; }

        public bool IsRecycle { set; get; }

        public int? RecycleId { set; get; }

        public bool Closed { set; get; }

        public int ItemCount { set; get; }

        public int Checked { set; get; }

        public string LastModifiedBy { set; get; }

        public string PtNote { set; get; }

        public bool ConfirmReceived { set; get; }

        public virtual ICollection<ItemMaster> ItemMasters { set; get; }

        public string Site {set;get;}

    }
}
