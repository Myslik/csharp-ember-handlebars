namespace Ember.Handlebars.ExampleWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Shows",
                c => new
                    {
                        ShowId = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 50),
                        Category = c.String(maxLength: 50),
                        Network = c.String(maxLength: 50),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShowId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Shows");
        }
    }
}
