namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("SanPham")]
    public partial class SanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPham()
        {
            AnhSanPhams = new HashSet<AnhSanPham>();
            BienTheSanPhams = new HashSet<BienTheSanPham>();
            BinhLuans = new HashSet<BinhLuan>();
            KhuyenMais = new HashSet<KhuyenMai>();
        }
        [DisplayName("ID")]
        public int SanPhamId { get; set; }
        [DisplayName("Mã sản phẩm")]
        [StringLength(50)]
        public string MaSanPham { get; set; }
        [DisplayName("Tên sản phẩm")]
        [Required]
        [StringLength(300)]
        public string TenSanPham { get; set; }
        [DisplayName("Slug")]
        [StringLength(300)]
        public string Slug { get; set; }
        [DisplayName("Mô tả ngắn")]
        [StringLength(500)]
        [AllowHtml]
        public string MoTaNgan { get; set; }
        [DisplayName("Chi tiết")]
        [AllowHtml]
        public string MoTaChiTiet { get; set; }
        [DisplayName("Giá bán")]
        public decimal GiaBan { get; set; }
        [DisplayName("Giá gốc")]
        public decimal? GiaGoc { get; set; }
        [DisplayName("Mã danh mục")]
        public int? DanhMucId { get; set; }
        [DisplayName("Trạng thái")]
        public int? TrangThai { get; set; }
        [DisplayName("Lượt xem")]
        public int? LuotXem { get; set; }
        [DisplayName("Ngày tạo")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayTao { get; set; }
        [DisplayName("Ngày cập nhật")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayCapNhat { get; set; }

        [NotMapped]
        public bool IsFavorite { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnhSanPham> AnhSanPhams { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }

        public virtual DanhMuc DanhMuc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KhuyenMai> KhuyenMais { get; set; }

        [NotMapped] public decimal ViewBag_FinalPrice { get; set; }
        [NotMapped] public decimal ViewBag_DiscountPercent { get; set; }
        [NotMapped] public string ViewBag_PromoName { get; set; }

    }
}
