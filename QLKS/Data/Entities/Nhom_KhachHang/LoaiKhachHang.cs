using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_KhachHang
{
    public class LoaiKhachHang
    {
        public int LoaiKhachHangId { get; set; }
        public string TenLoaiKhachHang { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public decimal TyLeUuDai { get; set; }

        // Navigation
        public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();
    }
}
