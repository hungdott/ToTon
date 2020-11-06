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


namespace Report.Controllers
{
    public class DescriptionErrorCodeController : Controller
    {
        ReportSysContext context = new ReportSysContext();
        // GET: DescriptionErrorCode
        //view
        public ActionResult Index()
        {
            var lstDesErrorCode = (from il in context.DesErrorCode
                                   select il).ToList();
            return View(lstDesErrorCode);
            
        }


        //add
        [HttpPost]
        public ActionResult AddDesErrorCode(DesErrorCodeVm _vm)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DesErrorCode DesErrorEt = new DesErrorCode();

            DesErrorEt.Compare(_vm);
            context.DesErrorCode.Add(DesErrorEt);
            context.SaveChanges();
            return Json("Add ok");
        }





        //edit
        [HttpPost]
        public ActionResult EditDesErrorCode(DesErrorCodeVm _vm)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dataEdit = (from il in context.DesErrorCode
                            where il.Id == _vm.ID
                            select il).FirstOrDefault();
            if (dataEdit != null)
            {
                dataEdit.Compare(_vm);
            }

            context.SaveChanges();
            return Json("Edit ok");
        }



        public ActionResult GetAll()
        {
            var lstLink = (from il in context.DesErrorCode
                           select il).ToList();
            return PartialView("~/Views/DescriptionErrorCode/DescriptionErrorCodePartial.cshtml", lstLink);
        }


    }
}