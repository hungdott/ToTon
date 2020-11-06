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
    public class RRByErrorCodeStationController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: RRByErrorCodeStation
        public ActionResult Index(string Shift, DateTime? FromDate = null, DateTime? ToDate = null)
        {
            var _now = ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : ToDate.Value.ToString("yyyyMMdd");
            var then = FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : FromDate.Value.ToString("yyyyMMdd");

            if (String.IsNullOrEmpty(Shift))
            {

                Shift = "1";
            }

            //  List<string> lstDay = new List<string>();


            string _sql = $@"  SELECT BM.WORK_DATE,BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,
                              ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE
                              FROM
                                (select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE, COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'R%' OR MODEL_NAME LIKE 'M%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%') ERC
                              WHERE ERC.WORK_DATE between {then} and {_now} AND ERC.GROUP_NAME IN('FT','FT1','FT2','PT','PT1','PT2','NFT','RC','RC1','RC2','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.WORK_DATE
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              LEFT JOIN 
                              (( SELECT *  FROM(
                                  SELECT  WORK_DATE, MODEL_NAME,  GROUP_NAME ,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-(SUM(FAIL_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RP1,
                                  ROUND(((SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS ReTestRate,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%' ) 
                                  WHERE  WORK_DATE between {then} and {_now}   AND GROUP_NAME IN('FT','FT1','FT2','PT','PT1','PT2','NFT','RC','RC1','RC2','RI')
                                  GROUP BY WORK_DATE, MODEL_NAME, GROUP_NAME
                                  ORDER BY ReTestRate DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME
                                WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BM.WORK_DATE=BH.WORK_DATE";

            //string _sql = $@"select errCode.WORK_DATE,errCode.MODEL_NAME,errCode.GROUP_NAME,errCode.ERROR_CODE,
            //            errCode.test_fail_qty,errCode.fail_qty,station.pass,station.fail,station.repass,station.refail, 
            //            ROUND(((errCode.test_fail_qty-errCode.fail_qty)/(station.pass+station.fail+station.repass+station.refail))*100,2) as RATE 
            //            from (select WORK_DATE,MODEL_NAME,GROUP_NAME,ERROR_CODE,SUM(TEST_FAIL_QTY) as test_fail_qty,SUM(FAIL_QTY) as fail_qty from SFISM4.R_ATE_ERRCODE_T 
            //            where  TO_DATE(WORK_DATE,'YYYYMMDD')                        
            //            BETWEEN TO_DATE('{then}','YYYYMMDD') and TO_DATE('{_now}','YYYYMMDD')
            //            and WORK_SECTION > 7 and WORK_SECTION < 20 
            //            and REGEXP_LIKE (MODEL_NAME, '^[R_C_E](*)') 
            //            and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC){{1}}')
            //            GROUP BY WORK_DATE,MODEL_NAME,GROUP_NAME,ERROR_CODE) errCode
            //            LEFT JOIN (select WORK_DATE,MODEL_NAME,GROUP_NAME,SUM(PASS_QTY) as pass,SUM(FAIL_QTY) as fail,SUM(REPASS_QTY) as repass, SUM(REFAIL_QTY) as refail from SFISM4.R_STATION_REC_T
            //            where TO_DATE(WORK_DATE,'YYYYMMDD')                        
            //            BETWEEN TO_DATE('{then}','YYYYMMDD') and TO_DATE('{_now}','YYYYMMDD')
            //            and WORK_SECTION > 7 and WORK_SECTION < 20 
            //            and REGEXP_LIKE (MODEL_NAME, '^[R_C_E](*)') 
            //            and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC){{1}}')
            //            GROUP BY WORK_DATE,MODEL_NAME,GROUP_NAME) station
            //            ON errCode.WORK_DATE = station.WORK_DATE 
            //            and errCode.MODEL_NAME = station.MODEL_NAME 
            //            and errCode.GROUP_NAME = station.GROUP_NAME";



            DataTable _result = conn.reDt(_sql);

            List<RRErrorCodeByStation> _lstResult = ConvertToObj.ConvertDataTable<RRErrorCodeByStation>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();



            List<string> lstDay_error = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => x.Key).OrderBy(x => x).ToList();

            List<DataByModel_error> lstByModel = _lstResult.GroupBy(x => x.MODEL_NAME).Select(x => new DataByModel_error()
            {
                MODEL_NAME = x.Key,
                count_row_model = x.Select(ss => ss.ERROR_CODE).Count(),
                RateByStation = x.GroupBy(j => j.GROUP_NAME).Select(j => new RateByStation_error()
                {
                    GROUP_NAME = j.Key,
                    count_row_station = j.GroupBy(q => q.ERROR_CODE).Count(),
                    RateByErrCode = j.GroupBy(q => q.ERROR_CODE).Select(q => new RateByErrCode_error()
                    {
                        ERROR_CODE = q.Key,
                        DataByErrCode = (from day in lstDay_error
                                         join errCode in q.GroupBy(k => k.WORK_DATE).Select(k => new DataByErrCode_error()
                                         {
                                             WORK_DATE = k.Key,
                                             //RATE =k.Sum(a => a.RATE)
                                             RATE = k.Sum(a=>a.RATE)
                                         }).ToList()
                                         on day equals errCode.WORK_DATE into tb_data
                                         from data in tb_data.DefaultIfEmpty()
                                         orderby day
                                         select new DataByErrCode_error()
                                         {
                                             RATE = data == null ? -5m : data.RATE,
                                             WORK_DATE = day
                                         }).ToList()

                    }).ToList()
                }).ToList()

            }).ToList();

            // var a1 = lstByModel;



            Data_Error DataError = new Data_Error();
            DataError.ListDay = lstDay_error;
            DataError.ListData = lstByModel;
            var l = DataError;
             return View(DataError);

            //RRByDayDataVM dataRRByDay = new RRByDayDataVM();
            //dataRRByDay.ListDay = lstDay;
            //dataRRByDay.RRByDay = lstRRByDay;


            // return View(dataRRByDay);

        }
    }
}