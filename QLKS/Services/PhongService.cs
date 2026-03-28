using Microsoft.EntityFrameworkCore;
using QLKS.Data;
using QLKS.Data.Entities.Nhom_Phong;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using System.Collections.Generic;
using System.Linq;

namespace QLKS.Services
{
    internal class PhongService
    {
        private readonly QLKSDbContext context;

        public PhongService()
        {
            context = new QLKSDbContext();
        }

        public List<Phong> LayDanhSach()
        {
            return context.Phongs
                .Include(p => p.Tang)
                .Include(p => p.LoaiPhong)
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.SoPhong)
                .ToList();
        }

        public Phong? LayTheoId(int id)
        {
            return context.Phongs
                .Include(p => p.Tang)
                .Include(p => p.LoaiPhong)
                .FirstOrDefault(p => p.PhongId == id && !p.IsDeleted);
        }

        public bool ThemPhong(Phong phong)
        {
            bool tonTai = context.Phongs
                .Any(p => p.SoPhong == phong.SoPhong && !p.IsDeleted);

            if (tonTai)
                return false;

            context.Phongs.Add(phong);
            context.SaveChanges();
            return true;
        }

        public bool SuaPhong(Phong phong)
        {
            var existing = context.Phongs.Find(phong.PhongId);

            if (existing == null || existing.IsDeleted)
                return false;

            bool tonTai = context.Phongs.Any(p =>
                p.SoPhong == phong.SoPhong &&
                p.PhongId != phong.PhongId &&
                !p.IsDeleted);

            if (tonTai)
                return false;

            existing.SoPhong = phong.SoPhong;
            existing.TangId = phong.TangId;
            existing.LoaiPhongId = phong.LoaiPhongId;
            existing.TrangThai = phong.TrangThai;
            existing.HinhAnh = phong.HinhAnh;


            context.SaveChanges();
            return true;
        }

        public bool XoaPhong(int phongId)
        {
            var phong = context.Phongs.Find(phongId);

            if (phong == null || phong.IsDeleted)
                return false;

            if (phong.TrangThai != TrangThaiPhong.Trong)
                return false;

            phong.IsDeleted = true;
            context.SaveChanges();
            return true;
        }
    }
}