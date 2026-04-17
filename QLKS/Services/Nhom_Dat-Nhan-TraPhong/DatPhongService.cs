using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_Phong;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using Microsoft.EntityFrameworkCore;
namespace QLKS.Services.Nhom_Dat_Nhan_TraPhong
{
    public class DatPhongService
    {
        private readonly QLKSDbContext context;

        public DatPhongService()
        {
            context = new QLKSDbContext();
        }

        public List<DatPhong> LayDanhSach()
        {
            return context.DatPhongs
                .Include(x => x.KhachHang)
                .Include(x => x.ChiTietDatPhongs)
                    .ThenInclude(x => x.Phong)
                .OrderByDescending(x => x.NgayDat)
                .ToList();
        }

        public DatPhong? LayTheoId(int datPhongId)
        {
            return context.DatPhongs
                .Include(x => x.KhachHang)
                .Include(x => x.ChiTietDatPhongs)
                    .ThenInclude(x => x.Phong)
                        .ThenInclude(x => x.LoaiPhong)
                .Include(x => x.ChiTietDatPhongs)
                    .ThenInclude(x => x.Phong)
                        .ThenInclude(x => x.Tang)
                .FirstOrDefault(x => x.DatPhongId == datPhongId);
        }

        public List<Phong> LayPhongCoTheDat(DateTime thoiGianNhan, DateTime thoiGianTra)
        {
            if (thoiGianNhan >= thoiGianTra)
                return new List<Phong>();

            var danhSachPhongBiTrung = context.ChiTietDatPhongs
                .Where(x =>
                    x.TrangThai != TrangThaiChiTietDatPhong.DaHuy &&
                    x.TrangThai != TrangThaiChiTietDatPhong.DaTra &&
                    thoiGianNhan < x.ThoiGianTraDuKien &&
                    thoiGianTra > x.ThoiGianNhanDuKien)
                .Select(x => x.PhongId);

            return context.Phongs
                .Include(x => x.LoaiPhong)
                .Include(x => x.Tang)
                .Where(x =>
                    !x.IsDeleted &&
                    x.TrangThai != TrangThaiPhong.BaoTri &&
                    !danhSachPhongBiTrung.Contains(x.PhongId))
                .OrderBy(x => x.SoPhong)
                .ToList();
        }

        public bool ThemDatPhong(DatPhong datPhong, List<ChiTietDatPhong> danhSachChiTiet)
        {
            if (datPhong == null || danhSachChiTiet == null || danhSachChiTiet.Count == 0)
                return false;

            if (!context.KhachHangs.Any(x => x.KhachHangId == datPhong.KhachHangId && !x.IsDeleted))
                return false;

            if (datPhong.TienCoc < 0)
                return false;

            if (string.IsNullOrWhiteSpace(datPhong.MaDatPhong))
                datPhong.MaDatPhong = TaoMaDatPhong();

            datPhong.MaDatPhong = datPhong.MaDatPhong.Trim();

            if (context.DatPhongs.Any(x => x.MaDatPhong == datPhong.MaDatPhong))
                return false;

            if (datPhong.NgayDat == default)
                datPhong.NgayDat = DateTime.Now;

            if (danhSachChiTiet.Select(x => x.PhongId).Distinct().Count() != danhSachChiTiet.Count)
                return false;

            var phongCache = new Dictionary<int, Phong>();

            foreach (var chiTiet in danhSachChiTiet)
            {
                if (!KiemTraChiTietDatPhongHopLe(chiTiet))
                    return false;

                var phong = context.Phongs
                    .Include(x => x.LoaiPhong)
                    .FirstOrDefault(x =>
                        x.PhongId == chiTiet.PhongId &&
                        !x.IsDeleted &&
                        x.TrangThai != TrangThaiPhong.BaoTri);

                if (phong == null)
                    return false;

                bool biTrungLich = context.ChiTietDatPhongs.Any(x =>
                    x.PhongId == chiTiet.PhongId &&
                    x.TrangThai != TrangThaiChiTietDatPhong.DaHuy &&
                    x.TrangThai != TrangThaiChiTietDatPhong.DaTra &&
                    chiTiet.ThoiGianNhanDuKien < x.ThoiGianTraDuKien &&
                    chiTiet.ThoiGianTraDuKien > x.ThoiGianNhanDuKien);

                if (biTrungLich)
                    return false;

                phongCache[phong.PhongId] = phong;
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                datPhong.TrangThai = TrangThaiDatPhong.DangDat;
                context.DatPhongs.Add(datPhong);
                context.SaveChanges();

                foreach (var chiTiet in danhSachChiTiet)
                {
                    var phong = phongCache[chiTiet.PhongId];

                    var chiTietMoi = new ChiTietDatPhong
                    {
                        DatPhongId = datPhong.DatPhongId,
                        PhongId = chiTiet.PhongId,
                        ThoiGianNhanDuKien = chiTiet.ThoiGianNhanDuKien,
                        ThoiGianTraDuKien = chiTiet.ThoiGianTraDuKien,
                        DonGia = chiTiet.DonGia > 0 ? chiTiet.DonGia : phong.LoaiPhong.GiaCoBan,
                        TrangThai = TrangThaiChiTietDatPhong.DaDat,
                        DaLapHoaDon = false
                    };

                    context.ChiTietDatPhongs.Add(chiTietMoi);

                    if (chiTietMoi.ThoiGianNhanDuKien.Date <= DateTime.Today &&
                        phong.TrangThai == TrangThaiPhong.Trong)
                    {
                        phong.TrangThai = TrangThaiPhong.DangDat;
                    }
                }

                context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool HuyChiTietDatPhong(int chiTietDatPhongId)
        {
            var chiTiet = context.ChiTietDatPhongs
                .Include(x => x.DatPhong)
                .Include(x => x.Phong)
                .FirstOrDefault(x => x.ChiTietDatPhongId == chiTietDatPhongId);

            if (chiTiet == null)
                return false;

            if (chiTiet.TrangThai == TrangThaiChiTietDatPhong.DaHuy ||
                chiTiet.TrangThai == TrangThaiChiTietDatPhong.DaTra ||
                chiTiet.TrangThai == TrangThaiChiTietDatPhong.DangO)
            {
                return false;
            }

            bool daNhanPhong = context.ChiTietNhanPhongs
                .Any(x => x.ChiTietDatPhongId == chiTietDatPhongId);

            if (daNhanPhong)
                return false;

            chiTiet.TrangThai = TrangThaiChiTietDatPhong.DaHuy;

            CapNhatTrangThaiPhongSauKhiHuyDat(chiTiet.PhongId, chiTiet.ChiTietDatPhongId);

            context.SaveChanges();

            bool tatCaDaHuy = context.ChiTietDatPhongs
                .Where(x => x.DatPhongId == chiTiet.DatPhongId)
                .All(x => x.TrangThai == TrangThaiChiTietDatPhong.DaHuy);

            if (tatCaDaHuy)
            {
                chiTiet.DatPhong.TrangThai = TrangThaiDatPhong.DaHuy;
                context.SaveChanges();
            }

            return true;
        }

        public bool HuyDatPhong(int datPhongId)
        {
            var datPhong = context.DatPhongs
                .Include(x => x.ChiTietDatPhongs)
                .FirstOrDefault(x => x.DatPhongId == datPhongId);

            if (datPhong == null)
                return false;

            bool daCoPhongNhan = context.ChiTietNhanPhongs.Any(x =>
                datPhong.ChiTietDatPhongs.Select(ct => ct.ChiTietDatPhongId).Contains(x.ChiTietDatPhongId));

            if (daCoPhongNhan)
                return false;

            using var transaction = context.Database.BeginTransaction();

            try
            {
                foreach (var chiTiet in datPhong.ChiTietDatPhongs)
                {
                    chiTiet.TrangThai = TrangThaiChiTietDatPhong.DaHuy;

                    CapNhatTrangThaiPhongSauKhiHuyDat(chiTiet.PhongId, chiTiet.ChiTietDatPhongId);
                }

                datPhong.TrangThai = TrangThaiDatPhong.DaHuy;

                context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        private bool KiemTraChiTietDatPhongHopLe(ChiTietDatPhong chiTiet)
        {
            if (chiTiet == null)
                return false;

            if (chiTiet.PhongId <= 0)
                return false;

            if (chiTiet.ThoiGianNhanDuKien >= chiTiet.ThoiGianTraDuKien)
                return false;

            if (chiTiet.DonGia < 0)
                return false;

            return true;
        }

        private string TaoMaDatPhong()
        {
            return $"DP{DateTime.Now:yyyyMMddHHmmssfff}";
        }

        private void CapNhatTrangThaiPhongSauKhiHuyDat(int phongId, int chiTietDatPhongIdBiHuy)
        {
            var phong = context.Phongs
                .FirstOrDefault(x => x.PhongId == phongId && !x.IsDeleted);

            if (phong == null || phong.TrangThai != TrangThaiPhong.DangDat)
                return;

            bool conDatKhac = context.ChiTietDatPhongs.Any(x =>
                x.PhongId == phongId &&
                x.ChiTietDatPhongId != chiTietDatPhongIdBiHuy &&
                x.TrangThai == TrangThaiChiTietDatPhong.DaDat);

            if (!conDatKhac)
            {
                phong.TrangThai = TrangThaiPhong.Trong;
            }
        }
    }
}

