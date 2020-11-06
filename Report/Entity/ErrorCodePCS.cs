using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class ErrorCodePCS
    {
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public decimal Sum_Test_Fail_ER { get; set; }
        public decimal Sum_Fail_ER { get; set; }
        public decimal TOTAL { get; set; }
        public decimal RATE { get; set; }
    }


    public class ErrorCodePCSByModelName
    {
        public string MODEL_NAME { get; set; }
        public List<ErrorCodePCSByGroupName> ErrorCodeModelName { get; set; }
    }
    public class ErrorCodePCSByGroupName
    {
        public string GROUP_NAME { get; set; }
        public List<ListErrorCodePCS> ListErrorCodePCS { get; set; }
    }
    public class ListErrorCodePCS
    {
        public string ERROR_CODE { get; set; }
        public List<DataErrorCodeByDate> DataErrorCodeDate { get; set; }
    }
    public class DataErrorCodeByDate
    {
        public string WORK_DATE { get; set; }
        public decimal Sum_Test_Fail_ER { get; set; }
        public decimal Sum_Fail_ER { get; set; }
        public decimal TOTAL { get; set; }
        public decimal RATE { get; set; }
    }
}