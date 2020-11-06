using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Report.Common;
using Report.EntityModel;
using Report.Entity;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Data;
using Newtonsoft.Json.Linq;
using Report.Models;
using System.Net;
using Report.Extension;

namespace Report.Controllers
{
    public class DetailReportController : Controller
    {
        // GET: DetailReport
        ReportSysContext context = new ReportSysContext();

        public ActionResult Index()
        {
            var NowDate = DateTime.Now;
            var them = DateTime.Now.AddDays(-30);

            var lstdata = (from dt in context.ActionErrorCode
                           where dt.WorkDate>= them && dt.WorkDate<=NowDate
                           select dt).OrderByDescending(x => x.WorkDate).ToList();
           
            return View(lstdata);

        }

        [HttpPost]
        public ActionResult EditData(EditActionErrorCodeByStationRequest _vm)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var data = (from il in context.ActionErrorCode
                        where il.Id == _vm.Id
                        select il).FirstOrDefault();


            var d = data;

            if (data != null)
            {
                data.ErrorCode = _vm.ErrorCode;
                data.RootCause = _vm.RootCause;
                data.Status = _vm.Status;
                data.ProblemDes = _vm.ProblemDes;
                data.Type = _vm.Type;
                data.Action = _vm.Action;
                data.Duedate = _vm.Duedate;
                data.Owner = _vm.Owner;
            }
            context.SaveChanges();
            return Json("Edit ok");



        }

        //Lay data do lai va trang sau khi load lai trang khi edit data
        public ActionResult GetAll()
        {

            var NowDate = DateTime.Now;
            var them = DateTime.Now.AddDays(-30);

            var lst = (from dt in context.ActionErrorCode
                       where dt.WorkDate >= them && dt.WorkDate <= NowDate
                       select dt).OrderByDescending(x => x.WorkDate).ToList();
            var l = lst;
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lst);
        }

        public string getAllData()
        {
            var lstdata = (from dt in context.ActionErrorCode
                           select dt).ToList();
            var sldta = JsonConvert.SerializeObject(lstdata);

            return sldta;
        }

        // get data by ModelName  fromdate toDate in ListBox to filter
        public string GetListModeName(string FromDate, string ToDate)
        {
            var DFormDate = DateTime.ParseExact(FromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);

            var lstModelName = (from lstModel in context.ActionErrorCode
                                where lstModel.WorkDate>= DFormDate && lstModel.WorkDate<= DToDate
                                select lstModel.ModelName).ToList();

            if(lstModelName.Count()>0)
            {
                lstModelName = lstModelName.GroupBy(x => x).Select(x => x.Key).ToList();
            }
            var l = lstModelName;

          
            return JsonConvert.SerializeObject(lstModelName);
        }
        //get data: apply time change => data change 
        public ActionResult AllDataInTime(string FromDate, string ToDate)
        {
            var DFormDate = DateTime.ParseExact(FromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);
            var lstData = (from data in context.ActionErrorCode
                           where data.WorkDate >= DFormDate && data.WorkDate <= DToDate
                           select data).ToList();
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lstData);
        }

        // function get data when click btn filter
        public ActionResult ResultDataFilter(string fromDate, string ToDate, string ModelName, string GroupName, string Status)
        {
            var DFromdate = DateTime.ParseExact(fromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);

            var lst = (from data in context.ActionErrorCode
                       where data.WorkDate >= DFromdate && data.WorkDate <= DToDate
                       select data)
                       .WhereIf(ModelName.Trim().Length > 0, x => ModelName.Contains(x.ModelName))
                       .WhereIf(GroupName.Trim().Length > 0, x => GroupName.Contains(x.Station))
                       .WhereIf(Status.Trim().Length > 0, x => Status.Contains(x.Status)).OrderByDescending(wd=>wd.WorkDate)
                       .ToList();
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lst);  /*DetailReportBodyPartial DetailReportPartial */
        }


        public ActionResult ResultDataFilterWeek(string Week, string fromDate, string ToDate)
        {
           
            var DFromdate = DateTime.ParseExact(fromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);
            var lst = (from data in context.ActionErrorCode
                       where data.WorkDate >= DFromdate && data.WorkDate <= DToDate
                       select data)
                      .WhereIf(Week.Trim().Length > 0, x => Week.Contains(x.Week.ToString())).OrderByDescending(wd=>wd.WorkDate).ToList();
           
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lst);
        }

        public ActionResult Filter_week_Type(string Week,string Type, string fromDate, string ToDate)
        {
            var DFromdate = DateTime.ParseExact(fromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);
            var lst = (from data in context.ActionErrorCode
                       where data.WorkDate >= DFromdate && data.WorkDate <= DToDate
                       select data)
                      .WhereIf(Week.Trim().Length > 0, x => Week.Contains(x.Week.ToString()))
                      .WhereIf(Type.Trim().Length>0,ty=> Type.Contains(ty.Type))
                      .OrderByDescending(wd => wd.WorkDate).ToList();
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lst);
        }

        public ActionResult Filter_week_Type_Model(string Week, string Type,string Model, string fromDate, string ToDate)
        {
            var DFromdate = DateTime.ParseExact(fromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);
            var lst = (from data in context.ActionErrorCode
                       where data.WorkDate >= DFromdate && data.WorkDate <= DToDate
                       select data)
                      .WhereIf(Week.Trim().Length > 0, x => Week.Contains(x.Week.ToString()))
                      .WhereIf(Type.Trim().Length > 0, ty => Type.Contains(ty.Type))
                      .WhereIf(Model.Trim().Length>0,mol=>Model.Contains(mol.ModelName))
                      .OrderByDescending(wd => wd.WorkDate).ToList();
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lst);
        }

        public ActionResult Filter_week_Type_Model_Group(string Week, string Type, string Model,string Group, string fromDate, string ToDate)
        {
            var DFromdate = DateTime.ParseExact(fromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);
            var lst = (from data in context.ActionErrorCode
                       where data.WorkDate >= DFromdate && data.WorkDate <= DToDate
                       select data)
                      .WhereIf(Week.Trim().Length > 0, x => Week.Contains(x.Week.ToString()))
                      .WhereIf(Type.Trim().Length > 0, ty => Type.Contains(ty.Type))
                      .WhereIf(Model.Trim().Length > 0, mol => Model.Contains(mol.ModelName))
                      .WhereIf(Group.Trim().Length>0,gr=>Group.Contains(gr.Station))
                      .OrderByDescending(wd => wd.WorkDate).ToList();
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lst);
        }


        public ActionResult Filter_week_Type_Model_Group_Sataus(string Week, string Type, string Model, string Group, string Status, string fromDate, string ToDate)
        {
            var DFromdate = DateTime.ParseExact(fromDate, "yyyyMMdd", null);
            var DToDate = DateTime.ParseExact(ToDate, "yyyyMMdd", null);

            var lst = (from data in context.ActionErrorCode
                       where data.WorkDate >= DFromdate && data.WorkDate <= DToDate
                       select data)
                      .WhereIf(Week.Trim().Length > 0, x => Week.Contains(x.Week.ToString()))
                      .WhereIf(Type.Trim().Length > 0, ty => Type.Contains(ty.Type))
                      .WhereIf(Model.Trim().Length > 0, mol => Model.Contains(mol.ModelName))
                      .WhereIf(Group.Trim().Length > 0, gr => Group.Contains(gr.Station))
                      .WhereIf(Status.Trim().Length>0,sta=> Status.Contains(sta.Status))
                      
                      .OrderByDescending(wd => wd.WorkDate).ToList();
            return PartialView("~/Views/DetailReport/DetailReportBodyPartial.cshtml", lst);
        }


    }
}