using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class YieldTrackingData
    {
        public string WORK_DATE { get; set; }
        public string GROUP_NAME { get; set; }
        public decimal INPUT { get; set; }
        public decimal FAIL { get; set; }
        public decimal OUTPUT { get; set; }
        public decimal SUM_FIRST_FAIL { get; set; }
        public decimal ACTUAL { get; set; }
        
    }

    public class DataStation
        {
        public string WORK_DATE { get; set; }
        public decimal INPUT { get; set; }
        public decimal FAIL { get; set; }
        public decimal OUTPUT { get; set; }
        public decimal SUM_FIRST_FAIL { get; set; }
        public decimal ACTUAL { get; set; }
        
    }
    public class DataDate
    {
        public string WORK_DATE;
        public List<DataStation> DataStation { get; set; }
    }

    public class DataGROUP_Name
    {
        public string GROUP_NAME;
        public List<DataStation> DataStation { get; set; }
    }

    public class DataVM
    {
        public List<DataGROUP_Name> DataGROUP_Name { get; set; }
        public List<string> ListStation { get; set; }
    }


    //==============================================

    public class DataStation1
    {
        public string WORK_DATE { get; set; }
        public string GROUP_NAME { get; set; }
        public decimal INPUT { get; set; }
        public decimal FAIL { get; set; }
        public decimal OUTPUT { get; set; }
        public decimal SUM_FIRST_FAIL { get; set; }
        public decimal ACTUAL { get; set; }

    }
    public class DataVM1
    {
        public string date1 { get; set; }
        public List<DataStation1> dataStation1 { get; set; }
       
    }

    public class ListDataVM1
    {
      
        public List<string> listDate1 { get; set; }
       
        public List<DataVM1> ListdataStation1 { get; set; }

    }



}