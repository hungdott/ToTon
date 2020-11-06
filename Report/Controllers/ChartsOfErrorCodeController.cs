using Report.Common;
using Report.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Models;

namespace Report.Controllers
{
    public class ChartsOfErrorCodeController : Controller
    {
        // GET: ChartsOfErrorCode
        // GET: RRByErrorCode
        ConnectDbSfis conn = new ConnectDbSfis();
        public ActionResult Index()
        {
            var _now = DateTime.Now.ToString("yyyyMMdd");
            var _then = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            string sql = $@"select errCode.WORK_DATE,errCode.MODEL_NAME,errCode.GROUP_NAME,errCode.ERROR_CODE,
                        errCode.test_fail_qty,errCode.fail_qty,station.pass,station.fail,station.repass,station.refail, 
                        ROUND(((errCode.test_fail_qty-errCode.fail_qty)/(station.pass+station.fail+station.repass+station.refail))*100,2) as rate 
                        from (select WORK_DATE,MODEL_NAME,GROUP_NAME,ERROR_CODE,SUM(TEST_FAIL_QTY) as test_fail_qty,SUM(FAIL_QTY) as fail_qty from SFISM4.R_ATE_ERRCODE_T 
                        where  TO_DATE(WORK_DATE,'YYYYMMDD')                        
                        BETWEEN TO_DATE('{_then}','YYYYMMDD') and TO_DATE('{_now}','YYYYMMDD')
                        and WORK_SECTION > 7 and WORK_SECTION < 20 
                        and REGEXP_LIKE (MODEL_NAME, '^[R_C_E](*)') 
                        and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC){{1}}')
                        GROUP BY WORK_DATE,MODEL_NAME,GROUP_NAME,ERROR_CODE) errCode
                        LEFT JOIN (select WORK_DATE,MODEL_NAME,GROUP_NAME,SUM(PASS_QTY) as pass,SUM(FAIL_QTY) as fail,SUM(REPASS_QTY) as repass, SUM(REFAIL_QTY) as refail from SFISM4.R_STATION_REC_T
                        where TO_DATE(WORK_DATE,'YYYYMMDD')                        
                        BETWEEN TO_DATE('{_then}','YYYYMMDD') and TO_DATE('{_now}','YYYYMMDD')
                        and WORK_SECTION > 7 and WORK_SECTION < 20 
                        and REGEXP_LIKE (MODEL_NAME, '^[R_C_E](*)') 
                        and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC){{1}}')
                        GROUP BY WORK_DATE,MODEL_NAME,GROUP_NAME) station
                        ON errCode.WORK_DATE = station.WORK_DATE 
                        and errCode.MODEL_NAME = station.MODEL_NAME 
                        and errCode.GROUP_NAME = station.GROUP_NAME
                        where (station.pass+station.fail+station.repass+station.refail)!=0";



            DataTable _result = conn.reDt(sql);
            List<ErrorCode> _lstResult = ConvertToObj.ConvertDataTable<ErrorCode>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();


            List<string> lstDay = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => x.Key).OrderBy(x => x).ToList();

            List<DataByModel> lstByModel = _lstResult.GroupBy(x => x.MODEL_NAME).Select(xy => new DataByModel()
            {
                MODEL_NAME = xy.Key,

                RateByStation = xy.GroupBy(j => j.GROUP_NAME).Select(j => new RateByStation()
                {
                    GROUP_NAME = j.Key,
                    count_row_station = j.GroupBy(q => q.ERROR_CODE).Count(),
                    RateByErrCode = j.GroupBy(q => q.ERROR_CODE).Select(q => new RateByErrCode()
                    {
                        ERROR_CODE = q.Key,
                        DataByErrCode = (from day in lstDay
                                         join errCode in q.GroupBy(k => k.WORK_DATE).Select(k => new DataByErrCode()
                                         {
                                             WORK_DATE = k.Key,
                                             RATE = k.Sum(a => a.RATE)
                                         }).ToList()
                                         on day equals errCode.WORK_DATE into tb_data //new { X1=day,X2 =day} equals new { X1= errCode.WORK_DATE,X2 = errCode.WORK_DATE} into tb_data
                                         from data in tb_data.DefaultIfEmpty()
                                         orderby day
                                         select new DataByErrCode()
                                         {
                                             RATE = data == null ? -5m : data.RATE,
                                             WORK_DATE = day
                                         }).ToList()

                    }).ToList()

                }).ToList()



            }).ToList();




            List<string> _lstModelName = lstByModel.Select(x => x.MODEL_NAME).ToList();

            ErrorCodeVm errCodeVm = new ErrorCodeVm();
            errCodeVm.days = lstDay;
            errCodeVm.lstDataByModel = lstByModel;
            errCodeVm.ModelNames = _lstModelName;

            return View(errCodeVm);

        }




        public ActionResult Filter(FilterRRByErrorCodeRequest request)
        {

            var _now = request.ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : request.ToDate.Value.ToString("yyyyMMdd");
            var _then = request.FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : request.FromDate.Value.ToString("yyyyMMdd");

            string sql = $@"select errCode.WORK_DATE,errCode.MODEL_NAME,errCode.GROUP_NAME,errCode.ERROR_CODE,
                        errCode.test_fail_qty,errCode.fail_qty,station.pass,station.fail,station.repass,station.refail, 
                        ROUND(((errCode.test_fail_qty-errCode.fail_qty)/(station.pass+station.fail+station.repass+station.refail))*100,2) as rate 
                        from (select WORK_DATE,MODEL_NAME,GROUP_NAME,ERROR_CODE,SUM(TEST_FAIL_QTY) as test_fail_qty,SUM(FAIL_QTY) as fail_qty from SFISM4.R_ATE_ERRCODE_T 
                        where  TO_DATE(WORK_DATE,'YYYYMMDD')                        
                        BETWEEN TO_DATE('{_then}','YYYYMMDD') and TO_DATE('{_now}','YYYYMMDD')";

            if (request.Shift == "1")
            {
                sql += " and WORK_SECTION > 7 and WORK_SECTION < 20 ";
            }
            else if (request.Shift == "2")
            {
                sql += " and (WORK_SECTION < 8 or WORK_SECTION > 19) ";
            }



            sql += $@" and REGEXP_LIKE (MODEL_NAME, '^[R_C_E](*)') 
                        and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC){{1}}')
                        GROUP BY WORK_DATE,MODEL_NAME,GROUP_NAME,ERROR_CODE) errCode
                        LEFT JOIN (select WORK_DATE,MODEL_NAME,GROUP_NAME,SUM(PASS_QTY) as pass,SUM(FAIL_QTY) as fail,SUM(REPASS_QTY) as repass, SUM(REFAIL_QTY) as refail from SFISM4.R_STATION_REC_T
                        where TO_DATE(WORK_DATE,'YYYYMMDD')                        
                        BETWEEN TO_DATE('{_then}','YYYYMMDD') and TO_DATE('{_now}','YYYYMMDD')";

            if (request.Shift == "1")
            {
                sql += " and WORK_SECTION > 7 and WORK_SECTION < 20 ";
            }
            else if (request.Shift == "2")
            {
                sql += " and (WORK_SECTION < 8 or WORK_SECTION > 19) ";
            }

            sql += $@" and REGEXP_LIKE (MODEL_NAME, '^[R_C_E](*)') 
                        and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC){{1}}')
                        GROUP BY WORK_DATE,MODEL_NAME,GROUP_NAME) station
                        ON errCode.WORK_DATE = station.WORK_DATE 
                        and errCode.MODEL_NAME = station.MODEL_NAME 
                        and errCode.GROUP_NAME = station.GROUP_NAME";



            DataTable _result = conn.reDt(sql);
            List<ErrorCode> _lstResult = ConvertToObj.ConvertDataTable<ErrorCode>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();


            List<string> lstDay = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => x.Key).OrderBy(x => x).ToList();


            List<DataByModel> lstByModel = _lstResult.GroupBy(x => x.MODEL_NAME).Select(xy => new DataByModel()
            {
                MODEL_NAME = xy.Key,

                RateByStation = xy.GroupBy(j => j.GROUP_NAME).Select(j => new RateByStation()
                {
                    GROUP_NAME = j.Key,
                    count_row_station = j.GroupBy(q => q.ERROR_CODE).Count(),
                    RateByErrCode = j.GroupBy(q => q.ERROR_CODE).Select(q => new RateByErrCode()
                    {
                        ERROR_CODE = q.Key,
                        DataByErrCode = (from day in lstDay
                                         join errCode in q.GroupBy(k => k.WORK_DATE).Select(k => new DataByErrCode()
                                         {
                                             WORK_DATE = k.Key,
                                             RATE = k.Sum(a => a.RATE)
                                         }).ToList()
                                         on day equals errCode.WORK_DATE into tb_data
                                         from data in tb_data.DefaultIfEmpty()
                                         orderby day
                                         select new DataByErrCode()
                                         {
                                             RATE = data == null ? -5m : data.RATE,
                                             WORK_DATE = day
                                         }).ToList()

                    }).ToList()

                }).ToList()



            }).ToList();


            ErrorCodeVm errCodeVm = new ErrorCodeVm();
            errCodeVm.days = lstDay;
            errCodeVm.lstDataByModel = lstByModel;

            return PartialView("~/Views/RRByErrorCode/RRByErrorPartial.cshtml", errCodeVm);
            //return View(request);
        }
    }
}