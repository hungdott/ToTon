using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Report.Models
{
    public class DesErrorCodeVm
    {
        [Required]
        public int ID { get; set; }
        [Required]
        public string ModelName { get; set; }
        [Required]
        public string Station { get; set; }
        [Required]
        public string ErrorCode { get; set; }
        [Required]
        public string DescriptionError { get; set; }
    }
}