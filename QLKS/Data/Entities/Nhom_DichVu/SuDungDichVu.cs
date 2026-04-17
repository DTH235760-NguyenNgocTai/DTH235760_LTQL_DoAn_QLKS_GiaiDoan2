using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DichVu
{
    public class SuDungDichVu
    {
        public int SuDungDichVuId { get; set; }
        public int ChiTietDatPhongId { get; set; }
        public int DichVuId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public DateTime ThoiGianSuDung { get; set; }
        public bool DaLapHoaDon { get; set; } = false; // Mặc định là chưa lập hóa đơn
        // Navigation
        public virtual ChiTietDatPhong ChiTietDatPhong { get; set; } = null!;
        public virtual DichVu DichVu { get; set; } = null!;
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}
