using QLKS.Data.Entities.Nhom_NhanVien;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DatNhanTraPhong
{
    public class TraPhong
    {
        public int TraPhongId { get; set; }
        public string MaTraPhong { get; set; } = string.Empty;
        public int NhanVienId { get; set; }
        public DateTime NgayLap { get; set; }
        // Navigation
        public virtual NhanVien NhanVien { get; set; } = null!;
        public virtual ICollection<ChiTietTraPhong> ChiTietTraPhongs { get; set; } = new List<ChiTietTraPhong>();
    }
}
