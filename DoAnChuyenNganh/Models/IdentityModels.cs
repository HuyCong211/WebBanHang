using DoAnChuyenNganh.Models.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DoAnChuyenNganh.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authType);
            return userIdentity;
        }

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    // Add custom user claims here
        //    return userIdentity;
        //}
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<AnhSanPham> AnhSanPhams { get; set; }
        public virtual DbSet<BienTheSanPham> BienTheSanPhams { get; set; }
        public virtual DbSet<BinhLuan> BinhLuans { get; set; }
        public virtual DbSet<CauHinh> CauHinhs { get; set; }
        public virtual DbSet<DanhMuc> DanhMucs { get; set; }
        public virtual DbSet<DiaChiGiaoHang> DiaChiGiaoHangs { get; set; }
        public virtual DbSet<DonHang> DonHangs { get; set; }
        public virtual DbSet<DonHangChiTiet> DonHangChiTiets { get; set; }
        public virtual DbSet<DonHangGiaoHang> DonHangGiaoHangs { get; set; }
        public virtual DbSet<GiaTriThuocTinh> GiaTriThuocTinhs { get; set; }
        public virtual DbSet<GioHang> GioHangs { get; set; }
        public virtual DbSet<GioHangChiTiet> GioHangChiTiets { get; set; }
        public virtual DbSet<HinhAnhTinTuc> HinhAnhTinTucs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<Kho> Khoes { get; set; }
        public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
        public virtual DbSet<LichSuGia> LichSuGias { get; set; }
        public virtual DbSet<LichSuTonKho> LichSuTonKhoes { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
        public virtual DbSet<PhuongThucThanhToan> PhuongThucThanhToans { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<ThuocTinh> ThuocTinhs { get; set; }
        public virtual DbSet<TinTuc> TinTucs { get; set; }
        public virtual DbSet<TonKho> TonKhoes { get; set; }
        public virtual DbSet<AnhSanPham_BienThe> AnhSanPham_BienThe { get; set; }
        public virtual DbSet<ThongKeDoanhThu> ThongKeDoanhThus { get; set; }
        public virtual DbSet<ThongKeSanPham> ThongKeSanPhams { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BienTheSanPham>()
                .HasMany(e => e.DonHangChiTiets)
                .WithRequired(e => e.BienTheSanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<BienTheSanPham>()
                .HasMany(e => e.GioHangChiTiets)
                .WithRequired(e => e.BienTheSanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<BienTheSanPham>()
                .HasMany(e => e.LichSuGias)
                .WithRequired(e => e.BienTheSanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<BienTheSanPham>()
                .HasMany(e => e.LichSuTonKhoes)
                .WithRequired(e => e.BienTheSanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<BienTheSanPham>()
                .HasMany(e => e.TonKhoes)
                .WithRequired(e => e.BienTheSanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<BienTheSanPham>()
                .HasMany(e => e.GiaTriThuocTinhs)
                .WithMany(e => e.BienTheSanPhams)
                .Map(m => m.ToTable("BienTheGiaTri").MapLeftKey("BienTheId").MapRightKey("GiaTriId"));

            modelBuilder.Entity<DanhMuc>()
                .HasMany(e => e.DanhMuc1)
                .WithOptional(e => e.DanhMuc2)
                .HasForeignKey(e => e.DanhMucChaId);

            modelBuilder.Entity<DonHang>()
                .HasMany(e => e.DonHangChiTiets)
                .WithRequired(e => e.DonHang)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DonHang>()
                .HasMany(e => e.DonHangGiaoHangs)
                .WithRequired(e => e.DonHang)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DonHang>()
                .HasOptional(d => d.DiaChiGiaoHang)
                .WithMany()
                .HasForeignKey(d => d.DiaChiGiaoHangId)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<DonHangChiTiet>()
                .Property(e => e.ThanhTien)
                .HasPrecision(29, 2);

            modelBuilder.Entity<GioHang>()
                .HasMany(e => e.GioHangChiTiets)
                .WithRequired(e => e.GioHang)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<GioHangChiTiet>()
                .Property(e => e.ThanhTien)
                .HasPrecision(29, 2);

            modelBuilder.Entity<Kho>()
                .HasMany(e => e.LichSuTonKhoes)
                .WithRequired(e => e.Kho)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Kho>()
                .HasMany(e => e.TonKhoes)
                .WithRequired(e => e.Kho)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<KhuyenMai>()
                .HasMany(e => e.SanPhams)
                .WithMany(e => e.KhuyenMais)
                .Map(m => m.ToTable("SanPhamKhuyenMai").MapLeftKey("KhuyenMaiId").MapRightKey("SanPhamId"));

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.AnhSanPhams)
                .WithRequired(e => e.SanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.BienTheSanPhams)
                .WithRequired(e => e.SanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<SanPham>()
                .HasMany(e => e.BinhLuans)
                .WithRequired(e => e.SanPham)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ThuocTinh>()
                .HasMany(e => e.GiaTriThuocTinhs)
                .WithRequired(e => e.ThuocTinh)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<TinTuc>()
                .HasMany(e => e.HinhAnhTinTucs)
                .WithRequired(e => e.TinTuc)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<AnhSanPham_BienThe>()
                .HasKey(t => new { t.AnhSanPhamId, t.BienTheId });

            modelBuilder.Entity<AnhSanPham_BienThe>()
                .HasRequired(t => t.AnhSanPham)
                .WithMany(a => a.AnhSanPham_BienThes)
                .HasForeignKey(t => t.AnhSanPhamId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AnhSanPham_BienThe>()
                .HasRequired(t => t.BienTheSanPham)
                .WithMany(b => b.AnhSanPham_BienThes)
                .HasForeignKey(t => t.BienTheId)
                .WillCascadeOnDelete(true);


        }
    }
}