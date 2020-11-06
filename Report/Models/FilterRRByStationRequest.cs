using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Models
{
    public class FilterRRByStationRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Shift { get; set; }

        public string optionDay { get; set; }
        public string ModelName { get; set; }

    }

}