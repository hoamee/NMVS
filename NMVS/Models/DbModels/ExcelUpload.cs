using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.DbModels
{
    public class UploadReport
    {
        [Key]
        public string UploadId { set; get; }

        public DateTime UploadTime { set; get; }

        public string UploadBy { set; get; }

        public string FileName { set; get; }

        public string UploadFunction { set; get; }

        public int TotalRecord { set; get; }

        public int Inserted { set; get; }

        public int Updated { set; get; }

        public int Errors { set; get; }

    }

    public class UploadError
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        public string UploadId { set; get; }

        public string Error { set; get; }
    }
}
