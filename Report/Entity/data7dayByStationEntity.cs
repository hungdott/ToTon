
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{

    //top 3 error code by station
    public class data7dayByStationEntity
    {
        public string WORK_DATE { get; set; }
        public decimal WORK_SECTION { get; set; }
        public string MODEL_NAME { get; set; }
        public string LINE_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public string STATION_NAME { get; set; }
        public decimal RATE_STATION { get; set; }
        public decimal Y_RATE_STATION { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
        public decimal TOTAL_STATION { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_FAIL { get; set; }
        public decimal RATE_EC { get; set; }
        public string ERROR_DESC2 { get; set; }



    }

    public class TOPDataByStation1
    {
        public string GroupName { get; set; }
        public List<DataByModelName1> DataByModelName { get; set; }
        public int count_row_station { get; set; }
        public int count_row_station1 { get; set; }

        public int count_model { get; set; }
        



    }
    public class DataByModelName1
    {
        public string ModelName { get; set; }
        public decimal Input { get; set; }
        public decimal Frist_fail_station { get; set; }
        public decimal Fail_Of_station_Name { get; set; }
        public decimal RR { get; set; }
        public decimal YR { get; set; }
        public int count_row_ModelName { get; set; }
        public List<FailByErrorCode1> FailByErrorCode { get; set; }
        public int count_row_ERR_in_model { get; set; }
       

    }

    public class FailByErrorCode1
    {

        public string ErrorCode { get; set; }
        public decimal FailQtyErrCode { get; set; }
        public decimal SecondFailQtyErrCode { get; set; }
        public int count_row_Errorcode { get; set; }
        public string ErrorDESC2 { get; set; }
        public List<FailByMachine1> FailByMachine { get; set; }
       
    }

    public class FailByMachine1
    {
        public string PCName { get; set; }
        public decimal FailQtyPC { get; set; }
        public decimal ReFail { get; set; }
    }


   
    public class TopData1
    {
        public List<TOPDataByStation1> listData { get; set; }
        public List<string> ModelName { get; set; }
        public List<string> GroupName { get; set; }
        public DateTime WorkDate { get; set; }
    }



    




}