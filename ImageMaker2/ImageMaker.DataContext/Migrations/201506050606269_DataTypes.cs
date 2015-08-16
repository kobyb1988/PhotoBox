namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataTypes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TemplateImages", "X", c => c.Double(nullable: false));
            AlterColumn("dbo.TemplateImages", "Y", c => c.Double(nullable: false));
            AlterColumn("dbo.TemplateImages", "Width", c => c.Double(nullable: false));
            AlterColumn("dbo.TemplateImages", "Height", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TemplateImages", "Height", c => c.Int(nullable: false));
            AlterColumn("dbo.TemplateImages", "Width", c => c.Int(nullable: false));
            AlterColumn("dbo.TemplateImages", "Y", c => c.Int(nullable: false));
            AlterColumn("dbo.TemplateImages", "X", c => c.Int(nullable: false));
        }
    }
}
