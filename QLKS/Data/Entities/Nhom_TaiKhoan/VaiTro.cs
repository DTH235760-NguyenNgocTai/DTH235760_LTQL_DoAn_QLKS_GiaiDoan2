using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_TaiKhoan
{
    public class VaiTro
    {
        public int VaiTroId { get; set; }
        public string TenVaiTro { get; set; } = string.Empty;
        public string? MoTa { get; set; }

        // Navigation
        public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
    }
}
