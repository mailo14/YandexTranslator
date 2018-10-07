namespace RStyleTranslator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Langs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 3),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Routes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FromLangId = c.String(nullable: false, maxLength: 3),
                        ToLangId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Routes");
            DropTable("dbo.Langs");
        }
    }
}
