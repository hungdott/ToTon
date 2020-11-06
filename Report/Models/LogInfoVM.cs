using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Report.Models
{
    public class LogInfoVM
    {
        //public DateTime? WorkDate { get; set; }
        public int? ID { get; set; }
        [Required]
        public string ModelName { get; set; }
        [Required]
        public string Station { get; set; }
        [Required]
        public string LinkLog { get; set; }
    }
}