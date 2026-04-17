using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_DichVu
{
    public class DichVu
    {
        public int DichVuId { get; set; }
        public string MaDichVu { get; set; } = string.Empty;
        public string TenDichVu { get; set; } = string.Empty;

        public decimal DonGia { get; set; }
        public string DonViTinh { get; set; } = string.Empty;

        public bool ConSuDung { get; set; } = true; // Mặc định là còn sử dụng
        public string? MoTa { get; set; }

        // Navigation
        public virtual ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
    }
}
