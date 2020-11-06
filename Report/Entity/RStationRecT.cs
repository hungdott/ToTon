using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class RStationRecT
    {
        public string WORK_DATE { get; set; }
        public int WORK_SECTION { get; set; }
        public string MO_NUMBER { get; set; }
        public string MODEL_NAME { get; set; }
        public string LINE_NAME { get; set; }
        public string SECTION_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public int WIP_QTY { get; set; }
        public int PASS_QTY { get; set; }
        public int FAIL_QTY { get; set; }
        public int REPASS_QTY { get; set; }

        public int REFAIL_QTY { get; set; }
        public int ECN_PASS_QTY { get; set; }
        public int ECN_FAIL_QTY { get; set; }
        public string LAST_FLAG { get; set; }
        public int DEFECT_NO { get; set; }
        public int RETEST { get; set; }
        public string CLASS { get; set; }
        public string CLASS_DATE { get; set; }
        public string MOVE_FLAG { get; set; }
        public string STATION_NAME { get; set; }
        public decimal FIRST_FAIL_QTY { get; set; }
    }

    public class Data
    {
        public string GROUP_NAME { get; set; }
        public int PASS_QTY { get; set; }
        public int FAIL_QTY { get; set; }
        public int REPASS_QTY { get; set; }
        public int REFAIL_QTY { get; set; }
        public decimal FIRST_FAIL_QTY { get; set; }
    }

    public class StationVM
    {
        public string MODEL_NAME { get; set; }
        public List<Data> Data { get; set; }
    }

    public class StationDayVM
    {
        public string WORK_DATE { get; set; }
        public List<StationVM> StationDay { get; set; }

    }

    public class DataByDay
    {
        public string GROUP_NAME { get; set; }
        public List<DataPerDay> DataPerDay { get; set; }
    }

    public class RRByDay
    {
        public string MODEL_NAME { get; set; }
        public List<DataByDay> DataByDay { get; set; }

    }
    public class DataPerDay
    {
        public string WORK_DATE { get; set; }
        public decimal RETEST_RATE { get; set; }
    }
    public class RRByDayTmp
    {
        public string MODEL_NAME { get; set; }
        public string WORK_DATE { get; set; }
        public decimal RETEST_RATE { get; set; }
        public string GROUP_NAME { get; set; }


    }

    public class RRByDayDataVM
    {
        public List<RRByDay> RRByDay { get; set; }
        public List<string> ListDay { get; set; }
    }

    // today
    public class ModelNameToday
    {
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
    }
}