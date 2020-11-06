using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class DetailReportEt
    {
        public int id { get; set; }
        public DateTime Workdate { get; set; }
        public string ModelName { get; set; }
        public string Station { get; set; }
        public string ErrorCode { get; set; }
        public string PcName { get; set; }
        public string Owner { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string RootCause { get; set; }

        public string WorkSection { get; set; }
        public int Week { get; set; }
        public DateTime? Begin { get; set; }
        public string Typt { get; set; }
        public string ProblemDes { get; set; }
        public string report { get; set; }
    }
}