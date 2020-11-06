using Report.EntityModel;
using Report.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Common
{
    public static class CompareDesErrorCode
    {
        public static DesErrorCode Compare(this DesErrorCode et, DesErrorCodeVm vm)  // ?? ham tinh
        {
            et.ErrorCode = vm.ErrorCode;
            et.Description = vm.DescriptionError;
            et.ModelName = vm.ModelName;
            et.Station = vm.Station;


            return et;
        }
    }
}