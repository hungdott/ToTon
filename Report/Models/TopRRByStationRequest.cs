using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.Models
{
    public class TopRRByStationRequest
    {
        public DateTime? Date { get; set; }
        public string Option { get; set; }
       
        public string group_name { get; set; }
        public string ModelName { get; set; }
        public DateTime? NextDate { get; set; }
        public string Shift { get; set; }
        public int PCS { get; set; }
        public int fromHour {get; set; }
        public int toHour { get; set; }
        public string StatusLine { get; set; }

        public decimal From_RRByStation { get; set; }
        public decimal TO_RRByStation{get;set;}

        public string StatusDay { get; set; }


    }

    public class TopRRByStationRequestgetPCName
    {
        public DateTime? Date { get; set; }
        public string Option { get; set; }

        public string group_name { get; set; }
        public string ModelName { get; set; }
        public DateTime? NextDate { get; set; }
        public string Shift { get; set; }
        public int PCS { get; set; }
        public int fromHour { get; set; }
        public int toHour { get; set; }
        public string StatusLine { get; set; }

        public decimal From_RRByStation { get; set; }
        public decimal TO_RRByStation { get; set; }

        public string StatusPc { get; set; }


    }
}