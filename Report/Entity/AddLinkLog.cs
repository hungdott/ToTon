using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Entity
{
    public class AddLinkLog
    {
        public string MODEL_NAME { get; set; }
        public string GROUP_NAME { get; set; }
        //public string WORK_DATE;

    }

    public class ListLinkLog
    {
        public List<AddLinkLog> lstLinkLog { get; set; }
    }
}