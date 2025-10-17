namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateData2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BinhLuan", "SanPhamId", "dbo.SanPham");
            DropForeignKey("dbo.DonHangChiTiet", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.GioHangChiTiet", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.LichSuGia", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.LichSuTonKho", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.TonKho", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.DonHangChiTiet", "DonHangId", "dbo.DonHang");
            DropForeignKey("dbo.DonHangGiaoHang", "DonHangId", "dbo.DonHang");
            DropForeignKey("dbo.GiaTriThuocTinh", "ThuocTinhId", "dbo.ThuocTinh");
            DropForeignKey("dbo.GioHangChiTiet", "GioHangId", "dbo.GioHang");
            DropForeignKey("dbo.LichSuTonKho", "KhoId", "dbo.Kho");
            DropForeignKey("dbo.TonKho", "KhoId", "dbo.Kho");
            DropForeignKey("dbo.HinhAnhTinTuc", "TinTucId", "dbo.TinTuc");
            AddForeignKey("dbo.BinhLuan", "SanPhamId", "dbo.SanPham", "SanPhamId", cascadeDelete: true);
            AddForeignKey("dbo.DonHangChiTiet", "BienTheId", "dbo.BienTheSanPham", "BienTheId", cascadeDelete: true);
            AddForeignKey("dbo.GioHangChiTiet", "BienTheId", "dbo.BienTheSanPham", "BienTheId", cascadeDelete: true);
            AddForeignKey("dbo.LichSuGia", "BienTheId", "dbo.BienTheSanPham", "BienTheId", cascadeDelete: true);
            AddForeignKey("dbo.LichSuTonKho", "BienTheId", "dbo.BienTheSanPham", "BienTheId", cascadeDelete: true);
            AddForeignKey("dbo.TonKho", "BienTheId", "dbo.BienTheSanPham", "BienTheId", cascadeDelete: true);
            AddForeignKey("dbo.DonHangChiTiet", "DonHangId", "dbo.DonHang", "DonHangId", cascadeDelete: true);
            AddForeignKey("dbo.DonHangGiaoHang", "DonHangId", "dbo.DonHang", "DonHangId", cascadeDelete: true);
            AddForeignKey("dbo.GiaTriThuocTinh", "ThuocTinhId", "dbo.ThuocTinh", "ThuocTinhId", cascadeDelete: true);
            AddForeignKey("dbo.GioHangChiTiet", "GioHangId", "dbo.GioHang", "GioHangId", cascadeDelete: true);
            AddForeignKey("dbo.LichSuTonKho", "KhoId", "dbo.Kho", "KhoId", cascadeDelete: true);
            AddForeignKey("dbo.TonKho", "KhoId", "dbo.Kho", "KhoId", cascadeDelete: true);
            AddForeignKey("dbo.HinhAnhTinTuc", "TinTucId", "dbo.TinTuc", "TinTucId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HinhAnhTinTuc", "TinTucId", "dbo.TinTuc");
            DropForeignKey("dbo.TonKho", "KhoId", "dbo.Kho");
            DropForeignKey("dbo.LichSuTonKho", "KhoId", "dbo.Kho");
            DropForeignKey("dbo.GioHangChiTiet", "GioHangId", "dbo.GioHang");
            DropForeignKey("dbo.GiaTriThuocTinh", "ThuocTinhId", "dbo.ThuocTinh");
            DropForeignKey("dbo.DonHangGiaoHang", "DonHangId", "dbo.DonHang");
            DropForeignKey("dbo.DonHangChiTiet", "DonHangId", "dbo.DonHang");
            DropForeignKey("dbo.TonKho", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.LichSuTonKho", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.LichSuGia", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.GioHangChiTiet", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.DonHangChiTiet", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.BinhLuan", "SanPhamId", "dbo.SanPham");
            AddForeignKey("dbo.HinhAnhTinTuc", "TinTucId", "dbo.TinTuc", "TinTucId");
            AddForeignKey("dbo.TonKho", "KhoId", "dbo.Kho", "KhoId");
            AddForeignKey("dbo.LichSuTonKho", "KhoId", "dbo.Kho", "KhoId");
            AddForeignKey("dbo.GioHangChiTiet", "GioHangId", "dbo.GioHang", "GioHangId");
            AddForeignKey("dbo.GiaTriThuocTinh", "ThuocTinhId", "dbo.ThuocTinh", "ThuocTinhId");
            AddForeignKey("dbo.DonHangGiaoHang", "DonHangId", "dbo.DonHang", "DonHangId");
            AddForeignKey("dbo.DonHangChiTiet", "DonHangId", "dbo.DonHang", "DonHangId");
            AddForeignKey("dbo.TonKho", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
            AddForeignKey("dbo.LichSuTonKho", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
            AddForeignKey("dbo.LichSuGia", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
            AddForeignKey("dbo.GioHangChiTiet", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
            AddForeignKey("dbo.DonHangChiTiet", "BienTheId", "dbo.BienTheSanPham", "BienTheId");
            AddForeignKey("dbo.BinhLuan", "SanPhamId", "dbo.SanPham", "SanPhamId");
        }
    }
}
