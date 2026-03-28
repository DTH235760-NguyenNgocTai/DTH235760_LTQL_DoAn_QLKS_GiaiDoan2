using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    internal class TienNghi
    {
        public int TienNghiId { get; set; }
        public string TenTienNghi { get; set; } = string.Empty;
        // Navigation
        public virtual ICollection<LoaiPhongTienNghi> LoaiPhongTienNghis { get; set; } = new List<LoaiPhongTienNghi>();
    }
}
