using Microsoft.EntityFrameworkCore;
using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;

namespace QLKS.Services.Nhom_HoaDon_ThanhToan
{
    public class HoaDonService
    {
        private readonly QLKSDbContext context;

        public HoaDonService()
        {
            context = new QLKSDbContext();
        }

        public List<HoaDon> LayDanhSach()
        {
            return context.HoaDons
                .Include(x => x.DatPhong)
                .Include(x => x.KhachHang)
                .Include(x => x.NhanVien)
                .Include(x => x.ThanhToans)
                .OrderByDescending(x => x.NgayLap)
                .ToList();
        }

        public List<HoaDon> LayDanhSachTheoDatPhong(int datPhongId)
        {
            return context.HoaDons
                .Include(x => x.KhachHang)
                .Include(x => x.NhanVien)
                .Include(x => x.ThanhToans)
                .Where(x => x.DatPhongId == datPhongId)
                .OrderByDescending(x => x.NgayLap)
                .ToList();
        }

        public HoaDon? LayTheoId(int hoaDonId)
        {
            return context.HoaDons
                .Include(x => x.DatPhong)
                    .ThenInclude(x => x.KhachHang)
                .Include(x => x.KhachHang)
                .Include(x => x.NhanVien)
                .Include(x => x.ChiTietHoaDons)
                    .ThenInclude(x => x.ChiTietDatPhong!)
                        .ThenInclude(x => x.Phong)
                            .ThenInclude(x => x.LoaiPhong)
                .Include(x => x.ChiTietHoaDons)
                    .ThenInclude(x => x.SuDungDichVu!)
                        .ThenInclude(x => x.DichVu)
                .Include(x => x.ThanhToans)
                .FirstOrDefault(x => x.HoaDonId == hoaDonId);
        }

        public bool TaoHoaDon(int datPhongId, int nhanVienId)
        {
            bool nhanVienHopLe = context.NhanViens
                .Any(x => x.NhanVienId == nhanVienId && x.TrangThaiLamViec == TrangThaiLamViec.DangLam);

            if (!nhanVienHopLe)
                return false;

            var datPhong = context.DatPhongs
                .Include(x => x.KhachHang)
                    .ThenInclude(x => x.LoaiKhachHang)
                .Include(x => x.ChiTietDatPhongs)
                    .ThenInclude(x => x.Phong)
                        .ThenInclude(x => x.LoaiPhong)
                .Include(x => x.ChiTietDatPhongs)
                    .ThenInclude(x => x.ChiTietNhanPhongs)
                        .ThenInclude(x => x.ChiTietTraPhongs)
                .Include(x => x.ChiTietDatPhongs)
                    .ThenInclude(x => x.SuDungDichVus)
                        .ThenInclude(x => x.DichVu)
                .FirstOrDefault(x => x.DatPhongId == datPhongId);

            if (datPhong == null)
                return false;

            var danhSachChiTietDaTra = datPhong.ChiTietDatPhongs
                .Where(x => x.TrangThai == TrangThaiChiTietDatPhong.DaTra)
                .ToList();

            var danhSachChiTietPhongChuaLapHoaDon = danhSachChiTietDaTra
                .Where(x => !x.DaLapHoaDon)
                .ToList();

            var danhSachSuDungDichVuChuaLapHoaDon = danhSachChiTietDaTra
                .SelectMany(x => x.SuDungDichVus)
                .Where(x => !x.DaLapHoaDon)
                .ToList();

            if (danhSachChiTietPhongChuaLapHoaDon.Count == 0 &&
                danhSachSuDungDichVuChuaLapHoaDon.Count == 0)
            {
                return false;
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                var hoaDon = new HoaDon
                {
                    MaHoaDon = TaoMaHoaDonKhongTrung(),
                    DatPhongId = datPhong.DatPhongId,
                    KhachHangId = datPhong.KhachHangId,
                    NhanVienId = nhanVienId,
                    NgayLap = DateTime.Now,
                    TongThanhToan = 0,
                    TrangThai = TrangThaiHoaDon.ChuaThanhToan
                };

                context.HoaDons.Add(hoaDon);
                context.SaveChanges();

                decimal tongThanhToan = 0;

                foreach (var chiTietDatPhong in danhSachChiTietPhongChuaLapHoaDon)
                {
                    int soLuong = TinhSoDemLuuTru(chiTietDatPhong);
                    decimal thanhTien = soLuong * chiTietDatPhong.DonGia;

                    context.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        HoaDonId = hoaDon.HoaDonId,
                        ChiTietDatPhongId = chiTietDatPhong.ChiTietDatPhongId,
                        SuDungDichVuId = null,
                        NoiDung = $"Tien phong {chiTietDatPhong.Phong.SoPhong}",
                        SoLuong = soLuong,
                        DonGia = chiTietDatPhong.DonGia,
                        ThanhTien = thanhTien
                    });

                    chiTietDatPhong.DaLapHoaDon = true;
                    tongThanhToan += thanhTien;
                }

                foreach (var suDungDichVu in danhSachSuDungDichVuChuaLapHoaDon)
                {
                    decimal thanhTien = suDungDichVu.SoLuong * suDungDichVu.DonGia;

                    context.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        HoaDonId = hoaDon.HoaDonId,
                        ChiTietDatPhongId = null,
                        SuDungDichVuId = suDungDichVu.SuDungDichVuId,
                        NoiDung = $"Dich vu {suDungDichVu.DichVu.TenDichVu}",
                        SoLuong = suDungDichVu.SoLuong,
                        DonGia = suDungDichVu.DonGia,
                        ThanhTien = thanhTien
                    });

                    suDungDichVu.DaLapHoaDon = true;
                    tongThanhToan += thanhTien;
                }

                decimal tyLeUuDai = datPhong.KhachHang.LoaiKhachHang?.TyLeUuDai ?? 0;
                if (tyLeUuDai > 0 && tongThanhToan > 0)
                {
                    decimal soTienUuDai = Math.Round(
                        tongThanhToan * tyLeUuDai / 100m,
                        2,
                        MidpointRounding.AwayFromZero);

                    if (soTienUuDai > tongThanhToan)
                    {
                        soTienUuDai = tongThanhToan;
                    }

                    if (soTienUuDai > 0)
                    {
                        context.ChiTietHoaDons.Add(new ChiTietHoaDon
                        {
                            HoaDonId = hoaDon.HoaDonId,
                            ChiTietDatPhongId = null,
                            SuDungDichVuId = null,
                            NoiDung = $"Uu dai {datPhong.KhachHang.LoaiKhachHang!.TenLoaiKhachHang} ({tyLeUuDai:0.##}%)",
                            SoLuong = 1,
                            DonGia = -soTienUuDai,
                            ThanhTien = -soTienUuDai
                        });

                        tongThanhToan -= soTienUuDai;
                    }
                }

                decimal soTienCocApDung = datPhong.TienCoc > tongThanhToan
                    ? tongThanhToan
                    : datPhong.TienCoc;

                if (soTienCocApDung > 0)
                {
                    context.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        HoaDonId = hoaDon.HoaDonId,
                        ChiTietDatPhongId = null,
                        SuDungDichVuId = null,
                        NoiDung = "Tru tien coc",
                        SoLuong = 1,
                        DonGia = -soTienCocApDung,
                        ThanhTien = -soTienCocApDung
                    });

                    tongThanhToan -= soTienCocApDung;
                }

                hoaDon.TongThanhToan = tongThanhToan;
                hoaDon.TrangThai = hoaDon.TongThanhToan == 0
                    ? TrangThaiHoaDon.ThanhToanThanhCong
                    : TrangThaiHoaDon.ChuaThanhToan;

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

        public bool HuyHoaDon(int hoaDonId)
        {
            var hoaDon = context.HoaDons
                .Include(x => x.ChiTietHoaDons)
                .Include(x => x.ThanhToans)
                .FirstOrDefault(x => x.HoaDonId == hoaDonId);

            if (hoaDon == null || hoaDon.TrangThai == TrangThaiHoaDon.DaHuy)
                return false;

            bool daCoThanhToan = hoaDon.ThanhToans.Any(x => x.SoTien > 0);

            if (daCoThanhToan)
                return false;

            using var transaction = context.Database.BeginTransaction();

            try
            {
                foreach (var chiTietHoaDon in hoaDon.ChiTietHoaDons)
                {
                    if (chiTietHoaDon.ChiTietDatPhongId.HasValue)
                    {
                        var chiTietDatPhong = context.ChiTietDatPhongs
                            .FirstOrDefault(x => x.ChiTietDatPhongId == chiTietHoaDon.ChiTietDatPhongId.Value);

                        if (chiTietDatPhong != null)
                        {
                            chiTietDatPhong.DaLapHoaDon = false;
                        }
                    }

                    if (chiTietHoaDon.SuDungDichVuId.HasValue)
                    {
                        var suDungDichVu = context.SuDungDichVus
                            .FirstOrDefault(x => x.SuDungDichVuId == chiTietHoaDon.SuDungDichVuId.Value);

                        if (suDungDichVu != null)
                        {
                            suDungDichVu.DaLapHoaDon = false;
                        }
                    }
                }

                hoaDon.TrangThai = TrangThaiHoaDon.DaHuy;
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

        public decimal TinhTongDaThanhToan(int hoaDonId)
        {
            return context.ThanhToans
                .Where(x => x.HoaDonId == hoaDonId)
                .Sum(x => x.SoTien);
        }

        public decimal TinhSoTienConLai(int hoaDonId)
        {
            var hoaDon = context.HoaDons
                .FirstOrDefault(x => x.HoaDonId == hoaDonId);

            if (hoaDon == null || hoaDon.TrangThai == TrangThaiHoaDon.DaHuy)
                return 0;

            decimal soTienConLai = hoaDon.TongThanhToan - TinhTongDaThanhToan(hoaDonId);
            return soTienConLai > 0 ? soTienConLai : 0;
        }

        private int TinhSoDemLuuTru(ChiTietDatPhong chiTietDatPhong)
        {
            DateTime thoiGianBatDau = chiTietDatPhong.ChiTietNhanPhongs
                .OrderByDescending(x => x.ThoiGianNhanThucTe)
                .Select(x => (DateTime?)x.ThoiGianNhanThucTe)
                .FirstOrDefault() ?? chiTietDatPhong.ThoiGianNhanDuKien;

            DateTime thoiGianKetThuc = chiTietDatPhong.ChiTietNhanPhongs
                .SelectMany(x => x.ChiTietTraPhongs)
                .OrderByDescending(x => x.ThoiGianTraThucTe)
                .Select(x => (DateTime?)x.ThoiGianTraThucTe)
                .FirstOrDefault() ?? chiTietDatPhong.ThoiGianTraDuKien;

            if (thoiGianKetThuc <= thoiGianBatDau)
                return 1;

            return Math.Max(1, (int)Math.Ceiling((thoiGianKetThuc - thoiGianBatDau).TotalDays));
        }

        private string TaoMaHoaDonKhongTrung()
        {
            string maHoaDon;

            do
            {
                maHoaDon = $"HD{DateTime.Now:yyyyMMddHHmmssfff}";
            }
            while (context.HoaDons.Any(x => x.MaHoaDon == maHoaDon));

            return maHoaDon;
        }
    }
}
