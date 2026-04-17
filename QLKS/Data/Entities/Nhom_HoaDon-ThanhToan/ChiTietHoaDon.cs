using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_DichVu;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_HoaDon_ThanhToan
{
    public class ChiTietHoaDon
    {
        public int ChiTietHoaDonId { get; set; }
        public int HoaDonId { get; set; }
        // Nếu là tiền phòng thì gắn với ChiTietDatPhong
        public int? ChiTietDatPhongId { get; set; }
        // Nếu là dịch vụ thì gắn với SuDungDichVu
        public int? SuDungDichVuId { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        // Navigation
        public virtual HoaDon HoaDon { get; set; } = null!;
        public virtual ChiTietDatPhong? ChiTietDatPhong { get; set; }
        public virtual SuDungDichVu? SuDungDichVu { get; set; }
    }
}
