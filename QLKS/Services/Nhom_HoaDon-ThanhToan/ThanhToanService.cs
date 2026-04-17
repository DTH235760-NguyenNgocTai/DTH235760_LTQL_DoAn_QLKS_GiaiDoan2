using Microsoft.EntityFrameworkCore;
using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;

namespace QLKS.Services.Nhom_HoaDon_ThanhToan
{
    public class ThanhToanService
    {
        private readonly QLKSDbContext context;

        public ThanhToanService()
        {
            context = new QLKSDbContext();
        }

        public List<ThanhToan> LayDanhSach()
        {
            return context.ThanhToans
                .Include(x => x.HoaDon)
                    .ThenInclude(x => x.KhachHang)
                .OrderByDescending(x => x.ThoiGianThanhToan)
                .ToList();
        }

        public List<ThanhToan> LayDanhSachTheoHoaDon(int hoaDonId)
        {
            return context.ThanhToans
                .Include(x => x.HoaDon)
                .Where(x => x.HoaDonId == hoaDonId)
                .OrderByDescending(x => x.ThoiGianThanhToan)
                .ToList();
        }

        public ThanhToan? LayTheoId(int thanhToanId)
        {
            return context.ThanhToans
                .Include(x => x.HoaDon)
                    .ThenInclude(x => x.KhachHang)
                .FirstOrDefault(x => x.ThanhToanId == thanhToanId);
        }

        public bool ThemThanhToan(ThanhToan thanhToan)
        {
            if (thanhToan == null || thanhToan.SoTien <= 0)
                return false;

            var hoaDon = context.HoaDons
                .Include(x => x.ThanhToans)
                .FirstOrDefault(x => x.HoaDonId == thanhToan.HoaDonId);

            if (hoaDon == null || hoaDon.TrangThai == TrangThaiHoaDon.DaHuy)
                return false;

            decimal tongDaThanhToan = hoaDon.ThanhToans.Sum(x => x.SoTien);
            decimal soTienConLai = hoaDon.TongThanhToan - tongDaThanhToan;

            if (soTienConLai <= 0 || thanhToan.SoTien > soTienConLai)
                return false;

            thanhToan.ThoiGianThanhToan = thanhToan.ThoiGianThanhToan == default
                ? DateTime.Now
                : thanhToan.ThoiGianThanhToan;
            thanhToan.MaGiaoDich = string.IsNullOrWhiteSpace(thanhToan.MaGiaoDich)
                ? null
                : thanhToan.MaGiaoDich.Trim();
            thanhToan.GhiChu = string.IsNullOrWhiteSpace(thanhToan.GhiChu)
                ? null
                : thanhToan.GhiChu.Trim();

            context.ThanhToans.Add(thanhToan);
            CapNhatTrangThaiHoaDonSauThanhToan(hoaDon, thanhToan.SoTien);

            return context.SaveChanges() > 0;
        }

        public bool SuaThanhToan(ThanhToan thanhToan)
        {
            if (thanhToan == null || thanhToan.SoTien <= 0)
                return false;

            var existing = context.ThanhToans
                .FirstOrDefault(x => x.ThanhToanId == thanhToan.ThanhToanId);

            if (existing == null)
                return false;

            var hoaDon = context.HoaDons
                .Include(x => x.ThanhToans)
                .FirstOrDefault(x => x.HoaDonId == existing.HoaDonId);

            if (hoaDon == null || hoaDon.TrangThai == TrangThaiHoaDon.DaHuy)
                return false;

            decimal tongKhac = hoaDon.ThanhToans
                .Where(x => x.ThanhToanId != existing.ThanhToanId)
                .Sum(x => x.SoTien);

            decimal soTienConLai = hoaDon.TongThanhToan - tongKhac;

            if (soTienConLai <= 0 || thanhToan.SoTien > soTienConLai)
                return false;

            existing.SoTien = thanhToan.SoTien;
            existing.ThoiGianThanhToan = thanhToan.ThoiGianThanhToan == default
                ? existing.ThoiGianThanhToan
                : thanhToan.ThoiGianThanhToan;
            existing.PhuongThucThanhToan = thanhToan.PhuongThucThanhToan;
            existing.MaGiaoDich = string.IsNullOrWhiteSpace(thanhToan.MaGiaoDich)
                ? null
                : thanhToan.MaGiaoDich.Trim();
            existing.GhiChu = string.IsNullOrWhiteSpace(thanhToan.GhiChu)
                ? null
                : thanhToan.GhiChu.Trim();

            CapNhatTrangThaiHoaDonSauThanhToan(hoaDon);

            return context.SaveChanges() > 0;
        }

        public bool XoaThanhToan(int thanhToanId)
        {
            var existing = context.ThanhToans
                .FirstOrDefault(x => x.ThanhToanId == thanhToanId);

            if (existing == null)
                return false;

            var hoaDon = context.HoaDons
                .Include(x => x.ThanhToans)
                .FirstOrDefault(x => x.HoaDonId == existing.HoaDonId);

            if (hoaDon == null || hoaDon.TrangThai == TrangThaiHoaDon.DaHuy)
                return false;

            context.ThanhToans.Remove(existing);
            CapNhatTrangThaiHoaDonSauThanhToan(hoaDon, -existing.SoTien);

            return context.SaveChanges() > 0;
        }

        private void CapNhatTrangThaiHoaDonSauThanhToan(HoaDon hoaDon, decimal phatSinh = 0)
        {
            decimal tongDaThanhToan = hoaDon.ThanhToans.Sum(x => x.SoTien) + phatSinh;

            if (tongDaThanhToan <= 0)
            {
                hoaDon.TrangThai = TrangThaiHoaDon.ChuaThanhToan;
                return;
            }

            if (tongDaThanhToan < hoaDon.TongThanhToan)
            {
                hoaDon.TrangThai = TrangThaiHoaDon.ThanhToanMotPhan;
                return;
            }

            hoaDon.TrangThai = TrangThaiHoaDon.ThanhToanThanhCong;
        }
    }
}
