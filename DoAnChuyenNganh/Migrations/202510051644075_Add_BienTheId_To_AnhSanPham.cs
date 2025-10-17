namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_BienTheId_To_AnhSanPham : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnhSanPham", "BienTheId", c => c.Int());
            CreateIndex("dbo.AnhSanPham", "BienTheId");
            AddForeignKey("dbo.AnhSanPham", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnhSanPham", "BienTheId", "dbo.BienTheSanPham");
            DropIndex("dbo.AnhSanPham", new[] { "BienTheId" });
            DropColumn("dbo.AnhSanPham", "BienTheId");
        }
    }
}
