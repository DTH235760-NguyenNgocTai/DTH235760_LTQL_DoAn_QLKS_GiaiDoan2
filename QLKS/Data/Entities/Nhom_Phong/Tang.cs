using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QLKS.Data.Entities.Nhom_Phong
{
    internal class Tang
    {
        public int TangId { get; set; }
        public int SoTang { get; set; }
        // Navigation
        public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
    }
}
