using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class FactoryMapEntity1
    {

        public string WORK_DATE { get; set; }
        public decimal WORK_SECTION { get; set; }
        public string LINE_NAME { get; set; }
        public string MODEL_NAME { get; set; }
       
        public string GROUP_NAME { get; set; }
       // public string ERROR_CODE { get; set; }
        public string STATION_NAME { get; set; }
        public decimal RATE_STATION { get; set; }
        public decimal Y_RATE_STATION { get; set; }
        public decimal INPUT { get; set; }
    }
    public class FactoryMapEntity
    {

       
        public string LINE_NAME { get; set; }
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }         
        public decimal FIRST_FAIL { get; set; }
        public decimal SUM_PASS { get; set; }
        public decimal SUM_FAIL { get; set; }
        public decimal SUM_REPASS { get; set; }
        public decimal SUM_RE_FAIL { get; set; }
        public decimal RR { get; set; }
        public decimal YR { get; set; }
        public decimal INPUT { get; set; }
    }


    public class LstDataLine
    {
        public string LINE_NAME { get; set; }
        public List<dataModelByLine> lstDataModelNameByLine { get; set; }

    }
    public class dataModelByLine
    {  
        public string MODEL_NAME { get; set; }
        public List<LineDataByGroupInModel> lstDataByGroupInModelName { get; set; }
    }

    public class LineDataByGroupInModel
    {
        public string GROUP_NAME { get; set; }
        public List<DataByGroupInLine> lstdataGroupInLine { get; set; }
         
    }
    public class DataByGroupInLine
    {
            public decimal INPUT { get; set; }
            public decimal RR { get; set; }
            public decimal YR { get; set; }
            public decimal FirstFail { get; set; }
            public decimal SumPass { get; set; }
            public decimal SumFail { get; set; }
            public decimal RePass { get; set; }
            public decimal ReFail { get; set; }
    }


    public class ModelCurrent
    {
        public string MODEL_NAME { get; set; }
    }


}