using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class TopErrorCodeByModelName
    {
        public string WORK_DATE { get; set; }
        public string WORK_SECTION { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public string STATION_NAME { get; set; }
        public decimal RATE_STATION { get; set; }
        public decimal Y_RATE_STATION { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
        public decimal TOTAL_STATION { get; set; }
        public decimal RATE_EC { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_FAIL { get; set; }
    }

       

    public class M_TOPDataByModelName
    {
        public string ModelName { get; set; }
        public List<M_DataByGroupName> DataByGroupName { get; set; } 
        public int count_row_station { get; set; }
        public int count_row_station1 { get; set; }
        public int count_groupName { get; set; }
    }

    public class M_DataByGroupName
    {

        public string GroupName { get; set; }
        public decimal Input { get; set; }
        public decimal First_Fail_Of_Model_Name { get; set; }
        public decimal Fail_Of_Model_Name { get; set; }
        public decimal RR { get; set; }
        public decimal YR { get; set; }
        public int count_row_ModelName { get; set; }
        public List<M_FailByErrorCode> FailByErrorCode { get; set; }
        public int count_row_ERR_in_groupName { get; set; }

    }

    public class M_FailByErrorCode
    {

        public string ErrorCode { get; set; }
        public decimal FailQtyErrCode { get; set; }
        public decimal ReFailQtyErrorcode { get; set; }
        public int count_row_Errorcode { get; set; }
        public List<M_FailByMachine> M_FailByMachine { get; set; }
    }

    public class M_FailByMachine
    {
        public string PCName { get; set; }
        public decimal FailQtyPC { get; set; }
        public decimal ReFail { get; set; }
    }

    public class M_TopData
    {
        public List<M_TOPDataByModelName> M_listData { get; set; }
        public List<string> ModelName { get; set; }
        public List<string> GroupName { get; set; }
        public DateTime WorkDate { get; set; }
    }



    //------------------------------

    public class SolutionActionData
    {
        public string WORK_DATE { get; set; }
        public string WORK_SECTION { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public string STATION_NAME { get; set; }
        public decimal RATE_STATION { get; set; }
        public decimal Y_RATE_STATION { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
        public decimal TOTAL_STATION { get; set; }
        public decimal RATE_EC { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_FAIL { get; set; }

        public string PCname { get; set; }
        public string Owner { get; set; }
        public DateTime? Duedate { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string RootCause { get; set; }
    }
    
}