using Microsoft.EntityFrameworkCore;
using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_DichVu;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;

namespace QLKS.Services.Nhom_DichVu
{
    public class SuDungDichVuService
    {
        private readonly QLKSDbContext context;

        public SuDungDichVuService()
        {
            context = new QLKSDbContext();
        }

        public List<SuDungDichVu> LayDanhSach()
        {
            return context.SuDungDichVus
                .Include(x => x.DichVu)
                .Include(x => x.ChiTietDatPhong)
                    .ThenInclude(x => x.DatPhong)
                        .ThenInclude(x => x.KhachHang)
                .Include(x => x.ChiTietDatPhong)
                    .ThenInclude(x => x.Phong)
                .OrderByDescending(x => x.ThoiGianSuDung)
                .ToList();
        }

        public List<SuDungDichVu> LayDanhSachTheoChiTietDatPhong(int chiTietDatPhongId)
        {
            return context.SuDungDichVus
                .Include(x => x.DichVu)
                .Include(x => x.ChiTietDatPhong)
                    .ThenInclude(x => x.Phong)
                .Where(x => x.ChiTietDatPhongId == chiTietDatPhongId)
                .OrderByDescending(x => x.ThoiGianSuDung)
                .ToList();
        }

        public SuDungDichVu? LayTheoId(int suDungDichVuId)
        {
            return context.SuDungDichVus
                .Include(x => x.DichVu)
                .Include(x => x.ChiTietDatPhong)
                    .ThenInclude(x => x.DatPhong)
                        .ThenInclude(x => x.KhachHang)
                .Include(x => x.ChiTietDatPhong)
                    .ThenInclude(x => x.Phong)
                .FirstOrDefault(x => x.SuDungDichVuId == suDungDichVuId);
        }

        public bool ThemSuDungDichVu(SuDungDichVu suDungDichVu)
        {
            if (suDungDichVu == null || suDungDichVu.SoLuong <= 0)
                return false;

            DateTime thoiGianSuDung = suDungDichVu.ThoiGianSuDung == default
                ? DateTime.Now
                : suDungDichVu.ThoiGianSuDung;

            var dichVu = context.DichVus
                .FirstOrDefault(x => x.DichVuId == suDungDichVu.DichVuId && x.ConSuDung);

            if (dichVu == null)
                return false;

            var chiTietDatPhong = LayChiTietDatPhongPhucVuDichVu(suDungDichVu.ChiTietDatPhongId);

            if (!KiemTraCoTheGhiNhanSuDungDichVu(chiTietDatPhong, thoiGianSuDung, true))
                return false;

            decimal donGia = suDungDichVu.DonGia > 0 ? suDungDichVu.DonGia : dichVu.DonGia;

            if (donGia < 0)
                return false;

            suDungDichVu.DonGia = donGia;
            suDungDichVu.ThoiGianSuDung = thoiGianSuDung;
            suDungDichVu.DaLapHoaDon = false;

            context.SuDungDichVus.Add(suDungDichVu);
            return context.SaveChanges() > 0;
        }

        public bool SuaSuDungDichVu(SuDungDichVu suDungDichVu)
        {
            if (suDungDichVu == null || suDungDichVu.SoLuong <= 0)
                return false;

            var existing = context.SuDungDichVus
                .FirstOrDefault(x => x.SuDungDichVuId == suDungDichVu.SuDungDichVuId);

            if (existing == null || existing.DaLapHoaDon)
                return false;

            DateTime thoiGianSuDung = suDungDichVu.ThoiGianSuDung == default
                ? existing.ThoiGianSuDung
                : suDungDichVu.ThoiGianSuDung;

            var dichVu = context.DichVus
                .FirstOrDefault(x => x.DichVuId == suDungDichVu.DichVuId);

            if (dichVu == null)
                return false;

            if (!dichVu.ConSuDung && existing.DichVuId != suDungDichVu.DichVuId)
                return false;

            var chiTietDatPhong = LayChiTietDatPhongPhucVuDichVu(suDungDichVu.ChiTietDatPhongId);

            if (!KiemTraCoTheGhiNhanSuDungDichVu(chiTietDatPhong, thoiGianSuDung, false))
                return false;

            decimal donGia = suDungDichVu.DonGia > 0 ? suDungDichVu.DonGia : dichVu.DonGia;

            if (donGia < 0)
                return false;

            existing.ChiTietDatPhongId = suDungDichVu.ChiTietDatPhongId;
            existing.DichVuId = suDungDichVu.DichVuId;
            existing.SoLuong = suDungDichVu.SoLuong;
            existing.DonGia = donGia;
            existing.ThoiGianSuDung = thoiGianSuDung;

            return context.SaveChanges() > 0;
        }

        public bool XoaSuDungDichVu(int suDungDichVuId)
        {
            var existing = context.SuDungDichVus
                .FirstOrDefault(x => x.SuDungDichVuId == suDungDichVuId);

            if (existing == null || existing.DaLapHoaDon)
                return false;

            context.SuDungDichVus.Remove(existing);
            return context.SaveChanges() > 0;
        }

        private ChiTietDatPhong? LayChiTietDatPhongPhucVuDichVu(int chiTietDatPhongId)
        {
            return context.ChiTietDatPhongs
                .Include(x => x.ChiTietNhanPhongs)
                    .ThenInclude(x => x.ChiTietTraPhongs)
                .FirstOrDefault(x => x.ChiTietDatPhongId == chiTietDatPhongId);
        }

        private bool KiemTraCoTheGhiNhanSuDungDichVu(
            ChiTietDatPhong? chiTietDatPhong,
            DateTime thoiGianSuDung,
            bool yeuCauDangO)
        {
            if (chiTietDatPhong == null)
                return false;

            if (chiTietDatPhong.TrangThai == TrangThaiChiTietDatPhong.DaDat ||
                chiTietDatPhong.TrangThai == TrangThaiChiTietDatPhong.DaHuy)
            {
                return false;
            }

            if (yeuCauDangO && chiTietDatPhong.TrangThai != TrangThaiChiTietDatPhong.DangO)
                return false;

            var chiTietNhanPhong = chiTietDatPhong.ChiTietNhanPhongs
                .OrderByDescending(x => x.ThoiGianNhanThucTe)
                .FirstOrDefault();

            if (chiTietNhanPhong == null)
                return false;

            if (thoiGianSuDung < chiTietNhanPhong.ThoiGianNhanThucTe)
                return false;

            DateTime? thoiGianTraThucTe = chiTietNhanPhong.ChiTietTraPhongs
                .OrderByDescending(x => x.ThoiGianTraThucTe)
                .Select(x => (DateTime?)x.ThoiGianTraThucTe)
                .FirstOrDefault();

            if (thoiGianTraThucTe.HasValue && thoiGianSuDung > thoiGianTraThucTe.Value)
                return false;

            return true;
        }
    }
}
