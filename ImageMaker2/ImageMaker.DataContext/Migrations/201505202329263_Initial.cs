namespace ImageMaker.DataContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatternDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatternType = c.Int(nullable: false),
                        Name = c.String(),
                        Data = c.Binary(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Patterns", t => t.PatternType, cascadeDelete: true)
                .Index(t => t.PatternType);
            
            CreateTable(
                "dbo.Patterns",
                c => new
                    {
                        PatternType = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.PatternType);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PatternDatas", "PatternType", "dbo.Patterns");
            DropIndex("dbo.PatternDatas", new[] { "PatternType" });
            DropTable("dbo.Patterns");
            DropTable("dbo.PatternDatas");
        }
    }
}
