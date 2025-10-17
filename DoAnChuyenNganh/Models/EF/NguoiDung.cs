namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NguoiDung")]
    public partial class NguoiDung
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NguoiDung()
        {
            KhachHangs = new HashSet<KhachHang>();
        }

        public Guid Id { get; set; }

        [StringLength(256)]
        public string TenDangNhap { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(200)]
        public string HoTen { get; set; }

        [StringLength(50)]
        public string DienThoai { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NgayTao { get; set; }

        public bool? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KhachHang> KhachHangs { get; set; }
    }
}
