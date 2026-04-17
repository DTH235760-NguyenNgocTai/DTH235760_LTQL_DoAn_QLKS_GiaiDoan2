using QLKS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_HoaDon_ThanhToan
{
    public class ThanhToan
    {
        public int ThanhToanId { get; set; }
        public int HoaDonId { get; set; }
        public decimal SoTien { get; set; }
        public DateTime ThoiGianThanhToan { get; set; }
        public PhuongThucThanhToan PhuongThucThanhToan { get; set; }
        public string? MaGiaoDich { get; set; }
        public string? GhiChu { get; set; }
        // Navigation
        public virtual HoaDon HoaDon { get; set; } = null!;
    }
}
