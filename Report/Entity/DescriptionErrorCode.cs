using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class DescriptionErrorCode
    {
        public string ModelName { get; set; }
        public string Station { get; set; }

        public string ErrorCode { get; set; }
        public string Description { get; set; }
    }

    public class LstDesErrorCode
    {
        List<DescriptionErrorCode> lstDes { get; set; }
    }

}