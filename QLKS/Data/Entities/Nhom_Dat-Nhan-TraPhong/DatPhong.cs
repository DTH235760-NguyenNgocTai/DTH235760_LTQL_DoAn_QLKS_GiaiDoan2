using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using QLKS.Data.Entities.Nhom_KhachHang;
using QLKS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DatNhanTraPhong
{
    public class DatPhong
    {
        public int DatPhongId { get; set; }
        public string MaDatPhong { get; set; } = string.Empty;
        public int KhachHangId { get; set; }
        public DateTime NgayDat { get; set; }
        public decimal TienCoc { get; set; }
        public TrangThaiDatPhong TrangThai { get; set; }
        // Phân biệt đặt trước hay khách tới trực tiếp
        public bool LaDatTruoc { get; set; } = false; // Mặc định là false, tức là khách tới trực tiếp
        // Navigation
        public virtual KhachHang KhachHang { get; set; } = null!;
        public virtual ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>();
        public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}
