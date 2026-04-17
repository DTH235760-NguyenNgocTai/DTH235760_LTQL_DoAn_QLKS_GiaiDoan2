using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DatNhanTraPhong
{
    public class ChiTietNhanPhong
    {
        public int ChiTietNhanPhongId { get; set; }
        public int NhanPhongId { get; set; }
        public int ChiTietDatPhongId { get; set; }

        // Thời gian nhận thực tế
        public DateTime ThoiGianNhanThucTe { get; set; }

        // Navigation
        public virtual NhanPhong NhanPhong { get; set; } = null!;
        public virtual ChiTietDatPhong ChiTietDatPhong { get; set; } = null!;
        public virtual ICollection<ChiTietTraPhong> ChiTietTraPhongs { get; set; } = new List<ChiTietTraPhong>();
    }
}
