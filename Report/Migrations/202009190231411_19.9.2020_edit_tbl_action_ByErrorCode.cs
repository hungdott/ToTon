namespace Report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1992020_edit_tbl_action_ByErrorCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActionErrorCode", "Week", c => c.Int(nullable: false));
            AddColumn("dbo.ActionErrorCode", "Begin", c => c.DateTime());
            AddColumn("dbo.ActionErrorCode", "Type", c => c.String());
            AddColumn("dbo.ActionErrorCode", "ProblemDes", c => c.String());
            AddColumn("dbo.ActionErrorCode", "report", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActionErrorCode", "report");
            DropColumn("dbo.ActionErrorCode", "ProblemDes");
            DropColumn("dbo.ActionErrorCode", "Type");
            DropColumn("dbo.ActionErrorCode", "Begin");
            DropColumn("dbo.ActionErrorCode", "Week");
        }
    }
}
