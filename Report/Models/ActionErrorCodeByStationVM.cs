using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Report.Models
{
    public class ActionErrorCodeByStationVM
    {
        public int? Id { get; set; }
        public DateTime? WorkDate { get; set; }
        [Required]
        public string ModelName { get; set; }
        [Required]
        public string Station { get; set; }
        [Required]
        public string ErrorCode { get; set; }
        [Required]
        public string PCName { get; set; }
        [Required]
        public string Owner { get; set; }
        public DateTime? Duedate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        public string RootCause { get; set; }

        [Required]
        public int Hour { get; set; }

        [Required]
        public int Week { get; set; }
        //[Required]
        //public DateTime? Begin { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string ProblemDes { get; set; }
        //[Required]
        //public string Report { get; set; }

    }
}