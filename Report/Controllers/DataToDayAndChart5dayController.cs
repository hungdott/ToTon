using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Entity;
using System.Data;
using Report.Common;
using Report.Models;
using System.Net;
using Report.EntityModel;
using Newtonsoft.Json;

namespace Report.Controllers
{
    public class DataToDayAndChart5dayController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: DataToDayAndChart5day
        public ActionResult Index()
        {
            string ToDate = DateTime.Now.ToString("yyyyMMdd");
            string FromDate = DateTime.Now.AddDays(-5).ToString("yyyyMMdd");
            string ModelName = "All Model";
            var lstData = new LstData();
            lstData.lstdataByModel = getDataSattionByModel(FromDate, ToDate,ModelName);
            return View(lstData);
        }

        public ActionResult FilterData(FilterDataStationAndChartRequest Request)
        {
            var _now = Request.ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : Request.ToDate.Value.ToString("yyyyMMdd");
            var _then = Request.FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : Request.FromDate.Value.ToString("yyyyMMdd");
            string ModelName =Request.ModelName;
            var lstData = new LstData();
            lstData.lstdataByModel = getDataSattionByModel(_then,_now, ModelName);

            return PartialView("~/Views/DataToDayAndChart5day/DataToDayAndChart5dayPartial.cshtml", lstData);
        }


        public ActionResult Chart(string fromDate, string toDate, string ModelName)
        {
            string sql;
            sql = $@" SELECT *  FROM(
                                  SELECT WORK_DATE, MODEL_NAME,  GROUP_NAME,(SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)) AS INPUT ,SUM(FIRST_FAIL_QTY) AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail, 
                                  
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF(SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR,
                                  100-ROUND((SUM(FAIL_QTY) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS YR
                                   
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%' ) 
                                  WHERE ";
            if (1 == 1)
            {
                
                 sql += $@" WORK_DATE between {fromDate} and {toDate}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT0','PT','PT1','PT2','PT3','NFT','RC','RC1','RC2','RI')  ";
            }
                 
            if (ModelName != "")
            {
                sql += $" and MODEL_NAME={'\'' + ModelName + '\''} ";
            }

            else
            {
                sql += "";
            }
            sql += $@"GROUP BY WORK_DATE,MODEL_NAME, GROUP_NAME
                                  ORDER BY RR DESC
                                 ) WHERE INPUT> 0";



            //between 20200807 and 20200808
            DataTable _result = conn.reDt(sql);
            List<DataToDayAndChart5dayE> _LstResult = ConvertToObj.ConvertDataTable<DataToDayAndChart5dayE>(_result).ToList();
            List<DataRRByModelName> lstByModelName = _LstResult.GroupBy(x => x.MODEL_NAME).Select(xy => new DataRRByModelName()
            {
                MODEL_NAME = xy.Key,
                lstDataByStation = xy.GroupBy(gr => gr.GROUP_NAME).Select(lsts => new DataByGroupName()
                {
                    GroupName = lsts.Key,
                    lstDataByGroupName = lsts.GroupBy(kdas => kdas.WORK_DATE).Select(das => new DataRRBySattionDay()
                    {
                        WORK_DATE = das.Key,
                        lstDataBysation = das.Select(dd => new DataBySattion()
                        {
                            // RATE = data == null ? -5m : data.RATE,
                            RR_s = dd == null ? -5m : dd.RR,
                            
                        }).ToList()

                    }).OrderBy(c=>c.WORK_DATE).ToList()
                }).ToList()
            }).ToList();
           // var result = JsonConvert.SerializeObject(lstByModelName);
            return Json(lstByModelName.FirstOrDefault());

        }




    



    public List<DataRRByModelName> getDataSattionByModel(string fromDate, string toDate,string ModelName)//string fromDate,string toDate 
        {
            //var _now = ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : ToDate.Value.ToString("yyyyMMdd");
            //var then = FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : FromDate.Value.ToString("yyyyMMdd");
            string sql;
            sql = $@" SELECT *  FROM(
                                  SELECT WORK_DATE, MODEL_NAME,  GROUP_NAME,(SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)) AS INPUT ,SUM(FIRST_FAIL_QTY) AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail, 
                                  
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF(SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR,
                                  100-ROUND((SUM(FAIL_QTY) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS YR
                                   
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%' ) 
                                  WHERE ";
           
                sql += $@" WORK_DATE = {toDate}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT0','PT','PT1','PT2','PT3','NFT','RC','RC1','RC2','RI')  ";
              

            

            if (ModelName == "All Model")
            {
                sql += "";
            }
            else if (ModelName != "")
            {
                sql += $" and MODEL_NAME={'\'' + ModelName + '\''} ";
            }

            else
            {
                sql += "";
            }
            sql += $@"GROUP BY WORK_DATE,MODEL_NAME, GROUP_NAME
                                  ORDER BY RR DESC
                                 ) WHERE INPUT> 0";



            //between 20200807 and 20200808
            DataTable _result = conn.reDt(sql);
            List<DataToDayAndChart5dayE> _LstResult = ConvertToObj.ConvertDataTable<DataToDayAndChart5dayE>(_result).ToList();
            List<DataRRByModelName> lstByModelName = _LstResult.GroupBy(x => x.MODEL_NAME).Select(xy => new DataRRByModelName()
            {
                MODEL_NAME = xy.Key,
                lstDataByStation = xy.GroupBy(gr => gr.GROUP_NAME).Select(lsts => new DataByGroupName()
                {
                    GroupName = lsts.Key,
                    lstDataByGroupName = lsts.GroupBy(kdas => kdas.WORK_DATE).Select(das => new DataRRBySattionDay()
                    {
                        WORK_DATE = das.Key,
                        lstDataBysation = das.Select(dd => new DataBySattion()
                        {
                            // RATE = data == null ? -5m : data.RATE,
                            INPUT = dd == null ? -5m : dd.INPUT,
                            SUM_PASS = dd.SUM_PASS,
                            SUM_FAIL = dd.SUM_FAIL,
                            FIRST_FAIL = dd == null ? -5m : dd.FIRST_FAIL,
                            SUM_REPASS = dd == null ? -5m : dd.SUM_REPASS,
                            SUM_RE_FAIL = dd == null ? -5m : dd.SUM_RE_FAIL,
                            RR_s = dd == null ? -5m : dd.RR,
                            YR_s = dd == null ? -5m : dd.YR,
                        }).ToList()

                    }).ToList()
                }).ToList()
            }).ToList();
            return lstByModelName;

        }




        



    }
}