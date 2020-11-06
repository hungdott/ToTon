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
    public class RRByStationController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: RRByStation
        public ActionResult Index(string Shift, DateTime? FromDate = null, DateTime? ToDate = null)
        {

            var _now = ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : ToDate.Value.ToString("yyyyMMdd");
            var then = FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : FromDate.Value.ToString("yyyyMMdd");

            var startDay = DateTime.Now.AddDays(-6).ToString("yyyyMMdd");
            var endDay = DateTime.Now.ToString("yyyyMMdd");
            var optionDay = "fiveDay";
            var today = DateTime.Now.ToString("yyyyMMdd");
            var yesterday = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            if (String.IsNullOrEmpty(Shift))
            {

                Shift = "3";
            }

            List<string> lstDay = new List<string>();

            //REGEXP_LIKE (MODEL_NAME, '^[R_C_E_V6510_L_N_X_M_U12H](*)') 
            //REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC|RI|NFT){{1}}')
            //string _sql = "";
            //_sql += $@"select * from SFISM4.R_STATION_REC_T where
            //                (MODEL_NAME LIKE 'R%' 
            //                    OR MODEL_NAME LIKE 'M%'
            //                    OR MODEL_NAME LIKE 'E%'
            //                    OR MODEL_NAME LIKE 'C%'
            //                    OR MODEL_NAME LIKE 'D%'
            //                    OR MODEL_NAME LIKE 'L%'
            //                    OR MODEL_NAME LIKE 'N%'
            //                    OR MODEL_NAME LIKE 'G%'
            //                    OR MODEL_NAME LIKE 'X%'
            //                    OR MODEL_NAME LIKE 'O%'
            //                    OR MODEL_NAME LIKE 'V6510%'
            //                    OR MODEL_NAME LIKE 'U12H%')                            
            //                and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC|RI|NFT){{1}}')  ";


            //  _sql+=$@" and TO_DATE(WORK_DATE,'YYYYMMDD')
            //            BETWEEN TO_DATE('{then}','YYYYMMDD')
            //            and TO_DATE('{_now}','YYYYMMDD') ";


            //if (Shift == "1")
            //{
            //    _sql += " and WORK_SECTION > 7 and WORK_SECTION < 20";
            //}
            //else if (Shift == "2")
            //{
            //    _sql += " and (WORK_SECTION < 8 or WORK_SECTION > 19)";
            //}
            //else if(Shift == "3")
            //{
            //    _sql += "";
            //}

            //_sql += " order by MODEL_NAME";



            string _sql = "";

            _sql += $@"select * from SFISM4.R_STATION_REC_T where
                            (MODEL_NAME LIKE 'R%' 
                                OR MODEL_NAME LIKE 'M%'
                                OR MODEL_NAME LIKE 'R%'
                                OR MODEL_NAME LIKE 'E%'
                                OR MODEL_NAME LIKE 'C%'
                                OR MODEL_NAME LIKE 'D%'
                                OR MODEL_NAME LIKE 'L%'
                                OR MODEL_NAME LIKE 'N%'
                                OR MODEL_NAME LIKE 'G%'
                                OR MODEL_NAME LIKE 'X%'
                                OR MODEL_NAME LIKE 'O%'
                                OR MODEL_NAME LIKE 'V6510%'
                                OR MODEL_NAME LIKE 'U12H%')   
                                and GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI') ";                         
                           // and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC|RI|NFT){{1}}')  ";

            if (optionDay == "fiveDay")
            {
                _sql += $@" and TO_DATE(WORK_DATE,'YYYYMMDD')
                        BETWEEN TO_DATE('{startDay}','YYYYMMDD')
                        and TO_DATE('{endDay}','YYYYMMDD') ";
            }
            else if (optionDay == "today")
            {
                _sql += $@" and WORK_DATE={today}";
            }
            else if (optionDay == "yesterday")
            {
                _sql += $@" and WORK_DATE={yesterday}";
            }


            if (Shift == "1")
            {
                _sql += " and WORK_SECTION > 7 and WORK_SECTION < 20";
            }
            else if (Shift == "2")
            {
                _sql += " and (WORK_SECTION < 8 or WORK_SECTION > 19)";
            }
            else if (Shift == "3")
            {
                _sql += "";
            }
            else
            {
                _sql += "";
            }

            _sql += " order by MODEL_NAME";



            DataTable _result = conn.reDt(_sql);
            List<RStationRecT> _lstResult = ConvertToObj.ConvertDataTable<RStationRecT>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();

            List<StationDayVM> _lstTmp = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => new StationDayVM()
            {
                WORK_DATE = x.Key,
                StationDay = x.GroupBy(j => new { j.MODEL_NAME, j.GROUP_NAME }).Select(j => new RStationRecT()
                {
                    MODEL_NAME = j.Key.MODEL_NAME,
                    GROUP_NAME = j.Key.GROUP_NAME,
                    PASS_QTY = j.Sum(c => c.PASS_QTY),
                    FAIL_QTY = j.Sum(c => c.FAIL_QTY),
                    REPASS_QTY = j.Sum(c => c.REPASS_QTY),
                    REFAIL_QTY = j.Sum(c => c.REFAIL_QTY),
                    FIRST_FAIL_QTY = j.Sum(c => c.FIRST_FAIL_QTY),

                }).GroupBy(k => k.MODEL_NAME).Select(k => new StationVM()
                {
                    MODEL_NAME = k.Key,
                    Data = (from j in k
                            select new Data()
                            {
                                GROUP_NAME = j.GROUP_NAME,
                                PASS_QTY = j.PASS_QTY,
                                FAIL_QTY = j.FAIL_QTY,
                                REFAIL_QTY = j.REFAIL_QTY,
                                REPASS_QTY = j.REPASS_QTY,
                                FIRST_FAIL_QTY = j.FIRST_FAIL_QTY
                            }).ToList()

                }).ToList()//.OrderBy(mn=>mn.MODEL_NAME)
            }).OrderBy(x => x.WORK_DATE).ToList();

            List<RRByDayTmp> lstRRByDayTmp = new List<RRByDayTmp>();
            foreach (var data in _lstTmp)
            {
                lstDay.Add(data.WORK_DATE);
                foreach (var day in data.StationDay)
                {

                    foreach (var dataDay in day.Data)
                    {
                        RRByDayTmp tmpData = new RRByDayTmp();
                        tmpData.MODEL_NAME = day.MODEL_NAME;
                        tmpData.GROUP_NAME = dataDay.GROUP_NAME;
                        //tmpData.RETEST_RATE = (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY) > 0 ? Convert.ToDecimal(Math.Round(dataDay.REPASS_QTY * 1.0 * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2)) : -5;
                        tmpData.RETEST_RATE = (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY) > 0 ? Convert.ToDecimal(Math.Round((dataDay.FIRST_FAIL_QTY-dataDay.FAIL_QTY-dataDay.REFAIL_QTY)* 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2)) : -5;
                        tmpData.WORK_DATE = data.WORK_DATE;
                        lstRRByDayTmp.Add(tmpData);

                        //RRByDayTmp tmpData = new RRByDayTmp();
                        //tmpData.MODEL_NAME = day.MODEL_NAME;
                        //tmpData.GROUP_NAME = dataDay.GROUP_NAME;
                        //tmpData.RETEST_RATE = Convert.ToDecimal(Math.Round(dataDay.REPASS_QTY * 1.0 * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2));
                        ////tmpData.RETEST_RATE = Convert.ToDecimal(Math.Round(dataDay.REPASS_QTY * 1.0 * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2));
                        ////tmpData.RETEST_RATE = Math.Round((dataDay.FIRST_FAIL_QTY - dataDay.FAIL_QTY) * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2);
                        //tmpData.WORK_DATE = data.WORK_DATE;
                        //lstRRByDayTmp.Add(tmpData);
                    }

                }
            }

            List<RRByDay> lstRRByDay = lstRRByDayTmp.GroupBy(x => x.MODEL_NAME).Select(x => new RRByDay()
            {
                MODEL_NAME = x.Key,
                DataByDay = x.GroupBy(j => j.GROUP_NAME).Select(k => new DataByDay()
                {
                    GROUP_NAME = k.Key,
                    DataPerDay = (from date in lstDay
                                  join data in (from j in k
                                                select new DataPerDay()
                                                {
                                                    RETEST_RATE = j.RETEST_RATE,
                                                    WORK_DATE = j.WORK_DATE

                                                }).OrderBy(j => j.WORK_DATE).ToList()
                                   on date equals data.WORK_DATE into tb_data
                                  from dataPerDate in tb_data.DefaultIfEmpty()
                                  orderby date
                                  select new DataPerDay()
                                  {
                                      WORK_DATE = date,
                                      RETEST_RATE = (dataPerDate == null) ? -5m : dataPerDate.RETEST_RATE
                                  }).ToList()

                }).ToList()

            }).OrderBy(imdn1=>imdn1.MODEL_NAME).ToList();



            RRByDayDataVM dataRRByDay = new RRByDayDataVM();
            dataRRByDay.ListDay = lstDay;
            dataRRByDay.RRByDay = lstRRByDay;

            return View(dataRRByDay);
        }

        public ActionResult Filter(FilterRRByStationRequest request)
        {

            var _now = request.ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : request.ToDate.Value.ToString("yyyyMMdd");
            var then = request.FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : request.FromDate.Value.ToString("yyyyMMdd");

            var startDay = DateTime.Now.AddDays(-7).ToString("yyyyMMdd");
            var endDay = DateTime.Now.ToString("yyyyMMdd");

            var today = DateTime.Now.ToString("yyyyMMdd");
            var yesterday = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            List<string> lstDay = new List<string>();

            //string _sql = $@"select * from SFISM4.R_STATION_REC_T where 
            //                REGEXP_LIKE (MODEL_NAME, '^[R_C_E](*)') 
            //                and REGEXP_LIKE (GROUP_NAME, '(PT|FT|RC){{1}}')
            //                and TO_DATE(WORK_DATE,'YYYYMMDD') 
            //                BETWEEN TO_DATE('{then}','YYYYMMDD')
            //                and TO_DATE('{_now}','YYYYMMDD')";
            //if (request.Shift == "1")
            //{
            //    _sql += " and WORK_SECTION > 7 and WORK_SECTION < 20";
            //}
            //else if (request.Shift == "2")
            //{
            //    _sql += " and (WORK_SECTION < 8 or WORK_SECTION > 19)";
            //}
            //else if(request.Shift=="3")
            //{
            //    _sql += "";
            //}
            //_sql += " order by MODEL_NAME";

            string _sql = "";

            _sql += $@"select * from SFISM4.R_STATION_REC_T where
                            (MODEL_NAME LIKE 'R%' 
                                OR MODEL_NAME LIKE 'M%'
                                OR MODEL_NAME LIKE 'R%'
                                OR MODEL_NAME LIKE 'E%'
                                OR MODEL_NAME LIKE 'C%'
                                OR MODEL_NAME LIKE 'D%'
                                OR MODEL_NAME LIKE 'L%'
                                OR MODEL_NAME LIKE 'N%'
                                OR MODEL_NAME LIKE 'G%'
                                OR MODEL_NAME LIKE 'X%'
                                OR MODEL_NAME LIKE 'O%'
                                OR MODEL_NAME LIKE 'V6510%'
                                OR MODEL_NAME LIKE 'U12H%')                            
                            and GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')   ";

          

            if (request.optionDay == "yesterday")
            {
                _sql += $@" and TO_DATE(WORK_DATE,'YYYYMMDD')
                        BETWEEN TO_DATE('{startDay}','YYYYMMDD')
                        and TO_DATE('{yesterday}','YYYYMMDD') ";
            }
            else
            {
                _sql += $@" and TO_DATE(WORK_DATE,'YYYYMMDD')
                        BETWEEN TO_DATE('{startDay}','YYYYMMDD')
                        and TO_DATE('{endDay}','YYYYMMDD') ";
            }

            //else if(request.optionDay== "today")
            //{
            //    _sql += $@" and WORK_DATE={today}";
            //}
            //else if(request.optionDay== "yesterday")
            //{
            //    _sql += $@" and WORK_DATE={yesterday}";
            //}


            if (request.Shift == "1")
            {
                _sql += " and WORK_SECTION > 7 and WORK_SECTION < 20";
            }
            else if (request.Shift == "2")
            {
                _sql += " and (WORK_SECTION < 8 or WORK_SECTION > 19)";
            }
            else if (request.Shift == "3")
            {
                _sql += "";
            }
            else
            {
                _sql += "";
            }



            if (request.ModelName == "All Model")
            {
                _sql += "";
            }
            else if (request.ModelName != "")
            {
                _sql += $" and MODEL_NAME={'\'' + request.ModelName + '\''} ";
            }


            _sql += " order by MODEL_NAME";


            DataTable _result = conn.reDt(_sql);
            List<RStationRecT> _lstResult = ConvertToObj.ConvertDataTable<RStationRecT>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();




            //=== Model ToDay
            //string sqlModelToday = "";
            //if (request.optionDay== "today")
            //{
            //     sqlModelToday = $@" SELECT WORK_DATE, MODEL_NAME
            //                      from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%')
            //                      WHERE WORK_DATE = {today}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
            //                      GROUP BY WORK_DATE,MODEL_NAME";
            //}
            //else if(request.optionDay== "yesterday")
            //{
            //     sqlModelToday = $@" SELECT WORK_DATE, MODEL_NAME
            //                      from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%')
            //                      WHERE WORK_DATE = {yesterday}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
            //                      GROUP BY WORK_DATE,MODEL_NAME";
            //}
            //else if(request.optionDay== "fiveDay")
            //{
            //     sqlModelToday = $@" SELECT WORK_DATE, MODEL_NAME
            //                      from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%')
            //                      WHERE WORK_DATE = {today}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
            //                      GROUP BY WORK_DATE,MODEL_NAME";
            //}
            //else
            //{
            //    sqlModelToday = $@" SELECT WORK_DATE, MODEL_NAME
            //                      from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%')
            //                      WHERE WORK_DATE = {today}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
            //                      GROUP BY WORK_DATE,MODEL_NAME";
            //}

            //

            string sqlModelToday = "";

             sqlModelToday = $@" SELECT WORK_DATE, MODEL_NAME
                                  from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%')
                                  WHERE WORK_DATE =";
          
            if (request.optionDay == "yesterday")
            {
                sqlModelToday += $@"{ yesterday}";
            }
            else
            {
                sqlModelToday += $@"{ today}";
            }
            sqlModelToday += $@" AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY WORK_DATE,MODEL_NAME";
            


            DataTable _resultToDay = conn.reDt(sqlModelToday);
            List<ModelNameToday> _lstResult_today = ConvertToObj.ConvertDataTable<ModelNameToday>(_resultToDay).ToList();
            var ll = _lstResult_today;
            //==== join  list
            var listDatatToday = from all in _lstResult_today
                                 join td in _lstResult on all.MODEL_NAME equals td.MODEL_NAME
                                 select new
                                 {                                     
                                     td.WORK_DATE,
                                     td.WORK_SECTION,
                                     td.MO_NUMBER,
                                     all.MODEL_NAME,
                                     td.LINE_NAME,
                                     td.SECTION_NAME,
                                     td.GROUP_NAME,
                                     td.WIP_QTY,
                                     td.PASS_QTY,
                                     td.FAIL_QTY,
                                     td.REPASS_QTY,
                                     td.REFAIL_QTY,
                                     td.ECN_PASS_QTY,
                                     td.ECN_FAIL_QTY,
                                     td.LAST_FLAG,
                                     td.DEFECT_NO,
                                     td.RETEST,
                                     td.CLASS,
                                     td.CLASS_DATE,
                                     td.MOVE_FLAG,
                                     td.STATION_NAME,
                                     td.FIRST_FAIL_QTY,
                                     
                                 };

            var ddddd = listDatatToday;
            //====

            List<StationDayVM> _lstTmp;
            if (request.optionDay== "fiveDay")
            {
                 _lstTmp = _lstResult.GroupBy(x => x.WORK_DATE).Select(x => new StationDayVM()
                {
                    WORK_DATE = x.Key,
                    StationDay = x.GroupBy(j => new { j.MODEL_NAME, j.GROUP_NAME }).Select(j => new RStationRecT()
                    {
                        MODEL_NAME = j.Key.MODEL_NAME,
                        GROUP_NAME = j.Key.GROUP_NAME,
                        PASS_QTY = j.Sum(c => c.PASS_QTY),
                        FAIL_QTY = j.Sum(c => c.FAIL_QTY),
                        REPASS_QTY = j.Sum(c => c.REPASS_QTY),
                        REFAIL_QTY = j.Sum(c => c.REFAIL_QTY),
                        FIRST_FAIL_QTY = j.Sum(c => c.FIRST_FAIL_QTY),

                    }).GroupBy(k => k.MODEL_NAME).Select(k => new StationVM()
                    {
                        MODEL_NAME = k.Key,
                        Data = (from j in k
                                select new Data()
                                {
                                    GROUP_NAME = j.GROUP_NAME,
                                    PASS_QTY = j.PASS_QTY,
                                    FAIL_QTY = j.FAIL_QTY,
                                    REFAIL_QTY = j.REFAIL_QTY,
                                    REPASS_QTY = j.REPASS_QTY,
                                    FIRST_FAIL_QTY = j.FIRST_FAIL_QTY
                                }).ToList()

                    }).ToList()
                }).OrderBy(x => x.WORK_DATE).ToList();
            }
            else if(request.optionDay== "today")
            {
                 _lstTmp = listDatatToday.GroupBy(x => x.WORK_DATE).Select(x => new StationDayVM()
                {
                    WORK_DATE = x.Key,
                    StationDay = x.GroupBy(j => new { j.MODEL_NAME, j.GROUP_NAME }).Select(j => new RStationRecT()
                    {
                        MODEL_NAME = j.Key.MODEL_NAME,
                        GROUP_NAME = j.Key.GROUP_NAME,
                        PASS_QTY = j.Sum(c => c.PASS_QTY),
                        FAIL_QTY = j.Sum(c => c.FAIL_QTY),
                        REPASS_QTY = j.Sum(c => c.REPASS_QTY),
                        REFAIL_QTY = j.Sum(c => c.REFAIL_QTY),
                        FIRST_FAIL_QTY = j.Sum(c => c.FIRST_FAIL_QTY),

                    }).GroupBy(k => k.MODEL_NAME).Select(k => new StationVM()
                    {
                        MODEL_NAME = k.Key,
                        Data = (from j in k
                                select new Data()
                                {
                                    GROUP_NAME = j.GROUP_NAME,
                                    PASS_QTY = j.PASS_QTY,
                                    FAIL_QTY = j.FAIL_QTY,
                                    REFAIL_QTY = j.REFAIL_QTY,
                                    REPASS_QTY = j.REPASS_QTY,
                                    FIRST_FAIL_QTY = j.FIRST_FAIL_QTY
                                }).ToList()

                    }).ToList()
                }).OrderBy(x => x.WORK_DATE).ToList();
            }
           
            else
            {
                _lstTmp = listDatatToday.GroupBy(x => x.WORK_DATE).Select(x => new StationDayVM()
                {
                    WORK_DATE = x.Key,
                    StationDay = x.GroupBy(j => new { j.MODEL_NAME, j.GROUP_NAME }).Select(j => new RStationRecT()
                    {
                        MODEL_NAME = j.Key.MODEL_NAME,
                        GROUP_NAME = j.Key.GROUP_NAME,
                        PASS_QTY = j.Sum(c => c.PASS_QTY),
                        FAIL_QTY = j.Sum(c => c.FAIL_QTY),
                        REPASS_QTY = j.Sum(c => c.REPASS_QTY),
                        REFAIL_QTY = j.Sum(c => c.REFAIL_QTY),
                        FIRST_FAIL_QTY = j.Sum(c => c.FIRST_FAIL_QTY),

                    }).GroupBy(k => k.MODEL_NAME).Select(k => new StationVM()
                    {
                        MODEL_NAME = k.Key,
                        Data = (from j in k
                                select new Data()
                                {
                                    GROUP_NAME = j.GROUP_NAME,
                                    PASS_QTY = j.PASS_QTY,
                                    FAIL_QTY = j.FAIL_QTY,
                                    REFAIL_QTY = j.REFAIL_QTY,
                                    REPASS_QTY = j.REPASS_QTY,
                                    FIRST_FAIL_QTY = j.FIRST_FAIL_QTY
                                }).ToList()

                    }).ToList()
                }).OrderBy(x => x.WORK_DATE).ToList();
            }



            List<RRByDayTmp> lstRRByDayTmp = new List<RRByDayTmp>();
            foreach (var data in _lstTmp)
            {
                lstDay.Add(data.WORK_DATE);
                foreach (var day in data.StationDay)
                {

                    foreach (var dataDay in day.Data)
                    {
                        RRByDayTmp tmpData = new RRByDayTmp();
                        tmpData.MODEL_NAME = day.MODEL_NAME;
                        tmpData.GROUP_NAME = dataDay.GROUP_NAME;
                        //tmpData.RETEST_RATE = Convert.ToDecimal(Math.Round((dataDay.FIRST_FAIL_QTY-dataDay.FAIL_QTY-dataDay.REFAIL_QTY)*100/ (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2));
                        tmpData.RETEST_RATE = (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY) > 0 ? Convert.ToDecimal(Math.Round((dataDay.FIRST_FAIL_QTY - dataDay.FAIL_QTY - dataDay.REFAIL_QTY) * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2)) : -5;
                        //tmpData.RETEST_RATE = Math.Round((dataDay.FIRST_FAIL_QTY - dataDay.FAIL_QTY) * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2);
                        tmpData.WORK_DATE = data.WORK_DATE;
                        lstRRByDayTmp.Add(tmpData);
                    }

                }
            }

            List<RRByDay> lstRRByDay = lstRRByDayTmp.GroupBy(x => x.MODEL_NAME).Select(x => new RRByDay()
            {
                MODEL_NAME = x.Key,
                DataByDay = x.GroupBy(j => j.GROUP_NAME).Select(k => new DataByDay()
                {
                    GROUP_NAME = k.Key,
                    DataPerDay = (from date in lstDay
                                  join data in (from j in k
                                                select new DataPerDay()
                                                {
                                                    RETEST_RATE = j.RETEST_RATE,
                                                    WORK_DATE = j.WORK_DATE

                                                }).OrderBy(j => j.WORK_DATE).ToList()
                                   on date equals data.WORK_DATE into tb_data
                                  from dataPerDate in tb_data.DefaultIfEmpty()
                                  orderby date
                                  select new DataPerDay()
                                  {
                                      WORK_DATE = date,
                                      RETEST_RATE = (dataPerDate == null) ? -5m : dataPerDate.RETEST_RATE
                                  }).ToList()
                }).ToList()

            }).OrderBy(fmd=>fmd.MODEL_NAME).ToList();

           



            RRByDayDataVM dataRRByDay = new RRByDayDataVM();
            dataRRByDay.ListDay = lstDay;
            dataRRByDay.RRByDay = lstRRByDay;
            return PartialView("~/Views/RRByStation/ErrorByStationPartial.cshtml", dataRRByDay);
            //return View(request);
        }






        public ActionResult FilterToDay(FilterRRByStationRequest request)
        {

            var _now = request.ToDate == null ? DateTime.Now.ToString("yyyyMMdd") : request.ToDate.Value.ToString("yyyyMMdd");
            var then = request.FromDate == null ? DateTime.Now.AddDays(-5).ToString("yyyyMMdd") : request.FromDate.Value.ToString("yyyyMMdd");

            var startDay = DateTime.Now.AddDays(-6).ToString("yyyyMMdd");
            var endDay = DateTime.Now.ToString("yyyyMMdd");

            var today = DateTime.Now.ToString("yyyyMMdd");
            var yesterday = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            List<string> lstDay = new List<string>();          
            string _sql = "";

            _sql += $@"select * from SFISM4.R_STATION_REC_T where
                            (MODEL_NAME LIKE 'R%' 
                                OR MODEL_NAME LIKE 'M%'
                                OR MODEL_NAME LIKE 'R%'
                                OR MODEL_NAME LIKE 'E%'
                                OR MODEL_NAME LIKE 'C%'
                                OR MODEL_NAME LIKE 'D%'
                                OR MODEL_NAME LIKE 'L%'
                                OR MODEL_NAME LIKE 'N%'
                                OR MODEL_NAME LIKE 'G%'
                                OR MODEL_NAME LIKE 'X%'
                                OR MODEL_NAME LIKE 'O%'
                                OR MODEL_NAME LIKE 'V6510%'
                                OR MODEL_NAME LIKE 'U12H%')                            
                            and GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')   ";

            if (request.optionDay == "fiveDay")
            {
                _sql += $@" and TO_DATE(WORK_DATE,'YYYYMMDD')
                        BETWEEN TO_DATE('{startDay}','YYYYMMDD')
                        and TO_DATE('{endDay}','YYYYMMDD') ";
            }
            else if (request.optionDay == "today")
            {
                _sql += $@" and WORK_DATE={today}";
            }
            else if (request.optionDay == "yesterday")
            {
                _sql += $@" and WORK_DATE={yesterday}";
            }


            if (request.Shift == "1")
            {
                _sql += " and WORK_SECTION > 7 and WORK_SECTION < 20";
            }
            else if (request.Shift == "2")
            {
                _sql += " and (WORK_SECTION < 8 or WORK_SECTION > 19)";
            }
            else if (request.Shift == "3")
            {
                _sql += "";
            }
            else
            {
                _sql += "";
            }



            if (request.ModelName == "All Model")
            {
                _sql += "";
            }
            else if (request.ModelName != "")
            {
                _sql += $" and MODEL_NAME={'\'' + request.ModelName + '\''} ";
            }


            _sql += " order by MODEL_NAME";


            DataTable _result = conn.reDt(_sql);
            List<RStationRecT> _lstResult = ConvertToObj.ConvertDataTable<RStationRecT>(_result)
                .Where(x => !x.GROUP_NAME.Contains("R_") && !x.GROUP_NAME.Contains("OBA")).ToList();


            //=== Model ToDay
            string sqlModelToday = $@" SELECT WORK_DATE, MODEL_NAME
                                  from(SELECT * FROM SFISM4.R_STATION_REC_T WHERE  MODEL_NAME LIKE 'M%' or MODEL_NAME LIKE 'R%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%' OR MODEL_NAME LIKE 'D%' OR MODEL_NAME LIKE 'L%' OR MODEL_NAME LIKE 'N%' OR MODEL_NAME LIKE 'G%' OR MODEL_NAME LIKE 'V6510%' OR MODEL_NAME LIKE 'U12H%')
                                  WHERE WORK_DATE = {today}   AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  GROUP BY WORK_DATE,MODEL_NAME";



            DataTable _resultToDay = conn.reDt(sqlModelToday);
            List<ModelNameToday> _lstResult_today = ConvertToObj.ConvertDataTable<ModelNameToday>(_resultToDay).ToList();
            var ll = _lstResult_today;
            //==== join  list
            var listDatatToday = from all in _lstResult_today
                                 join td in _lstResult on all.MODEL_NAME equals td.MODEL_NAME
                                 select new
                                 {
                                     td.WORK_DATE,
                                     td.WORK_SECTION,
                                     td.MO_NUMBER,
                                     all.MODEL_NAME,
                                     td.LINE_NAME,
                                     td.SECTION_NAME,
                                     td.GROUP_NAME,
                                     td.WIP_QTY,
                                     td.PASS_QTY,
                                     td.FAIL_QTY,
                                     td.REPASS_QTY,
                                     td.REFAIL_QTY,
                                     td.ECN_PASS_QTY,
                                     td.ECN_FAIL_QTY,
                                     td.LAST_FLAG,
                                     td.DEFECT_NO,
                                     td.RETEST,
                                     td.CLASS,
                                     td.CLASS_DATE,
                                     td.MOVE_FLAG,
                                     td.STATION_NAME,
                                     td.FIRST_FAIL_QTY,
                                 };

            var ddddd = listDatatToday;
            //====

            List<StationDayVM> _lstTmp = listDatatToday.GroupBy(x => x.WORK_DATE).Select(x => new StationDayVM()
            {
                WORK_DATE = x.Key,
                StationDay = x.GroupBy(j => new { j.MODEL_NAME, j.GROUP_NAME }).Select(j => new RStationRecT()
                {
                    MODEL_NAME = j.Key.MODEL_NAME,
                    GROUP_NAME = j.Key.GROUP_NAME,
                    PASS_QTY = j.Sum(c => c.PASS_QTY),
                    FAIL_QTY = j.Sum(c => c.FAIL_QTY),
                    REPASS_QTY = j.Sum(c => c.REPASS_QTY),
                    REFAIL_QTY = j.Sum(c => c.REFAIL_QTY),
                    FIRST_FAIL_QTY = j.Sum(c => c.FIRST_FAIL_QTY),

                }).GroupBy(k => k.MODEL_NAME).Select(k => new StationVM()
                {
                    MODEL_NAME = k.Key,
                    Data = (from j in k
                            select new Data()
                            {
                                GROUP_NAME = j.GROUP_NAME,
                                PASS_QTY = j.PASS_QTY,
                                FAIL_QTY = j.FAIL_QTY,
                                REFAIL_QTY = j.REFAIL_QTY,
                                REPASS_QTY = j.REPASS_QTY,
                                FIRST_FAIL_QTY = j.FIRST_FAIL_QTY
                            }).ToList()

                }).ToList()
            }).OrderBy(x => x.WORK_DATE).ToList();

            List<RRByDayTmp> lstRRByDayTmp = new List<RRByDayTmp>();
            foreach (var data in _lstTmp)
            {
                lstDay.Add(data.WORK_DATE);
                foreach (var day in data.StationDay)
                {

                    foreach (var dataDay in day.Data)
                    {
                        RRByDayTmp tmpData = new RRByDayTmp();
                        tmpData.MODEL_NAME = day.MODEL_NAME;
                        tmpData.GROUP_NAME = dataDay.GROUP_NAME;
                        //tmpData.RETEST_RATE = Convert.ToDecimal(Math.Round((dataDay.FIRST_FAIL_QTY-dataDay.FAIL_QTY-dataDay.REFAIL_QTY)*100/ (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2));
                        tmpData.RETEST_RATE = (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY) > 0 ? Convert.ToDecimal(Math.Round((dataDay.FIRST_FAIL_QTY - dataDay.FAIL_QTY - dataDay.REFAIL_QTY) * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2)) : -5;
                        //tmpData.RETEST_RATE = Math.Round((dataDay.FIRST_FAIL_QTY - dataDay.FAIL_QTY) * 100 / (dataDay.FAIL_QTY + dataDay.PASS_QTY + dataDay.REFAIL_QTY + dataDay.REPASS_QTY), 2);
                        tmpData.WORK_DATE = data.WORK_DATE;
                        lstRRByDayTmp.Add(tmpData);
                    }

                }
            }

            List<RRByDay> lstRRByDay = lstRRByDayTmp.GroupBy(x => x.MODEL_NAME).Select(x => new RRByDay()
            {
                MODEL_NAME = x.Key,
                DataByDay = x.GroupBy(j => j.GROUP_NAME).Select(k => new DataByDay()
                {
                    GROUP_NAME = k.Key,
                    DataPerDay = (from date in lstDay
                                  join data in (from j in k
                                                select new DataPerDay()
                                                {
                                                    RETEST_RATE = j.RETEST_RATE,
                                                    WORK_DATE = j.WORK_DATE

                                                }).OrderBy(j => j.WORK_DATE).ToList()
                                   on date equals data.WORK_DATE into tb_data
                                  from dataPerDate in tb_data.DefaultIfEmpty()
                                  orderby date
                                  select new DataPerDay()
                                  {
                                      WORK_DATE = date,
                                      RETEST_RATE = (dataPerDate == null) ? -5m : dataPerDate.RETEST_RATE
                                  }).ToList()
                }).ToList()

            }).OrderBy(tfmd=>tfmd.MODEL_NAME).ToList();

           

            RRByDayDataVM dataRRByDay = new RRByDayDataVM();
            dataRRByDay.ListDay = lstDay;
            dataRRByDay.RRByDay = lstRRByDay;
            return PartialView("~/Views/RRByStation/ErrorByStationPartial.cshtml", dataRRByDay);
            
        }
    }
}