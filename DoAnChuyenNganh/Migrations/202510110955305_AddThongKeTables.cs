namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddThongKeTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThongKeDoanhThu",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ngay = c.DateTime(nullable: false),
                        DoanhThu = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DonHangHoanThanh = c.Int(nullable: false),
                        SanPhamBanRa = c.Int(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ThongKeSanPham",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SanPhamId = c.Int(nullable: false),
                        LuotXem = c.Int(nullable: false),
                        SoLuongBan = c.Int(nullable: false),
                        SoLuongHuy_Khach = c.Int(nullable: false),
                        SoLuongHuy_Admin = c.Int(nullable: false),
                        NgayCapNhat = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SanPham", t => t.SanPhamId, cascadeDelete: true)
                .Index(t => t.SanPhamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ThongKeSanPham", "SanPhamId", "dbo.SanPham");
            DropIndex("dbo.ThongKeSanPham", new[] { "SanPhamId" });
            DropTable("dbo.ThongKeSanPham");
            DropTable("dbo.ThongKeDoanhThu");
        }
    }
}
