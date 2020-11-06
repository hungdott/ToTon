using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    //data chart 7 day by day and shift
    public class DataOneDay
    {
        public string WORK_DATE { get; set; }
        public List<DataToDayAndChart5dayE> DataToDayAndChart5dayE { get; set; }

    }

    public class DataToDayAndChart5dayE
    {
        public Int16 WORK_SECTION { get; set; }
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public decimal INPUT { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_PASS { get; set; }
        public decimal SUM_FAIL { get; set; }
        public decimal SUM_REPASS { get; set; }
        public decimal SUM_RE_FAIL { get; set; }
        public decimal RR { get; set; }
        public decimal YR { get; set; }
    }

    //===== hove to show action 7 day===============
    public class DataOneDayHoveToShowAction
    {
        public string WORK_DATE { get; set; }
        public List<DataToDayAndChart5dayE> DataToDayAndChart5dayOrcl { get; set; }
        public List<string> lstAction { get; set; }
    }
    public class DataToDayAndChart5dayEJoinSql
    {
        public Int16 WORK_SECTION { get; set; }
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public decimal INPUT { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_PASS { get; set; }
        public decimal SUM_FAIL { get; set; }
        public decimal SUM_REPASS { get; set; }
        public decimal SUM_RE_FAIL { get; set; }
        public decimal RR { get; set; }
        public decimal YR { get; set; }
        public string Action { get; set; }
    }
    //================================================

    public class DataByDayStation
    {
        public string WORK_DATE { get; set; }
        public decimal INPUT { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_PASS { get; set; }
        public decimal SUM_FAIL { get; set; }
        public decimal SUM_REPASS { get; set; }
        public decimal SUM_RE_FAIL { get; set; }
        public decimal RR_s { get; set; }
        public decimal YR_s { get; set; }
        public DateTime WorkDate
        {
            get
            {
                return DateTime.ParseExact(this.WORK_DATE,"yyyyMMdd",null);
            }
        }
        public List<string> LstAction { get; set; }
    }

   

    public class DataBySattion
    {
        public decimal INPUT { get; set; }
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_PASS { get; set; }
        public decimal SUM_FAIL { get; set; }
        public decimal SUM_REPASS { get; set; }
        public decimal SUM_RE_FAIL { get; set; }
        public decimal RR_s { get; set; }
        public decimal YR_s { get; set; }
        public List<LisAction> lstAction { get; set; }
    }

    public class LisAction
    {
        public int WorkSection { get; set; }
        public string Action { get; set; }
    }


    public class DataRRBySattionDay
    {
        public string WORK_DATE { get; set; }
        public List<DataBySattion> lstDataBysation { get; set; }

    }
    public class DataByGroupName
    {
        public string GroupName { get; set; }
        public List<DataRRBySattionDay> lstDataByGroupName { get; set; }
    }

    public class DataRRByModelName
    {

        public string MODEL_NAME { get; set; }
        public List<DataByGroupName> lstDataByStation { get; set; }
    }

    public class dataByDay
    {
        public string Work_date;
        public List<DataRRByModelName> databyDay;
    }

    public class LstData
    {
        public List<DataRRByModelName> lstdataByModel { get; set; }
      
    }

    public class AllData
    {
        public List<DataRRByModelName> lisDataByModel { get; set; }
        public List<TOPDataByStation> listdataInChart { get; set; }

        public List<DataByDayStation> lstDataChartByDayAndNight { get; set; }
    }

}