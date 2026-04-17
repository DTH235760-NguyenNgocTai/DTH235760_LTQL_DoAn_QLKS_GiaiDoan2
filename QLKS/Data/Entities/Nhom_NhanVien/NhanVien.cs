using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using QLKS.Data.Entities.Nhom_TaiKhoan;
using QLKS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_NhanVien
{
    public class NhanVien
    {
        public int NhanVienId { get; set; }
        public string MaNhanVien { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; }
        public GioiTinh GioiTinh { get; set; }
        public string SoDienThoai { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public int ChucVuId { get; set; }
        public DateTime NgayVaoLam { get; set; }
        public decimal LuongCoBan { get; set; }
        public TrangThaiLamViec TrangThaiLamViec { get; set; }
        public string? GhiChu { get; set; }

        // Navigation
        public virtual ChucVu ChucVu { get; set; } = null!;
        public virtual ICollection<NhanPhong> NhanPhongs { get; set; } = new List<NhanPhong>();
        public virtual ICollection<TraPhong> TraPhongs { get; set; } = new List<TraPhong>();
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        //1 NhanVien chi co 1 TaiKhoan
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
