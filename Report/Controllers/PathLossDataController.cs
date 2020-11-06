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
using Report.EntityPathloss;


namespace Report.Controllers
{
    public class PathLossDataController : Controller
    {
        // GET: PathLossData
        PathLossEntities context = new PathLossEntities();
        public ActionResult Index()
        {
            string Date = DateTime.Now.ToString("yyyyMMdd");
            var a = from dt in context.PathlossByShifts
                    select dt;
            var dataOneDay = getData(Date);       
            return View(dataOneDay);
        }

        public ActionResult Filter(string GetDate,string ModelName)
        {
            string Date = DateTime.Now.ToString("yyyyMMdd");
            var _date = GetDate != null ? GetDate : Date;
            var a = from dt in context.PathlossByShifts
                    select dt;

            var dataOneDay = getData(_date);

            //if(ModelName!="")
            //{
            //    dataOneDay = dataOneDay.Where(md => md.Dotname.Contains(ModelName)).ToList();
            //}
            return PartialView("~/Views/PathLossData/PathLossDataPartial.cshtml", dataOneDay);
        }



        public List<PathlossByShift> getData(string Date)
        {
           
        var DataPathLoss = (from dt in context.PathlossByShifts

                                select dt).ToList();
            if (DataPathLoss.Count() > 0)
            {
                DataPathLoss = DataPathLoss.Where(wd => wd.DateTest.Value.ToString("yyyyMMdd").Contains(Date)).OrderBy(md=>md.Dotname).ToList();
            }


            //foreach (var item in DataPathLoss)
            //{
            //    IEnumerable<string> ss= item.Status.Split(' ').ToArray().Reverse().ToString();
            //    item.Status = item.Status.Split(' ').Reverse().ToString();
            //}
            return DataPathLoss;
        }

        


    }
}