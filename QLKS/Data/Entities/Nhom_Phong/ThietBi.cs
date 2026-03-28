using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    internal class ThietBi
    {
        public int ThietBiId { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public string? DonViTinh { get; set; }
        public decimal GiaTri { get; set; }

        // Navigation
        public virtual ICollection<LoaiPhongThietBi> LoaiPhongThietBis { get; set; } = new List<LoaiPhongThietBi>();
    }
}
