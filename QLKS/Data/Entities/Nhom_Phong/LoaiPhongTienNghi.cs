using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    internal class LoaiPhongTienNghi
    {
        public int LoaiPhongTienNghiId { get; set; }
        public int LoaiPhongId { get; set; }
        public int TienNghiId { get; set; }
        // Navigation
        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual TienNghi? TienNghi { get; set; }
    }
}
