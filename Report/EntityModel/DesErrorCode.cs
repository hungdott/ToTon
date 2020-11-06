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
    [Table("DescriptionErrorCode1")]
    public class DesErrorCode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DesErrorCode()
        {

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ModelName { get; set; }
        public string Station { get; set; }
        public string ErrorCode { get; set; }

        public string Description { get; set; }

       
    }
}
