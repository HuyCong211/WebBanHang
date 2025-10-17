namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhuongThucThanhToan")]
    public partial class PhuongThucThanhToan
    {
        [Key]
        public int PhuongThucId { get; set; }

        [Required]
        [StringLength(200)]
        public string TenPhuongThuc { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }

        public bool? Active { get; set; }
    }
}
