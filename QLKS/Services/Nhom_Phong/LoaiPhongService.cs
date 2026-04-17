using QLKS.Data.Entities.Nhom_Phong;
using QLKS.Data.QLKSDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Services.Nhom_Phong
{
    public class LoaiPhongService
    {
        private readonly QLKSDbContext context;

        public LoaiPhongService()
        {
            context = new QLKSDbContext();
        }

        public List<LoaiPhong> LayDanhSach()
        {
            return context.LoaiPhongs
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TenLoaiPhong)
                .ToList();
        }

        public List<LoaiPhong> LayDanhSachChoComboBox()
        {
            return context.LoaiPhongs
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.TenLoaiPhong)
                .ToList();
        }

        public LoaiPhong? LayTheoId(int loaiPhongId)
        {
            return context.LoaiPhongs
                .FirstOrDefault(x => x.LoaiPhongId == loaiPhongId && !x.IsDeleted);
        }

        public bool ThemLoaiPhong(LoaiPhong loaiPhong)
        {
            if (loaiPhong == null)
                return false;

            string tenLoai = loaiPhong.TenLoaiPhong?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tenLoai))
                return false;

            if (loaiPhong.GiaCoBan < 0 || loaiPhong.SoNguoiToiDa <= 0 || loaiPhong.DienTich <= 0)
                return false;

            bool daTonTai = context.LoaiPhongs
                .Any(x => x.TenLoaiPhong.ToLower() == tenLoai.ToLower() && !x.IsDeleted);

            if (daTonTai)
                return false;

            loaiPhong.TenLoaiPhong = tenLoai;

            context.LoaiPhongs.Add(loaiPhong);
            return context.SaveChanges() > 0;
        }

        public bool SuaLoaiPhong(LoaiPhong loaiPhong)
        {
            if (loaiPhong == null)
                return false;

            string tenLoai = loaiPhong.TenLoaiPhong?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tenLoai))
                return false;

            if (loaiPhong.GiaCoBan < 0 || loaiPhong.SoNguoiToiDa <= 0 || loaiPhong.DienTich <= 0)
                return false;

            var existing = context.LoaiPhongs
                .FirstOrDefault(x => x.LoaiPhongId == loaiPhong.LoaiPhongId && !x.IsDeleted);

            if (existing == null)
                return false;

            bool biTrung = context.LoaiPhongs
                .Any(x => x.TenLoaiPhong.ToLower() == tenLoai.ToLower() &&
                          x.LoaiPhongId != loaiPhong.LoaiPhongId &&
                          !x.IsDeleted);

            if (biTrung)
                return false;

            existing.TenLoaiPhong = tenLoai;
            existing.GiaCoBan = loaiPhong.GiaCoBan;
            existing.SoNguoiToiDa = loaiPhong.SoNguoiToiDa;
            existing.DienTich = loaiPhong.DienTich;

            return context.SaveChanges() > 0;
        }

        public bool XoaLoaiPhong(int loaiPhongId)
        {
            var existing = context.LoaiPhongs
                .FirstOrDefault(x => x.LoaiPhongId == loaiPhongId && !x.IsDeleted);

            if (existing == null)
                return false;

            bool dangDuocSuDung = context.Phongs
                .Any(x => x.LoaiPhongId == loaiPhongId && !x.IsDeleted);

            if (dangDuocSuDung)
                return false;

            existing.IsDeleted = true;
            return context.SaveChanges() > 0;
        }
    }
}
