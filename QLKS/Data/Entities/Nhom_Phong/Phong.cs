using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using QLKS.Enums;

namespace QLKS.Data.Entities.Nhom_Phong
{
    internal class Phong
    {
        public int PhongId { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public int LoaiPhongId { get; set; }
        public int TangId { get; set; }
        public  TrangThaiPhong TrangThai { get; set; }
        public string? HinhAnh { get; set; }
        public bool IsDeleted { get; set; } = false; // Thêm cột IsDeleted để đánh dấu xóa mềm, fale có nghĩa là chưa xóa, true có nghĩa là đã xóa
        // Navigation
        public virtual LoaiPhong? LoaiPhong { get; set; }
        public virtual Tang? Tang { get; set; }
    }
}
