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
    public class AddLinkLogController : Controller
    {

        ReportSysContext context = new ReportSysContext();
        // GET: AddLinkLog
        public ActionResult Index()
        {

            var lstLink = (from il in context.InfoLog
                           select il).ToList();
            return View(lstLink);
        }


        [HttpPost]
        public ActionResult AddModelLink(LogInfoVM _vm)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            InfoLog logEt = new InfoLog();

            logEt.Compare(_vm);
            context.InfoLog.Add(logEt);
            context.SaveChanges();
            return Json("Add ok");
        }

        [HttpPost]
        public ActionResult EditModelLink(LogInfoVM _vm)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var dataEdit = (from il in context.InfoLog
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
            var lstLink = (from il in context.InfoLog
                           select il).ToList();
            return PartialView("~/Views/AddLinkLog/AddLinkLogPartial.cshtml",lstLink);
        }
    }
}