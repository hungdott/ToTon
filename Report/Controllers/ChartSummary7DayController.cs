using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Entity;
using System.Data;
using Report.Common;
using Report.Models;
using Newtonsoft.Json;
using System.Dynamic;
using Report.EntityModel;
using System.Net;

namespace Report.Controllers
{
    public class ChartSummary7DayController : Controller
    {
        ReportSysContext context = new ReportSysContext();
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: TopErrorByStation
        protected bool EqualsUpToDay(DateTime dt1, DateTime dt2)
        {
            dt1 = dt1.AddDays(1);
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day;
        }
        public ActionResult Index()
        {

            var _now = DateTime.Now.ToString("yyyyMMdd");
            var _nextDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            string shift = "";
            string option = "all";
            string groupName = "";
            string modelName = "";
            int pcs = 0;
            int FromHour = 30;
            int ToHour = 30;

            string statusLine = "allLine";
            var lstByGroupName = GetData(_now, _nextDate, shift, option, groupName, modelName, pcs, FromHour, ToHour, statusLine);
            var data = new TopData();

            data.listData = lstByGroupName;
            data.WorkDate = DateTime.ParseExact(_now, "yyyyMMdd", null);

            List<string> lstModel = new List<string>();
            foreach (var gr in lstByGroupName)
            {
                foreach (var md in gr.DataByModelName)
                {
                    lstModel.Add(md.ModelName);
                }
            }

            var dataModel = lstModel.GroupBy(x => x).Select(x => x.Key).OrderBy(x => x).ToList();
            var ModelInTwoMonth = ModelInTowMonth();
            ViewBag.lstmodel = dataModel;
            ViewBag.lstmodelInTwoMonth = ModelInTwoMonth;
            return View(data);
        }

        public ActionResult GetTop3(TopRRByStationRequest request)
        {

            var _now = request.Date == null ? DateTime.Now.ToString("yyyyMMdd") : request.Date.Value.ToString("yyyyMMdd");
            var NextDate = DateTime.ParseExact(request.Date.Value.ToString("yyyyMMdd"), "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");
            //var NextDate = request.NextDate == null ? DateTime.Now.AddDays(1).ToString("yyyyMMdd") : request.NextDate.Value.ToString("yyyyMMdd");
            string shift = request.Shift;
            // bool option = request.Option;
            string option = request.Option;
            string groupName = request.group_name == null ? "" : request.group_name;
            string modelName = request.ModelName == null ? "" : request.ModelName;
            int pcs = request.PCS == null ? 0 : request.PCS;
            int fromHour = request.fromHour;
            int toHour = request.toHour;
            string statusLine = request.StatusLine;
            var lstByGroupName = GetData(_now, NextDate, shift, option, groupName, modelName, pcs, fromHour, toHour, statusLine);
            var data = new TopData();
            data.listData = lstByGroupName;
            data.WorkDate = DateTime.ParseExact(_now, "yyyyMMdd", null);
            return PartialView("~/Views/TopErrorByStation/TopErrorByStationPartial.cshtml", data);//lstByGroupName
        }

        //Show ang hide pac name
        public ActionResult statusPcName(TopRRByStationRequestgetPCName request)
        {

            var _now = request.Date == null ? DateTime.Now.ToString("yyyyMMdd") : request.Date.Value.ToString("yyyyMMdd");
            var NextDate = DateTime.ParseExact(request.Date.Value.ToString("yyyyMMdd"), "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");
            //var NextDate = request.NextDate == null ? DateTime.Now.AddDays(1).ToString("yyyyMMdd") : request.NextDate.Value.ToString("yyyyMMdd");
            string shift = request.Shift;
            // bool option = request.Option;
            string option = request.Option;
            string groupName = request.group_name == null ? "" : request.group_name;
            string modelName = request.ModelName == null ? "" : request.ModelName;
            int pcs = request.PCS == null ? 0 : request.PCS;
            int fromHour = request.fromHour;
            int toHour = request.toHour;
            string statusLine = request.StatusLine;
            var lstByGroupName = GetData(_now, NextDate, shift, option, groupName, modelName, pcs, fromHour, toHour, statusLine);
            var data = new TopData();
            data.listData = lstByGroupName;
            data.WorkDate = DateTime.ParseExact(_now, "yyyyMMdd", null);
            var dd = data;
            var statusPc = request.StatusPc;
            if (request.StatusPc == "ShowPc")
            {
                return PartialView("TopErrorByStationPartial", data);
            }
            else if (request.StatusPc == "HidePc")
            {
                return PartialView("TopErrorByStationHidePCtPartial", data);
            }
            else
            {
                return PartialView("TopErrorByStationPartial", data);
            }




        }

        //Get Data To Show
        public List<TOPDataByStation> GetData(string date, string nextDate, string Shift, string option, string Group_name, string Model_name, int pcs, int fromHour, int toHour, string statusLine)//string date, bool option, string Group_name, string Model_name
        {

            string _sql = "";
            _sql = $@"SELECT * FROM (SELECT BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RATE_STATION,BH.Y_RATE_STATION,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as TOTAL_STATION,
                              BH.First_Fail,BH.Sum_FAIL,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE_EC
                              FROM
                                (select ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE (MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%')) ERC   ";

            if (fromHour == 30 && toHour == 30)
            {
                if (Shift == "1")
                {
                    _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20";
                }
                else if (Shift == "2")
                {
                    _sql += $@"  WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7)) ";

                }

                else if (Shift == "0")
                {

                    // _sql += $@"  WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7)) or (ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20) ";
                    // _sql += $@"  WHERE  (ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7) or (ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20) ";
                    _sql += $@" WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE={date} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={nextDate} and ERC.WORK_SECTION <=7)) ";
                }
                else
                {
                    _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20";
                }
            }
            else
            {
                _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >{fromHour} and ERC.WORK_SECTION <= {toHour}";
                //_sql += $@" WHERE ERC.WORK_DATE ={date} and (ERC.WORK_SECTION >={fromHour} and ERC.WORK_SECTION < {toHour})";
            }


            // LEFT JOIN 
            //

            _sql += $@" AND ERC.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              LEFT JOIN 
                              (( SELECT *  FROM(
                                  SELECT MODEL_NAME,  GROUP_NAME,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RATE_STATION,  
                                  ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS Y_RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  
                                                                                MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%') ";
            if (fromHour == 30 && toHour == 30)
            {
                if (Shift == "1")
                {
                    _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
                else if (Shift == "2")
                {

                    _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) ";
                }

                else if (Shift == "0")
                {
                    // _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) or (WORK_DATE ={date} and WORK_SECTION >7 and WORK_SECTION < 20) ";
                    // _sql += $@"  WHERE (WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7) or (WORK_DATE ={date} and WORK_SECTION >7 and WORK_SECTION < 20) ";
                    _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE={date} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7)) ";
                }
                else
                {
                    _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
            }
            else
            {
                _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>{fromHour} and WORK_SECTION<= {toHour} ";
                // _sql += $@" WHERE  WORK_DATE= {date}  and (WORK_SECTION>={fromHour} and WORK_SECTION< {toHour}) ";
            }


            _sql += $@"AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY MODEL_NAME, GROUP_NAME
                                  ORDER BY RATE_STATION DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME";

            if (fromHour == 30 && toHour == 30)
            {
                _sql += $@" WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>{pcs}  ";
            }
            else
            {
                _sql += $@" WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>1  ";
            }


            if (Group_name == "All Station")
            {
                _sql += "";
            }
            else if (Group_name != "")
            {
                _sql += $" and BH.GROUP_NAME ={'\'' + Group_name + '\''} ";
            }
            else
            {
                _sql += "";
            }


            if (Model_name == "All Model")
            {
                _sql += "";
            }
            else if (Model_name != "")
            {
                _sql += $" and BH.MODEL_NAME={'\'' + Model_name + '\''} ";
            }

            else
            {
                _sql += "";
            }

            _sql += $@") bbb left join 
                                    (select SFIS1.C_ERROR_CODE_T.ERROR_CODE,ERROR_DESC2 from SFIS1.C_ERROR_CODE_T ) BH                                
                                    on bbb.ERROR_CODE = BH.ERROR_CODE ";

            string sql = _sql;





            DataTable _result = conn.reDt(sql);
            List<Top3ErrorCodeByStation> _lstResult = ConvertToObj.ConvertDataTable<Top3ErrorCodeByStation>(_result).ToList();


            List<TOPDataByStation> lstByGroupName = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xy => new TOPDataByStation()
            {
                GroupName = xy.Key,
                count_row_station = xy.GroupBy(rs => rs.STATION_NAME).Count(),
                count_row_station1 = xy.GroupBy(rm => rm.STATION_NAME).Count(),
                count_model = xy.GroupBy(countM => countM.MODEL_NAME).Count(),


                DataByModelName = xy.GroupBy(j => j.MODEL_NAME).Select(ex => new DataByModelName()
                {
                    ModelName = ex.Key,

                    Input = ex.Max(a => a.TOTAL_STATION),
                    Frist_fail_station = ex.Max(ffs => ffs.FIRST_FAIL),
                    Fail_Of_station_Name = ex.Max(fm => fm.SUM_FAIL),

                    RR = ex.Max(rr => rr.RATE_STATION),
                    YR = ex.Max(yr => yr.Y_RATE_STATION),
                    count_row_ModelName = ex.GroupBy(rm => rm.STATION_NAME).Count(),//jhfjhjh
                    count_row_ERR_in_model = ex.GroupBy(count_err => count_err.ERROR_CODE).Count(),

                    FailByErrorCode = ex.GroupBy(e => e.ERROR_CODE).Select(k => new FailByErrorCode()
                    {
                        ErrorCode = k.Key,
                        ErrorDESC2 = k.Select(sdfgsjdf => sdfgsjdf.ERROR_DESC2).Last(),
                        count_row_Errorcode = k.GroupBy(rrr => rrr.STATION_NAME).Count(),
                        FailQtyErrCode = k.Sum(q => q.SUM_TEST_FAIL_ER),
                        //FailQtyErrCode = k.OrderByDescending(asda=>asda.Sum(q => q.SUM_TEST_FAIL_ER)),

                        SecondFailQtyErrCode = k.Sum(ffer => ffer.SUM_FAIL_ER),
                        FailByMachine = k.Select(ee => new FailByMachine()
                        {
                            PCName = ee.STATION_NAME,
                            FailQtyPC = ee.SUM_TEST_FAIL_ER,
                            ReFail = ee.SUM_FAIL_ER,
                        }).OrderByDescending(errc => errc.FailQtyPC).ToList(),


                    }).OrderByDescending(sfhskdf => sfhskdf.FailQtyErrCode).ToList()


                }).OrderByDescending(x => x.RR).ToList()
            }).OrderBy(grn => grn.GroupName).ToList();




            //get Line change or no change
            //if(statusLine== "LineChange")
            //{
            //    foreach (var grName in lstByGroupName)
            //    {
            //        grName.DataByModelName = grName.DataByModelName.ToList();
            //        grName.count_model = grName.DataByModelName.Count;
            //        foreach (var item in grName.DataByModelName)
            //        {
            //            item.FailByErrorCode = item.FailByErrorCode
            //                                .OrderByDescending(x => x.FailByMachine.Select(pc=>pc.PCName.Contains("L01,L02,L03,L04")))
            //                                .ToList();
            //            item.count_row_ERR_in_model = item.FailByErrorCode.Count;
            //        }
            //    }
            //}
            //else if(statusLine== "NoChange")
            //{
            //    foreach (var grName in lstByGroupName)
            //    {
            //        grName.DataByModelName = grName.DataByModelName.ToList();
            //        grName.count_model = grName.DataByModelName.Count;
            //        foreach (var item in grName.DataByModelName)
            //        {
            //            item.FailByErrorCode = item.FailByErrorCode
            //                                .OrderByDescending(x => x.FailByMachine.Select(pc => pc.PCName.Contains("L05,L06,L07,L08")))
            //                                .ToList();
            //            item.count_row_ERR_in_model = item.FailByErrorCode.Count;
            //        }
            //    }
            //}

            //



            //take top 3
            if (option == "top3Er")
            {

                foreach (var grName in lstByGroupName)
                {
                    grName.DataByModelName = grName.DataByModelName.ToList();
                    grName.count_model = grName.DataByModelName.Count;
                    foreach (var item in grName.DataByModelName)
                    {
                        item.FailByErrorCode = item.FailByErrorCode
                                            .OrderByDescending(x => x.FailQtyErrCode)
                                            .Take(3).ToList();
                        item.count_row_ERR_in_model = item.FailByErrorCode.Count;
                    }
                }
            }
            else if (option == "top3Mo")
            {
                foreach (var grName in lstByGroupName)
                {
                    grName.DataByModelName = grName.DataByModelName.Take(3).ToList();
                    grName.count_model = grName.DataByModelName.Count;
                    foreach (var item in grName.DataByModelName)
                    {
                        item.FailByErrorCode = item.FailByErrorCode
                                            .OrderByDescending(x => x.FailQtyErrCode).ToList();
                        item.count_row_ERR_in_model = item.FailByErrorCode.Count;
                    }
                }
            }
            else if (option == "topMoER")
            {
                foreach (var grName in lstByGroupName)
                {
                    grName.DataByModelName = grName.DataByModelName.Take(3).ToList();
                    grName.count_model = grName.DataByModelName.Count;
                    foreach (var item in grName.DataByModelName)
                    {
                        item.FailByErrorCode = item.FailByErrorCode
                                            .OrderByDescending(x => x.FailQtyErrCode)
                                            .Take(3).ToList();
                        item.count_row_ERR_in_model = item.FailByErrorCode.Count;
                    }
                }
            }



            return lstByGroupName;

        }

        public List<TOPDataByStation> GetData11(string date, string nextDate, string Shift, string option, string Group_name, string Model_name, int pcs, int fromHour, int toHour, string statusLine)//string date, bool option, string Group_name, string Model_name
        {

            string _sql = "";
            _sql = $@"SELECT * FROM (SELECT BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RATE_STATION,BH.Y_RATE_STATION,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as TOTAL_STATION,
                              BH.First_Fail,BH.Sum_FAIL,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE_EC
                              FROM
                                (select ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE (MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%')) ERC   ";

            if (fromHour == 30 && toHour == 30)
            {
                if (Shift == "1")
                {
                    _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20";
                }
                else if (Shift == "2")
                {
                    _sql += $@"  WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7)) ";

                }

                else if (Shift == "0")
                {

                    // _sql += $@"  WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7)) or (ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20) ";
                    // _sql += $@"  WHERE  (ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7) or (ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20) ";
                    _sql += $@" WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE={date} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={nextDate} and ERC.WORK_SECTION <=7)) ";
                }
                else
                {
                    _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >7 and ERC.WORK_SECTION < 20";
                }
            }
            else
            {
                _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >{fromHour} and ERC.WORK_SECTION <= {toHour}";
                //_sql += $@" WHERE ERC.WORK_DATE ={date} and (ERC.WORK_SECTION >={fromHour} and ERC.WORK_SECTION < {toHour})";
            }


            // LEFT JOIN 

            _sql += $@" AND ERC.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              right JOIN 
                              (( SELECT *  FROM(
                                  SELECT MODEL_NAME,  GROUP_NAME,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS Y_RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  
                                                                                MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%') ";
            if (fromHour == 30 && toHour == 30)
            {
                if (Shift == "1")
                {
                    _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
                else if (Shift == "2")
                {

                    _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) ";
                }

                else if (Shift == "0")
                {
                    // _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) or (WORK_DATE ={date} and WORK_SECTION >7 and WORK_SECTION < 20) ";
                    // _sql += $@"  WHERE (WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7) or (WORK_DATE ={date} and WORK_SECTION >7 and WORK_SECTION < 20) ";
                    _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE={date} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7)) ";
                }
                else
                {
                    _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
            }
            else
            {
                _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>{fromHour} and WORK_SECTION<= {toHour} ";
                // _sql += $@" WHERE  WORK_DATE= {date}  and (WORK_SECTION>={fromHour} and WORK_SECTION< {toHour}) ";
            }


            _sql += $@"AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY MODEL_NAME, GROUP_NAME
                                  ORDER BY RATE_STATION DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME";

            if (fromHour == 30 && toHour == 30)
            {
                _sql += $@" WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>{pcs}  ";
            }
            else
            {
                _sql += $@" WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>1  ";
            }

            //_sql+= $@") bbb left join 
            //                        (select SFIS1.C_ERROR_CODE_T.ERROR_CODE,ERROR_DESC2 from SFIS1.C_ERROR_CODE_T ) BH                                
            //                        on bbb.ERROR_CODE = BH.ERROR_CODE ";



            //and BH.MODEL_NAME= 'RAX20-100NASV1'
            if (Group_name == "All Station")
            {
                _sql += "";
            }
            else if (Group_name != "")
            {
                _sql += $" and BH.GROUP_NAME ={'\'' + Group_name + '\''} ";
            }
            else
            {
                _sql += "";
            }


            if (Model_name == "All Model")
            {
                _sql += "";
            }
            else if (Model_name != "")
            {
                _sql += $" and BH.MODEL_NAME={'\'' + Model_name + '\''} ";
            }

            else
            {
                _sql += "";
            }

            _sql += $@") bbb left join 
                                    (select SFIS1.C_ERROR_CODE_T.ERROR_CODE,ERROR_DESC2 from SFIS1.C_ERROR_CODE_T ) BH                                
                                    on bbb.ERROR_CODE = BH.ERROR_CODE ";

            string sql = _sql;





            DataTable _result = conn.reDt(sql);
            List<Top3ErrorCodeByStation> _lstResult = ConvertToObj.ConvertDataTable<Top3ErrorCodeByStation>(_result).ToList();


            List<TOPDataByStation> lstByGroupName = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xy => new TOPDataByStation()
            {
                GroupName = xy.Key,
                count_row_station = xy.GroupBy(rs => rs.STATION_NAME).Count(),
                count_row_station1 = xy.GroupBy(rm => rm.STATION_NAME).Count(),
                count_model = xy.GroupBy(countM => countM.MODEL_NAME).Count(),


                DataByModelName = xy.GroupBy(j => j.MODEL_NAME).Select(ex => new DataByModelName()
                {
                    ModelName = ex.Key,

                    Input = ex.Max(a => a.TOTAL_STATION),
                    Frist_fail_station = ex.Max(ffs => ffs.FIRST_FAIL),
                    Fail_Of_station_Name = ex.Max(fm => fm.SUM_FAIL),

                    RR = ex.Max(rr => rr.RATE_STATION),
                    YR = ex.Max(yr => yr.Y_RATE_STATION),
                    count_row_ModelName = ex.GroupBy(rm => rm.STATION_NAME).Count(),//jhfjhjh
                    count_row_ERR_in_model = ex.GroupBy(count_err => count_err.ERROR_CODE).Count(),

                    FailByErrorCode = ex.GroupBy(e => e.ERROR_CODE).Select(k => new FailByErrorCode()
                    {
                        ErrorCode = k.Key,
                        ErrorDESC2 = k.Select(sdfgsjdf => sdfgsjdf.ERROR_DESC2).Last(),
                        count_row_Errorcode = k.GroupBy(rrr => rrr.STATION_NAME).Count(),
                        FailQtyErrCode = k.Sum(q => q.SUM_TEST_FAIL_ER),
                        //FailQtyErrCode = k.OrderByDescending(asda=>asda.Sum(q => q.SUM_TEST_FAIL_ER)),

                        SecondFailQtyErrCode = k.Sum(ffer => ffer.SUM_FAIL_ER),
                        FailByMachine = k.Select(ee => new FailByMachine()
                        {
                            PCName = ee.STATION_NAME,
                            FailQtyPC = ee.SUM_TEST_FAIL_ER,
                            ReFail = ee.SUM_FAIL_ER,
                        }).OrderByDescending(errc => errc.FailQtyPC).ToList(),


                    }).OrderByDescending(sfhskdf => sfhskdf.FailQtyErrCode).ToList()


                }).OrderByDescending(x => x.RR).ToList()
            }).ToList();




            //get Line change or no change
            //if(statusLine== "LineChange")
            //{
            //    foreach (var grName in lstByGroupName)
            //    {
            //        grName.DataByModelName = grName.DataByModelName.ToList();
            //        grName.count_model = grName.DataByModelName.Count;
            //        foreach (var item in grName.DataByModelName)
            //        {
            //            item.FailByErrorCode = item.FailByErrorCode
            //                                .OrderByDescending(x => x.FailByMachine.Select(pc=>pc.PCName.Contains("L01,L02,L03,L04")))
            //                                .ToList();
            //            item.count_row_ERR_in_model = item.FailByErrorCode.Count;
            //        }
            //    }
            //}
            //else if(statusLine== "NoChange")
            //{
            //    foreach (var grName in lstByGroupName)
            //    {
            //        grName.DataByModelName = grName.DataByModelName.ToList();
            //        grName.count_model = grName.DataByModelName.Count;
            //        foreach (var item in grName.DataByModelName)
            //        {
            //            item.FailByErrorCode = item.FailByErrorCode
            //                                .OrderByDescending(x => x.FailByMachine.Select(pc => pc.PCName.Contains("L05,L06,L07,L08")))
            //                                .ToList();
            //            item.count_row_ERR_in_model = item.FailByErrorCode.Count;
            //        }
            //    }
            //}

            //



            //take top 3
            if (option == "top3Er")
            {

                foreach (var grName in lstByGroupName)
                {
                    grName.DataByModelName = grName.DataByModelName.ToList();
                    grName.count_model = grName.DataByModelName.Count;
                    foreach (var item in grName.DataByModelName)
                    {
                        item.FailByErrorCode = item.FailByErrorCode
                                            .OrderByDescending(x => x.FailQtyErrCode)
                                            .Take(3).ToList();
                        item.count_row_ERR_in_model = item.FailByErrorCode.Count;
                    }
                }
            }
            else if (option == "top3Mo")
            {
                foreach (var grName in lstByGroupName)
                {
                    grName.DataByModelName = grName.DataByModelName.Take(3).ToList();
                    grName.count_model = grName.DataByModelName.Count;
                    foreach (var item in grName.DataByModelName)
                    {
                        item.FailByErrorCode = item.FailByErrorCode
                                            .OrderByDescending(x => x.FailQtyErrCode).ToList();
                        item.count_row_ERR_in_model = item.FailByErrorCode.Count;
                    }
                }
            }
            else if (option == "topMoER")
            {
                foreach (var grName in lstByGroupName)
                {
                    grName.DataByModelName = grName.DataByModelName.Take(3).ToList();
                    grName.count_model = grName.DataByModelName.Count;
                    foreach (var item in grName.DataByModelName)
                    {
                        item.FailByErrorCode = item.FailByErrorCode
                                            .OrderByDescending(x => x.FailQtyErrCode)
                                            .Take(3).ToList();
                        item.count_row_ERR_in_model = item.FailByErrorCode.Count;
                    }
                }
            }



            return lstByGroupName;

        }


        //Model Name run 2 month
        public List<ModelNameLaster> ModelInTowMonth()
        {

            var Today = DateTime.Now.ToString("yyyyMMdd");
            var Fromday = DateTime.Now.AddDays(-60).ToString("yyyyMMdd").ToString();

            string sql = $@"select distinct MODEL_NAME
                            from SFISM4.R_ATE_ERRCODE_T 
                            where REGEXP_LIKE(MODEL_NAME, '^(R|M|E|C|D|L|N|X|O|V6510){{1}}')
                            and WORK_DATE between {Fromday} and {Today}
                          ";
            DataTable _result = conn.reDt(sql);
            List<ModelNameLaster> _lstResult = ConvertToObj.ConvertDataTable<ModelNameLaster>(_result).ToList();
            var mm = _lstResult;
            return _lstResult;
        }
        //data ta in 2 month
        public ActionResult findDateLaster(string Model_name)
        {
            var toDay = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            var FromDay = DateTime.Now.AddDays(-60).ToString("yyyyMMdd");

            //R_STATION_REC_T
            string sql_getModelAndDate = $@"select WORK_DATE,WORK_SECTION
                                    from SFISM4.R_ATE_ERRCODE_T
                                    WHERE  WORK_DATE between {FromDay} and {toDay} and MODEL_NAME={'\'' + Model_name + '\''}
                                            and GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                    GROUP BY WORK_DATE,WORK_SECTION";



            DataTable _resultModelAndDate = conn.reDt(sql_getModelAndDate);
            List<ModelAndDate> _lstResultModelAndDate = ConvertToObj.ConvertDataTable<ModelAndDate>(_resultModelAndDate).ToList();





            var GetthisDate = _lstResultModelAndDate.OrderByDescending(xx => xx.WORK_DATE).First().WORK_DATE;
            var t = GetthisDate;

            var thisDate = "";
            foreach (var item in _lstResultModelAndDate.OrderByDescending(x => x.WORK_DATE).OrderByDescending(h => h.WORK_SECTION))
            {

                if (item.WORK_DATE.Contains(GetthisDate) && item.WORK_SECTION > 7)
                {
                    thisDate = GetthisDate;
                    break;
                }
                else if (item.WORK_DATE.Contains(GetthisDate) && item.WORK_SECTION <= 7)
                {
                    thisDate = DateTime.ParseExact(item.WORK_DATE, "yyyyMMdd", null).AddDays(-1).ToString("yyyyMMdd");
                    break;

                }



            }




            var nextDate = DateTime.ParseExact(thisDate, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");


            string sql_getData = $@"SELECT * FROM (SELECT BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RATE_STATION,BH.Y_RATE_STATION,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as TOTAL_STATION,
                              BH.First_Fail,BH.Sum_FAIL,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE_EC
                              FROM
                                (select ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T) ERC WHERE ERC.WORK_DATE between {thisDate} and {nextDate} AND ((ERC.WORK_DATE={thisDate} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={nextDate} and ERC.WORK_SECTION <=7)) AND ERC.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              LEFT JOIN 
                              (( SELECT *  FROM(
                                  SELECT MODEL_NAME,  GROUP_NAME,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RATE_STATION,  
                                  ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS Y_RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T )  WHERE WORK_DATE between {thisDate} and {nextDate} AND ((WORK_DATE={thisDate} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7))  AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY MODEL_NAME, GROUP_NAME
                                  ORDER BY RATE_STATION DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>0   and BH.MODEL_NAME={'\'' + Model_name + '\''} and BM.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI') ) bbb left join 
                                    (select SFIS1.C_ERROR_CODE_T.ERROR_CODE,ERROR_DESC2 from SFIS1.C_ERROR_CODE_T ) BH                                
                                    on bbb.ERROR_CODE = BH.ERROR_CODE ";

            string test = sql_getData;
            DataTable _result = conn.reDt(sql_getData);
            List<Top3ErrorCodeByStation> _lstResult = ConvertToObj.ConvertDataTable<Top3ErrorCodeByStation>(_result).ToList();

            List<TOPDataByStation> lstByGroupName = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xy => new TOPDataByStation()
            {
                GroupName = xy.Key,
                count_row_station = xy.GroupBy(rs => rs.STATION_NAME).Count(),
                count_row_station1 = xy.GroupBy(rm => rm.STATION_NAME).Count(),
                count_model = xy.GroupBy(countM => countM.MODEL_NAME).Count(),


                DataByModelName = xy.GroupBy(j => j.MODEL_NAME).Select(ex => new DataByModelName()
                {
                    ModelName = ex.Key,

                    Input = ex.Max(a => a.TOTAL_STATION),
                    Frist_fail_station = ex.Max(ffs => ffs.FIRST_FAIL),
                    Fail_Of_station_Name = ex.Max(fm => fm.SUM_FAIL),

                    RR = ex.Max(rr => rr.RATE_STATION),
                    YR = ex.Max(yr => yr.Y_RATE_STATION),
                    count_row_ModelName = ex.GroupBy(rm => rm.STATION_NAME).Count(),//jhfjhjh
                    count_row_ERR_in_model = ex.GroupBy(count_err => count_err.ERROR_CODE).Count(),

                    FailByErrorCode = ex.GroupBy(e => e.ERROR_CODE).Select(k => new FailByErrorCode()
                    {
                        ErrorCode = k.Key,
                        ErrorDESC2 = k.Select(sdfgsjdf => sdfgsjdf.ERROR_DESC2).Last(),
                        count_row_Errorcode = k.GroupBy(rrr => rrr.STATION_NAME).Count(),
                        FailQtyErrCode = k.Sum(q => q.SUM_TEST_FAIL_ER),
                        //FailQtyErrCode = k.OrderByDescending(asda=>asda.Sum(q => q.SUM_TEST_FAIL_ER)),

                        SecondFailQtyErrCode = k.Sum(ffer => ffer.SUM_FAIL_ER),
                        FailByMachine = k.Select(ee => new FailByMachine()
                        {
                            PCName = ee.STATION_NAME,
                            FailQtyPC = ee.SUM_TEST_FAIL_ER,
                            ReFail = ee.SUM_FAIL_ER,
                        }).OrderByDescending(errc => errc.FailQtyPC).ToList(),


                    }).OrderByDescending(sfhskdf => sfhskdf.FailQtyErrCode).ToList()


                }).OrderByDescending(x => x.RR).ToList()
            }).OrderBy(grn => grn.GroupName).ToList();

            var ll = lstByGroupName;


            var getdata = new TopData();
            getdata.listData = lstByGroupName;
            return PartialView("~/Views/TopErrorByStation/TopErrorByStationPartial.cshtml", getdata);

        }



        public string GetStation(DateTime? day, DateTime? Nextday, string shift)
        {
            if (day.HasValue)
            {
                string dataSt = GetStationByDay(day.Value);
                string dataMo = GetModelByDay(day.Value, Nextday.Value, shift);
                dynamic dataResult = new ExpandoObject();
                dataResult.lstStation = dataSt;
                dataResult.lstModel = dataMo;
                string result = JsonConvert.SerializeObject(dataResult);
                return result;
            }
            return "";

        }
        public string GetModel(DateTime? day, DateTime? nextDate, string station)
        {
            string sql;
            List<string> LisModelName = new List<string>();
            if (day.HasValue)
            {
                string date = day.Value.ToString("yyyyMMdd");
                string nextdate = nextDate.Value.ToString("yyyyMMdd");
                //sql = $@"select distinct MODEL_NAME from SFISM4.R_ATE_ERRCODE_T where WORK_DATE between {date} and {nextdate} ";//where WORK_DATE between {date} and {nextdate}

                sql = $@"select distinct MODEL_NAME from SFISM4.R_STATION_REC_T where ((WORK_DATE = {date}  and WORK_SECTION>7 and WORK_SECTION<=23) or  (WORK_DATE ={nextdate} and WORK_SECTION<=7))  ";
                if (station == "All Station")
                {
                    sql += "";
                }
                else if (station != "")
                {
                    sql += $@" and GROUP_NAME ='{station}' ";
                }
                else
                {
                    sql += "";
                }

                sql += $@" and  (MODEL_NAME LIKE 'R%' 
                                OR MODEL_NAME LIKE 'M%'
                                OR MODEL_NAME LIKE 'E%'
                                OR MODEL_NAME LIKE 'C%'
                                OR MODEL_NAME LIKE 'D%'
                                OR MODEL_NAME LIKE 'L%'
                                OR MODEL_NAME LIKE 'N%'
                                OR MODEL_NAME LIKE 'G%'
                                OR MODEL_NAME LIKE 'X%'
                                OR MODEL_NAME LIKE 'O%'
                                OR MODEL_NAME LIKE 'V6510%'
                                OR MODEL_NAME LIKE 'U12H%') ";

                DataTable _result = conn.reDt(sql);
                if (_result.Rows.Count > 0)
                {
                    var lstMd = (from DataRow dr in _result.Rows
                                 select new
                                 {
                                     ModelName = dr["MODEL_NAME"].ToString(),
                                 }).ToList();

                    foreach (var item in lstMd)
                    {
                        LisModelName.Add(item.ModelName);
                    }
                }

            }
            string modelName = JsonConvert.SerializeObject(LisModelName);
            return modelName;

        }


        public string GetStationByDay(DateTime day)
        {
            string date = day.ToString("yyyyMMdd");
            string sql = $"select distinct GROUP_NAME from SFISM4.R_STATION_REC_T where WORK_DATE = {date} and GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')";
            DataTable _result = conn.reDt(sql);

            if (_result.Rows.Count > 0)
            {
                var lstSt = (from DataRow dr in _result.Rows
                             select new
                             {

                                 StationName = dr["GROUP_NAME"].ToString(),

                             }).ToList();
                return JsonConvert.SerializeObject(lstSt.Select(x => x.StationName).ToList());
            }
            return "";
        }


        public string GetModelByDay(DateTime day, DateTime Nextday, string shift)
        {
            string date = day.ToString("yyyyMMdd");
            string nextDate = Nextday.ToString("yyyyMMdd");

            string sql = $@"select distinct MODEL_NAME 
                            from SFISM4.R_STATION_REC_T";
            if (shift == "1")
            {
                sql += $@" where (WORK_DATE = {date}  and WORK_SECTION>7 and WORK_SECTION<=23)";
            }
            else if (shift == "2")
            {
                sql += $@" where ((WORK_DATE = {date}  and WORK_SECTION>=20) or  (WORK_DATE ={nextDate} and WORK_SECTION<=7))";
            }
            else if (shift == "0")
            {
                sql += $@" where ((WORK_DATE = {date}  and WORK_SECTION>7 and WORK_SECTION<=23) or  (WORK_DATE ={nextDate} and WORK_SECTION<=7))";
            }
            else
            {
                sql += $@" where ((WORK_DATE = {date}  and WORK_SECTION>7 and WORK_SECTION<=23) or  (WORK_DATE ={nextDate} and WORK_SECTION<=7))";
            }
            //sql += $@" where ((WORK_DATE = {date}  and WORK_SECTION>7 and WORK_SECTION<=23) or  (WORK_DATE ={nextDate} and WORK_SECTION<=7))";

            sql += $@" and REGEXP_LIKE(MODEL_NAME, '^(R|M|E|C|D|L|N|G|X|O|U12H|V6510){{1}}')
                         AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')";
            DataTable _result = conn.reDt(sql);

            if (_result.Rows.Count > 0)
            {
                var lstSt = (from DataRow dr in _result.Rows
                             select new
                             {
                                 ModelName = dr["MODEL_NAME"].ToString(),

                             }).ToList();
                return JsonConvert.SerializeObject(lstSt.Select(x => x.ModelName).ToList());
            }
            return "";

        }

        //Main chart 7 day
        public ActionResult showChartByModelOneStation(string DataTime, string ModelName, string StationName)
        {
            string toDate = DateTime.Now.ToString("yyyyMMdd");
            string fromDate = DateTime.Now.AddDays(-60).ToString("yyyyMMdd");

            var nextDay1 = DateTime.ParseExact(DataTime, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");

            string sql;
            sql = $@" SELECT *  FROM(
                                  SELECT WORK_SECTION, WORK_DATE, MODEL_NAME,GROUP_NAME,(SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)) AS INPUT ,SUM(FIRST_FAIL_QTY) AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail, 
                                  
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF(SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR,
                                  100-ROUND((SUM(FAIL_QTY) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS YR
                                   
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%' or MODEL_NAME LIKE 'X%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%' ) 
                                  WHERE ";
            if (1 == 1)
            {

                sql += $@" WORK_DATE between {fromDate} and {nextDay1}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT0','PT','PT1','PT2','PT3','NFT','RC','RC1','RC2','RI')  ";
            }

            if (ModelName != "")
            {
                sql += $" and MODEL_NAME={'\'' + ModelName + '\''} ";
            }
            else
            {
                sql += "";
            }

            //====
            if (StationName != "")
            {
                sql += $" and GROUP_NAME={'\'' + StationName + '\''} ";
            }

            else
            {
                sql += "";
            }
            sql += $@" GROUP BY MODEL_NAME, GROUP_NAME,WORK_DATE,WORK_SECTION
                                  ORDER BY WORK_DATE DESC
                                 ) WHERE INPUT>0";



            //between 20200807 and 20200808
            DataTable _result = conn.reDt(sql);
            List<DataToDayAndChart5dayE> _LstResult = ConvertToObj.ConvertDataTable<DataToDayAndChart5dayE>(_result).ToList();
            //var dataTest = _LstResult.Where(x => x.WORK_DATE.Contains("20200920")).ToList();

            //===========old
            List<string> lstDay = _LstResult
                .Where(x => x.WORK_SECTION > 7
                || (_LstResult.Where(j => j.WORK_SECTION <= 7 && j.WORK_DATE.Contains(DateTime.ParseExact(x.WORK_DATE, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd"))).ToList().Count > 1))
                .GroupBy(dd => dd.WORK_DATE)
                .Select(d => d.Key).OrderByDescending(s => s).Take(7).OrderBy(sd => sd).ToList();

            //=======================new
            List<string> lstDay1 = _LstResult.GroupBy(dd => dd.WORK_DATE)
                .Select(d => d.Key).OrderByDescending(s => s).Take(7).OrderBy(sd => sd).ToList();




            var ldm = lstDay;
            List<DataOneDay> lstDataByDay = new List<DataOneDay>();

            foreach (var day in lstDay)
            {
                //var lastDay = "";
                //if (_LstResult.Where(xd => xd.WORK_DATE.Equals(_day) && xd.WORK_SECTION < 7).Count() > 0)
                //{
                //     lastDay = DateTime.ParseExact(_day, "yyyyMMdd", null).AddDays(-1).ToString("yyyyMMdd");
                //}
                //else
                //{
                //     lastDay = _day;
                //}
                //var day = lastDay;


                var dataCrrentDay = _LstResult.Where(x => x.WORK_DATE.Contains(day) && x.WORK_SECTION > 7).ToList();
                var Day1 = DateTime.ParseExact(day, "yyyyMMdd", null);
                var nextDay = Day1.AddDays(1).ToString("yyyyMMdd");
                var test = _LstResult.Where(x => x.WORK_DATE.Contains(nextDay)).ToList();
                var dataNextDay = _LstResult.Where(nd => nd.WORK_DATE.Contains(nextDay) && nd.WORK_SECTION <= 7).ToList();
                var dataTmp = dataCrrentDay.Concat(dataNextDay).ToList();

                lstDataByDay.Add(new DataOneDay() { WORK_DATE = day, DataToDayAndChart5dayE = dataTmp });
            }

            List<DataByDayStation> DataResult = new List<DataByDayStation>();
            foreach (var dataByDay in lstDataByDay)
            {
                DataByDayStation dtTmp = new DataByDayStation();
                dtTmp.WORK_DATE = dataByDay.WORK_DATE;
                dtTmp.SUM_PASS = dataByDay.DataToDayAndChart5dayE.Sum(x => x.SUM_PASS);
                dtTmp.SUM_REPASS = dataByDay.DataToDayAndChart5dayE.Sum(y => y.SUM_REPASS);
                dtTmp.FIRST_FAIL = dataByDay.DataToDayAndChart5dayE.Sum(ff => ff.FIRST_FAIL);
                dtTmp.SUM_FAIL = dataByDay.DataToDayAndChart5dayE.Sum(f => f.SUM_FAIL);
                dtTmp.SUM_RE_FAIL = dataByDay.DataToDayAndChart5dayE.Sum(k => k.SUM_RE_FAIL);
                dtTmp.INPUT = dataByDay.DataToDayAndChart5dayE.Sum(input => input.INPUT);
                dtTmp.RR_s = Math.Round((dtTmp.FIRST_FAIL - dtTmp.SUM_RE_FAIL - dtTmp.SUM_FAIL) / (dtTmp.SUM_PASS + dtTmp.SUM_REPASS + dtTmp.SUM_FAIL + dtTmp.SUM_RE_FAIL) * 100, 2); //dataByDay.DataToDayAndChart5dayE.Sum(rr => rr.RR);//
                dtTmp.YR_s = Math.Round((dtTmp.SUM_PASS + dtTmp.SUM_REPASS) / (dtTmp.SUM_PASS + dtTmp.SUM_REPASS + dtTmp.SUM_RE_FAIL + dtTmp.SUM_FAIL) * 100, 2);  //dataByDay.DataToDayAndChart5dayE.Sum(yr => yr.YR);

                DataResult.Add(dtTmp);
            }
            var dt = DataResult;


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
                            INPUT = dd.INPUT,
                            FIRST_FAIL = dd.FIRST_FAIL,
                            SUM_FAIL = dd.SUM_FAIL,
                            SUM_RE_FAIL = dd.SUM_RE_FAIL,
                            RR_s = dd == null ? -5m : dd.RR,

                        }).ToList()

                    }).OrderByDescending(c => DateTime.ParseExact(c.WORK_DATE, "yyyyMMdd", null)).Take(7).ToList()
                }).ToList()
            }).ToList();
            var l = lstByModelName;

            var dataInChart = new List<TOPDataByStation>();
            dataInChart = lstDataInChart(DataTime, StationName, ModelName);

            var alldata = new AllData();
            alldata.lisDataByModel = lstByModelName;
            alldata.listdataInChart = dataInChart;
            alldata.lstDataChartByDayAndNight = DataResult;

            return Json(alldata);

            // var result = JsonConvert.SerializeObject(lstByModelName);
            // return Json(lstByModelName.FirstOrDefault());
        }






        #region Draw Chart bay Error Code 

        //chart detail by error code
        public ActionResult ShowChartErrorCodeByPCName(DateTime? dateTime, string station, string ModelName)
        {

            // string Date= dateTime == null?  DateTime.Now.ToString(): dateTime.Value.ToString("yyyyMMdd");
            var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");


            var nextDate = DateTime.ParseExact(Date, "yyyyMMdd", null);
            var sNextDate = nextDate.AddDays(1).ToString("yyyyMMdd");

            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE between {Date} and {sNextDate}  AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";


            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

            //
            var dataThisDay = _lstResult.Where(td => td.WORK_DATE.Contains(Date) && td.WORK_SECTION > 7).ToList();
            var dataNextDay = _lstResult.Where(nd => nd.WORK_DATE.Contains(sNextDate) && nd.WORK_SECTION <= 7).ToList();
            var sumData = dataThisDay.Concat(dataNextDay).ToList();
            //

            List<dataByStationChart> listDataChart = sumData.GroupBy(x => x.GROUP_NAME).Select(xx => new dataByStationChart()
            {
                Station = xx.Key,
                lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new dataByModelNameChart()
                {
                    ModelName = mm.Key,
                    lstError = mm.GroupBy(ee => ee.ERROR_CODE).Select(lster => new lstByErrorCodeChart()
                    {
                        ERROR_CODE = lster.Key,
                        DataPCName = lster.GroupBy(pc => pc.STATION_NAME).Select(lps => new DataByPCName()
                        {
                            StattionName = lps.Key,
                            dataHour = lps.Select(dfsfs => new dataByHour()
                            {
                                WORK_SECTION = dfsfs.WORK_SECTION,
                                SUM_FAIL_ER = dfsfs.SUM_FAIL_ER,
                                SUM_TEST_FAIL_ER = dfsfs.SUM_TEST_FAIL_ER

                            }).OrderBy(ss => ss.WORK_SECTION).ToList()
                        }).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            var l = listDataChart;

            return Json(listDataChart.FirstOrDefault());
        }


        //chart summary by error code when click Main char 7 day
        public ActionResult ShowChartSummaryByErrorCode1(DateTime? dateTime, string station, string ModelName)
        {

            // string Date= dateTime == null?  DateTime.Now.ToString(): dateTime.Value.ToString("yyyyMMdd");
            var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");
            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE ={Date} AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";




            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

            List<lstStationByErrorSummary> listDataChart = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xx => new lstStationByErrorSummary()
            {
                Station = xx.Key,
                lstModel = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new lstModelNameByEroSummary()
                {
                    ModelName = mm.Key,
                    lstErrorSummary = mm.GroupBy(ee => ee.ERROR_CODE).Select(lster => new lstErrorCodeSummary()
                    {
                        ErrorCode = lster.Key,
                        dataErrorSum = lster.GroupBy(Tn => Tn.WORK_SECTION).Select(ltn => new DataByErrorCodeSummary()
                        {
                            WorkSection = ltn.Key,
                            FirstFailER = ltn.Sum(sf => sf.SUM_TEST_FAIL_ER),
                            ReFail = ltn.Sum(srf => srf.SUM_FAIL_ER)
                        }).OrderBy(sort => sort.WorkSection).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            var l = listDataChart;

            return Json(listDataChart.FirstOrDefault());
        }
        public ActionResult ShowChartSummaryByErrorCode(DateTime? dateTime, string station, string ModelName)
        {

            // string Date= dateTime == null?  DateTime.Now.ToString(): dateTime.Value.ToString("yyyyMMdd");
            var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");
            var nextdate = DateTime.ParseExact(Date, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");
            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE between {Date} and {nextdate}  AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";




            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();
            var ll = _lstResult;

            //
            var dataThisDay = _lstResult.Where(td => td.WORK_DATE.Contains(Date) && td.WORK_SECTION > 7).ToList();
            var dataNextDay = _lstResult.Where(nd => nd.WORK_DATE.Contains(nextdate) && nd.WORK_SECTION <= 7).ToList();
            var sumData = dataThisDay.Concat(dataNextDay).ToList();


            //


            List<lstStationByErrorSummary> listDataChart = sumData.GroupBy(x => x.GROUP_NAME).Select(xx => new lstStationByErrorSummary()
            {
                Station = xx.Key,
                lstModel = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new lstModelNameByEroSummary()
                {
                    ModelName = mm.Key,
                    lstErrorSummary = mm.GroupBy(ee => ee.ERROR_CODE).Select(lster => new lstErrorCodeSummary()
                    {
                        ErrorCode = lster.Key,
                        dataErrorSum = lster.GroupBy(Tn => Tn.WORK_SECTION).Select(ltn => new DataByErrorCodeSummary()
                        {
                            WorkSection = ltn.Key,
                            FirstFailER = ltn.Sum(sf => sf.SUM_TEST_FAIL_ER),
                            ReFail = ltn.Sum(srf => srf.SUM_FAIL_ER)
                        }).OrderBy(sort => sort.WorkSection).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            var l = listDataChart;

            return Json(listDataChart.FirstOrDefault());
        }


        //==================oki chua up server
        public ActionResult ShowChartSummaryByOneError(string dateTime, string station, string ModelName, string ErrorCode)
        {
            //var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");
            var nextDate = DateTime.ParseExact(dateTime, "yyyyMMdd", null);
            var sNextDate = nextDate.AddDays(1).ToString("yyyyMMdd");

            var FromDate = DateTime.Now.AddDays(-60).ToString("yyyyMMdd");
            var Todate = DateTime.Now.ToString("yyyyMMdd");

            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE between  {dateTime} and {sNextDate} AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''} and ERC.ERROR_CODE={'\'' + ErrorCode + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";



            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

            List<dataErrorCodeByPCName> _lstResultByDayAndNight = _lstResult.Where(x => ((x.WORK_DATE == dateTime && x.WORK_SECTION > 7) || (x.WORK_DATE == sNextDate && x.WORK_SECTION <= 7))).ToList();
            var ll = _lstResultByDayAndNight;


            List<lstStationByErrorSummary> listDataChart = _lstResultByDayAndNight.GroupBy(x => x.GROUP_NAME).Select(xx => new lstStationByErrorSummary()
            {
                Station = xx.Key,
                lstModel = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new lstModelNameByEroSummary()
                {
                    ModelName = mm.Key,
                    lstErrorSummary = mm.GroupBy(ee => ee.ERROR_CODE).Select(lster => new lstErrorCodeSummary()
                    {
                        ErrorCode = lster.Key,
                        dataErrorSum = lster.GroupBy(Tn => Tn.WORK_SECTION).Select(ltn => new DataByErrorCodeSummary()
                        {
                            WorkSection = ltn.Key,
                            FirstFailER = ltn.Sum(sf => sf.SUM_TEST_FAIL_ER),
                            ReFail = ltn.Sum(srf => srf.SUM_FAIL_ER)
                        }).OrderBy(sort => sort.WorkSection).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            var l = listDataChart;
            // lst detai by one Error Code

            List<dataByStationChart> listDataOneChart = _lstResultByDayAndNight.GroupBy(x => x.GROUP_NAME).Select(xx => new dataByStationChart()
            {
                Station = xx.Key,
                lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new dataByModelNameChart()
                {
                    ModelName = mm.Key,
                    lstError = mm.GroupBy(ee => ee.ERROR_CODE).Select(lster => new lstByErrorCodeChart()
                    {
                        ERROR_CODE = lster.Key,
                        DataPCName = lster.GroupBy(pc => pc.STATION_NAME).Select(lps => new DataByPCName()
                        {
                            StattionName = lps.Key,
                            dataHour = lps.Select(dfsfs => new dataByHour()
                            {
                                WORK_SECTION = dfsfs.WORK_SECTION,
                                SUM_FAIL_ER = dfsfs.SUM_FAIL_ER,
                                SUM_TEST_FAIL_ER = dfsfs.SUM_TEST_FAIL_ER

                            }).OrderBy(ss => ss.WORK_SECTION).ToList()
                        }).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            //get data 5 day to draw chart by Error Code
            string sql5dayByError = $@" select ERC.WORK_DATE,ERC.WORK_SECTION, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE between {FromDate} and {sNextDate} AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''} and ERC.ERROR_CODE={'\'' + ErrorCode + '\''}
                              GROUP BY ERC.GROUP_NAME, ERC.MODEL_NAME,ERC.ERROR_CODE,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";

            DataTable _resultData5day = conn.reDt(sql5dayByError);
            List<MData5day> _lstResultdata5day = ConvertToObj.ConvertDataTable<MData5day>(_resultData5day).ToList();

            //====================== old
            List<string> lstDay = _lstResultdata5day.Where(x1 => x1.WORK_SECTION > 7).GroupBy(xx => xx.WORK_DATE).Select(k => k.Key).OrderByDescending(wd => wd).Take(7).OrderBy(sw => sw).ToList();
            var ld = lstDay;

            //======================= new
            List<string> lstDay1 = _lstResultdata5day
               .Where(x => x.WORK_SECTION > 7
               || (_lstResultdata5day.Where(j => j.WORK_SECTION <= 7 && j.WORK_DATE.Contains(DateTime.ParseExact(x.WORK_DATE, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd"))).ToList().Count > 1))
               .GroupBy(dd => dd.WORK_DATE)
               .Select(d => d.Key).OrderByDescending(s => s).Take(7).OrderBy(sd => sd).ToList();
            var ldm = lstDay1;

            //======================

            List<data5daybyDayAndNight> lstdata7Day = new List<data5daybyDayAndNight>();

            foreach (var day in lstDay1)
            {
                LstData5Day oneDay = new LstData5Day();
                var dataThisDay = _lstResultdata5day.Where(x => x.WORK_DATE.Contains(day) && x.WORK_SECTION > 7).ToList();
                var nextday = DateTime.ParseExact(day, "yyyyMMdd", null);
                var sNextDay = nextday.AddDays(1).ToString("yyyyMMdd");
                var dataNextday = _lstResultdata5day.Where(nda => nda.WORK_DATE.Contains(sNextDay) && nda.WORK_SECTION <= 7).ToList();
                var dataTmp = dataThisDay.Concat(dataNextday).ToList();
                lstdata7Day.Add(new data5daybyDayAndNight() { WORK_DATE = day, listDayDayAndNight = dataTmp });
            }

            List<LstData5Day> lstDataBy7dayAndNight = new List<LstData5Day>();
            foreach (var dd in lstdata7Day)
            {
                LstData5Day itemdata = new LstData5Day();
                itemdata.Date = dd.WORK_DATE;
                itemdata.SUM_FAIL_ER = dd.listDayDayAndNight.Sum(sf => sf.SUM_FAIL_ER);
                itemdata.SUM_TEST_FAIL_ER = dd.listDayDayAndNight.Sum(sff => sff.SUM_TEST_FAIL_ER);
                lstDataBy7dayAndNight.Add(itemdata);
            }

            var ddd = lstDataBy7dayAndNight.OrderBy(ww => ww.Date);



            //thua
            List<LstData5DayByErrorCode> lstdata5day = _lstResultdata5day.GroupBy(x => x.GROUP_NAME).Select(xx => new LstData5DayByErrorCode()
            {
                Station = xx.Key,
                lstModel = xx.GroupBy(lMo => lMo.MODEL_NAME).Select(dtMo => new LstModelBy5Day()
                {
                    ModelName = dtMo.Key,
                    lst5DayError = dtMo.GroupBy(lstEr => lstEr.ERROR_CODE).Select(dtEr => new lstDataError5Day()
                    {
                        ErrorCode = dtEr.Key,
                        lisData5Day = dtEr.Select(wdd => new LstData5Day()
                        {
                            Date = wdd.WORK_DATE,
                            SUM_FAIL_ER = wdd.SUM_FAIL_ER,
                            SUM_TEST_FAIL_ER = wdd.SUM_TEST_FAIL_ER,
                        }).OrderByDescending(ss => DateTime.ParseExact(ss.Date, "yyyyMMdd", null)).Take(7).ToList()
                    }).ToList()
                }).ToList()
            }).ToList();

            var lll = lstdata5day;


            var allData = new DataSummaryAndDetailOneChart();
            allData.lstSummay = listDataChart;
            allData.lstDetail = listDataOneChart;
            allData.lst5DayChartByEr = lstdata5day;
            allData.lstData7dayDayAndNight = lstDataBy7dayAndNight;
            return Json(allData);
            //return Json(listDataChart.FirstOrDefault());
        }


        //public ActionResult ShowChartDetailByOneError(DateTime? dateTime, string station, string ModelName, string ErrorCode)
        //{
        //    var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");
        //    string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
        //                      ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
        //                      from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'R%' OR MODEL_NAME LIKE 'M%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%') ERC
        //                      WHERE ERC.WORK_DATE ={Date} AND ERC.GROUP_NAME={'\'' + station + '\''}
        //                      and ERC.MODEL_NAME={'\'' + ModelName + '\''} and ERC.ERROR_CODE={'\'' + ErrorCode + '\''}
        //                      GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
        //                      ORDER BY  Sum_Test_Fail_ER DESC ";

        //    DataTable _result = conn.reDt(sql);
        //    List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

        //    List<dataByStationChart> listDataChart = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xx => new dataByStationChart()
        //    {
        //        Station = xx.Key,
        //        lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new dataByModelNameChart()
        //        {
        //            ModelName = mm.Key,
        //            lstError = mm.GroupBy(ee => ee.ERROR_CODE).Select(lster => new lstByErrorCodeChart()
        //            {
        //                ERROR_CODE = lster.Key,
        //                DataPCName = lster.GroupBy(pc => pc.STATION_NAME).Select(lps => new DataByPCName()
        //                {
        //                    StattionName = lps.Key,
        //                    dataHour = lps.Select(dfsfs => new dataByHour()
        //                    {
        //                        WORK_SECTION = dfsfs.WORK_SECTION,
        //                        SUM_FAIL_ER = dfsfs.SUM_FAIL_ER,
        //                        SUM_TEST_FAIL_ER = dfsfs.SUM_TEST_FAIL_ER

        //                    }).OrderBy(ss => ss.WORK_SECTION).ToList()
        //                }).ToList()
        //            }).ToList()


        //        }).ToList()
        //    }).ToList();


        //    var l = listDataChart;

        //    return Json(listDataChart.FirstOrDefault());
        //}
        #endregion



        #region Draw Chart By PCName
        //====chart detail by pc name in page have data and chart
        public ActionResult ShowChartByPCName(DateTime? dateTime, string station, string ModelName)
        {

            // string Date= dateTime == null?  DateTime.Now.ToString(): dateTime.Value.ToString("yyyyMMdd");
            var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");
            var nextDate = DateTime.ParseExact(Date, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");

            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE between {Date} and {nextDate} AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";


            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

            //
            var dataThisDay = _lstResult.Where(td => td.WORK_DATE.Contains(Date) && td.WORK_SECTION > 7).ToList();
            var dataNextDay = _lstResult.Where(nd => nd.WORK_DATE.Contains(nextDate) && nd.WORK_SECTION <= 7).ToList();
            var sumData = dataThisDay.Concat(dataNextDay).ToList();
            //

            List<dataByStationByPc> listDataChart = sumData.GroupBy(x => x.GROUP_NAME).Select(xx => new dataByStationByPc()
            {
                Station = xx.Key,
                lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new ModelNameByPC()
                {
                    ModelName = mm.Key,
                    lstPC = mm.GroupBy(ee => ee.STATION_NAME).Select(lster => new lstPCName()
                    {
                        PCName = lster.Key,
                        lstErrorCodeByPc = lster.GroupBy(pc => pc.ERROR_CODE).Select(lps => new lstErrorCodeByPc()
                        {
                            ErrorCode = lps.Key,
                            lstdataErrorByPc = lps.Select(dfsfs => new DataByErrorCode()
                            {
                                WorkSection = dfsfs.WORK_SECTION,
                                SumReFailER = dfsfs.SUM_FAIL_ER,
                                SumTestFailER = dfsfs.SUM_TEST_FAIL_ER

                            }).OrderBy(ss => ss.WorkSection).ToList()
                        }).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            var l = listDataChart;

            return Json(listDataChart.FirstOrDefault());
        }
        //====page have data and chart when click in main chart in 7 day
        public ActionResult ShowChartSummaryFailByPCName1(DateTime? dateTime, string station, string ModelName)
        {
            #region

            #endregion
            // string Date= dateTime == null?  DateTime.Now.ToString(): dateTime.Value.ToString("yyyyMMdd");
            var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");
            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE ={Date} AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";


            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();


            List<SummaryByStationPc> listDataChart = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xx => new SummaryByStationPc()
            {
                Station = xx.Key,
                lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new SumLstModelNameByPC()
                {
                    ModelName = mm.Key,
                    lstPC = mm.GroupBy(ee => ee.STATION_NAME).Select(lsPc => new SumlstPCName()
                    {
                        PCName = lsPc.Key,
                        lstDataPc = lsPc.GroupBy(hh => hh.WORK_SECTION).Select(Pc => new SumDataByPc()
                        {
                            WorkSection = Pc.Key,
                            SumReFailER = Pc.Sum(sF => sF.SUM_FAIL_ER),
                            SumTestFailER = Pc.Sum(sff => sff.SUM_TEST_FAIL_ER)
                        }).OrderBy(hour => hour.WorkSection).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            var l = listDataChart;
            //title by model name in page chart
            var dataInChart = new List<TOPDataByStation>();
            dataInChart = lstDataInChart(Date, station, ModelName);
            var alldata = new AllDataInChart();
            alldata.summaryChart = listDataChart;
            alldata.dataViewInChart = dataInChart;
            return Json(alldata);
            //return Json(listDataChart.FirstOrDefault());
        }
        public ActionResult ShowChartSummaryFailByPCName(DateTime? dateTime, string station, string ModelName)
        {
            #region

            #endregion
            // string Date= dateTime == null?  DateTime.Now.ToString(): dateTime.Value.ToString("yyyyMMdd");
            var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");

            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE ={Date} AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";


            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

            //===data to draw chart detail and summary by PC and Errorcode when click in main char 7 day
            List<string> lstDay = _lstResult
               .Where(x => x.WORK_SECTION > 7
               || (_lstResult.Where(j => j.WORK_SECTION <= 7 && j.WORK_DATE.Contains(DateTime.ParseExact(x.WORK_DATE, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd"))).ToList().Count > 1))
               .GroupBy(dd => dd.WORK_DATE)
               .Select(d => d.Key).OrderByDescending(s => s).Take(7).OrderBy(sd => sd).ToList();

            var ldm = lstDay;
            List<chartDayAndNightShifInMainChart> lstDataByDay = new List<chartDayAndNightShifInMainChart>();

            foreach (var day in lstDay)
            {

                var dataCrrentDay = _lstResult.Where(x => x.WORK_DATE.Contains(day) && x.WORK_SECTION > 7).ToList();
                var Day1 = DateTime.ParseExact(day, "yyyyMMdd", null);
                var nextDay = Day1.AddDays(1).ToString("yyyyMMdd");
                var test = _lstResult.Where(x => x.WORK_DATE.Contains(nextDay)).ToList();
                var dataNextDay = _lstResult.Where(nd => nd.WORK_DATE.Contains(nextDay) && nd.WORK_SECTION <= 7).ToList();
                var dataTmp = dataCrrentDay.Concat(dataNextDay).ToList();

                lstDataByDay.Add(new chartDayAndNightShifInMainChart() { WORK_DATE = day, datatDayAndNight = dataTmp });
            }


            List<datachartDayAndNightShifInMainChart> DataResult = new List<datachartDayAndNightShifInMainChart>();
            foreach (var dataByDay in lstDataByDay)
            {
                datachartDayAndNightShifInMainChart dtTmp = new datachartDayAndNightShifInMainChart();
                dtTmp.WORK_DATE = dataByDay.WORK_DATE;
                dtTmp.SUM_FAIL_ER = dataByDay.datatDayAndNight.Sum(x => x.SUM_FAIL_ER);
                dtTmp.SUM_TEST_FAIL_ER = dataByDay.datatDayAndNight.Sum(y => y.SUM_TEST_FAIL_ER);
                DataResult.Add(dtTmp);
            }
            var dt = DataResult;



            //title by model name in page chart
            var dataInChart = new List<TOPDataByStation>();
            dataInChart = lstDataInChart(Date, station, ModelName);
            var alldata = new AllDataInChart();

            alldata.summaryChart = dataChartSumaryByPc(dateTime, station, ModelName);
            alldata.dataViewInChart = dataInChart;
            //data to draw chart by day and night shift when click in chart
            alldata.dataChartDayAndNightShift = DataResult;
            return Json(alldata);
            //return Json(listDataChart.FirstOrDefault());
        }


        //===data to draw chart Sumary by pc name in page have chart and data
        public List<SummaryByStationPc> dataChartSumaryByPc(DateTime? dateTime, string station, string ModelName)
        {
            var Date = dateTime == null ? DateTime.Now.ToString("yyyyMMdd") : dateTime.Value.ToString("yyyyMMdd");
            var nextDate = DateTime.ParseExact(Date, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");

            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE between {Date} and {nextDate}  AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";


            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

            //
            var dataThisDay = _lstResult.Where(td => td.WORK_DATE.Contains(Date) && td.WORK_SECTION > 7).ToList();
            var dataNextDay = _lstResult.Where(nd => nd.WORK_DATE.Contains(nextDate) && nd.WORK_SECTION <= 7).ToList();
            var sumData = dataThisDay.Concat(dataNextDay).ToList();
            //

            List<SummaryByStationPc> listDataChart = sumData.GroupBy(x => x.GROUP_NAME).Select(xx => new SummaryByStationPc()
            {
                Station = xx.Key,
                lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new SumLstModelNameByPC()
                {
                    ModelName = mm.Key,
                    lstPC = mm.GroupBy(ee => ee.STATION_NAME).Select(lsPc => new SumlstPCName()
                    {
                        PCName = lsPc.Key,
                        lstDataPc = lsPc.GroupBy(hh => hh.WORK_SECTION).Select(Pc => new SumDataByPc()
                        {
                            WorkSection = Pc.Key,
                            SumReFailER = Pc.Sum(sF => sF.SUM_FAIL_ER),
                            SumTestFailER = Pc.Sum(sff => sff.SUM_TEST_FAIL_ER)
                        }).OrderBy(hour => hour.WorkSection).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();

            return listDataChart;
        }



        //===================== Main chart one Pc name==========================
        //====show chart when click PcName in main page============
        public ActionResult ShowChartSummaryAndDetailByOnePCName(string dateTime, string station, string ModelName, string PCName)
        {

            var nextDate = DateTime.ParseExact(dateTime, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");
            string sql = $@" select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,ERC.WORK_SECTION
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  or MODEL_NAME LIKE 'X%' OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%') ERC
                              WHERE ERC.WORK_DATE between {dateTime} and {nextDate} AND ERC.GROUP_NAME={'\'' + station + '\''}
                              and ERC.MODEL_NAME={'\'' + ModelName + '\''} and ERC.STATION_NAME={'\'' + PCName + '\''}
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                              ORDER BY  Sum_Test_Fail_ER DESC ";
            DataTable _result = conn.reDt(sql);
            List<dataErrorCodeByPCName> _lstResult = ConvertToObj.ConvertDataTable<dataErrorCodeByPCName>(_result).ToList();

            //
            var dataThisDay = _lstResult.Where(td => td.WORK_DATE.Contains(dateTime) && td.WORK_SECTION > 7).ToList();
            var dataNextDay = _lstResult.Where(nd => nd.WORK_DATE.Contains(nextDate) && nd.WORK_SECTION <= 7).ToList();
            var _sumData = dataThisDay.Concat(dataNextDay).ToList();
            //

            List<SummaryByStationPc> listDataSummaryChart = _sumData.GroupBy(x => x.GROUP_NAME).Select(xx => new SummaryByStationPc()
            {
                Station = xx.Key,
                lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new SumLstModelNameByPC()
                {
                    ModelName = mm.Key,
                    lstPC = mm.GroupBy(ee => ee.STATION_NAME).Select(lsPc => new SumlstPCName()
                    {
                        PCName = lsPc.Key,
                        lstDataPc = lsPc.GroupBy(hh => hh.WORK_SECTION).Select(Pc => new SumDataByPc()
                        {
                            WorkSection = Pc.Key,
                            SumReFailER = Pc.Sum(sF => sF.SUM_FAIL_ER),
                            SumTestFailER = Pc.Sum(sff => sff.SUM_TEST_FAIL_ER)
                        }).OrderBy(hour => hour.WorkSection).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();




            //===
            List<dataByStationByPc> listDataChartDetailByPc = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xx => new dataByStationByPc()
            {
                Station = xx.Key,
                lstModelName = xx.GroupBy(ss => ss.MODEL_NAME).Select(mm => new ModelNameByPC()
                {
                    ModelName = mm.Key,
                    lstPC = mm.GroupBy(ee => ee.STATION_NAME).Select(lster => new lstPCName()
                    {
                        PCName = lster.Key,
                        lstErrorCodeByPc = lster.GroupBy(pc => pc.ERROR_CODE).Select(lps => new lstErrorCodeByPc()
                        {
                            ErrorCode = lps.Key,
                            lstdataErrorByPc = lps.Select(dfsfs => new DataByErrorCode()
                            {
                                WorkSection = dfsfs.WORK_SECTION,
                                SumReFailER = dfsfs.SUM_FAIL_ER,
                                SumTestFailER = dfsfs.SUM_TEST_FAIL_ER

                            }).OrderBy(ss => ss.WorkSection).ToList()
                        }).ToList()
                    }).ToList()


                }).ToList()
            }).ToList();


            var AllData = new DataSummaryAndDetailOnePc();
            AllData.lstSummaryByOnePc = listDataSummaryChart;
            AllData.lsDetailByOnePC = listDataChartDetailByPc;


            return Json(AllData);
        }
        #endregion


        #region data titel by ModelName In chart
        public List<TOPDataByStation> lstDataInChart(string date, string Group_name, string Model_name)
        {

            var nextDate = DateTime.ParseExact(date, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");

            string _sql = "";
            _sql = $@"SELECT * FROM (SELECT BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RATE_STATION,BH.Y_RATE_STATION,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as TOTAL_STATION,
                              BH.First_Fail,BH.Sum_FAIL,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE_EC
                              FROM
                                (select ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE (MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%')) ERC   WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE={date} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={nextDate} and ERC.WORK_SECTION <=7))  ";// WHERE ERC.WORK_DATE ={date} ";

            _sql += $@" AND ERC.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              LEFT JOIN 
                              (( SELECT *  FROM(
                                  SELECT MODEL_NAME,  GROUP_NAME,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS Y_RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  
                                                                                MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%') WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE={date} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7)) ";// WHERE  WORK_DATE= {date}";



            _sql += $@"AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY MODEL_NAME, GROUP_NAME
                                  ORDER BY RATE_STATION DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME";
            _sql += $@" WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>1  ";





            if (Group_name == "All Station")
            {
                _sql += "";
            }
            else if (Group_name != "")
            {
                _sql += $" and BH.GROUP_NAME ={'\'' + Group_name + '\''} ";
            }
            else
            {
                _sql += "";
            }


            if (Model_name == "All Model")
            {
                _sql += "";
            }
            else if (Model_name != "")
            {
                _sql += $" and BH.MODEL_NAME={'\'' + Model_name + '\''} ";
            }

            else
            {
                _sql += "";
            }

            _sql += $@") bbb left join 
                                    (select SFIS1.C_ERROR_CODE_T.ERROR_CODE,ERROR_DESC2 from SFIS1.C_ERROR_CODE_T ) BH                                
                                    on bbb.ERROR_CODE = BH.ERROR_CODE ";

            string sql = _sql;

            DataTable _result = conn.reDt(sql);
            List<Top3ErrorCodeByStation> _lstResult = ConvertToObj.ConvertDataTable<Top3ErrorCodeByStation>(_result).ToList();


            List<TOPDataByStation> lstByGroupName = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xy => new TOPDataByStation()
            {
                GroupName = xy.Key,
                count_row_station = xy.GroupBy(rs => rs.STATION_NAME).Count(),
                count_row_station1 = xy.GroupBy(rm => rm.STATION_NAME).Count(),
                count_model = xy.GroupBy(countM => countM.MODEL_NAME).Count(),


                DataByModelName = xy.GroupBy(j => j.MODEL_NAME).Select(ex => new DataByModelName()
                {
                    ModelName = ex.Key,

                    Input = ex.Max(a => a.TOTAL_STATION),
                    Frist_fail_station = ex.Max(ffs => ffs.FIRST_FAIL),
                    Fail_Of_station_Name = ex.Max(fm => fm.SUM_FAIL),

                    RR = ex.Max(rr => rr.RATE_STATION),
                    YR = ex.Max(yr => yr.Y_RATE_STATION),
                    count_row_ModelName = ex.GroupBy(rm => rm.STATION_NAME).Count(),//jhfjhjh
                    count_row_ERR_in_model = ex.GroupBy(count_err => count_err.ERROR_CODE).Count(),

                    FailByErrorCode = ex.GroupBy(e => e.ERROR_CODE).Select(k => new FailByErrorCode()
                    {
                        ErrorCode = k.Key,
                        ErrorDESC2 = k.Select(sdfgsjdf => sdfgsjdf.ERROR_DESC2).Last(),
                        count_row_Errorcode = k.GroupBy(rrr => rrr.STATION_NAME).Count(),
                        FailQtyErrCode = k.Sum(q => q.SUM_TEST_FAIL_ER),
                        //FailQtyErrCode = k.OrderByDescending(asda=>asda.Sum(q => q.SUM_TEST_FAIL_ER)),

                        SecondFailQtyErrCode = k.Sum(ffer => ffer.SUM_FAIL_ER),
                        FailByMachine = k.Select(ee => new FailByMachine()
                        {
                            PCName = ee.STATION_NAME,
                            FailQtyPC = ee.SUM_TEST_FAIL_ER,
                            ReFail = ee.SUM_FAIL_ER,
                        }).OrderByDescending(errc => errc.FailQtyPC).ToList(),


                    }).OrderByDescending(sfhskdf => sfhskdf.FailQtyErrCode).ToList()


                }).OrderByDescending(x => x.RR).ToList()
            }).ToList();
            return lstByGroupName;
        }


        public ActionResult ViewDataInChart(DateTime? date, string Group_name, string Model_name)
        {
            var Date = date == null ? DateTime.Now.ToString("yyyyMMdd") : date.Value.ToString("yyyyMMdd");
            var nextDate = DateTime.ParseExact(Date, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");

            string _sql = "";
            _sql = $@"SELECT * FROM (SELECT BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RATE_STATION,BH.Y_RATE_STATION,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as TOTAL_STATION,
                              BH.First_Fail,BH.Sum_FAIL,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE_EC
                              FROM
                                (select ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE (MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%')) ERC WHERE ERC.WORK_DATE between {Date} and {nextDate} AND ((ERC.WORK_DATE={Date} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={nextDate} and ERC.WORK_SECTION <=7))    ";//WHERE ERC.WORK_DATE ={Date} ";

            _sql += $@" AND ERC.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              LEFT JOIN 
                              (( SELECT *  FROM(
                                  SELECT MODEL_NAME,  GROUP_NAME,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS Y_RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  
                                                                                MODEL_NAME LIKE 'R%' 
                                                                                OR MODEL_NAME LIKE 'M%'  
                                                                                OR MODEL_NAME LIKE 'E%' 
                                                                                OR MODEL_NAME LIKE 'C%' 
                                                                                OR MODEL_NAME LIKE 'D%' 
                                                                                OR MODEL_NAME LIKE 'L%' 
                                                                                OR MODEL_NAME LIKE 'N%' 
                                                                                OR MODEL_NAME LIKE 'G%' 
                                                                                OR MODEL_NAME LIKE 'X%'
                                                                                OR MODEL_NAME LIKE 'O%'
                                                                                OR MODEL_NAME LIKE 'U12H%'
                                                                                OR MODEL_NAME LIKE 'V6510%')  WHERE WORK_DATE between {Date} and {nextDate} AND ((WORK_DATE={Date} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7))   ";// WHERE  WORK_DATE= {Date}";



            _sql += $@"AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY MODEL_NAME, GROUP_NAME
                                  ORDER BY RATE_STATION DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME";
            _sql += $@" WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>1  ";





            if (Group_name == "All Station")
            {
                _sql += "";
            }
            else if (Group_name != "")
            {
                _sql += $" and BH.GROUP_NAME ={'\'' + Group_name + '\''} ";
            }
            else
            {
                _sql += "";
            }


            if (Model_name == "All Model")
            {
                _sql += "";
            }
            else if (Model_name != "")
            {
                _sql += $" and BH.MODEL_NAME={'\'' + Model_name + '\''} ";
            }

            else
            {
                _sql += "";
            }

            _sql += $@") bbb left join 
                                    (select SFIS1.C_ERROR_CODE_T.ERROR_CODE,ERROR_DESC2 from SFIS1.C_ERROR_CODE_T ) BH                                
                                    on bbb.ERROR_CODE = BH.ERROR_CODE ";

            string sql = _sql;

            DataTable _result = conn.reDt(sql);
            List<Top3ErrorCodeByStation> _lstResult = ConvertToObj.ConvertDataTable<Top3ErrorCodeByStation>(_result).ToList();


            List<TOPDataByStation> lstByGroupName = _lstResult.GroupBy(x => x.GROUP_NAME).Select(xy => new TOPDataByStation()
            {
                GroupName = xy.Key,
                count_row_station = xy.GroupBy(rs => rs.STATION_NAME).Count(),
                count_row_station1 = xy.GroupBy(rm => rm.STATION_NAME).Count(),
                count_model = xy.GroupBy(countM => countM.MODEL_NAME).Count(),


                DataByModelName = xy.GroupBy(j => j.MODEL_NAME).Select(ex => new DataByModelName()
                {
                    ModelName = ex.Key,

                    Input = ex.Max(a => a.TOTAL_STATION),
                    Frist_fail_station = ex.Max(ffs => ffs.FIRST_FAIL),
                    Fail_Of_station_Name = ex.Max(fm => fm.SUM_FAIL),

                    RR = ex.Max(rr => rr.RATE_STATION),
                    YR = ex.Max(yr => yr.Y_RATE_STATION),
                    count_row_ModelName = ex.GroupBy(rm => rm.STATION_NAME).Count(),//jhfjhjh
                    count_row_ERR_in_model = ex.GroupBy(count_err => count_err.ERROR_CODE).Count(),

                    FailByErrorCode = ex.GroupBy(e => e.ERROR_CODE).Select(k => new FailByErrorCode()
                    {
                        ErrorCode = k.Key,
                        ErrorDESC2 = k.Select(sdfgsjdf => sdfgsjdf.ERROR_DESC2).Last(),
                        count_row_Errorcode = k.GroupBy(rrr => rrr.STATION_NAME).Count(),
                        FailQtyErrCode = k.Sum(q => q.SUM_TEST_FAIL_ER),
                        //FailQtyErrCode = k.OrderByDescending(asda=>asda.Sum(q => q.SUM_TEST_FAIL_ER)),

                        SecondFailQtyErrCode = k.Sum(ffer => ffer.SUM_FAIL_ER),
                        FailByMachine = k.Select(ee => new FailByMachine()
                        {
                            PCName = ee.STATION_NAME,
                            FailQtyPC = ee.SUM_TEST_FAIL_ER,
                            ReFail = ee.SUM_FAIL_ER,
                        }).OrderByDescending(errc => errc.FailQtyPC).ToList(),


                    }).OrderByDescending(sfhskdf => sfhskdf.FailQtyErrCode).ToList()


                }).OrderByDescending(x => x.RR).ToList()
            }).ToList();

            // return lstByGroupName;
            var data = new TopData();
            data.listData = lstByGroupName;
            return PartialView("~/Views/TopErrorByStation/TopErrorByStationPartial.cshtml", data);
        }

        #endregion

        #region action Data
        public ActionResult AddAction(ActionErrorCodeByStationVM _vm)
        {
            // _vm.Duedate = DateTime.Parse(_vm.Duedate);

            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActionErrorCode actionEt = new ActionErrorCode();
            actionEt.Compare(_vm);
            context.ActionErrorCode.Add(actionEt);
            context.SaveChanges();
            return Json("Add ok");
        }


        //edit 

        public ActionResult EditActionByStation(ActionErrorCodeByStationVM _vm)
        {
            var dataEdit = (from data in context.ActionErrorCode
                            where data.Id == _vm.Id
                            select data
                           ).FirstOrDefault();

            if (dataEdit != null)
            {
                dataEdit.Compare(_vm);
            }

            context.SaveChanges();
            return Json("Add ok");
        }
        #endregion



    }
}