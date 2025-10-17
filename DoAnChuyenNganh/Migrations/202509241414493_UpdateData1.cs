namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateData1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BienTheSanPham", "SanPhamId", "dbo.SanPham");
            AddForeignKey("dbo.BienTheSanPham", "SanPhamId", "dbo.SanPham", "SanPhamId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BienTheSanPham", "SanPhamId", "dbo.SanPham");
            AddForeignKey("dbo.BienTheSanPham", "SanPhamId", "dbo.SanPham", "SanPhamId");
        }
    }
}
