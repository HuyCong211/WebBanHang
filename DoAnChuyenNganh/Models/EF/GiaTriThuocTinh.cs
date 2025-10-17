namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiaTriThuocTinh")]
    public partial class GiaTriThuocTinh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GiaTriThuocTinh()
        {
            BienTheSanPhams = new HashSet<BienTheSanPham>();
        }
        [DisplayName("Mã giá trị thuộc tính")]
        [Key]
        public int GiaTriId { get; set; }
        [DisplayName("Mã thuộc tính")]
        public int ThuocTinhId { get; set; }
        [DisplayName("Tên giá trị")]
        [Required]
        [StringLength(200)]
        public string TenGiaTri { get; set; }

        public virtual ThuocTinh ThuocTinh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; set; }
    }
}
