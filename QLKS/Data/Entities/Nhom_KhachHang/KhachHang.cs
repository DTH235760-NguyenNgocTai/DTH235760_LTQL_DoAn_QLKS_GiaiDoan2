using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using QLKS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_KhachHang
{
    public class KhachHang
    {
        public int KhachHangId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public GioiTinh GioiTinh { get; set; }
        public string? CCCD_Passport { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public string? QuocTich { get; set; }
        public int LoaiKhachHangId { get; set; }
        //public string? GhiChu { get; set; }
        public bool IsDeleted { get; set; } = false; // Thêm cột IsDeleted để đánh dấu xóa mềm, false có nghĩa là chưa xóa

        // Navigation
        public virtual LoaiKhachHang LoaiKhachHang { get; set; } = null!;
        public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
