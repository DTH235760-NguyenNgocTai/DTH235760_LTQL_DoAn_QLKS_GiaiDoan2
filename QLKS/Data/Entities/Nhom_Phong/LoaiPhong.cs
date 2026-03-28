using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    internal class LoaiPhong
    {
        public int LoaiPhongId { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaCoBan { get; set; }
        public int SoNguoiToiDa { get; set; }
        public double DienTich { get; set; }
        // Navigation
        public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
        public virtual ICollection<LoaiPhongTienNghi> LoaiPhongTienNghis { get; set; } = new List<LoaiPhongTienNghi>();
        public virtual ICollection<LoaiPhongThietBi> LoaiPhongThietBis { get; set; } = new List<LoaiPhongThietBi>();
    }
}
