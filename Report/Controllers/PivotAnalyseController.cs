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
    public class PivotAnalyseController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: PivotAnalyse
        public ActionResult Index()
        {
           
            string sql = $@"  select ERC.WORK_DATE, ERC.MODEL_NAME, ERC.GROUP_NAME,ERC.STATION_NAME,ERC.ERROR_CODE,
                              SUM(ERC.TEST_FAIL_QTY)AS Sum_Test_Fail_ER,SUM(ERC.FAIL_QTY)AS Sum_Fail_ER
                              from (SELECT * FROM SFISM4.R_ATE_ERRCODE_T  WHERE MODEL_NAME LIKE 'R%' OR MODEL_NAME LIKE 'M%'  OR MODEL_NAME LIKE 'E%' OR MODEL_NAME LIKE 'C%') ERC
                              WHERE ERC.WORK_DATE =20200808 AND ERC.GROUP_NAME IN('FT','FT1','FT2','PT','PT1','PT2','NFT','RC','RC1','RC2','RI') and ERC.MODEL_NAME='RBR750-FXN-EUS' and ERC.GROUP_NAME='PT'
                              GROUP BY ERC.MODEL_NAME,ERC.GROUP_NAME,ERC.ERROR_CODE,STATION_NAME,ERC.WORK_DATE
                              ORDER BY COUNT(ERC.TEST_FAIL_QTY) DESC ";

            DataTable _result = conn.reDt(sql);
            List<PivotAnalyseET> _lstResult = ConvertToObj.ConvertDataTable<PivotAnalyseET>(_result).ToList();

            List<LstErrorCode> LstEr = _lstResult.GroupBy(x => x.ERROR_CODE).Select(xx => new LstErrorCode()
            {
                ErrorCode = xx.Key,
            }).ToList();


            
            //List<lstDataByModel> lstData = _lstResult.GroupBy(x => x.MODEL_NAME).Select(xx => new lstDataByModel()
            //{
            //    ModelName = xx.Key,
            //    lstStation = xx.GroupBy(station => station.GROUP_NAME).Select(lstStation => new LstDataByStation() {
            //        Station = lstStation.Key,
            //        lstDataPc = lstStation.GroupBy(pc => pc.STATION_NAME).Select(lspc => new LstDataByPc() {
            //            PCName=lspc.Key,
            //            lstEr=(from xx1 in LstEr
            //                   join xx2 in ( from jj in lspc
            //                                 select new DataByError() {
            //                       Error=jj.ERROR_CODE,
            //                       Fail=jj.SUM_TEST_FAIL_ER
            //                   } ).ToList()

            //                   on xx1 equals xx2.Error into tbl_data

            //                   from btl in tbl_data.DefaultIfEmpty()
            //                   select new DataByError()
            //                   {
            //                       Error= LstEr,
            //                       Fail= (btl== null)? -5m: btl.SUM_TEST_FAIL_ER
            //                   }
            //                   ).ToList()
            //        }).ToList()
            //    }).ToList()
            //}).ToList();




            var l = _lstResult;
            return View();
        }
    }
}