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
using Report.Extension;

namespace Report.Controllers
{
    public class TopErrorByStationController : Controller
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

        public ActionResult GetTop31(TopRRByStationRequest request)
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

        public ActionResult GetTop3(TopRRByStationRequest request)
        {
            var data = new TopData();
            if (request.StatusDay== "Today" || request.StatusDay== "Yesterday")
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
                var lstByGroupName = GetDataMuntiple(_now, NextDate, shift, option, groupName, modelName, pcs, fromHour, toHour, statusLine);
               // var data = new TopData();
                data.listData = lstByGroupName;
                data.WorkDate = DateTime.ParseExact(_now, "yyyyMMdd", null);
               
            }
            else if(request.StatusDay== "TowMonth")
            {
                data = findDateLaster(request.ModelName);
                
            }
            else
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
                var lstByGroupName = GetDataMuntiple(_now, NextDate, shift, option, groupName, modelName, pcs, fromHour, toHour, statusLine);
                // var data = new TopData();
                data.listData = lstByGroupName;
                data.WorkDate = DateTime.ParseExact(_now, "yyyyMMdd", null);
            }

            return PartialView("~/Views/TopErrorByStation/TopErrorByStationHidePCtPartial.cshtml", data);//lstByGroupName

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

        public List<TOPDataByStation> GetDataMuntiple(string date, string nextDate, string Shift, string option, string Group_name, string Model_name, int pcs, int fromHour, int toHour, string statusLine)//string date, bool option, string Group_name, string Model_name
        {
            //PT,FT,RC=>'PT','FT','RC'
            var lstGroup = String.Join(",",(from gr in Group_name.Split(',').ToList()
                            select "'"+gr+"'" ).ToList());

            var lstModel = String.Join(",",(from model in Model_name.Split(',').ToList()
                            select "'" + model + "'").ToList());

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
                _sql += $" and BH.GROUP_NAME  in ({lstGroup}) ";
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
                _sql += $" and BH.MODEL_NAME in ({lstModel}) ";
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
                                 and GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                            and WORK_DATE between {Fromday} and {Today}
                          ";
            DataTable _result = conn.reDt(sql);
            List<ModelNameLaster> _lstResult = ConvertToObj.ConvertDataTable<ModelNameLaster>(_result).ToList();
            var mm = _lstResult;
            return _lstResult;
        }
        //data ta in 2 month
        public ActionResult findDateLaster1(string Model_name)
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
        public TopData findDateLaster(string Model_name)
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

            var GetthisDate = DateTime.Now.ToString("yyyyMMdd");
            if (_lstResultModelAndDate.Count()>0)
            {
                 GetthisDate = _lstResultModelAndDate.OrderByDescending(xx => xx.WORK_DATE).First().WORK_DATE;
                var t = GetthisDate;
            }
           

            var thisDate = DateTime.Now.ToString("yyyyMMdd");
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
                                 ) WHERE  Sum_Pass>=0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>0   and BH.MODEL_NAME={'\'' + Model_name + '\''} and BM.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI') ) bbb left join 
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
            getdata.WorkDate =DateTime.ParseExact(GetthisDate,"yyyyMMdd",null);
            return getdata;
            //return PartialView("~/Views/TopErrorByStation/TopErrorByStationPartial.cshtml", getdata);

        }



        //function to filter fast
        public List<Top3ErrorCodeByStation> QueryDataToFilter(string StatusDay)
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var nexToday = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

            var yesterday = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            

            string sql = $@" SELECT * FROM (SELECT BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RATE_STATION,BH.Y_RATE_STATION,
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
                                                                                OR MODEL_NAME LIKE 'V6510%')) ERC ";
            if(StatusDay== "Today")
            {
                sql += $@" WHERE ERC.WORK_DATE between {today} and {nexToday} AND ((ERC.WORK_DATE={today} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={nexToday} and ERC.WORK_SECTION <=7)) ";
            }
            else if(StatusDay== "Yesterday")
            {
                sql += $@" WHERE ERC.WORK_DATE between {yesterday} and {today} AND ((ERC.WORK_DATE={yesterday} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={today} and ERC.WORK_SECTION <=7)) ";
            }
           

            sql+= $@" AND ERC.GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
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
            if(StatusDay== "Today")
            {
                sql += $@" WHERE WORK_DATE between {today} and {nexToday} AND ((WORK_DATE={today} and WORK_SECTION >7) or(WORK_DATE={nexToday} and WORK_SECTION <=7)) ";
            }
            else if(StatusDay== "Yesterday")
            {
                sql += $@" WHERE WORK_DATE between {yesterday} and {today} AND ((WORK_DATE={yesterday} and WORK_SECTION >7) or(WORK_DATE={today} and WORK_SECTION <=7)) ";
            }


           

            sql+=$@" AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY MODEL_NAME, GROUP_NAME
                                  ORDER BY RATE_STATION DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>0  ) bbb left join 
                                    (select SFIS1.C_ERROR_CODE_T.ERROR_CODE,ERROR_DESC2 from SFIS1.C_ERROR_CODE_T ) BH                                
                                    on bbb.ERROR_CODE = BH.ERROR_CODE ";

            DataTable _result = conn.reDt(sql);
            List<Top3ErrorCodeByStation> _lstResult = ConvertToObj.ConvertDataTable<Top3ErrorCodeByStation>(_result).ToList();
            return _lstResult;
        }
        public List<TOPDataByStation> getDataToFilter(string StatusDay)
        {
            var _lstResult = QueryDataToFilter(StatusDay);


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

            return lstByGroupName;
        }
        // change show data today yesterday 
        public ActionResult FilterFast11(string StatusDay)
        {
            var data = new TopData();       
            var ListdataBygroup = getDataToFilter(StatusDay);
            data.listData = ListdataBygroup;        
            return PartialView("~/Views/TopErrorByStation/TopErrorByStationHidePCtPartial.cshtml", data);
        }

        //change filter data 
        public string FilterFast(string StatusDay)
        {
           
            var thisDate = DateTime.Now;
            var NextDate = DateTime.Now.AddDays(1);

            if (StatusDay == "Today")
            {
                 thisDate = DateTime.Now;
                 NextDate = DateTime.Now.AddDays(1);
            }
            else if(StatusDay == "Yesterday")
            {
                 thisDate = DateTime.Now.AddDays(-1);
                 NextDate = DateTime.Now;
            }
           

            string dataSt = GetStationByDay(thisDate);
            string dataMo = GetModelByDay(thisDate, NextDate, "0");
            var modelTowMonth = ModelInTowMonth();
          
            dynamic dataResult = new ExpandoObject();
            dataResult.lstStation = dataSt;
            dataResult.lstModel = dataMo;
            dataResult.lstModelTowMoth = JsonConvert.SerializeObject(modelTowMonth);
            string result = JsonConvert.SerializeObject(dataResult);
            return result;

        }


        //function for input filter
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

            var S_lstStation = String.Join(",", (from gr in station.Split(',').ToList() select "'"+gr+"'").ToList());




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
                else if (S_lstStation != "")
                {
                    sql += $@" and GROUP_NAME in ({S_lstStation}) ";
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
                                 ) WHERE INPUT> 0";





            //between 20200807 and 20200808
            DataTable _result = conn.reDt(sql);
            List<DataToDayAndChart5dayE> _LstResult = ConvertToObj.ConvertDataTable<DataToDayAndChart5dayE>(_result).ToList();

            //====data in sql
            var _data = (from data in context.ActionErrorCode
                         where data.ModelName == ModelName
                               && data.Station == StationName
                         select data).ToList();
            var count = _data;          
            if (_data.Count > 0)
            {
                _data = _data.Where(wd => wd.WorkDate >= DateTime.ParseExact(fromDate, "yyyyMMdd",null) && wd.WorkDate<=DateTime.ParseExact(nextDay1,"yyyyMMdd",null)).ToList();
            }

            var dsq = _data;
            //==================

            //====================Join SQL and Orcl

            var _resultOracle = (from data_orcl in _LstResult
                                 join data_sql in _data on
                                 new { X1 = data_orcl.WORK_DATE, X2 = data_orcl.MODEL_NAME, X3 = data_orcl.GROUP_NAME } equals
                                 new { X1 = data_sql.WorkDate.Value.ToString("yyyyMMdd"), X2 = data_sql.ModelName, X3 = data_sql.Station }

                                 into tb_dtJoin

                                 from dtjoin in tb_dtJoin.DefaultIfEmpty()
                                 select new DataToDayAndChart5dayEJoinSql
                                 {
                                     WORK_DATE = data_orcl.WORK_DATE,
                                     WORK_SECTION = data_orcl.WORK_SECTION,

                                     MODEL_NAME = data_orcl.MODEL_NAME,
                                     GROUP_NAME = data_orcl.GROUP_NAME,
                                     INPUT = data_orcl.INPUT,
                                     SUM_PASS = data_orcl.SUM_PASS,
                                     FIRST_FAIL = data_orcl.FIRST_FAIL,
                                     SUM_FAIL = data_orcl.SUM_FAIL,
                                     SUM_RE_FAIL = data_orcl.SUM_RE_FAIL,
                                    RR=data_orcl.RR,
                                    YR=data_orcl.YR,
                                    Action= dtjoin!=null? dtjoin.Action:"",                 
                                 }).ToList();

            var dataj = _resultOracle;

            //====================



            //===========old
            List<string> lstDay = _LstResult
                .Where(x => x.WORK_SECTION > 7
                || (_LstResult.Where(j => j.WORK_SECTION <= 7 && j.WORK_DATE.Contains(DateTime.ParseExact(x.WORK_DATE, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd"))).ToList().Count > 1))
                .GroupBy(dd => dd.WORK_DATE)
                .Select(d => d.Key).OrderByDescending(s => s).Take(7).OrderBy(sd => sd).ToList();
   
            var ldm = lstDay;
            List<DataOneDayHoveToShowAction> lstDataByDay = new List<DataOneDayHoveToShowAction>();        

            foreach (var day in lstDay)
            {

                //get data one day             
                var dataCrrentDay = _LstResult.Where(x => x.WORK_DATE.Contains(day) && x.WORK_SECTION > 7).ToList();
               
                var nextDay = DateTime.ParseExact(day, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");
                var test = _LstResult.Where(x => x.WORK_DATE.Contains(nextDay)).ToList();
                var dataNextDay = _LstResult.Where(nd => nd.WORK_DATE.Contains(nextDay) && nd.WORK_SECTION <= 7).ToList();
                var dataTmp = dataCrrentDay.Concat(dataNextDay).ToList();

                //=====get lstAction oneDay
                var dataCrrentDay1 = _resultOracle.Where(x => x.WORK_DATE.Contains(day) && x.WORK_SECTION > 7).ToList();            
                var nextDay11 = DateTime.ParseExact(day, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");              
                var dataNextDay1 = _resultOracle.Where(nd => nd.WORK_DATE.Contains(nextDay11) && nd.WORK_SECTION <= 7).ToList();
                var dataTmpToGetAction = dataCrrentDay1.Concat(dataNextDay1).ToList();
                var GetLstAction = dataTmpToGetAction.GroupBy(ac=>ac.Action).Select(k=>k.Key).ToList();


                lstDataByDay.Add(new DataOneDayHoveToShowAction() { WORK_DATE = day, DataToDayAndChart5dayOrcl = dataTmp,lstAction= GetLstAction });
            }


        List<DataByDayStation> DataResult = new List<DataByDayStation>();
            foreach (var dataByDay in lstDataByDay)
            {
                DataByDayStation dtTmp = new DataByDayStation();
                dtTmp.WORK_DATE = dataByDay.WORK_DATE;
                dtTmp.SUM_PASS = dataByDay.DataToDayAndChart5dayOrcl.Sum(x => x.SUM_PASS);
                dtTmp.SUM_REPASS = dataByDay.DataToDayAndChart5dayOrcl.Sum(y => y.SUM_REPASS);
                dtTmp.FIRST_FAIL = dataByDay.DataToDayAndChart5dayOrcl.Sum(ff => ff.FIRST_FAIL);
                dtTmp.SUM_FAIL = dataByDay.DataToDayAndChart5dayOrcl.Sum(f => f.SUM_FAIL);
                dtTmp.SUM_RE_FAIL = dataByDay.DataToDayAndChart5dayOrcl.Sum(k => k.SUM_RE_FAIL);
                dtTmp.INPUT = dataByDay.DataToDayAndChart5dayOrcl.Sum(input => input.INPUT);
                dtTmp.RR_s = Math.Round((dtTmp.FIRST_FAIL - dtTmp.SUM_RE_FAIL - dtTmp.SUM_FAIL) / (dtTmp.SUM_PASS + dtTmp.SUM_REPASS + dtTmp.SUM_FAIL + dtTmp.SUM_RE_FAIL) * 100, 2); //dataByDay.DataToDayAndChart5dayE.Sum(rr => rr.RR);//
                dtTmp.YR_s = Math.Round((dtTmp.SUM_PASS + dtTmp.SUM_REPASS) / (dtTmp.SUM_PASS + dtTmp.SUM_REPASS + dtTmp.SUM_RE_FAIL + dtTmp.SUM_FAIL) * 100, 2);  //dataByDay.DataToDayAndChart5dayE.Sum(yr => yr.YR);
                dtTmp.LstAction = dataByDay.lstAction;
                DataResult.Add(dtTmp);
            }


    var dt = DataResult;

            //========================================


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



        //data to show in table action
        public List<dataActionInOrcl> dataOrclToJoinTableAction(string Station, string ModelName, string ErrorCode, string Workdate)
        {

            //ERC.WORK_DATE between {Workdate} and {nextDate} and ERC.WORK_SECTION>7     
            //WORK_DATE between {Workdate} and {nextDate} AND WORK_SECTION>7

            var nextDate = DateTime.ParseExact(Workdate, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");
            string sql = $@"    select WORK_DATE,WORK_SECTION,MODEL_NAME,GROUP_NAME,ERROR_CODE, sum(Sum_Test_Fail_ER) as SUM_TEST_FAIL_ER,sum(Sum_Fail_ER) as SUM_FAIL_ER 
                                from(
                               SELECT BM.WORK_DATE,BM.WORK_SECTION,BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RP1,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as Total,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE_EC
                              FROM
                                (select ERC.WORK_DATE,ERC.WORK_SECTION, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from  SFISM4.R_ATE_ERRCODE_T  ERC
                              WHERE ERC.MODEL_NAME ='{ModelName}' and ERC.GROUP_NAME ='{Station}' and ERC.ERROR_CODE='{ErrorCode}'  and  ((ERC.WORK_DATE={Workdate} and ERC.WORK_SECTION >7) or(ERC.WORK_DATE={nextDate} and ERC.WORK_SECTION <=7))                                                    
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE,ERC.WORK_SECTION
                             ) BM 
                              LEFT JOIN 
                              (( 
                              SELECT *  FROM(
                                  SELECT WORK_DATE,WORK_SECTION, MODEL_NAME,  GROUP_NAME ,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RP1,
                                  
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  
                                  from SFISM4.R_STATION_REC_T 
                                  WHERE  ((WORK_DATE={Workdate} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7)) AND GROUP_NAME ='{Station}' AND MODEL_NAME='{ModelName}'
                                  GROUP BY WORK_DATE,MODEL_NAME, GROUP_NAME,WORK_SECTION
                                  ORDER BY RP1 DESC 
                                 ) WHERE  Sum_Pass>0                              
                                 ) BH)
                                 ON BM.MODEL_NAME=BH.MODEL_NAME
                                WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BM.WORK_DATE=BH.WORK_DATE AND BM.WORK_SECTION=BH.WORK_SECTION  and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>0  
                                ) group by WORK_DATE,WORK_SECTION,MODEL_NAME,GROUP_NAME,ERROR_CODE";

            DataTable _result = conn.reDt(sql);
            List<dataActionInOrcl> _lstResult_Orcl = ConvertToObj.ConvertDataTable<dataActionInOrcl>(_result).ToList();
            var ll = _lstResult_Orcl;
            return _lstResult_Orcl;
        }


        public DataResultTableAction comperData(DataResultTableAction a, ActionErrorCode b)
        {
            a.Week = b.Week;
            a.ModelName = b.ModelName;
            a.GroupName = b.Station;
            a.WorkSection = b.WorkSection;
            a.Proplem = b.ProblemDes;
            a.RootCause = b.RootCause;
            a.Status = b.Status;
            a.owner = b.Owner;
            a.Action = b.Action;
            a.DueDate = b.Duedate.Value.ToString("yyyyMMdd");
            a.WorkDate = b.WorkDate.Value.ToString("yyyyMMdd");
            a.WorkSection = b.WorkSection;
            return a;
        }

        public ActionResult ViewActionTable(string Station, string ModelName, string ErrorCode, DateTime Workdate)
        {
           // Workdate = Workdate.AddDays(-1);

            //data.WorkDate.Value.ToString("yyyyMMdd")== date
            var date = Workdate.ToString("yyyyMMdd");
            var _data = (from data in context.ActionErrorCode
                         where data.Station == Station
                         && data.ModelName == ModelName
                         && data.ErrorCode==ErrorCode
                         //&& data.WorkDate.Value.ToString("yyyyMMdd").Contains("20200917")
                         select data).ToList();

            if (_data.Count > 0)
            {
                _data = _data.Where(x => x.WorkDate.Value.ToString("yyyyMMdd").Contains(date)).ToList();
            }


            var _result_sql = JsonConvert.SerializeObject(_data);
            var data_sql = _data;
            var data_orcl = dataOrclToJoinTableAction(Station, ModelName, ErrorCode, date);



            //X1 = sql.WorkSection,
            //X1 = (int)orcl.WORK_SECTION,

            var _resultOracle = (from orcl in data_orcl
                           join sql in data_sql on
                           new { X1 = (int)orcl.WORK_SECTION, X2 = orcl.WORK_DATE, X3 = orcl.ERROR_CODE } equals
                           new { X1 = sql.WorkSection, X2 = sql.WorkDate.Value.ToString("yyyyMMdd"), X3 = sql.ErrorCode }

                           into tb_dtJoin

                           from dt in tb_dtJoin.DefaultIfEmpty()

                           select new DataResultTableAction
                           {
                               WorkDate = orcl.WORK_DATE,
                               DueDate = (dt != null) ? dt.Duedate.Value.ToString("yyyyMMdd") : "",
                               WorkSection = (int)orcl.WORK_SECTION,
                               ModelName = orcl.MODEL_NAME,
                               GroupName = orcl.GROUP_NAME,
                               ErrorCode = orcl.ERROR_CODE,
                               Proplem = (dt != null) ? dt.ProblemDes : "",
                               RootCause = (dt != null) ? dt.RootCause : "",
                               Action = (dt != null) ? dt.Action : "",
                               Status = (dt != null) ? dt.Status : "",
                               owner = (dt != null) ? dt.Owner : "",
                               Week = (dt != null) ? dt.Week : -5,
                               Hour = (dt != null) ? dt.WorkSection : -5,
                               FirstFail = orcl.SUM_TEST_FAIL_ER,
                           }).ToList();

            var _resultSql= (from sql in data_sql
                             join orcl in data_orcl on
                             new { X1 = sql.WorkSection, X2 = sql.WorkDate.Value.ToString("yyyyMMdd"), X3 = sql.ErrorCode } equals
                             new { X1 = (int)orcl.WORK_SECTION, X2 = orcl.WORK_DATE, X3 = orcl.ERROR_CODE } 

                             into tb_dtJoin

                             from dt in tb_dtJoin.DefaultIfEmpty()

                             select new DataResultTableAction
                             {
                                 WorkDate = sql.WorkDate.Value.ToString("yyyyMMdd"),
                                 DueDate = sql.Duedate.Value.ToString("yyyyMMdd"),
                                 WorkSection = sql.WorkSection,
                                 ModelName = sql.ModelName,
                                 GroupName = sql.Station,
                                 ErrorCode = sql.ErrorCode,
                                 Proplem = sql.ProblemDes,
                                 RootCause = sql.RootCause ,
                                 Action = sql.Action,
                                 Status = sql.Status,
                                 owner = sql.Owner,
                                 Week =sql.Week,
                                 Hour = sql.WorkSection,
                                 FirstFail = dt!=null? dt.SUM_TEST_FAIL_ER:-5,
                             }).ToList();

            var _result = _resultOracle.Union(_resultSql).DistinctBy(x=>x.WorkSection).OrderBy(ws=>ws.WorkSection).ToList();
            
            List<DataResultTableAction> dataRult = new List<DataResultTableAction>();
            dataRult = _result;


            var lstData = dataRult.GroupBy(x => x.GroupName).Select(xx => new lstAllData()
            {
                Station = xx.Key,

                ModelName = xx.Select(mm => mm.ModelName).Last(),
                lstdatabyWorkDate = xx.GroupBy(lstdd => lstdd.WorkDate).Select(dd => new lstDataActionByDay()
                {
                    WorkDate = dd.Key,
                    ErrorCode = dd.Select(er => er.ErrorCode).Last(),


                    lstDataActionbyDay = dd.Select(databyaction => new dataByStation()
                    {
                        WorkSection = databyaction.WorkSection,
                        Week = databyaction.Week,
                        Action = databyaction.Action,
                        Status = databyaction.Status,
                        DueDate = databyaction.DueDate,
                        FirstFail = databyaction.FirstFail,
                        owner = databyaction.owner,
                        Proplem = databyaction.Proplem,
                        RootCause = databyaction.RootCause,

                    }).ToList()
                }).OrderBy(wd=>DateTime.ParseExact(wd.WorkDate,"yyyyMMdd",null)).ToList(),


            }).ToList();




            var testdd = lstData;
            return PartialView("~/Views/TopErrorByStation/TableActionPartial.cshtml", lstData);
            // return View(lstData);  // Json(_result);

        }
        #endregion







    }
}