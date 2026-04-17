using QLKS.Data.Entities.Nhom_Phong;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using Microsoft.EntityFrameworkCore;
namespace QLKS.Services.Nhom_Phong
{
    public class PhongService
    {
        private readonly QLKSDbContext context;

        public PhongService()
        {
            context = new QLKSDbContext();
        }

        public List<Phong> LayDanhSach()
        {
            return context.Phongs
                .Include(x => x.LoaiPhong)
                .Include(x => x.Tang)
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SoPhong)
                .ToList();
        }

        public List<Phong> LayPhongTrong()
        {
            return context.Phongs
                .Include(x => x.LoaiPhong)
                .Include(x => x.Tang)
                .Where(x => !x.IsDeleted && x.TrangThai == TrangThaiPhong.Trong)
                .OrderBy(x => x.SoPhong)
                .ToList();
        }

        public Phong? LayTheoId(int phongId)
        {
            return context.Phongs
                .Include(x => x.LoaiPhong)
                .Include(x => x.Tang)
                .FirstOrDefault(x => x.PhongId == phongId && !x.IsDeleted);
        }

        public List<Phong> TimKiemPhong(
            string? tuKhoa = null,
            int? loaiPhongId = null,
            int? tangId = null,
            TrangThaiPhong? trangThai = null)
        {
            var query = context.Phongs
                .Include(x => x.LoaiPhong)
                .Include(x => x.Tang)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                query = query.Where(x =>
                    x.SoPhong.Contains(tuKhoa) ||
                    x.LoaiPhong.TenLoaiPhong.Contains(tuKhoa) ||
                    x.Tang.SoTang.ToString().Contains(tuKhoa));
            }

            if (loaiPhongId.HasValue)
            {
                query = query.Where(x => x.LoaiPhongId == loaiPhongId.Value);
            }

            if (tangId.HasValue)
            {
                query = query.Where(x => x.TangId == tangId.Value);
            }

            if (trangThai.HasValue)
            {
                query = query.Where(x => x.TrangThai == trangThai.Value);
            }

            return query
                .OrderBy(x => x.SoPhong)
                .ToList();
        }

        public bool ThemPhong(Phong phong)
        {
            if (phong == null)
                return false;

            string soPhong = phong.SoPhong?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(soPhong))
                return false;

            bool tangHopLe = context.Tangs
                .Any(x => x.TangId == phong.TangId && !x.IsDeleted);

            if (!tangHopLe)
                return false;

            bool loaiPhongHopLe = context.LoaiPhongs
                .Any(x => x.LoaiPhongId == phong.LoaiPhongId && !x.IsDeleted);

            if (!loaiPhongHopLe)
                return false;

            bool daTonTai = context.Phongs
                .Any(x => x.SoPhong == soPhong && !x.IsDeleted);

            if (daTonTai)
                return false;

            phong.SoPhong = soPhong;

            context.Phongs.Add(phong);
            return context.SaveChanges() > 0;
        }

        public bool SuaPhong(Phong phong)
        {
            if (phong == null)
                return false;

            string soPhong = phong.SoPhong?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(soPhong))
                return false;

            var existing = context.Phongs
                .FirstOrDefault(x => x.PhongId == phong.PhongId && !x.IsDeleted);

            if (existing == null)
                return false;

            bool tangHopLe = context.Tangs
                .Any(x => x.TangId == phong.TangId && !x.IsDeleted);

            if (!tangHopLe)
                return false;

            bool loaiPhongHopLe = context.LoaiPhongs
                .Any(x => x.LoaiPhongId == phong.LoaiPhongId && !x.IsDeleted);

            if (!loaiPhongHopLe)
                return false;

            bool biTrungSoPhong = context.Phongs
                .Any(x => x.SoPhong == soPhong &&
                          x.PhongId != phong.PhongId &&
                          !x.IsDeleted);

            if (biTrungSoPhong)
                return false;

            existing.SoPhong = soPhong;
            existing.LoaiPhongId = phong.LoaiPhongId;
            existing.TangId = phong.TangId;
            existing.TrangThai = phong.TrangThai;
            existing.HinhAnh = phong.HinhAnh;

            return context.SaveChanges() > 0;
        }

        public bool CapNhatTrangThaiPhong(int phongId, TrangThaiPhong trangThaiMoi)
        {
            var existing = context.Phongs
                .FirstOrDefault(x => x.PhongId == phongId && !x.IsDeleted);

            if (existing == null)
                return false;

            existing.TrangThai = trangThaiMoi;
            return context.SaveChanges() > 0;
        }

        public bool XacNhanDaDonPhong(int phongId)
        {
            var existing = context.Phongs
                .FirstOrDefault(x => x.PhongId == phongId && !x.IsDeleted);

            if (existing == null || existing.TrangThai != TrangThaiPhong.DangDon)
                return false;

            bool coDatPhongHomNay = context.ChiTietDatPhongs
                .Any(x =>
                    x.PhongId == phongId &&
                    x.TrangThai == TrangThaiChiTietDatPhong.DaDat &&
                    x.ThoiGianNhanDuKien.Date <= DateTime.Today);

            existing.TrangThai = coDatPhongHomNay
                ? TrangThaiPhong.DangDat
                : TrangThaiPhong.Trong;

            return context.SaveChanges() > 0;
        }

        public bool XoaPhong(int phongId)
        {
            var existing = context.Phongs
                .FirstOrDefault(x => x.PhongId == phongId && !x.IsDeleted);

            if (existing == null)
                return false;

            if (existing.TrangThai == TrangThaiPhong.DangDat ||
                existing.TrangThai == TrangThaiPhong.DangO)
            {
                return false;
            }

            existing.IsDeleted = true;
            return context.SaveChanges() > 0;
        }
    }
}
