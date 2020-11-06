using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{

    //top 3 error code by station
    public class Top3ErrorCodeByStation
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

    public class TOPDataByStation
    {
        public string GroupName { get; set; }
        public List<DataByModelName> DataByModelName { get; set; }
        public int count_row_station { get; set; }
        public int count_row_station1 { get; set; }

        public int count_model { get; set; }
        public int DataByModelNameCount
        {
            get
            {
                return this.DataByModelName.Sum(x => x.FailByErrorCodeCount);
            }
        }



    }
    public class DataByModelName
    {
        public string ModelName { get; set; }
        public decimal Input { get; set; }
        public decimal Frist_fail_station { get; set; }
        public decimal Fail_Of_station_Name { get; set; }
        public decimal RR { get; set; }
        public decimal YR { get; set; }
        public int count_row_ModelName { get; set; }
        public List<FailByErrorCode> FailByErrorCode { get; set; }
        public int count_row_ERR_in_model { get; set; }
        public int FailByErrorCodeCount
        {
            get
            {
                return this.FailByErrorCode.Sum(x => x.FailByMachineCount);
            }
        }

    }

    public class FailByErrorCode
    {

        public string ErrorCode { get; set; }
        public decimal FailQtyErrCode { get; set; }
        public decimal SecondFailQtyErrCode { get; set; }
        public int count_row_Errorcode { get; set; }
        public string ErrorDESC2 { get; set; }
        public List<FailByMachine> FailByMachine { get; set; }
        public int FailByMachineCount
        {
            get
            {
                return this.FailByMachine.Count+1;
            }
        }
    }

    public class FailByMachine
    {
        public string PCName { get; set; }
        public decimal FailQtyPC { get; set; }
        public decimal ReFail { get; set; }
    }


    public class Line
    {
        public string line { get; set; }
        public List<TOPDataByStation> lstModelName { get; set; }
    }

    public class TopData
    {
        public List<TOPDataByStation> listData { get; set; }
        public List<string> ModelName { get; set; }
        public List<string> GroupName { get; set; }
        public DateTime WorkDate { get; set; }
    }



    // top 3 error code by ModelName
    // char error code by Error code
    public class dataErrorCodeByPCName
    {
        public string WORK_DATE { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public string STATION_NAME { get; set; }
        public decimal WORK_SECTION { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
    }

    //=======
    public class dataByStationChart
    {
        public string Station { get; set; }
        public List<dataByModelNameChart> lstModelName { get; set; }
    }


    public class dataByModelNameChart
    {
        public string ModelName { get; set; }
        public List<lstByErrorCodeChart> lstError { get; set; }
    }


    //char by Error COde
    public class lstByErrorCodeChart
    {
        public string ERROR_CODE { get; set; }
        public List<DataByPCName> DataPCName { get; set; }

    }

    public class DataByPCName
    {
        public string StattionName { get; set; }
        public List<dataByHour> dataHour { get; set; }
    }

    public class dataByHour
    {
        public decimal WORK_SECTION { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
    }

    //Char summary by Error code==========
    public class lstStationByErrorSummary
    {
        public string Station { get; set; }
        public List<lstModelNameByEroSummary> lstModel { get; set; }
    }
    public class lstModelNameByEroSummary
    {
        public string ModelName { get; set; }
        public List<lstErrorCodeSummary> lstErrorSummary { get; set; }
    }
    public class lstErrorCodeSummary
    {
        public string ErrorCode { get; set; }
        public List<DataByErrorCodeSummary> dataErrorSum { get; set; }
    }
    public class DataByErrorCodeSummary
    {
        public decimal WorkSection { get; set; }
        public decimal FirstFailER { get; set; }
        public decimal ReFail { get; set; }

    }

    //================================


    //char by pc name

    public class dataByStationByPc
    {
        public string Station { get; set; }
        public List<ModelNameByPC> lstModelName { get; set; }
    }
    public class ModelNameByPC
    {
        public string ModelName { get; set; }
        public List<lstPCName> lstPC { get; set; }
    }
    public class lstPCName
    {
        public string PCName { get; set; }
        public List<lstErrorCodeByPc> lstErrorCodeByPc { get; set; }


    }
    public class lstErrorCodeByPc
    {
        public string ErrorCode { get; set; }
        public List<DataByErrorCode> lstdataErrorByPc { get; set; }
    }

    public class DataByErrorCode
    {
        public decimal WorkSection { get; set; }
        public decimal SumTestFailER { get; set; }
        public decimal SumReFailER { get; set; }
    }

    //summary fail by pc name

        //======================
    public class chartDayAndNightShifInMainChart
        {
        public string WORK_DATE { get; set; }
       
        public List<dataErrorCodeByPCName> datatDayAndNight { get; set; }
    }

    public class datachartDayAndNightShifInMainChart
    {
        public string WORK_DATE { get; set; }            
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
    }
    //===============
    public class SummaryByStationPc
    {
        public string Station { get; set; }
        public List<SumLstModelNameByPC> lstModelName { get; set; }
    }
    public class SumLstModelNameByPC
    {
        public string ModelName { get; set; }
        public List<SumlstPCName> lstPC { get; set; }
    }
    public class SumlstPCName
    {
        public string PCName { get; set; }
        public List<SumDataByPc> lstDataPc { get; set; }
    }
    public class SumDataByPc
    {
        public decimal WorkSection { get; set; }
        public decimal SumTestFailER { get; set; }
        public decimal SumReFailER { get; set; }
    }


    //summary fail by error code
    //chart summay and detail by ErrorCode
    public class DataSummaryAndDetailOneChart
    {
        public List<lstStationByErrorSummary> lstSummay { get; set; }
        public List<dataByStationChart> lstDetail { get; set; }
        public List<LstData5DayByErrorCode> lst5DayChartByEr { get; set; }
        public List<LstData5Day> lstData7dayDayAndNight { get; set; }
    }


    //chart 5 day by Error Code 
    public class data5daybyDayAndNight
    {
        public string WORK_DATE { get; set; }
        public List<MData5day> listDayDayAndNight { get; set; }
    }
    public class MData5day
    {
        public string WORK_DATE { get; set; }
        public decimal WORK_SECTION { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
    }
    public class LstData5DayByErrorCode
    {
        public string Station;
        public List<LstModelBy5Day> lstModel { get; set; }
    }
    public class LstModelBy5Day
    {
        public string ModelName { get; set; }
        public List<lstDataError5Day> lst5DayError { get; set; }
    }
    public class lstDataError5Day
    {
        public string ErrorCode { get; set; }
        public List<LstData5Day> lisData5Day { get; set; }
    }
    public class LstData5Day
    {
        public string Date { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
    }




    //chart Summary and detail By PC Name
    public class DataSummaryAndDetailOnePc
    {
        public List<SummaryByStationPc> lstSummaryByOnePc { get; set; }
        public List<dataByStationByPc> lsDetailByOnePC { get; set; }

    }

    //all data view ang chart in chart
    public class AllDataInChart
    {
        public List<SummaryByStationPc> summaryChart { get; set; }
        public List<TOPDataByStation> dataViewInChart { get; set; }

        public List<datachartDayAndNightShifInMainChart> dataChartDayAndNightShift { get; set; }
    }

    //find ModelName laster
    public class ModelNameLaster
    {

        public string MODEL_NAME { get; set; }
    }

    public class DataLaster
    {
        public string WorkDate { get; set; }
        public List<TOPDataByStation> data { get; set; }
    }
    

    public class ModelAndDate
    {
      
        public string WORK_DATE { get; set; }
        public decimal WORK_SECTION { get; set; }

    }


    //table action 

    public class dataActionInOrcl
    {
        public string WORK_DATE { get; set; }
        public decimal WORK_SECTION { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        public string ERROR_CODE { get; set; }
      //  public string STATION_NAME { get; set; }
       // public decimal RP1 { get; set; }
        public decimal SUM_TEST_FAIL_ER { get; set; }
        public decimal SUM_FAIL_ER { get; set; }
       // public decimal TOTAL { get; set; }
    }


    //thua
    //public class LstdatatoJoin
    //{
    //    public string workDate { get; set; }
    //   public List<DataByWorkSection> lstDataWorkSection { get; set; }


    //}
    //public class DataByWorkSection
    //{
    //    public decimal WorkSection { get; set; }
    //    public string modelName { get; set; }
    //    public decimal sumFirstFail { get; set; }
    //    public decimal sumFail { get; set; }
    //}



    //public class DataJoin
    //{
    //    public string workDate { get; set; }
    //    public decimal WorkSection { get; set; }
    //    public string modelName { get; set; }
    //    public decimal sumFirstFail { get; set; }
    //    public decimal sumFail { get; set; }
    //}

    //thua


    public class DataResultTableAction
    {
        public string WorkDate { get;set;}
        public string DueDate { get;set;}
        public int WorkSection { get;set;}
        public string ModelName { get;set;}
        public string GroupName { get;set;}
        public string ErrorCode { get;set;}
        public string Proplem { get;set;}
        public string RootCause { get; set; }
        public string Action { get;set;}
        public string Status { get;set;}
        public string owner { get;set;}
        public int Week { get;set;}
        public int Hour { get;set;}
        public decimal FirstFail { get; set; }    

    }




    public  class lstAllData
    {
        public string ModelName { get; set; }
        public string Station { get; set; }
     
        public List<lstDataActionByDay> lstdatabyWorkDate { get; set; }
    }
    public class lstDataActionByDay
    {
        public string WorkDate { get; set; }
        public string ErrorCode { get; set; }
    
        public List<dataByStation> lstDataActionbyDay { get; set; }
        
    }

    public class dataByStation
    {
        public decimal WorkSection { get; set; }
        public string DueDate { get; set; }
        public string Proplem { get; set; }
        public string RootCause { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public string owner { get; set; }
        public int Week { get; set; }
     
        public decimal FirstFail { get; set; }
        
    }

   




}