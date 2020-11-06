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
    public class ProjectTodayEriController : Controller
    {
        // GET: ProjectTodayEri
        public ActionResult Index()
        {
            // GET: RRByErrorCode
            ConnectDbSfis conn = new ConnectDbSfis();
            var date = DateTime.Now.ToString("yyyyMMdd");
            var NextDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            string shift = "day";
            //string sql=$@"Select DISTINCT MODEL_NAME,GROUP_NAME
            //                     From (SELECT * FROM SFISM4.R_STATION_REC_T  WHERE  
            //                                                                    MODEL_NAME LIKE 'R%' 
            //                                                                    OR MODEL_NAME LIKE 'M%'  
            //                                                                    OR MODEL_NAME LIKE 'E%' 
            //                                                                    OR MODEL_NAME LIKE 'C%' 
            //                                                                    OR MODEL_NAME LIKE 'D%' 
            //                                                                    OR MODEL_NAME LIKE 'L%' 
            //                                                                    OR MODEL_NAME LIKE 'N%' 
            //                                                                    OR MODEL_NAME LIKE 'G%' 
            //                                                                    OR MODEL_NAME LIKE 'X%'
            //                                                                    OR MODEL_NAME LIKE 'O%'
            //                                                                    OR MODEL_NAME LIKE 'U12H%'
            //                                                                    OR MODEL_NAME LIKE 'V6510%') 
            //                     where  WORK_DATE = {date}";


            string sql = $@"Select DISTINCT MODEL_NAME
                                 From (SELECT * FROM SFISM4.R_STATION_REC_T  WHERE  
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
                                                                               
                                                                                OR MODEL_NAME LIKE 'V6510%')";

            if (shift == "day")
            {
                sql += $@"where  WORK_DATE = {date} and (WORK_SECTION>=8 and WORK_SECTION <=20)";
            }
            else if (shift == "night")
            {
                sql += $@"where  (WORK_DATE = {date} and WORK_SECTION >20) or (WORK_DATE = {NextDate} and WORK_SECTION <8 )";
            }
            else if (shift == "AllDay")
            {
                sql += $@"where  (WORK_DATE = {date} and (WORK_SECTION>=8 and WORK_SECTION <=20)) or ( (WORK_DATE = {date} and WORK_SECTION >20) or (WORK_DATE = {NextDate} and WORK_SECTION <8 ))";
            }
            else
            {
                sql += $@"where  (WORK_DATE = {date} and (WORK_SECTION>=8 and WORK_SECTION <=20)) or ( (WORK_DATE = {date} and WORK_SECTION >20) or (WORK_DATE = {NextDate} and WORK_SECTION <8 ))";
            }


            DataTable _result = conn.reDt(sql);
            List<ProjectToDay> _lstResult = ConvertToObj.ConvertDataTable<ProjectToDay>(_result).OrderByDescending(a => a.MODEL_NAME).ToList();
            var ll = _lstResult;




            return View(_lstResult);
        }
        public ActionResult Filter(string shift)
        {
            ConnectDbSfis conn = new ConnectDbSfis();
            var date = DateTime.Now.ToString("yyyyMMdd");
            var NextDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            var s = shift;
            string sql = $@"Select DISTINCT MODEL_NAME
                                 From (SELECT * FROM SFISM4.R_STATION_REC_T  WHERE  
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
                                                                              
                                                                                OR MODEL_NAME LIKE 'V6510%')";

            if (shift == "day")
            {
                sql += $@"where  WORK_DATE = {date} and (WORK_SECTION>=8 and WORK_SECTION <=20)";
            }
            else if (shift == "night")
            {
                sql += $@"where  (WORK_DATE = {date} and WORK_SECTION >20) or (WORK_DATE = {NextDate} and WORK_SECTION <8 )";
            }
            else if (shift == "AllDay")
            {
                sql += $@"where  (WORK_DATE = {date} and (WORK_SECTION>=8 and WORK_SECTION <=20)) or ( (WORK_DATE = {date} and WORK_SECTION >20) or (WORK_DATE = {NextDate} and WORK_SECTION <8 ))";
            }
            else
            {
                sql += $@"where  (WORK_DATE = {date} and (WORK_SECTION>=8 and WORK_SECTION <=20)) or ( (WORK_DATE = {date} and WORK_SECTION >20) or (WORK_DATE = {NextDate} and WORK_SECTION <8 ))";
            }


            DataTable _result = conn.reDt(sql);
            List<ProjectToDay> _lstResult = ConvertToObj.ConvertDataTable<ProjectToDay>(_result).OrderByDescending(a => a.MODEL_NAME).ToList();
            var ll = _lstResult.OrderByDescending(x => x.MODEL_NAME);

            return PartialView("~/Views/ProjectToDayEri/ProjectToDayEriPartial.cshtml", _lstResult);
        }
    }
}
