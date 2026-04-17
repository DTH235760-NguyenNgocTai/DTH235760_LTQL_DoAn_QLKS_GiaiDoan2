using QLKS.Data.Entities.Nhom_KhachHang;
using QLKS.Data.QLKSDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Services.Nhom_Khachhang
{
    internal class LoaiKhachHangService
    {
        private readonly QLKSDbContext context;

        public LoaiKhachHangService()
        {
            context = new QLKSDbContext();
        }

        public List<LoaiKhachHang> LayDanhSach()
        {
            return context.LoaiKhachHangs
                .OrderBy(x => x.TenLoaiKhachHang)
                .ToList();
        }

        public List<LoaiKhachHang> LayDanhSachChoComboBox()
        {
            return context.LoaiKhachHangs
                .OrderBy(x => x.TenLoaiKhachHang)
                .ToList();
        }

        public LoaiKhachHang? LayTheoId(int loaiKhachHangId)
        {
            return context.LoaiKhachHangs
                .FirstOrDefault(x => x.LoaiKhachHangId == loaiKhachHangId);
        }

        public bool ThemLoaiKhachHang(LoaiKhachHang loaiKhachHang)
        {
            if (loaiKhachHang == null)
                return false;

            string tenLoai = loaiKhachHang.TenLoaiKhachHang?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenLoai))
                return false;

            if (loaiKhachHang.TyLeUuDai < 0 || loaiKhachHang.TyLeUuDai > 100)
                return false;

            bool daTonTai = context.LoaiKhachHangs
                .Any(x => x.TenLoaiKhachHang.ToLower() == tenLoai.ToLower());

            if (daTonTai)
                return false;

            loaiKhachHang.TenLoaiKhachHang = tenLoai;
            loaiKhachHang.MoTa = string.IsNullOrWhiteSpace(loaiKhachHang.MoTa)
                ? null
                : loaiKhachHang.MoTa.Trim();

            context.LoaiKhachHangs.Add(loaiKhachHang);
            return context.SaveChanges() > 0;
        }

        public bool SuaLoaiKhachHang(LoaiKhachHang loaiKhachHang)
        {
            if (loaiKhachHang == null)
                return false;

            string tenLoai = loaiKhachHang.TenLoaiKhachHang?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenLoai))
                return false;

            if (loaiKhachHang.TyLeUuDai < 0 || loaiKhachHang.TyLeUuDai > 100)
                return false;

            var existing = context.LoaiKhachHangs
                .FirstOrDefault(x => x.LoaiKhachHangId == loaiKhachHang.LoaiKhachHangId);

            if (existing == null)
                return false;

            bool biTrung = context.LoaiKhachHangs
                .Any(x => x.TenLoaiKhachHang.ToLower() == tenLoai.ToLower()
                       && x.LoaiKhachHangId != loaiKhachHang.LoaiKhachHangId);

            if (biTrung)
                return false;

            existing.TenLoaiKhachHang = tenLoai;
            existing.TyLeUuDai = loaiKhachHang.TyLeUuDai;
            existing.MoTa = string.IsNullOrWhiteSpace(loaiKhachHang.MoTa)
                ? null
                : loaiKhachHang.MoTa.Trim();

            return context.SaveChanges() > 0;
        }

        public bool XoaLoaiKhachHang(int loaiKhachHangId)
        {
            var existing = context.LoaiKhachHangs
                .FirstOrDefault(x => x.LoaiKhachHangId == loaiKhachHangId);

            if (existing == null)
                return false;

            bool dangDuocSuDung = context.KhachHangs
                .Any(x => x.LoaiKhachHangId == loaiKhachHangId && !x.IsDeleted);

            if (dangDuocSuDung)
                return false;

            context.LoaiKhachHangs.Remove(existing);
            return context.SaveChanges() > 0;
        }
    }
}
