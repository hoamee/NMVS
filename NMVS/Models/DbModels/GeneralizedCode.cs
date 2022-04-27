using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NMVS.Models.DbModels
{
    public class GeneralizedCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CodeNo { set; get; }
        public string CodeFldName { set; get; }
        public string CodeValue { set; get; }
        public string CodeDesc { set; get; }

    }
}
