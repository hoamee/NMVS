using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models.ViewModels
{
    public class IssueToVehicleDto
    {
        public int Id { get; set; }
        public string Date { set; get; }
        public string Desc { set; get; }
        public int Total { set; get; }
        public int Completed { set; get; }
        public bool Status { get; set; }

    }
}