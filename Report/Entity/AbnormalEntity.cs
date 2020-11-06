using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class AbnormalEntity
    {
        public string LINE_NAME { get; set; }
        public string WORK_DATE { get; set; }
        public Int16 WORK_SECTION { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_PASS { get; set; }
        public decimal SUM_FAIL { get; set; }
        public decimal SUM_REPASS { get; set; }
        public decimal SUM_RE_FAIL { get; set;}
        public decimal INPUT  {get; set;}
        public decimal RR { get; set; }

    }

    public class DataInPutModelNamebyShift
    {
        public string ModelName { get; set; }
        public List<DataModelNameByShift> LstDataByGroupName { get; set; }

    }

    public class DataModelNameByShift
    {
        public string GroupName { get; set; }
        public decimal Target { get; set; }
        public decimal InPut { get; set; }
        public decimal Obtain { get; set; }
    }

    public class DataToDrawChart
    {      
        public string GROUP_NAME { get; set; }
        public List<dataGroupNameByHour> dataGroupNameByHour { get; set; }

       
    }
    public class dataGroupNameByHour
    {
        public int WORK_SECTION { get; set; }
        public decimal INPUT { get; set; }
    }


  
}