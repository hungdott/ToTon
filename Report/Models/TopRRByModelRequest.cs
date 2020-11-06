using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Models
{
    public class TopRRByModelRequest
    {
        public DateTime? Date { get; set; }
        public DateTime? NextDate { get; set; }
        public bool Option { get; set; }
        public string Shift { get; set; }
        public string group_name { get; set; }
        public string ModelName { get; set; }



    }


    public class  ActionByErrorCode
    {
        public string Model { get; set; }
        public string Owner { get; set; }
        public string DueDate { get; set; }
        public string Station { get; set; }
        public string Action { get; set; }
        public string RootCause { get; set; }
        public string ErrCode { get; set; }
        public string Workdate { get; set; }
    }
}