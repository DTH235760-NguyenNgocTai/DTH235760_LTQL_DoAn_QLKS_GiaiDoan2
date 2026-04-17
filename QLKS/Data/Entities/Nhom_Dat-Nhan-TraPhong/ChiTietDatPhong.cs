using QLKS.Data.Entities.Nhom_Phong;
using QLKS.Data.Entities.Nhom_DichVu;
using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using QLKS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DatNhanTraPhong
{
    public class ChiTietDatPhong
    {
        public int ChiTietDatPhongId { get; set; }
        public int DatPhongId { get; set; }
        public int PhongId { get; set; }

        // Thời gian dự kiến
        public DateTime ThoiGianNhanDuKien { get; set; }
        public DateTime ThoiGianTraDuKien { get; set; }

        public decimal DonGia { get; set; }
        public TrangThaiChiTietDatPhong TrangThai { get; set; }
        public bool DaLapHoaDon { get; set; } = false; // Mặc định là chưa lập hóa đơn

        // Navigation
        public virtual DatPhong DatPhong { get; set; } = null!;
        public virtual Phong Phong { get; set; } = null!;

        public virtual ICollection<ChiTietNhanPhong> ChiTietNhanPhongs { get; set; } = new List<ChiTietNhanPhong>();
        public virtual ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}
