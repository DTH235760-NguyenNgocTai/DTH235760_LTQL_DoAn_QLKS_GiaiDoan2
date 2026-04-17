using QLKS.Data.Entities.Nhom_NhanVien;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DatNhanTraPhong
{
    public class NhanPhong
    {
        public int NhanPhongId { get; set; }
        public string MaNhanPhong { get; set; } = string.Empty;
        public int NhanVienId { get; set; }
        public DateTime NgayLap { get; set; }
        // Navigation
        public virtual NhanVien NhanVien { get; set; } = null!;
        public virtual ICollection<ChiTietNhanPhong> ChiTietNhanPhongs { get; set; } = new List<ChiTietNhanPhong>();
    }
}
