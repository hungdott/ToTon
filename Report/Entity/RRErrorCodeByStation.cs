using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class RRErrorCodeByStation
    {
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public decimal RATE { get; set; }
    }



    public class ErrorCode_error
    {
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public decimal TEST_FAIL_QTY { get; set; }
        public decimal FAIL_QTY { get; set; }
        public decimal PASS { get; set; }
        public decimal FAIL { get; set; }
        public decimal REPASS { get; set; }
        public decimal REFAIL { get; set; }
        public decimal RATE { get; set; }
    }
    public class DataDay_error
    {
        public string WORK_DATE { get; set; }
        public List<DataByModel_error> DataByModel { get; set; }
    }

    public class DataByModel_error
    {
        public string MODEL_NAME { get; set; }
        public List<RateByStation_error> RateByStation { get; set; }
        public int count_row_model { get; set; }
    }

    public class RateByStation_error
    {
        public string GROUP_NAME { get; set; }
        public List<RateByErrCode_error> RateByErrCode { get; set; }
        public int count_row_station { get; set; }
    }
    public class RateByErrCode_error
    {
        public string ERROR_CODE { get; set; }
        public List<DataByErrCode_error> DataByErrCode { get; set; }

    }
    public class DataByErrCode_error
    {

        public string WORK_DATE { get; set; }
        public decimal RATE { get; set; }
    }

    public class Data_Error
    {
        public List<string> ListDay { get; set; }
        public List<DataByModel_error> ListData { get; set; }
    }
}