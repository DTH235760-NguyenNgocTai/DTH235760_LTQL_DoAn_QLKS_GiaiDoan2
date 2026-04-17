using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_KhachHang;
using QLKS.Data.Entities.Nhom_NhanVien;
using QLKS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_HoaDon_ThanhToan
{
    public class HoaDon
    {
        public int HoaDonId { get; set; }
        public string MaHoaDon { get; set; } = string.Empty;
        public int DatPhongId { get; set; }
        public int KhachHangId { get; set; }
        public int NhanVienId { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongThanhToan { get; set; }
        public TrangThaiHoaDon TrangThai { get; set; }
        //public string? GhiChu { get; set; }
        public virtual DatPhong DatPhong { get; set; } = null!;
        public virtual KhachHang KhachHang { get; set; } = null!;
        public virtual NhanVien NhanVien { get; set; } = null!;
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
        public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
    }
}
