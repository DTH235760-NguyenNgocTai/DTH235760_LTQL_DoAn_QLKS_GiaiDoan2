using QLKS.Data.Entities.Nhom_NhanVien;
using QLKS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_TaiKhoan
{
    public class TaiKhoan
    {
        public int TaiKhoanId { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;

        public int NhanVienId { get; set; }
        public int VaiTroId { get; set; }

        public TrangThaiTaiKhoan TrangThai { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public virtual NhanVien NhanVien { get; set; } = null!;
        public virtual VaiTro VaiTro { get; set; } = null!;
    }
}
