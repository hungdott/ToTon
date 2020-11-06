using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class PivotAnalyseET
    {
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string STATION_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
    }
    public class LstErrorCode
    {
        public string ErrorCode { get; set; }
    }

    public class lstDataByModel
    {
        public string ModelName { get; set; }
        public List<LstDataByStation> lstStation { get; set; }
    }
    public class LstDataByStation
    {
        public string Station { get; set; }
        public List<LstDataByPc> lstDataPc{get;set;}
    }
    public class LstDataByPc
    {
        public string PCName { get; set; }
        public List<DataByError> lstEr {get; set; }
    }

    public class DataByError
    {
        public string Error { get; set; }
        public decimal Fail { get; set; }
    }
}