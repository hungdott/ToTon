using Report.EntityModel;
using Report.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Report.Common
{
    public static class CompareInfoLog
    {
        public static InfoLog Compare(this InfoLog et, LogInfoVM vm)  // ?? ham tinh
        {
            et.ModelName = vm.ModelName;
            et.GroupName = vm.Station;
            et.LinkLog = vm.LinkLog; 
            return et;
        }
    }
}