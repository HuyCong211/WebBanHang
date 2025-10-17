namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThuocTinh")]
    public partial class ThuocTinh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ThuocTinh()
        {
            GiaTriThuocTinhs = new HashSet<GiaTriThuocTinh>();
        }
        [DisplayName("Mã thuộc tính")]
        public int ThuocTinhId { get; set; }
        [DisplayName("Tên thuộc tính")]
        [Required]
        [StringLength(100)]
        public string TenThuocTinh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GiaTriThuocTinh> GiaTriThuocTinhs { get; set; }
    }
}
