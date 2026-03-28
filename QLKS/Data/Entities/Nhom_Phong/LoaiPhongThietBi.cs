using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    internal class LoaiPhongThietBi
    {
        public int LoaiPhongThietBiId { get; set; }
        public int LoaiPhongId { get; set; }
        public int ThietBiId { get; set; }
        public int? SoLuong { get; set; }
        public string? GhiChu { get; set; }
        // Navigation
        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual ThietBi? ThietBi { get; set; }
    }
}
