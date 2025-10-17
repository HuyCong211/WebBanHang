namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BienTheSanPham")]
    public partial class BienTheSanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BienTheSanPham()
        {
            DonHangChiTiets = new HashSet<DonHangChiTiet>();
            GioHangChiTiets = new HashSet<GioHangChiTiet>();
            LichSuGias = new HashSet<LichSuGia>();
            LichSuTonKhoes = new HashSet<LichSuTonKho>();
            TonKhoes = new HashSet<TonKho>();
            GiaTriThuocTinhs = new HashSet<GiaTriThuocTinh>();
            // ← thêm init
            this.AnhSanPham_BienThes = new HashSet<AnhSanPham_BienThe>();
        }

        [Key]
        [DisplayName("Mã biến thể")]
        public int BienTheId { get; set; }
        [DisplayName("Mã sản phẩm")]
        public int SanPhamId { get; set; }
        [DisplayName("Mã định danh biến thể")]
        [StringLength(100)]
        public string SKU { get; set; }
        [DisplayName("Giá")]
        public decimal? Gia { get; set; }
        [DisplayName("Giá khuyến mại")]

        public decimal? GiaKhuyenMai { get; set; }
        [DisplayName("Mã vạch")]
        [StringLength(100)]
        public string MaVach { get; set; }
        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }
        [DisplayName("Ngày tạo")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayTao { get; set; }

        public virtual SanPham SanPham { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichSuGia> LichSuGias { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichSuTonKho> LichSuTonKhoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TonKho> TonKhoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaTriThuocTinh> GiaTriThuocTinhs { get; set; }
        // ← thêm navigation collection
        public virtual ICollection<AnhSanPham_BienThe> AnhSanPham_BienThes { get; set; }
    }
}
