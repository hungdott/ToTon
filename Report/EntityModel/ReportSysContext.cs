namespace Report.EntityModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;

    public partial class ReportSysContext : IdentityDbContext<ApplicationUser>
    {
        public ReportSysContext()
            : base("name=ReportSysContext")
        {
           
        }
        public static ReportSysContext Create()
        {
            return new ReportSysContext();
        }

        public virtual DbSet<ActionErrorCode> ActionErrorCode { get; set; }

        public virtual DbSet<InfoLog> InfoLog { get; set; }

        public virtual DbSet<DesErrorCode> DesErrorCode { get; set; }

        //public virtual DbSet<dataTest> data { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
