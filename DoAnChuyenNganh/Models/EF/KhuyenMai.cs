namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KhuyenMai")]
    public partial class KhuyenMai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhuyenMai()
        {
            SanPhams = new HashSet<SanPham>();
        }
        [DisplayName("ID")]
        public int KhuyenMaiId { get; set; }
        [DisplayName("Mã khuyến mại")]
        [StringLength(100, ErrorMessage ="{0} không được nhiều hơn 100 ký tự")]
        public string MaKM { get; set; }
        [DisplayName("Tên khuyến mại")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [StringLength(300, ErrorMessage = "{0} không được nhiều hơn 300 ký tự")]
        public string TenKM { get; set; }
        [DisplayName("Loại")]
        [StringLength(50, ErrorMessage = "{0} không được nhiều hơn 50 ký tự")]
        public string Loai { get; set; }
        [DisplayName("Giá trị")]
        public decimal? GiaTri { get; set; }
        [DisplayName("Ngày bắt đầu")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayBatDau { get; set; }
        [DisplayName("Ngày kết thúc")]
        [Column(TypeName = "datetime2")]
        public DateTime? NgayKetThuc { get; set; }
        [DisplayName("Điều kiện")]
        [StringLength(500, ErrorMessage = "{0} không được nhiều hơn 500 ký tự")]
        public string DieuKien { get; set; }
        [DisplayName("Trạng thái")]
        public bool? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}
