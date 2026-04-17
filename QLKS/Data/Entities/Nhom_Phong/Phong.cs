using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Enums;

namespace QLKS.Data.Entities.Nhom_Phong
{
    public class Phong
    {
        public int PhongId { get; set; }
        public string SoPhong { get; set; } = string.Empty;
        public int LoaiPhongId { get; set; }
        public int TangId { get; set; }
        public  TrangThaiPhong TrangThai { get; set; }
        public string? HinhAnh { get; set; }
        public bool IsDeleted { get; set; } = false; // Thêm cột IsDeleted để đánh dấu xóa mềm, fale có nghĩa là chưa xóa
        // Navigation
        public virtual LoaiPhong LoaiPhong { get; set; } = null!;
        public virtual Tang Tang { get; set; } = null!;
        public virtual ICollection<ChiTietDatPhong> ChiTietDatPhongs { get; set; } = new List<ChiTietDatPhong>(); // Một phòng có thể có nhiều chi tiết đặt phòng (nhiều lần đặt khác nhau)
    }
}
