namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("TinTuc")]
    public partial class TinTuc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TinTuc()
        {
            HinhAnhTinTucs = new HashSet<HinhAnhTinTuc>();
        }
        [DisplayName("ID tin tức")]
        public int TinTucId { get; set; }
        [DisplayName("Tiêu đề")]
        [Required]
        [StringLength(400)]
        public string TieuDe { get; set; }
        [DisplayName("Slug")]
        [StringLength(400)]
        public string Slug { get; set; }
        [DisplayName("Tóm tắt")]
        [StringLength(1000)]
        [AllowHtml]
        public string TomTat { get; set; }
        [DisplayName("Nội dung")]
        [AllowHtml]
        public string NoiDung { get; set; }
        [DisplayName("Ngày đăng")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayDang { get; set; }
        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HinhAnhTinTuc> HinhAnhTinTucs { get; set; }
    }
}
