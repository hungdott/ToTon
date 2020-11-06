namespace Report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20200917_edit_table_ActionErrorCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActionErrorCode", "WorkSection", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActionErrorCode", "WorkSection");
        }
    }
}
