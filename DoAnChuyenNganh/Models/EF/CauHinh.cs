namespace DoAnChuyenNganh.Models.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CauHinh")]
    public partial class CauHinh
    {
        [Key]
        [StringLength(200)]
        public string KeyName { get; set; }

        public string Value { get; set; }

        [StringLength(500)]
        public string MoTa { get; set; }
    }
}
