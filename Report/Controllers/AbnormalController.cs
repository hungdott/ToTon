using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Entity;
using Report.Common;
using Report.Models;
using System.Data;

namespace Report.Controllers
{
    public class AbnormalController : Controller
    {
        // GET: Abnormal
        ConnectDbSfis conn = new ConnectDbSfis();
        public ActionResult Index()
        {
            string Shift = "day";
            string date = DateTime.Now.ToString("yyyyMMdd");
            string nextDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            int FromHour = 30;
            int ToHour = 30;


            var lstdataBymodle = dataModelByShift(date, nextDate, Shift,  FromHour,  ToHour);

            return View(lstdataBymodle);
        }
        public List<AbnormalEntity> GetData1(string date, string nextDate, string Shift)
        {
            string sql = "";
            sql = $@" SELECT * FROM(
                                  SELECT LINE_NAME,WORK_DATE,WORK_SECTION, MODEL_NAME,  GROUP_NAME ,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY) as INPUT,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR                                                                                                    
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T) ";


            if (Shift == "day")
            {
                sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
            }
            else if (Shift == "night")
            {

                sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) ";
            }

            else if (Shift == "Allday")
            {

                sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE={date} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7)) ";
            }
            else
            {
                sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
            }


            sql += $@"  AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  AND  (MODEL_NAME LIKE 'R%' 
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
                                        OR MODEL_NAME LIKE 'V6510%')                             
                                  GROUP BY WORK_DATE,MODEL_NAME, GROUP_NAME,LINE_NAME,WORK_SECTION
                                  ORDER BY RR DESC )
                                  WHERE INPUT>10  ";
            DataTable _result = conn.reDt(sql);
            List<AbnormalEntity> _lstResult = ConvertToObj.ConvertDataTable<AbnormalEntity>(_result);
            return _lstResult;
        }

        public List<AbnormalEntity> GetData(string date, string nextDate, string Shift,int FromHour,int ToHour)
        {
            string sql = "";
            sql = $@" SELECT * FROM(
                                  SELECT LINE_NAME,WORK_DATE,WORK_SECTION, MODEL_NAME,  GROUP_NAME ,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY) as INPUT,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR                                                                                                    
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T) ";

            if(FromHour==30)
            {
                if (Shift == "day")
                {
                    sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
                else if (Shift == "night")
                {

                    sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE ={date} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) ";
                }

                else if (Shift == "Allday")
                {

                    sql += $@"  WHERE WORK_DATE between {date} and {nextDate} AND ((WORK_DATE={date} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7)) ";
                }
                else
                {
                    sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
            }
            else if(FromHour<30 && ToHour<=30)
            {
                sql += $@" WHERE  WORK_DATE= {date}  and WORK_SECTION>={FromHour} and WORK_SECTION<{ToHour} ";
            }
           
            


            sql += $@"  AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  AND  (MODEL_NAME LIKE 'R%' 
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
                                        OR MODEL_NAME LIKE 'V6510%')                             
                                  GROUP BY WORK_DATE,MODEL_NAME, GROUP_NAME,LINE_NAME,WORK_SECTION
                                  ORDER BY RR DESC )
                                  WHERE INPUT>10  ";
            DataTable _result = conn.reDt(sql);
            List<AbnormalEntity> _lstResult = ConvertToObj.ConvertDataTable<AbnormalEntity>(_result);
            return _lstResult;
        }

        public List<DataInPutModelNamebyShift> dataModelByShift(string date, string nextDate, string Shift, int FromHour, int ToHour)
        {
            var data = GetData(date, nextDate, Shift,  FromHour,  ToHour);
            List<DataInPutModelNamebyShift> DataResult = data.GroupBy(x => x.MODEL_NAME).Select(xx => new DataInPutModelNamebyShift()
            {
                ModelName = xx.Key,
                LstDataByGroupName = xx.GroupBy(gr => gr.GROUP_NAME).Select(dgr => new DataModelNameByShift()
                {
                    GroupName = dgr.Key,
                    InPut = dgr.Sum(ip => ip.INPUT),
                }).ToList()
            }).OrderBy(md => md.ModelName).ToList();

            return DataResult;
        }

        public ActionResult DataFilter(string thisDate, string Shift, int FromHour, int ToHour)
        {
            var nextDate = DateTime.ParseExact(thisDate, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");


            var dataResult = dataModelByShift(thisDate, nextDate, Shift,  FromHour,  ToHour);
            return PartialView("~/Views/Abnormal/AbnormalPartial.cshtml", dataResult);

        }

        public ActionResult DataDrawChartByModelByAbnormal(string ModelName, string DataTime, string Shift, int FromHour, int ToHour)
        {
            var thisTime = DataTime != null ? DataTime : DateTime.Now.ToString("yyyyMMdd");
            var nextDate = DateTime.ParseExact(thisTime, "yyyyMMdd", null).AddDays(1).ToString("yyyyMMdd");


            string sql = $@" SELECT * FROM(
                                  SELECT LINE_NAME,WORK_DATE,WORK_SECTION, MODEL_NAME,  GROUP_NAME ,SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                                  SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY) as INPUT,
                                  ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR                                                                                                    
                                  from (SELECT * FROM SFISM4.R_STATION_REC_T) ";

            if(FromHour==30)
            {
                if (Shift == "day")
                {
                    sql += $@"  WHERE  WORK_DATE= {thisTime}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
                else if (Shift == "night")
                {

                    sql += $@"  WHERE WORK_DATE between {thisTime} and {nextDate} AND ((WORK_DATE ={thisTime} and WORK_SECTION >=20) OR (WORK_DATE ={nextDate} and WORK_SECTION <= 7)) ";
                }

                else if (Shift == "Allday")
                {

                    sql += $@"  WHERE WORK_DATE between {thisTime} and {nextDate} AND ((WORK_DATE={thisTime} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7)) ";
                }
                else
                {
                    sql += $@" WHERE  WORK_DATE= {thisTime}  and WORK_SECTION>7 and WORK_SECTION<20 ";
                }
            }
            else if(FromHour < 30 && ToHour <= 30)                
            {
                sql += $@" WHERE  WORK_DATE= {thisTime}  and WORK_SECTION>={FromHour} and WORK_SECTION<{ToHour} ";
            }
           

            // sql += $@"  WHERE  WORK_DATE between {thisTime} and {nextDate} AND ((WORK_DATE={thisTime} and WORK_SECTION >7) or(WORK_DATE={nextDate} and WORK_SECTION <=7))   ";

            sql += $@"  AND GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                                  AND  MODEL_NAME='{ModelName}'                           
                                  GROUP BY WORK_DATE,MODEL_NAME, GROUP_NAME,LINE_NAME,WORK_SECTION
                                  ORDER BY RR DESC )
                                  WHERE INPUT>10 ";
            DataTable _result = conn.reDt(sql);
            List<AbnormalEntity> _lstResult = ConvertToObj.ConvertDataTable<AbnormalEntity>(_result);
            var lstWorkSection = _lstResult.GroupBy(ws => ws.WORK_SECTION).Select(ww => ww.Key).ToList();           

            var lstDataToJoin = _lstResult.GroupBy(g => g.GROUP_NAME).ToList();
            var Lstdata = _lstResult.GroupBy(x => x.GROUP_NAME).Select(lstGr => new DataToDrawChart()
            {
                GROUP_NAME = lstGr.Key,
                dataGroupNameByHour = (from workSection in lstWorkSection
                                       join d in (lstGr.GroupBy(ws => ws.WORK_SECTION).Select(da => new dataGroupNameByHour()
                                       {
                                           WORK_SECTION = da.Key,
                                           INPUT = da.Sum(su => su.INPUT)
                                       }).OrderBy(Swd => Swd.WORK_SECTION).ToList())
                                     on workSection equals d.WORK_SECTION
                                     into tbl_data
                                       from data in tbl_data.DefaultIfEmpty()
                                       select new dataGroupNameByHour
                                       {
                                           WORK_SECTION = workSection,
                                           INPUT = data != null ? data.INPUT : 0,
                                       }).OrderBy(sws => sws.WORK_SECTION).ToList()
            }).ToList();

            return Json(Lstdata);


        }
    }
}