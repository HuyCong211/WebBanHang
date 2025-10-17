namespace DoAnChuyenNganh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnhSanPham",
                c => new
                    {
                        AnhSanPhamId = c.Int(nullable: false, identity: true),
                        SanPhamId = c.Int(nullable: false),
                        Url = c.String(nullable: false, maxLength: 1000),
                        ThuTu = c.Int(),
                        MacDinh = c.Boolean(),
                        MoTa = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.AnhSanPhamId)
                .ForeignKey("dbo.SanPham", t => t.SanPhamId, cascadeDelete: true)
                .Index(t => t.SanPhamId);
            
            CreateTable(
                "dbo.SanPham",
                c => new
                    {
                        SanPhamId = c.Int(nullable: false, identity: true),
                        MaSanPham = c.String(maxLength: 50),
                        TenSanPham = c.String(nullable: false, maxLength: 300),
                        Slug = c.String(maxLength: 300),
                        MoTaNgan = c.String(maxLength: 500),
                        MoTaChiTiet = c.String(),
                        GiaBan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GiaGoc = c.Decimal(precision: 18, scale: 2),
                        DanhMucId = c.Int(),
                        TrangThai = c.Int(),
                        LuotXem = c.Int(),
                        NgayTao = c.DateTime(precision: 7, storeType: "datetime2"),
                        NgayCapNhat = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.SanPhamId)
                .ForeignKey("dbo.DanhMuc", t => t.DanhMucId)
                .Index(t => t.DanhMucId);
            
            CreateTable(
                "dbo.BienTheSanPham",
                c => new
                    {
                        BienTheId = c.Int(nullable: false, identity: true),
                        SanPhamId = c.Int(nullable: false),
                        SKU = c.String(maxLength: 100),
                        Gia = c.Decimal(precision: 18, scale: 2),
                        GiaKhuyenMai = c.Decimal(precision: 18, scale: 2),
                        MaVach = c.String(maxLength: 100),
                        TrangThai = c.Boolean(),
                        NgayTao = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.BienTheId)
                .ForeignKey("dbo.SanPham", t => t.SanPhamId)
                .Index(t => t.SanPhamId);
            
            CreateTable(
                "dbo.DonHangChiTiet",
                c => new
                    {
                        DonHangChiTietId = c.Long(nullable: false, identity: true),
                        DonHangId = c.Long(nullable: false),
                        BienTheId = c.Int(nullable: false),
                        SoLuong = c.Int(nullable: false),
                        DonGia = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ThanhTien = c.Decimal(precision: 29, scale: 2),
                        GhiChu = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.DonHangChiTietId)
                .ForeignKey("dbo.DonHang", t => t.DonHangId)
                .ForeignKey("dbo.BienTheSanPham", t => t.BienTheId)
                .Index(t => t.DonHangId)
                .Index(t => t.BienTheId);
            
            CreateTable(
                "dbo.DonHang",
                c => new
                    {
                        DonHangId = c.Long(nullable: false, identity: true),
                        MaDonHang = c.String(maxLength: 100),
                        KhachHangId = c.Int(),
                        NguoiDungId = c.Guid(),
                        TongTien = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PhiVanChuyen = c.Decimal(precision: 18, scale: 2),
                        PhuongThucThanhToanId = c.Int(),
                        DiaChiGiaoHangId = c.Int(),
                        TrangThai = c.Int(),
                        NgayTao = c.DateTime(precision: 7, storeType: "datetime2"),
                        NgayCapNhat = c.DateTime(precision: 7, storeType: "datetime2"),
                        GhiChu = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.DonHangId)
                .ForeignKey("dbo.KhachHang", t => t.KhachHangId)
                .Index(t => t.KhachHangId);
            
            CreateTable(
                "dbo.DonHangGiaoHang",
                c => new
                    {
                        DonHangGiaoId = c.Long(nullable: false, identity: true),
                        DonHangId = c.Long(nullable: false),
                        NguoiPhuTrach = c.Guid(),
                        MaDonHangNhaVanChuyen = c.String(maxLength: 200),
                        TrangThai = c.Int(),
                        NgayGiao = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.DonHangGiaoId)
                .ForeignKey("dbo.DonHang", t => t.DonHangId)
                .Index(t => t.DonHangId);
            
            CreateTable(
                "dbo.KhachHang",
                c => new
                    {
                        KhachHangId = c.Int(nullable: false, identity: true),
                        NguoiDungId = c.Guid(),
                        NgaySinh = c.DateTime(storeType: "date"),
                        GioiTinh = c.String(maxLength: 20),
                        DiaChiMacDinhId = c.Int(),
                        TongDiemTichLuy = c.Int(),
                    })
                .PrimaryKey(t => t.KhachHangId)
                .ForeignKey("dbo.NguoiDung", t => t.NguoiDungId)
                .Index(t => t.NguoiDungId);
            
            CreateTable(
                "dbo.DiaChiGiaoHang",
                c => new
                    {
                        DiaChiId = c.Int(nullable: false, identity: true),
                        KhachHangId = c.Int(),
                        HoTen = c.String(nullable: false, maxLength: 200),
                        DienThoai = c.String(nullable: false, maxLength: 50),
                        DiaChiChiTiet = c.String(nullable: false, maxLength: 500),
                        Tinh = c.String(maxLength: 200),
                        Huyen = c.String(maxLength: 200),
                        Xa = c.String(maxLength: 200),
                        MacDinh = c.Boolean(),
                    })
                .PrimaryKey(t => t.DiaChiId)
                .ForeignKey("dbo.KhachHang", t => t.KhachHangId)
                .Index(t => t.KhachHangId);
            
            CreateTable(
                "dbo.NguoiDung",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TenDangNhap = c.String(maxLength: 256),
                        Email = c.String(maxLength: 256),
                        HoTen = c.String(maxLength: 200),
                        DienThoai = c.String(maxLength: 50),
                        NgayTao = c.DateTime(precision: 7, storeType: "datetime2"),
                        TrangThai = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GiaTriThuocTinh",
                c => new
                    {
                        GiaTriId = c.Int(nullable: false, identity: true),
                        ThuocTinhId = c.Int(nullable: false),
                        TenGiaTri = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.GiaTriId)
                .ForeignKey("dbo.ThuocTinh", t => t.ThuocTinhId)
                .Index(t => t.ThuocTinhId);
            
            CreateTable(
                "dbo.ThuocTinh",
                c => new
                    {
                        ThuocTinhId = c.Int(nullable: false, identity: true),
                        TenThuocTinh = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.ThuocTinhId);
            
            CreateTable(
                "dbo.GioHangChiTiet",
                c => new
                    {
                        GioHangChiTietId = c.Long(nullable: false, identity: true),
                        GioHangId = c.Long(nullable: false),
                        BienTheId = c.Int(nullable: false),
                        SoLuong = c.Int(nullable: false),
                        DonGia = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ThanhTien = c.Decimal(precision: 29, scale: 2),
                    })
                .PrimaryKey(t => t.GioHangChiTietId)
                .ForeignKey("dbo.GioHang", t => t.GioHangId)
                .ForeignKey("dbo.BienTheSanPham", t => t.BienTheId)
                .Index(t => t.GioHangId)
                .Index(t => t.BienTheId);
            
            CreateTable(
                "dbo.GioHang",
                c => new
                    {
                        GioHangId = c.Long(nullable: false, identity: true),
                        KhachHangId = c.Int(),
                        NguoiDungId = c.Guid(),
                        NgayTao = c.DateTime(precision: 7, storeType: "datetime2"),
                        TrangThai = c.Int(),
                    })
                .PrimaryKey(t => t.GioHangId);
            
            CreateTable(
                "dbo.LichSuGia",
                c => new
                    {
                        LichSuGiaId = c.Int(nullable: false, identity: true),
                        BienTheId = c.Int(nullable: false),
                        GiaCu = c.Decimal(precision: 18, scale: 2),
                        GiaMoi = c.Decimal(precision: 18, scale: 2),
                        NguoiThucHien = c.Guid(),
                        NgayThucHien = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.LichSuGiaId)
                .ForeignKey("dbo.BienTheSanPham", t => t.BienTheId)
                .Index(t => t.BienTheId);
            
            CreateTable(
                "dbo.LichSuTonKho",
                c => new
                    {
                        LichSuId = c.Int(nullable: false, identity: true),
                        BienTheId = c.Int(nullable: false),
                        KhoId = c.Int(nullable: false),
                        SoThayDoi = c.Int(nullable: false),
                        GhiChu = c.String(maxLength: 500),
                        NguoiThucHien = c.Guid(),
                        NgayThucHien = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.LichSuId)
                .ForeignKey("dbo.Kho", t => t.KhoId)
                .ForeignKey("dbo.BienTheSanPham", t => t.BienTheId)
                .Index(t => t.BienTheId)
                .Index(t => t.KhoId);
            
            CreateTable(
                "dbo.Kho",
                c => new
                    {
                        KhoId = c.Int(nullable: false, identity: true),
                        TenKho = c.String(nullable: false, maxLength: 200),
                        DiaChi = c.String(maxLength: 500),
                        DienThoai = c.String(maxLength: 50),
                        TrangThai = c.Boolean(),
                    })
                .PrimaryKey(t => t.KhoId);
            
            CreateTable(
                "dbo.TonKho",
                c => new
                    {
                        TonKhoId = c.Int(nullable: false, identity: true),
                        BienTheId = c.Int(nullable: false),
                        KhoId = c.Int(nullable: false),
                        SoLuong = c.Int(),
                        NgayCapNhat = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.TonKhoId)
                .ForeignKey("dbo.Kho", t => t.KhoId)
                .ForeignKey("dbo.BienTheSanPham", t => t.BienTheId)
                .Index(t => t.BienTheId)
                .Index(t => t.KhoId);
            
            CreateTable(
                "dbo.BinhLuan",
                c => new
                    {
                        BinhLuanId = c.Int(nullable: false, identity: true),
                        SanPhamId = c.Int(nullable: false),
                        KhachHangId = c.Int(),
                        NoiDung = c.String(nullable: false, maxLength: 2000),
                        Sao = c.Int(),
                        NgayTao = c.DateTime(precision: 7, storeType: "datetime2"),
                        TrangThai = c.Boolean(),
                    })
                .PrimaryKey(t => t.BinhLuanId)
                .ForeignKey("dbo.SanPham", t => t.SanPhamId)
                .Index(t => t.SanPhamId);
            
            CreateTable(
                "dbo.DanhMuc",
                c => new
                    {
                        DanhMucId = c.Int(nullable: false, identity: true),
                        TenDanhMuc = c.String(nullable: false, maxLength: 200),
                        Slug = c.String(maxLength: 200),
                        MoTa = c.String(maxLength: 1000),
                        DanhMucChaId = c.Int(),
                        TrangThai = c.Boolean(),
                        NgayTao = c.DateTime(precision: 7, storeType: "datetime2"),
                        NgayCapNhat = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.DanhMucId)
                .ForeignKey("dbo.DanhMuc", t => t.DanhMucChaId)
                .Index(t => t.DanhMucChaId);
            
            CreateTable(
                "dbo.KhuyenMai",
                c => new
                    {
                        KhuyenMaiId = c.Int(nullable: false, identity: true),
                        MaKM = c.String(maxLength: 100),
                        TenKM = c.String(nullable: false, maxLength: 300),
                        Loai = c.String(maxLength: 50),
                        GiaTri = c.Decimal(precision: 18, scale: 2),
                        NgayBatDau = c.DateTime(precision: 7, storeType: "datetime2"),
                        NgayKetThuc = c.DateTime(precision: 7, storeType: "datetime2"),
                        DieuKien = c.String(maxLength: 500),
                        TrangThai = c.Boolean(),
                    })
                .PrimaryKey(t => t.KhuyenMaiId);
            
            CreateTable(
                "dbo.CauHinh",
                c => new
                    {
                        KeyName = c.String(nullable: false, maxLength: 200),
                        Value = c.String(),
                        MoTa = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.KeyName);
            
            CreateTable(
                "dbo.HinhAnhTinTuc",
                c => new
                    {
                        HinhAnhId = c.Int(nullable: false, identity: true),
                        TinTucId = c.Int(nullable: false),
                        Url = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.HinhAnhId)
                .ForeignKey("dbo.TinTuc", t => t.TinTucId)
                .Index(t => t.TinTucId);
            
            CreateTable(
                "dbo.TinTuc",
                c => new
                    {
                        TinTucId = c.Int(nullable: false, identity: true),
                        TieuDe = c.String(nullable: false, maxLength: 400),
                        Slug = c.String(maxLength: 400),
                        TomTat = c.String(maxLength: 1000),
                        NoiDung = c.String(),
                        NgayDang = c.DateTime(precision: 7, storeType: "datetime2"),
                        TrangThai = c.Boolean(),
                    })
                .PrimaryKey(t => t.TinTucId);
            
            CreateTable(
                "dbo.PhuongThucThanhToan",
                c => new
                    {
                        PhuongThucId = c.Int(nullable: false, identity: true),
                        TenPhuongThuc = c.String(nullable: false, maxLength: 200),
                        MoTa = c.String(maxLength: 500),
                        Active = c.Boolean(),
                    })
                .PrimaryKey(t => t.PhuongThucId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Fullname = c.String(),
                        Phone = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.BienTheGiaTri",
                c => new
                    {
                        BienTheId = c.Int(nullable: false),
                        GiaTriId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BienTheId, t.GiaTriId })
                .ForeignKey("dbo.BienTheSanPham", t => t.BienTheId, cascadeDelete: true)
                .ForeignKey("dbo.GiaTriThuocTinh", t => t.GiaTriId, cascadeDelete: true)
                .Index(t => t.BienTheId)
                .Index(t => t.GiaTriId);
            
            CreateTable(
                "dbo.SanPhamKhuyenMai",
                c => new
                    {
                        KhuyenMaiId = c.Int(nullable: false),
                        SanPhamId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.KhuyenMaiId, t.SanPhamId })
                .ForeignKey("dbo.KhuyenMai", t => t.KhuyenMaiId, cascadeDelete: true)
                .ForeignKey("dbo.SanPham", t => t.SanPhamId, cascadeDelete: true)
                .Index(t => t.KhuyenMaiId)
                .Index(t => t.SanPhamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.HinhAnhTinTuc", "TinTucId", "dbo.TinTuc");
            DropForeignKey("dbo.SanPhamKhuyenMai", "SanPhamId", "dbo.SanPham");
            DropForeignKey("dbo.SanPhamKhuyenMai", "KhuyenMaiId", "dbo.KhuyenMai");
            DropForeignKey("dbo.SanPham", "DanhMucId", "dbo.DanhMuc");
            DropForeignKey("dbo.DanhMuc", "DanhMucChaId", "dbo.DanhMuc");
            DropForeignKey("dbo.BinhLuan", "SanPhamId", "dbo.SanPham");
            DropForeignKey("dbo.BienTheSanPham", "SanPhamId", "dbo.SanPham");
            DropForeignKey("dbo.TonKho", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.LichSuTonKho", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.TonKho", "KhoId", "dbo.Kho");
            DropForeignKey("dbo.LichSuTonKho", "KhoId", "dbo.Kho");
            DropForeignKey("dbo.LichSuGia", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.GioHangChiTiet", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.GioHangChiTiet", "GioHangId", "dbo.GioHang");
            DropForeignKey("dbo.BienTheGiaTri", "GiaTriId", "dbo.GiaTriThuocTinh");
            DropForeignKey("dbo.BienTheGiaTri", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.GiaTriThuocTinh", "ThuocTinhId", "dbo.ThuocTinh");
            DropForeignKey("dbo.DonHangChiTiet", "BienTheId", "dbo.BienTheSanPham");
            DropForeignKey("dbo.KhachHang", "NguoiDungId", "dbo.NguoiDung");
            DropForeignKey("dbo.DonHang", "KhachHangId", "dbo.KhachHang");
            DropForeignKey("dbo.DiaChiGiaoHang", "KhachHangId", "dbo.KhachHang");
            DropForeignKey("dbo.DonHangGiaoHang", "DonHangId", "dbo.DonHang");
            DropForeignKey("dbo.DonHangChiTiet", "DonHangId", "dbo.DonHang");
            DropForeignKey("dbo.AnhSanPham", "SanPhamId", "dbo.SanPham");
            DropIndex("dbo.SanPhamKhuyenMai", new[] { "SanPhamId" });
            DropIndex("dbo.SanPhamKhuyenMai", new[] { "KhuyenMaiId" });
            DropIndex("dbo.BienTheGiaTri", new[] { "GiaTriId" });
            DropIndex("dbo.BienTheGiaTri", new[] { "BienTheId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.HinhAnhTinTuc", new[] { "TinTucId" });
            DropIndex("dbo.DanhMuc", new[] { "DanhMucChaId" });
            DropIndex("dbo.BinhLuan", new[] { "SanPhamId" });
            DropIndex("dbo.TonKho", new[] { "KhoId" });
            DropIndex("dbo.TonKho", new[] { "BienTheId" });
            DropIndex("dbo.LichSuTonKho", new[] { "KhoId" });
            DropIndex("dbo.LichSuTonKho", new[] { "BienTheId" });
            DropIndex("dbo.LichSuGia", new[] { "BienTheId" });
            DropIndex("dbo.GioHangChiTiet", new[] { "BienTheId" });
            DropIndex("dbo.GioHangChiTiet", new[] { "GioHangId" });
            DropIndex("dbo.GiaTriThuocTinh", new[] { "ThuocTinhId" });
            DropIndex("dbo.DiaChiGiaoHang", new[] { "KhachHangId" });
            DropIndex("dbo.KhachHang", new[] { "NguoiDungId" });
            DropIndex("dbo.DonHangGiaoHang", new[] { "DonHangId" });
            DropIndex("dbo.DonHang", new[] { "KhachHangId" });
            DropIndex("dbo.DonHangChiTiet", new[] { "BienTheId" });
            DropIndex("dbo.DonHangChiTiet", new[] { "DonHangId" });
            DropIndex("dbo.BienTheSanPham", new[] { "SanPhamId" });
            DropIndex("dbo.SanPham", new[] { "DanhMucId" });
            DropIndex("dbo.AnhSanPham", new[] { "SanPhamId" });
            DropTable("dbo.SanPhamKhuyenMai");
            DropTable("dbo.BienTheGiaTri");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PhuongThucThanhToan");
            DropTable("dbo.TinTuc");
            DropTable("dbo.HinhAnhTinTuc");
            DropTable("dbo.CauHinh");
            DropTable("dbo.KhuyenMai");
            DropTable("dbo.DanhMuc");
            DropTable("dbo.BinhLuan");
            DropTable("dbo.TonKho");
            DropTable("dbo.Kho");
            DropTable("dbo.LichSuTonKho");
            DropTable("dbo.LichSuGia");
            DropTable("dbo.GioHang");
            DropTable("dbo.GioHangChiTiet");
            DropTable("dbo.ThuocTinh");
            DropTable("dbo.GiaTriThuocTinh");
            DropTable("dbo.NguoiDung");
            DropTable("dbo.DiaChiGiaoHang");
            DropTable("dbo.KhachHang");
            DropTable("dbo.DonHangGiaoHang");
            DropTable("dbo.DonHang");
            DropTable("dbo.DonHangChiTiet");
            DropTable("dbo.BienTheSanPham");
            DropTable("dbo.SanPham");
            DropTable("dbo.AnhSanPham");
        }
    }
}
