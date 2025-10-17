namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AnhSanPham")]
    public partial class AnhSanPham
    {
        public AnhSanPham()
        {
            // nếu class đã có constructor khác, thêm init trong đó
            this.AnhSanPham_BienThes = new HashSet<AnhSanPham_BienThe>();
        }
        [DisplayName("Mã ảnh")]
        public int AnhSanPhamId { get; set; }
        [DisplayName("Mã sản phẩm")]
        public int SanPhamId { get; set; }
       
        [DisplayName("Ảnh")]
        [Required]
        [StringLength(1000)]
        public string Url { get; set; }
        [DisplayName("Thứ tự")]
        public int? ThuTu { get; set; }
        [DisplayName("Mặc định")]
        public bool? MacDinh { get; set; }
        [DisplayName("Mô tả")]
        [StringLength(500)]
        public string MoTa { get; set; }

        public virtual SanPham SanPham { get; set; }
        // ← thêm navigation collection
        public virtual ICollection<AnhSanPham_BienThe> AnhSanPham_BienThes { get; set; }
    }
}
