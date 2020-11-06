using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Report.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("ActionErrorCode")]
    public partial class ActionErrorCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ActionErrorCode()
        {

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? WorkDate { get; set; }
        [StringLength(50)]

        public string ModelName { get; set; }

        public string Station { get; set; }

        public string ErrorCode { get; set; }

        public string PCname { get; set; }

        public string Owner { get; set; }

        public DateTime? Duedate { get; set; }

        public string Status { get; set; }

        public string Action { get; set; }

        public string RootCause { get; set; }

        public int WorkSection { get; set; }

        public int Week { get; set; }
        public DateTime? Begin { get; set; }
        public string Type { get; set; }
        public string ProblemDes { get; set; }
        public string report { get; set; }



    }




  
}