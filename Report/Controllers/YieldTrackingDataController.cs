using Report.Common;
using Report.Entity;
using Report.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace Report.Controllers
{
    public class YieldTrackingDataController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: YieldTrackingData
        public ActionResult Index(string Shift, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            var _now = ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : ToDate.Value.ToString("yyyyMMdd");
            var then = FromDate == null ? DateTime.Now.AddDays(-14).ToString("yyyyMMdd") : FromDate.Value.ToString("yyyyMMdd");

            if (String.IsNullOrEmpty(Shift))
            {

                Shift = "1";
            }

            List<string> lstDay = new List<string>();

            //string _sql = $@" SELECT WORK_DATE, GROUP_NAME ,SUM(PASS_QTY)+SUM(FAIL_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY) AS INPUT,SUM(FAIL_QTY)AS FAIL,
            //                 SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)-SUM(FAIL_QTY) AS OUTPUT,SUM(FIRST_FAIL_QTY)as SUM_FIRST_FAIL,
            //               ROUND ((( SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY))/(SUM(PASS_QTY)+SUM(FAIL_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)))*100,2) AS Actual
            //              from (SELECT *FROM SFISM4.R_STATION_REC_T WHERE MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%') 
            //              WHERE 
            //                 WORK_DATE BETWEEN {then} AND {_now} 
            //                 AND  GROUP_NAME IN ('PT','FT','RC')
            //                GROUP BY GROUP_NAME,WORK_DATE
            //                ORDER BY WORK_DATE ASC";

            string _sql = $@" SELECT WORK_DATE, GROUP_NAME ,SUM(PASS_QTY)+SUM(FAIL_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY) AS INPUT,SUM(FAIL_QTY)AS FAIL,
                             SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)-SUM(FAIL_QTY) AS OUTPUT,SUM(FIRST_FAIL_QTY)as SUM_FIRST_FAIL,
                           ROUND ((( SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY))/(SUM(PASS_QTY)+SUM(FAIL_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)))*100,2) AS Actual
                          from (SELECT *FROM SFISM4.R_STATION_REC_T WHERE MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%') 
                          WHERE 
                             WORK_DATE BETWEEN {then} AND {_now} 
                             AND  GROUP_NAME IN ('PT','FT','RC')
                            GROUP BY GROUP_NAME,WORK_DATE
                            ORDER BY WORK_DATE ASC";


            //DataTable _result = conn.reDt(_sql);
            //List<YieldTrackingData> _lstResult = ConvertToObj.ConvertDataTable<YieldTrackingData>(_result)
            //    .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();

            //List<decimal> listActual = _lstResult.Select(s => s.Actual).ToList();
            //var ac = listActual;


            DataTable _result = conn.reDt(_sql);
            List<YieldTrackingData> _lstResult = ConvertToObj.ConvertDataTable<YieldTrackingData>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();

            List<decimal> listActual = _lstResult.Select(s => s.ACTUAL).ToList();
            var ac = _lstResult;

           

            List<DataGROUP_Name> _lstTmp = _lstResult.GroupBy(x => x.GROUP_NAME).Select(x => new DataGROUP_Name()
            {
                GROUP_NAME = x.Key,
                DataStation = x.Select(j => new DataStation()
                {
                    WORK_DATE = j.WORK_DATE,
                    INPUT = j.INPUT,
                    FAIL = j.FAIL,
                    OUTPUT = j.OUTPUT,
                    SUM_FIRST_FAIL = j.SUM_FIRST_FAIL,
                    ACTUAL = j.ACTUAL
                }).ToList()
            } ).ToList();




            var a = _lstTmp;
            DataVM dt = new DataVM();
            // List<string> listStation = _lstResult.GroupBy(x => x.GROUP_NAME).Select(x=>x.Key).ToList();
            List<string> listStation=new List<string>();
            foreach (var item in _lstTmp)
            {
                listStation.Add(item.GROUP_NAME);
            }

            //List<DataGROUP_Name> group_data = new List<DataGROUP_Name>();
            List<DataGROUP_Name> group_data = _lstResult.GroupBy(x => x.GROUP_NAME).Select(x => new DataGROUP_Name()
            {
                GROUP_NAME = x.Key,
                DataStation = x.Select(j => new DataStation()
                {
                    WORK_DATE = j.WORK_DATE,
                    INPUT = j.INPUT,
                    FAIL = j.FAIL,
                    OUTPUT = j.OUTPUT,
                    SUM_FIRST_FAIL = j.SUM_FIRST_FAIL,
                    ACTUAL = j.ACTUAL
                }).ToList()
            }).ToList();

            DataVM dataVM = new DataVM();
            dataVM.ListStation = listStation;
            dataVM.DataGROUP_Name = group_data;

            //======================
            List<string> listDay = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => x.Key).ToList();
            var dd = listDay;

            List<DataVM1> l = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => new DataVM1()
            {
                date1 = x.Key,
                dataStation1 = x.Select(j => new DataStation1()
                {
                    WORK_DATE = j.WORK_DATE,
                    GROUP_NAME = j.GROUP_NAME,
                  INPUT = j.INPUT,
                  FAIL = j.FAIL,
                  OUTPUT = j.OUTPUT,
                  SUM_FIRST_FAIL = j.SUM_FIRST_FAIL,
                  ACTUAL =j.ACTUAL
              }).ToList()
          }).ToList();

            var lll = l;
            ListDataVM1 ListData = new ListDataVM1();

            ListData.listDate1 = listDay;
            ListData.ListdataStation1 = l;





            return View(ListData);
           
        }



        public ActionResult Filter(TrackingRequest request)
        {

            var _now = request.ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : request.ToDate.Value.ToString("yyyyMMdd");
            var then = request.FromDate == null ? DateTime.Now.AddDays(-14).ToString("yyyyMMdd") : request.FromDate.Value.ToString("yyyyMMdd");

            List<string> lstDay = new List<string>();

            string _sql = $@"SELECT WORK_DATE, GROUP_NAME ,SUM(PASS_QTY)+SUM(FAIL_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY) AS INPUT,SUM(FAIL_QTY)AS FAIL,
                             SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)-SUM(FAIL_QTY) AS OUTPUT,SUM(FIRST_FAIL_QTY)as SUM_FIRST_FAIL,
                           ROUND (( SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY))/(SUM(PASS_QTY)+SUM(FAIL_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY))*100,2) AS Actual
                          from (SELECT *FROM SFISM4.R_STATION_REC_T WHERE MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%') 
                          WHERE 
                             WORK_DATE BETWEEN {then} AND {_now} 
                             AND  GROUP_NAME IN ('PT','FT','RC')
                            GROUP BY GROUP_NAME,WORK_DATE
                            ORDER BY WORK_DATE ASC";


            DataTable _result = conn.reDt(_sql);
            List<YieldTrackingData> _lstResult = ConvertToObj.ConvertDataTable<YieldTrackingData>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();

            List<DataGROUP_Name> _lstTmp = _lstResult.GroupBy(x => x.GROUP_NAME).Select(x => new DataGROUP_Name()
            {
                GROUP_NAME = x.Key,
                DataStation = x.Select(j => new DataStation()
                {
                    WORK_DATE = j.WORK_DATE,
                    INPUT = j.INPUT,
                    FAIL = j.FAIL,
                    OUTPUT = j.OUTPUT,
                    SUM_FIRST_FAIL = j.SUM_FIRST_FAIL,
                    ACTUAL = j.ACTUAL
                }).ToList()
            }).ToList();




            var a = _lstTmp;
            DataVM dt = new DataVM();
            // List<string> listStation = _lstResult.GroupBy(x => x.GROUP_NAME).Select(x=>x.Key).ToList();
            List<string> listStation = new List<string>();
            foreach (var item in _lstTmp)
            {
                listStation.Add(item.GROUP_NAME);
            }

            //List<DataGROUP_Name> group_data = new List<DataGROUP_Name>();
            List<DataGROUP_Name> group_data = _lstResult.GroupBy(x => x.GROUP_NAME).Select(x => new DataGROUP_Name()
            {
                GROUP_NAME = x.Key,
                DataStation = x.Select(j => new DataStation()
                {
                    WORK_DATE = j.WORK_DATE,
                    INPUT = j.INPUT,
                    FAIL = j.FAIL,
                    OUTPUT = j.OUTPUT,
                    SUM_FIRST_FAIL = j.SUM_FIRST_FAIL,
                    ACTUAL = j.ACTUAL
                }).ToList()
            }).ToList();

            DataVM dataVM = new DataVM();
            dataVM.ListStation = listStation;
            dataVM.DataGROUP_Name = group_data;

            //======================
            List<string> listDay = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => x.Key).ToList();
            var dd = listDay;

            List<DataVM1> l = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => new DataVM1()
            {
                date1 = x.Key,
                dataStation1 = x.Select(j => new DataStation1()
                {
                    WORK_DATE = j.WORK_DATE,
                    GROUP_NAME = j.GROUP_NAME,
                    INPUT = j.INPUT,
                    FAIL = j.FAIL,
                    OUTPUT = j.OUTPUT,
                    SUM_FIRST_FAIL = j.SUM_FIRST_FAIL,
                    ACTUAL = j.ACTUAL
                }).ToList()
            }).ToList();
            
            ListDataVM1 ListData = new ListDataVM1();

            ListData.listDate1 = listDay;
            ListData.ListdataStation1 = l;


            return PartialView("~/Views/YieldTrackingData/TrackingPartial.cshtml", ListData);
            //return View(request);
        }
    }
}