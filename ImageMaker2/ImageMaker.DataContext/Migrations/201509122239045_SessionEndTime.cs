namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionEndTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sessions", "EndTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sessions", "EndTime", c => c.DateTime(nullable: false));
        }
    }
}
