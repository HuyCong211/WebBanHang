namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Kho")]
    public partial class Kho
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kho()
        {
            LichSuTonKhoes = new HashSet<LichSuTonKho>();
            TonKhoes = new HashSet<TonKho>();
        }
        [DisplayName("Mã kho")]
        public int KhoId { get; set; }
        [DisplayName("Tên kho")]
        [Required]
        [StringLength(200)]
        public string TenKho { get; set; }
        [DisplayName("Địa chỉ")]
        [StringLength(500)]
        public string DiaChi { get; set; }
        [DisplayName("Điện chỉ")]
        [StringLength(50)]
        public string DienThoai { get; set; }
        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichSuTonKho> LichSuTonKhoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TonKho> TonKhoes { get; set; }
    }
}
