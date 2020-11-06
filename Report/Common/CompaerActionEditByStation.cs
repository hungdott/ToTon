using Report.EntityModel;
using Report.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Common
{
    public static class CompaerActionEditByStation
    {
        
        public static ActionErrorCode Compare(this ActionErrorCode et, EditActionErrorCodeByStationVM vm)  // ?? ham tinh
        {
            et.ModelName = vm.ModelName;
            et.Owner = vm.Owner;
            et.Action = vm.Action;
            et.Duedate = vm.Duedate;
            et.ErrorCode = vm.ErrorCode;
            et.RootCause = vm.RootCause;
            et.Status = vm.Status;
            et.Station = vm.Station;
            et.WorkDate = vm.WorkDate;
            et.WorkSection = vm.Hour;
            et.PCname = vm.PCName;


           // et.Begin = vm.Begin;
            et.ProblemDes = vm.ProblemDes;
          //  et.report = vm.Report;
            et.Week = vm.Week;
            et.Type = vm.Type;

            return et;
        }

    }
}