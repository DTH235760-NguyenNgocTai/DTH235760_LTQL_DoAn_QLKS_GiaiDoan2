using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_KhachHang
{
    internal class KhachHang
    {
        public int KhachHangId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? CCCD_Passport { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public string? QuocTich { get; set; }
        public int LoaiKhachHangId { get; set; }
        public string? GhiChu { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public virtual LoaiKhachHang? LoaiKhachHang { get; set; }
    }
}
