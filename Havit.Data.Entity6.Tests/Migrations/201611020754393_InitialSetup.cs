namespace Havit.Data.Entity.Tests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSetup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Master",
                c => new
                    {
                        MasterId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.MasterId);
            
            CreateTable(
                "dbo.Child",
                c => new
                    {
                        ChildId = c.Int(nullable: false, identity: true),
                        MasterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildId)
                .ForeignKey("dbo.Master", t => t.MasterId)
                .Index(t => t.MasterId, name: "IDX_Child_MasterId");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Child", "MasterId", "dbo.Master");
            DropIndex("dbo.Child", "IDX_Child_MasterId");
            DropTable("dbo.Child");
            DropTable("dbo.Master");
        }
    }
}
