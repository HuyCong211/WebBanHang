namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GioHang")]
    public partial class GioHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GioHang()
        {
            GioHangChiTiets = new HashSet<GioHangChiTiet>();
        }

        public long GioHangId { get; set; }

        public int? KhachHangId { get; set; }

        public Guid? NguoiDungId { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? NgayTao { get; set; }

        public int? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; }
    }
}
