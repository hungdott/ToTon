using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
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

    public class ErrorCodeVm
    {
        public List<string> days { get; set; }
        public List<string> ModelNames { get; set; }
        public List<DataByModel> lstDataByModel { get; set; }


    }

    public class ErrorCode
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
    public class DataDay
    {
        public string WORK_DATE { get; set; }
        public List<DataByModel> DataByModel { get; set; }
       
    }

    public class DataByModel
    {
        public string MODEL_NAME { get; set; }
        public List<RateByStation> RateByStation { get; set; }
        public int count_row_model { get; set; }
        
    }

    public class RateByStation
    {
        public string GROUP_NAME { get; set; }
        public List<RateByErrCode> RateByErrCode { get; set; }
        public int count_row_station { get; set; }

    }
    public class RateByErrCode
    {
        public string ERROR_CODE { get; set; }
        public List<DataByErrCode> DataByErrCode { get; set; }
        public int count_row_rate { get; set; }
    }
    public class DataByErrCode
    {
        public string WORK_DATE { get; set; }
        public decimal RATE { get; set; }
    }
    
    public class DataErrorCode
    {
        public List<string> ListDate;
        public List<DataByModel> listDataByModel;
    }

    public class count_row_model
    {
       public List<int> listCout { get; set; }
        public int count { get; set; }
       
    }
    public class ListModelName
    {
        public string ModelName { get; set; }
    }


}