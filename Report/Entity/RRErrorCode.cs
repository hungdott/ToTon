using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class RRErrorCode
    {
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public string SumTestFail { get; set; }
        public string Sum_Fail_ER { get; set; }
        public string Sum_Pass { get; set; }
        public string First_Fail { get; set; }
        public string Sum_FAIL { get; set; }
        public string Sum_RePass { get; set; }
        public string Sum_Re_Fail { get; set; }
        public string RR_ERROR_CODE { get; set; }


    }


    //public class RRErrorCode_error
    //{
    //    public string MODEL_NAME { get; set; }
    //    public string GROUP_NAME { get; set; }
    //    public string ERROR_CODE { get; set; }
    //    public double RR_ERROR_CODE { get; set; }

    //}




    //public class ErrorCode
    //{
    //    public string WORK_DATE { get; set; }
    //    public string MODEL_NAME { get; set; }
    //    public string GROUP_NAME { get; set; }
    //    public string ERROR_CODE { get; set; }
    //    public decimal TEST_FAIL_QTY { get; set; }
    //    public decimal FAIL_QTY { get; set; }
    //    public decimal PASS { get; set; }
    //    public decimal FAIL { get; set; }
    //    public decimal REPASS { get; set; }
    //    public decimal REFAIL { get; set; }
    //    public decimal RATE { get; set; }
    //}
    //public class DataDay
    //{
    //    public string WORK_DATE { get; set; }
    //    public List<DataByModel> DataByModel { get; set; }
    //}

    //public class DataByModel
    //{
    //    public string MODEL_NAME { get; set; }
    //    public List<RateByStation> RateByStation { get; set; }
    //}

    //public class RateByStation
    //{
    //    public string GROUP_NAME { get; set; }
    //    public List<RateByErrCode> RateByErrCode { get; set; }
    //}
    //public class RateByErrCode
    //{
    //    public string ERROR_CODE { get; set; }
    //    public List<DataByErrCode> DataByErrCode { get; set; }

    //}
    //public class DataByErrCode
    //{

    //    public string WORK_DATE { get; set; }
    //    public decimal RATE { get; set; }
    //}
}