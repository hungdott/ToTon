using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class YieldTrackingFPY_OBA
    {
        public string WORK_DATE { get; set; }
        public string GROUP_NAME { get; set; }
        public decimal ACTUAL { get; set; }
        public decimal RR { get; set; }
        public decimal INPUT { get; set; }
        public decimal OUTPUT { get; set; }
        public decimal SUM_FIRSR_FAIL_ALL { get; set; }
        public decimal SUM_FAIL_ALL { get; set; }
    }

    public class Station_FPY_OBA
    {
        public string WORK_DATE1 { get; set; }
        public decimal SUM_FIRSR_FAIL_ALL { get; set; }
        public decimal SUM_FAIL_ALL { get; set; }
        public DataFail_OBA dataFail { get; set; }
        public List<DataByStation_FPY_OBA> DataStation { get; set; }
    }

    public class DataByStation_FPY_OBA
    {

        public string GROUP_NAME { get; set; }
        public decimal ACTUAL { get; set; }
        public decimal RR { get; set; }
        public decimal INPUT { get; set; }
        public decimal OUTPUT { get; set; }
        public decimal SUM_FIRSR_FAIL_ALL { get; set; }
        public decimal SUM_FAIL_ALL { get; set; }
    }
    public class DataFail_OBA
    {
        public decimal SUM_FIRSR_FAIL_ALL { get; set; }
        public decimal SUM_FAIL_ALL { get; set; }
    }


    public class ListDataFPY_OPA
    {
       public List<Station_FPY_OBA> Listdata_OBA { get; set; }
    }
}