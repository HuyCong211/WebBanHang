namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DonHang")]
    public partial class DonHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DonHang()
        {
            DonHangChiTiets = new HashSet<DonHangChiTiet>();
            DonHangGiaoHangs = new HashSet<DonHangGiaoHang>();
        }

        public long DonHangId { get; set; }

        [StringLength(100)]
        public string MaDonHang { get; set; }

        public int? KhachHangId { get; set; }

        public Guid? NguoiDungId { get; set; }
        [StringLength(150)]
        public string Email { get; set; }

        public decimal TongTien { get; set; }

        public decimal? PhiVanChuyen { get; set; }

        public int? PhuongThucThanhToanId { get; set; }

        public int? DiaChiGiaoHangId { get; set; }

        public int? TrangThai { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NgayTao { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NgayCapNhat { get; set; }

        [StringLength(1000)]
        public string GhiChu { get; set; }

        public virtual KhachHang KhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHangChiTiet> DonHangChiTiets { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHangGiaoHang> DonHangGiaoHangs { get; set; }
        public virtual DiaChiGiaoHang DiaChiGiaoHang { get; set; }

    }
}
