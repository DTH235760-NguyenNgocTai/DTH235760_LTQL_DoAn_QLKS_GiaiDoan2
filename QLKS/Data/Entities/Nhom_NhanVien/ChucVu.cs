using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_NhanVien
{
    public class ChucVu
    {
        public int ChucVuId { get; set; }
        public string TenChucVu { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        // Navigation
        public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
    }
}
