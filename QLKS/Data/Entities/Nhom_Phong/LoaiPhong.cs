using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    public class LoaiPhong
    {
        public int LoaiPhongId { get; set; }
        public string TenLoaiPhong { get; set; } = string.Empty;
        public decimal GiaCoBan { get; set; }
        public int SoNguoiToiDa { get; set; }
        public double DienTich { get; set; }
        public bool IsDeleted { get; set; } = false; // Thêm cột IsDeleted để đánh dấu xóa mềm, fale có nghĩa là chưa xóa
        // Navigation
        public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
    }
}
