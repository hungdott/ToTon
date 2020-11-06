using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Common;
using Report.Entity;
using Report.Models;
using System.Data;

namespace Report.Controllers
{
    public class ErrorCode_pcsController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: ErrorCode_spcs
        public ActionResult Index()
        {
            var _then = DateTime.Now.ToString("yyyyMMdd");
            var _now = DateTime.Now.AddDays(-5).ToString("yyyyMMdd");
           

            string sql=$@" SELECT BM.WORK_DATE,BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as Total,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE
                              FROM
                                (select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE, COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'R%' OR MODEL_NAME LIKE 'M%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%') ERC
                              WHERE ERC.WORK_DATE between {_then} and {_now} AND ERC.GROUP_NAME IN('FT','FT1','FT2','PT','PT1','PT2','NFT','RC','RC1','RC2','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.WORK_DATE
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              LEFT JOIN 
                              (( SELECT *  FROM(
                                  SELECT WORK_DATE, MODEL_NAME,  GROUP_NAME ,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-(SUM(FAIL_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RP1,
                                  ROUND(((SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS ReTestRate,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'R%' OR  MODEL_NAME LIKE 'C%'  OR  MODEL_NAME LIKE 'M%' OR  MODEL_NAME LIKE 'E%' ) 
                                  WHERE  WORK_DATE between {_then} and {_now}   AND GROUP_NAME IN('FT','FT1','FT2','PT','PT1','PT2','NFT','RC','RC1','RC2','RI')
                                  GROUP BY WORK_DATE,MODEL_NAME, GROUP_NAME
                                  ORDER BY ReTestRate DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME
                                WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BM.WORK_DATE=BH.WORK_DATE";


            DataTable _result = conn.reDt(sql);
            List<ErrorCodePCS> _lstResult = ConvertToObj.ConvertDataTable<ErrorCodePCS>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();


           


            return View();
        }
    }
}