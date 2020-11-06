using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Common;
using Report.Entity;
using Report.Models;
using System.Data;
using Newtonsoft.Json;
using System.Dynamic;
using Report.EntityModel;
using System.Net;
using Report.Extension;

namespace Report.Controllers
{
    public class FactoryMapController : Controller
    {
        ConnectDbSfis conn = new ConnectDbSfis();
        // GET: FactoryMap
        public ActionResult Index()
        {
            List<string> LstModel = ModelNameCurrent();
            var currentDate = DateTime.Now;
            var CurrentHour = DateTime.Now.Hour;
            var thisMinus = DateTime.Now.Minute;
            var LastDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            var thisDate = DateTime.Now.ToString("yyyyMMdd");
            var NextDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
            string sql = "";

;            if ((CurrentHour==7 && thisMinus>=30) || (CurrentHour>=8 && CurrentHour <=19))
            {
                sql += $@" select LINE_NAME, MODEL_NAME,GROUP_NAME,
                            SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                            ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR,
                            ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS YR,
                             SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)as INPUT
                            from (select * from  SFISM4.R_STATION_REC_T where GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI'))
                             where (MODEL_NAME LIKE 'R%' 
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
                                   and LINE_NAME in('E1','E2','E3','E4','E5','E6','E7','E8','L1','L2','L3','L4','L5','L6','L7','L8','L9')
                                    and  WORK_DATE={thisDate}
                                     and WORK_SECTION>=8 and WORK_SECTION<=18 
                             group by LINE_NAME, MODEL_NAME,GROUP_NAME";
            }
            else if((CurrentHour==19 && thisMinus>30) ||CurrentHour >=20)
            {
                sql += $@" select LINE_NAME, MODEL_NAME,GROUP_NAME,
                            SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                            ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR,
                            ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS YR,
                             SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)as INPUT
                            from (select * from  SFISM4.R_STATION_REC_T where GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI'))
                             where (MODEL_NAME LIKE 'R%' 
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
                                    and LINE_NAME in ('E1','E2','E3','E4','E5','E6','E7','E8','L1','L2','L3','L4','L5','L6','L7','L8','L9')                             
                                    and  ((WORK_DATE={thisDate} and WORK_SECTION>=20) or(WORK_DATE={NextDate} and WORK_SECTION<=7)) 
                                      
                             group by LINE_NAME, MODEL_NAME,GROUP_NAME";
            }
           else if (CurrentHour <= 7)
            {
                sql += $@" select LINE_NAME, MODEL_NAME,GROUP_NAME,
                            SUM(FIRST_FAIL_QTY)AS First_Fail,SUM(PASS_QTY)AS Sum_Pass,SUM(FAIL_QTY)AS Sum_FAIL,SUM(REPASS_QTY) AS Sum_RePass,SUM(REFAIL_QTY)AS Sum_Re_Fail,
                            ROUND(((SUM(FIRST_FAIL_QTY)-SUM(FAIL_QTY)-SUM(REFAIL_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS RR,
                            ROUND(((SUM(PASS_QTY)+SUM(REPASS_QTY)) /NULLIF( SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY),0))*100,2) AS YR,
                             SUM(FAIL_QTY)+ SUM(PASS_QTY)+SUM(REPASS_QTY)+SUM(REFAIL_QTY)as INPUT
                            from (select * from  SFISM4.R_STATION_REC_T where GROUP_NAME IN ('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI'))
                             where (MODEL_NAME LIKE 'R%' 
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
                                   and LINE_NAME in('E1','E2','E3','E4','E5','E6','E7','E8','L1','L2','L3','L4','L5','L6','L7','L8','L9')                               
                                    and  ((WORK_DATE={thisDate} and WORK_SECTION<=6) or(WORK_DATE={LastDate} and WORK_SECTION>=20)) 
                                      
                             group by LINE_NAME, MODEL_NAME,GROUP_NAME";
            }
             

            DataTable _result = conn.reDt(sql);
            List<FactoryMapEntity> _lstResult = ConvertToObj.ConvertDataTable<FactoryMapEntity>(_result).ToList();

            List<LstDataLine> lstdataLineName = _lstResult.GroupBy(x => x.LINE_NAME).Select(dtLine => new LstDataLine()
            {
                LINE_NAME = dtLine.Key,
                lstDataModelNameByLine = dtLine.GroupBy(model => model.MODEL_NAME).Select(dataModel => new dataModelByLine()
                {
                    MODEL_NAME = dataModel.Key,
                    lstDataByGroupInModelName = dataModel.GroupBy(gr => gr.GROUP_NAME).Select(dataGr => new LineDataByGroupInModel()
                    {
                        GROUP_NAME = dataGr.Key,
                        lstdataGroupInLine = dataGr.Select(dtOneGr => new DataByGroupInLine()
                        {
                            INPUT = dtOneGr.INPUT,
                            RR = dtOneGr.RR,
                            YR = dtOneGr.YR,
                            SumFail = dtOneGr.SUM_FAIL,
                            SumPass=dtOneGr.SUM_PASS,
                            ReFail=dtOneGr.SUM_RE_FAIL,
                            RePass=dtOneGr.SUM_REPASS,
                            FirstFail=dtOneGr.FIRST_FAIL,
                            
                        }).ToList()
                    }).OrderBy(SGR => SGR.GROUP_NAME).ToList()
                }).ToList()
            }).OrderBy(SLine => SLine.LINE_NAME).ToList();


            return View(lstdataLineName);
        }


        //model data run iin current time
        public List<string> ModelNameCurrent()
        {
            var currentDate = DateTime.Now.ToString("yyyyMMdd");
            var thisHour = DateTime.Now.Hour;
            string sql = $@"select MODEL_NAME
                            from SFISM4.R_STATION_REC_T
                            where GROUP_NAME IN('FT0','FT','FT1','FT2','FT3','PT','PT0','PT1','PT2','PT3','NFT','RC','RC1','RC2','RC3','RI')
                            and (MODEL_NAME LIKE 'R%' 
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
                            and WORK_DATE={currentDate}
                            and WORK_SECTION={thisHour} ";
            DataTable _result = conn.reDt(sql);

            List<ModelCurrent> _lstResult = ConvertToObj.ConvertDataTable<ModelCurrent>(_result).ToList();

            List<string> data = _lstResult.Select(x => x.MODEL_NAME).ToList();
            return data;


        }
    }
}