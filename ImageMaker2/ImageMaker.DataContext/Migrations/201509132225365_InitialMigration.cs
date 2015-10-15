namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileDatas",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                        SessionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TemplateImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        X = c.Double(nullable: false),
                        Y = c.Double(nullable: false),
                        Width = c.Double(nullable: false),
                        Height = c.Double(nullable: false),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);
            
            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        BackgroundId = c.Int(),
                        OverlayId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileDatas", t => t.BackgroundId)
                .ForeignKey("dbo.FileDatas", t => t.OverlayId)
                .Index(t => t.BackgroundId)
                .Index(t => t.OverlayId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AppSettings = c.Binary(),
                        CameraSettings = c.Binary(),
                        ThemeSettings = c.Binary(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Templates", "OverlayId", "dbo.FileDatas");
            DropForeignKey("dbo.TemplateImages", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Templates", "BackgroundId", "dbo.FileDatas");
            DropForeignKey("dbo.Images", "SessionId", "dbo.Sessions");
            DropIndex("dbo.Templates", new[] { "OverlayId" });
            DropIndex("dbo.Templates", new[] { "BackgroundId" });
            DropIndex("dbo.TemplateImages", new[] { "TemplateId" });
            DropIndex("dbo.Images", new[] { "SessionId" });
            DropTable("dbo.Users");
            DropTable("dbo.Templates");
            DropTable("dbo.TemplateImages");
            DropTable("dbo.Sessions");
            DropTable("dbo.Images");
            DropTable("dbo.FileDatas");
        }
    }
}
