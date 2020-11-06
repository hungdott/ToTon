using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Entity;
using System.Data;
using Report.Common;
using Report.Models;

namespace Report.Controllers
{
    public class YieldTrackingDataFPY_OBAController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: YieldTrackingDataFPY_OBA
        public ActionResult Index()
        {
            //var _now1 = ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : ToDate.Value.ToString("yyyyMMdd");
            //var then1 = FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : FromDate.Value.ToString("yyyyMMdd");
            var _now = DateTime.Now.ToString("yyyyMMdd");
            var then = DateTime.Now.AddDays(-14).ToString("yyyyMMdd");

            string sql = $@" SELECT BANG1.WORK_DATE,BANG2.GROUP_NAME,BANG2.ACTUAL,BANG2.RR,BANG2.INPUT,BANG2.OUTPUT,BANG1.SUM_FIRSR_FAIL_ALL,BANG1.SUM_FAIL_ALL
                           FROM (
                           select WORK_DATE, SUM(F_F_ALL)AS SUM_FIRSR_FAIL_ALL, SUM(FAIL_ALL) AS SUM_FAIL_ALL
                           from(SELECT WORK_DATE, GROUP_NAME, SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) AS INPUT, SUM(FAIL_QTY)AS FAIL_ALL,
                           SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) - SUM(FAIL_QTY) AS OUTPUT, SUM(FIRST_FAIL_QTY)AS F_F_ALL,
                           100 - ROUND(SUM(FAIL_QTY) / (SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY)), 2) AS Actual,
                           ROUND(((SUM(FIRST_FAIL_QTY) - (SUM(FAIL_QTY))) / (SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY))) * 100, 2) AS RR
                           from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%')
                           WHERE
                           WORK_DATE BETWEEN {then} AND {_now}  AND  GROUP_NAME IN('PT', 'FT', 'RC')
                           GROUP BY GROUP_NAME, WORK_DATE)
                           group by  WORK_DATE
                           ORDER BY WORK_DATE ASC ) BANG1 LEFT JOIN((SELECT WORK_DATE, GROUP_NAME, SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) AS INPUT, SUM(FAIL_QTY)AS FAIL,
                           SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) - SUM(FAIL_QTY) AS OUTPUT, SUM(FIRST_FAIL_QTY),
                           ROUND((SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY)) / (SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY)) * 100, 2) AS Actual,
                           ROUND(((SUM(FIRST_FAIL_QTY) - (SUM(FAIL_QTY))) / NULLIF(SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY), 0)) * 100, 2) AS RR
                           from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%')
                           WHERE
                           WORK_DATE BETWEEN {then} AND {_now}  AND  GROUP_NAME IN('PT', 'FT', 'RC')
                           GROUP BY GROUP_NAME, WORK_DATE
                           ORDER BY WORK_DATE ASC)BANG2) ON(BANG1.WORK_DATE = BANG2.WORK_DATE)
                           WHERE BANG1.WORK_DATE = BANG2.WORK_DATE";

            DataTable _result = conn.reDt(sql);
            List<YieldTrackingFPY_OBA> _lstResult = ConvertToObj.ConvertDataTable<YieldTrackingFPY_OBA>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();

            List<string> listDay = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => x.Key).ToList();
            var dd = listDay;


            List<Station_FPY_OBA> ls = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => new Station_FPY_OBA()
            {
                WORK_DATE1 = x.Key,
               
                DataStation = x.Select(j => new DataByStation_FPY_OBA()
                {
                    GROUP_NAME = j.GROUP_NAME,
                    INPUT = j.INPUT,
                    OUTPUT = j.OUTPUT,
                    ACTUAL = j.ACTUAL,
                    RR = j.RR,
                    SUM_FAIL_ALL=j.SUM_FAIL_ALL,
                    SUM_FIRSR_FAIL_ALL=j.SUM_FIRSR_FAIL_ALL
                    
                }).ToList()

            }).ToList();

            ListDataFPY_OPA ListData = new ListDataFPY_OPA();
            ListData.Listdata_OBA = ls;
           

            return View(ListData);
        }


        public ActionResult Filter(TrackingRequest request)
        {

            var _now = request.ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : request.ToDate.Value.ToString("yyyyMMdd");
            var then = request.FromDate == null ? DateTime.Now.AddDays(-14).ToString("yyyyMMdd") : request.FromDate.Value.ToString("yyyyMMdd");
            string sql = $@" SELECT BANG1.WORK_DATE,BANG2.GROUP_NAME,BANG2.ACTUAL,BANG2.RR,BANG2.INPUT,BANG2.OUTPUT,BANG1.SUM_FIRSR_FAIL_ALL,BANG1.SUM_FAIL_ALL
                           FROM (
                           select WORK_DATE, SUM(F_F_ALL)AS SUM_FIRSR_FAIL_ALL, SUM(FAIL_ALL) AS SUM_FAIL_ALL
                           from(SELECT WORK_DATE, GROUP_NAME, SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) AS INPUT, SUM(FAIL_QTY)AS FAIL_ALL,
                           SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) - SUM(FAIL_QTY) AS OUTPUT, SUM(FIRST_FAIL_QTY)AS F_F_ALL,
                           100 - ROUND(SUM(FAIL_QTY) / (SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY)), 2) AS Actual,
                           ROUND(((SUM(FIRST_FAIL_QTY) - (SUM(FAIL_QTY))) / (SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY))) * 100, 2) AS RR
                           from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%')
                           WHERE
                           WORK_DATE BETWEEN {then} AND {_now}  AND  GROUP_NAME IN('PT', 'FT', 'RC')
                           GROUP BY GROUP_NAME, WORK_DATE)
                           group by  WORK_DATE
                           ORDER BY WORK_DATE ASC ) BANG1 LEFT JOIN((SELECT WORK_DATE, GROUP_NAME, SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) AS INPUT, SUM(FAIL_QTY)AS FAIL,
                           SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY) - SUM(FAIL_QTY) AS OUTPUT, SUM(FIRST_FAIL_QTY),
                           ROUND((SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY)) / (SUM(PASS_QTY) + SUM(FAIL_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY)) * 100, 2) AS Actual,
                           ROUND(((SUM(FIRST_FAIL_QTY) - (SUM(FAIL_QTY))) / NULLIF(SUM(FAIL_QTY) + SUM(PASS_QTY) + SUM(REPASS_QTY) + SUM(REFAIL_QTY), 0)) * 100, 2) AS RR
                           from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%')
                           WHERE
                           WORK_DATE BETWEEN {then} AND {_now}  AND  GROUP_NAME IN('PT', 'FT', 'RC')
                           GROUP BY GROUP_NAME, WORK_DATE
                           ORDER BY WORK_DATE ASC)BANG2) ON(BANG1.WORK_DATE = BANG2.WORK_DATE)
                           WHERE BANG1.WORK_DATE = BANG2.WORK_DATE";

            DataTable _result = conn.reDt(sql);
            List<YieldTrackingFPY_OBA> _lstResult = ConvertToObj.ConvertDataTable<YieldTrackingFPY_OBA>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();

            List<string> listDay = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => x.Key).ToList();
            var dd = listDay;


            List<Station_FPY_OBA> ls = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => new Station_FPY_OBA()
            {
                WORK_DATE1 = x.Key,

                DataStation = x.Select(j => new DataByStation_FPY_OBA()
                {
                    GROUP_NAME = j.GROUP_NAME,
                    INPUT = j.INPUT,
                    OUTPUT = j.OUTPUT,
                    ACTUAL = j.ACTUAL,
                    RR = j.RR,
                    SUM_FAIL_ALL = j.SUM_FAIL_ALL,
                    SUM_FIRSR_FAIL_ALL = j.SUM_FIRSR_FAIL_ALL

                }).ToList()

            }).ToList();

            ListDataFPY_OPA ListData = new ListDataFPY_OPA();
            ListData.Listdata_OBA = ls;
            return PartialView("~/Views/YieldTrackingDataFPY_OBA/TrackingFPY_OBAPartial.cshtml", ListData);
        }

        }
    }