namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("DanhMuc")]
    public partial class DanhMuc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DanhMuc()
        {
            DanhMuc1 = new HashSet<DanhMuc>();
            SanPhams = new HashSet<SanPham>();
        }
        [DisplayName("Mã danh mục")]
        public int DanhMucId { get; set; }
        [DisplayName("Tên danh mục")]
        [Required]
        [StringLength(200)]
        public string TenDanhMuc { get; set; }
        [DisplayName("Slug")]
        [StringLength(200)]
        public string Slug { get; set; }
        [DisplayName("Mô tả")]
        [StringLength(1000)]
        public string MoTa { get; set; }
        [DisplayName("Mã danh mục cha")]
        [AllowHtml]
        public int? DanhMucChaId { get; set; }
        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }
        [DisplayName("Ngày tạo")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayTao { get; set; }
        [DisplayName("Ngày cập nhật")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayCapNhat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DanhMuc> DanhMuc1 { get; set; }

        public virtual DanhMuc DanhMuc2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
