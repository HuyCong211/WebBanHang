namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HinhAnhTinTuc")]
    public partial class HinhAnhTinTuc
    {
        [Key]
        [DisplayName("ID ảnh tin tức")]
        public int HinhAnhId { get; set; }
        [DisplayName("ID tin tức")]
        public int TinTucId { get; set; }
        [DisplayName("Hình ảnh")]
        [Required]
        [StringLength(1000)]
        public string Url { get; set; }

        public virtual TinTuc TinTuc { get; set; }
    }
}
