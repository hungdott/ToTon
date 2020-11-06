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
using System.Dynamic;
using System.Data.Entity.Migrations.Sql;


namespace Report.Controllers
{
    public class TopErrorCodeByModelNameController : Controller
    {
        // GET: TopErrorCodeByModelName
        ConnectDbSfis conn = new ConnectDbSfis();
        ReportSysContext context = new ReportSysContext();
        // GET: TopErrorByStation
        public ActionResult Index()
        {
            //var _now = request.Date == null ? DateTime.Now.ToString("yyyyMMdd") : request.Date.Value.ToString("yyyyMMdd");
            //bool option =request.Option;
            var _now = DateTime.Now.ToString("yyyyMMdd");
            var _nextDate =DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            string shift = "";
            bool option = false;
            string groupName = "";
            string modelName = "";

            var lstByGroupName = GetData(_now, _nextDate, shift, option, groupName, modelName);
            var data = new M_TopData();
            data.M_listData = lstByGroupName;
            data.WorkDate = DateTime.ParseExact(_now, "yyyyMMdd", null);

            List<string> lstModel = new List<string>();

            foreach(var md in lstByGroupName)
            {
                lstModel.Add(md.ModelName);
            }
            ViewBag.lstmodel = lstModel;

            return View(data);
        }

        public ActionResult GetTop3(TopRRByModelRequest request)//bool checkTop3
        {
            var _now = request.Date == null ? DateTime.Now.ToString("yyyyMMdd") : request.Date.Value.ToString("yyyyMMdd");
            var NextDate=request.NextDate==null? DateTime.Now.AddDays(1).ToString("yyyyMMdd") : request.NextDate.Value.ToString("yyyyMMdd");
            string shift = request.Shift;
            bool option = request.Option;
            string groupName = request.group_name == null ? "" : request.group_name;
            string modelName = request.ModelName == null ? "" : request.ModelName;

            var lstByModelName = GetData(_now, NextDate, shift, option, groupName, modelName);
            var data = new M_TopData();
            data.M_listData = lstByModelName;
            data.WorkDate = DateTime.ParseExact(_now, "yyyyMMdd", null);
            return PartialView("~/Views/TopErrorCodeByModelName/TopErrorByModelPartial.cshtml", data);//lstByGroupName
        }


        public List<M_TOPDataByModelName> GetData(string date, string nextDate, string Shift, bool option, string Group_name, string Model_name)//bool takeTop3
        {
            #region From SQl
           
            string ll = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6,2);
            DateTime datetime = Convert.ToDateTime(ll);
            // var datetime = DateTime.Parse(ll);
            // var datetime = DateTime.Parse(ll).ToString("yyyyMMdd");
            // var datetime = Convert.ToDateTime(ll).ToString("yyyyMMdd");


            //tam thoi comment
            var lstActionErrorCode = (from aec in context.ActionErrorCode
                                          // where aec.WorkDate.Value.ToString("yyyyMMdd") == datetime
                                      where aec.WorkDate == datetime
                                      select aec).ToList();

            var lll = lstActionErrorCode;

            #endregion

            #region FromSfis
            string _sql = "";
            _sql = $@"SELECT BH.MODEL_NAME,BH.GROUP_NAME,BM.ERROR_CODE,BM.STATION_NAME,BH.RATE_STATION,BH.Y_RATE_STATION,
                              BM.Sum_Test_Fail_ER,BM.Sum_Fail_ER,( BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail) as TOTAL_STATION,
                              BH.First_Fail,BH.Sum_FAIL,
                               ROUND((BM.Sum_Test_Fail_ER-BM.Sum_Fail_ER)/(BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail)*100,2) AS RATE_EC
                              FROM
                                (select ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.ERROR_CODE,ERC.STATION_NAME,COUNT(ERC.ERROR_CODE)
                              ,SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE (MODEL_NAME LIKE 'R%' OR MODEL_NAME LIKE 'M%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V%' OR MODEL_NAME LIKE 'U12H%')) ERC
                              ";

            if (Shift == "1")
            {
                _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=7 and ERC.WORK_SECTION < 20";
            }
            else if (Shift == "2")
            {
                 _sql += $@"  WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7)) ";
               // _sql += $@" WHERE ERC.WORK_DATE = {date} and ERC.WORK_DATE ={ date} and ERC.WORK_SECTION >= 20";
            }
           
            else if(Shift == "0")
            {          
              
                _sql += $@"  WHERE ERC.WORK_DATE between {date} and {nextDate} AND ((ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=20) OR (ERC.WORK_DATE ={nextDate} and ERC.WORK_SECTION <= 7)) or (ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=7 and ERC.WORK_SECTION < 20) ";
            }
            else
            {
                _sql += $@" WHERE ERC.WORK_DATE ={date} and ERC.WORK_SECTION >=7 and ERC.WORK_SECTION < 20";
            }


            _sql += $@" AND ERC.GROUP_NAME IN('FT','FT1','FT2','PT','PT1','PT2','NFT','RC','RC1','RC2','RI')
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME
                              ORDER BY COUNT(ERC.ERROR_CODE) DESC) BM 
                              LEFT JOIN 
                              (( SELECT *  FROM(
                                  SELECT MODEL_NAME,  GROUP_NAME,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS Y_RATE_STATION,
                                  ROUND(((SUM(PASS_QTY)+(SUM(REPASS_QTY))) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'R%' OR MODEL_NAME LIKE 'M%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V%' OR MODEL_NAME LIKE 'U12H%' ) ";

          // _sql+= $@"WHERE  WORK_DATE  BETWEEN  {date} AND {nextDate} ";
            if (Shift == "1")
            {
                _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>=7 and WORK_SECTION<20 ";
            }
            else if(Shift=="2")
            {
               // _sql += $@" where  WORK_DATE ={ date} and WORK_SECTION >=20 ";
                _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) ";
            }
           
            else if(Shift=="0")
            {
                _sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) or (WORK_DATE ={date} and WORK_SECTION >=7 and WORK_SECTION < 20) ";
            }
            else
            {             
                _sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>=7 and WORK_SECTION<20 ";
            }

 
            _sql +=$@"AND GROUP_NAME IN('FT','FT1','FT2','PT','PT1','PT2','NFT','RC','RC1','RC2','RI')
                                  GROUP BY MODEL_NAME, GROUP_NAME
                                  ORDER BY RATE_STATION DESC 
                                 ) WHERE  Sum_Pass>0 ) BH) ON BM.MODEL_NAME=BH.MODEL_NAME
                                WHERE  BM.GROUP_NAME=BH.GROUP_NAME and BH.Sum_Pass+BH.Sum_FAIL+BH.Sum_RePass+BH.Sum_Re_Fail>10  ";



            //and BH.MODEL_NAME= 'RAX20-100NASV1'
            if(Group_name== "All Station")
            {
                _sql += "";
            }
            else if (Group_name != "")
            {
                _sql += $" and BH.GROUP_NAME ={'\''+Group_name+'\''} ";
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

            string sql = _sql;

            DataTable _result = conn.reDt(sql);
            List<TopErrorCodeByModelName> _lstResult = ConvertToObj.ConvertDataTable<TopErrorCodeByModelName>(_result).ToList();




            //=============================
            //var joinData = _lstResult.Join(lstActionErrorCode,
            //                                    ActionErrorCode_sfis => ActionErrorCode_sfis.MODEL_NAME,
            //                                    ModelName_Sql => ModelName_Sql.ModelName,

            //                                    (sfis, Sql) => new
            //                                    {                                                                                                       
            //                                         WorkSection= sfis.WORK_SECTION,
            //                                        MODEL_NAME = sfis.MODEL_NAME,
            //                                        GROUP_NAME = sfis.GROUP_NAME,
            //                                        ERROR_CODE = sfis.ERROR_CODE,
            //                                        STATION_NAME = sfis.STATION_NAME,
            //                                        RATE_STATION = sfis.RATE_STATION,
            //                                        Y_RATE_STATION = sfis.Y_RATE_STATION,
            //                                        SUM_TEST_FAIL_ER = sfis.SUM_TEST_FAIL_ER,
            //                                        SUM_FAIL_ER = sfis.SUM_FAIL_ER,
            //                                        TOTAL_STATION = sfis.TOTAL_STATION,
            //                                        RATE_EC = sfis.RATE_EC,
            //                                        FIRST_FAIL = sfis.FIRST_FAIL,
            //                                        SUM_FAIL = sfis.SUM_FAIL,

            //                                         workDate= Sql.WorkDate,
            //                                         PCName= Sql.PCname,
            //                                        Owner= Sql.Owner,
            //                                        Duedate= Sql.Duedate,
            //                                        Status= Sql.Status,
            //                                        action = Sql.Action,
            //                                        RootCause= Sql.RootCause,
            //                                    });

            //var joindataList = joinData;



            //comment
            var joinData = _lstResult.Join(lstActionErrorCode,
                                                ActionErrorCode_sfis => ActionErrorCode_sfis.MODEL_NAME,
                                                ModelName_Sql => ModelName_Sql.ModelName,

                                                (sfis, Sql) => new
                                                {
                                                    WorkSection = sfis.WORK_SECTION,
                                                    MODEL_NAME = sfis.MODEL_NAME,
                                                    GROUP_NAME = sfis.GROUP_NAME,
                                                    ERROR_CODE = sfis.ERROR_CODE,
                                                    STATION_NAME = sfis.STATION_NAME,
                                                    RATE_STATION = sfis.RATE_STATION,
                                                    Y_RATE_STATION = sfis.Y_RATE_STATION,
                                                    SUM_TEST_FAIL_ER = sfis.SUM_TEST_FAIL_ER,
                                                    SUM_FAIL_ER = sfis.SUM_FAIL_ER,
                                                    TOTAL_STATION = sfis.TOTAL_STATION,
                                                    RATE_EC = sfis.RATE_EC,
                                                    FIRST_FAIL = sfis.FIRST_FAIL,
                                                    SUM_FAIL = sfis.SUM_FAIL,

                                                    workDate = Sql.WorkDate,
                                                    PCName = Sql.PCname,
                                                    Owner = Sql.Owner,
                                                    Duedate = Sql.Duedate,
                                                    Status = Sql.Status,
                                                    action = Sql.Action,
                                                    RootCause = Sql.RootCause,
                                                });

            var joindataList = joinData;

            //=============================================================

           
          //  var lstActionERByDay = lstActionErrorCode.Where(x =>x.WorkDate.Equals(date));

            var jj = (from sfis in _lstResult
                      join Sql in lstActionErrorCode
                      on new {station=sfis.GROUP_NAME, model = sfis.MODEL_NAME, err = sfis.ERROR_CODE }//,wd = sfis.WORK_DATE
                      equals new { station=Sql.Station, model = Sql.ModelName, err = Sql.ErrorCode }//, wd = Sql.WorkDate.Value.ToString("yyyyMMdd")
                      into tb_Datajjjj
                      from data111 in tb_Datajjjj    //.DefaultIfEmpty()
                      select new SolutionActionData()
                      {
                          WORK_SECTION = sfis.WORK_SECTION,
                          MODEL_NAME = sfis.MODEL_NAME,
                          GROUP_NAME = sfis.GROUP_NAME,
                          ERROR_CODE = sfis.ERROR_CODE,
                          STATION_NAME = sfis.STATION_NAME,
                          RATE_STATION = sfis.RATE_STATION,
                          Y_RATE_STATION = sfis.Y_RATE_STATION,
                          SUM_TEST_FAIL_ER = sfis.SUM_TEST_FAIL_ER,
                          SUM_FAIL_ER = sfis.SUM_FAIL_ER,
                          TOTAL_STATION = sfis.TOTAL_STATION,
                          RATE_EC = sfis.RATE_EC,
                          FIRST_FAIL = sfis.FIRST_FAIL,
                          SUM_FAIL = sfis.SUM_FAIL,

                          Action = (data111!=null)?data111.Action:"",
                          PCname = (data111 != null) ? data111.PCname:"",
                          Owner = (data111 != null) ? data111.Owner:"",
                          RootCause = (data111 != null) ? data111.RootCause:"",
                          Status = (data111 != null) ? data111.Status:"",
                          
                      }).ToList();

            var testJ = jj;

            //========================================================
            List<M_TOPDataByModelName> lstByModelName = _lstResult.GroupBy(x => x.MODEL_NAME).Select(xy => new M_TOPDataByModelName()//_lstResult
            {
                ModelName = xy.Key,
                //count_row_station = xy.GroupBy(rs => rs.STATION_NAME).Count(),
                //count_row_station1 = xy.GroupBy(rm => rm.STATION_NAME).Count(),
                count_groupName = xy.GroupBy(countM => countM.GROUP_NAME).Count(),


                DataByGroupName = xy.GroupBy(j => j.GROUP_NAME).Select(ex => new M_DataByGroupName()
                {
                    GroupName = ex.Key,

                    Input = ex.Max(a => a.TOTAL_STATION),
                    RR = ex.Max(rr => rr.RATE_STATION),
                    YR = ex.Max(yr => yr.Y_RATE_STATION),
                    First_Fail_Of_Model_Name = ex.Max(ffm => ffm.FIRST_FAIL),
                   Fail_Of_Model_Name =ex.Max(fm=>fm.SUM_FAIL),


                    //count_row_ModelName = ex.GroupBy(rm => rm.STATION_NAME).Count(),//jhfjhjh
                    count_row_ERR_in_groupName = ex.GroupBy(count_err => count_err.ERROR_CODE).Count(),

                    FailByErrorCode = ex.GroupBy(e => e.ERROR_CODE).Select(k => new M_FailByErrorCode()
                    {
                        ErrorCode = k.Key,
                        count_row_Errorcode = k.GroupBy(rrr => rrr.STATION_NAME).Count(),
                        FailQtyErrCode = k.Sum(q => q.SUM_TEST_FAIL_ER),
                        ReFailQtyErrorcode=k.Sum(rfe=>rfe.SUM_FAIL_ER),
                        M_FailByMachine = k.Select(ee => new M_FailByMachine()
                        {
                            PCName = ee.STATION_NAME,
                            FailQtyPC = ee.SUM_TEST_FAIL_ER,
                            ReFail = ee.SUM_FAIL_ER,
                        }).ToList(),


                    }).ToList()


                }).OrderByDescending(x => x.RR).ToList()
            }).ToList();

            //take top 3
            if (option == true)
            {

                foreach (var grName in lstByModelName)
                {
                    //grName.DataByGroupName = grName.DataByGroupName.Take(3).ToList();
                    //grName.count_groupName = grName.DataByGroupName.Count;
                    foreach (var item in grName.DataByGroupName)
                    {
                        item.FailByErrorCode = item.FailByErrorCode
                                            .OrderByDescending(x => x.FailQtyErrCode)
                                            .Take(3).ToList();
                        item.count_row_ERR_in_groupName = item.FailByErrorCode.Count;
                    }
                }
            }
            #endregion
            var l = lstByModelName;

          
            return lstByModelName;

        }

        public ActionResult AddAction(ActionErrorCodeVM _vm)
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




        //onApply
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

            sql += $@" and REGEXP_LIKE(MODEL_NAME, '^(R|M|E|C|D|L|N|G|X|O|U|V){{1}}')
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
        //



    }
}
