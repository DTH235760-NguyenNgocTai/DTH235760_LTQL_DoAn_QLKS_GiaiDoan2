using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    public class Tang
    {
        public int TangId { get; set; }
        public int SoTang { get; set; }
        public bool IsDeleted { get; set; } = false; // Thêm cột IsDeleted để đánh dấu xóa mềm, fale có nghĩa là chưa xóa

        // Navigation
        public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
    }
}
