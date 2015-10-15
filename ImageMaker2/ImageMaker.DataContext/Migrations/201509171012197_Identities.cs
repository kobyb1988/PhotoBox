namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Identities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Templates", "BackgroundId", "dbo.FileDatas");
            DropForeignKey("dbo.Templates", "OverlayId", "dbo.FileDatas");
            DropPrimaryKey("dbo.FileDatas");
            AlterColumn("dbo.FileDatas", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.FileDatas", "Id");
            AddForeignKey("dbo.Templates", "BackgroundId", "dbo.FileDatas", "Id");
            AddForeignKey("dbo.Templates", "OverlayId", "dbo.FileDatas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "OverlayId", "dbo.FileDatas");
            DropForeignKey("dbo.Templates", "BackgroundId", "dbo.FileDatas");
            DropPrimaryKey("dbo.FileDatas");
            AlterColumn("dbo.FileDatas", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.FileDatas", "Id");
            AddForeignKey("dbo.Templates", "OverlayId", "dbo.FileDatas", "Id");
            AddForeignKey("dbo.Templates", "BackgroundId", "dbo.FileDatas", "Id");
        }
    }
}
