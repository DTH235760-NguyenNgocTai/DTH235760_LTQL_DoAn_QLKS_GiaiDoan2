using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DatNhanTraPhong
{
    public class ChiTietTraPhong
    {
        public int ChiTietTraPhongId { get; set; }
        public int TraPhongId { get; set; }

        public int ChiTietNhanPhongId { get; set; }

        // Thời gian trả thực tế
        public DateTime ThoiGianTraThucTe { get; set; }

        // Navigation
        public virtual TraPhong TraPhong { get; set; } = null!;
        public virtual ChiTietNhanPhong ChiTietNhanPhong { get; set; } = null!;
    }
}
