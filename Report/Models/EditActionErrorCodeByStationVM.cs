using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Report.Models
{
    public class EditActionErrorCodeByStationVM
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
        [Required]
        //public DateTime? Begin { get; set; }
        //[Required]
        public string Type { get; set; }
        [Required]
        public string ProblemDes { get; set; }
        //[Required]
        //public string Report { get; set; }
    }
    public class EditActionErrorCodeByStationRequest
    {
        public int? Id { get; set; }
        public string Owner { get; set; }
        public string ErrorCode { get; set; }
        public DateTime? Duedate { get; set; }
        public string Action { get; set; }
        public string RootCause { get; set; }
        public string ProblemDes { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}