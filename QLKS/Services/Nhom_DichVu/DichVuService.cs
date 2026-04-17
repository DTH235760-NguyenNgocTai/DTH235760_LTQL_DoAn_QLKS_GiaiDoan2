using QLKS.Data.Entities.Nhom_DichVu;
using QLKS.Data.QLKSDbContext;

namespace QLKS.Services.Nhom_DichVu
{
    public class DichVuService
    {
        private readonly QLKSDbContext context;

        public DichVuService()
        {
            context = new QLKSDbContext();
        }

        public List<DichVu> LayDanhSach()
        {
            return context.DichVus
                .OrderBy(x => x.TenDichVu)
                .ToList();
        }

        public List<DichVu> LayDanhSachChoComboBox()
        {
            return context.DichVus
                .Where(x => x.ConSuDung)
                .OrderBy(x => x.TenDichVu)
                .ToList();
        }

        public DichVu? LayTheoId(int dichVuId)
        {
            return context.DichVus
                .FirstOrDefault(x => x.DichVuId == dichVuId);
        }

        public List<DichVu> TimKiemDichVu(string? tuKhoa = null, bool? conSuDung = null)
        {
            var query = context.DichVus.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                query = query.Where(x =>
                    x.MaDichVu.Contains(tuKhoa) ||
                    x.TenDichVu.Contains(tuKhoa) ||
                    x.DonViTinh.Contains(tuKhoa) ||
                    (x.MoTa != null && x.MoTa.Contains(tuKhoa)));
            }

            if (conSuDung.HasValue)
            {
                query = query.Where(x => x.ConSuDung == conSuDung.Value);
            }

            return query
                .OrderBy(x => x.TenDichVu)
                .ToList();
        }

        public bool ThemDichVu(DichVu dichVu)
        {
            if (dichVu == null)
                return false;

            string maDichVu = dichVu.MaDichVu?.Trim() ?? string.Empty;
            string tenDichVu = dichVu.TenDichVu?.Trim() ?? string.Empty;
            string donViTinh = dichVu.DonViTinh?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(maDichVu) ||
                string.IsNullOrWhiteSpace(tenDichVu) ||
                string.IsNullOrWhiteSpace(donViTinh) ||
                dichVu.DonGia < 0)
            {
                return false;
            }

            bool trungMa = context.DichVus
                .Any(x => x.MaDichVu.ToLower() == maDichVu.ToLower());

            if (trungMa)
                return false;

            bool trungTen = context.DichVus
                .Any(x => x.TenDichVu.ToLower() == tenDichVu.ToLower());

            if (trungTen)
                return false;

            dichVu.MaDichVu = maDichVu;
            dichVu.TenDichVu = tenDichVu;
            dichVu.DonViTinh = donViTinh;
            dichVu.MoTa = string.IsNullOrWhiteSpace(dichVu.MoTa)
                ? null
                : dichVu.MoTa.Trim();

            context.DichVus.Add(dichVu);
            return context.SaveChanges() > 0;
        }

        public bool SuaDichVu(DichVu dichVu)
        {
            if (dichVu == null)
                return false;

            string maDichVu = dichVu.MaDichVu?.Trim() ?? string.Empty;
            string tenDichVu = dichVu.TenDichVu?.Trim() ?? string.Empty;
            string donViTinh = dichVu.DonViTinh?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(maDichVu) ||
                string.IsNullOrWhiteSpace(tenDichVu) ||
                string.IsNullOrWhiteSpace(donViTinh) ||
                dichVu.DonGia < 0)
            {
                return false;
            }

            var existing = context.DichVus
                .FirstOrDefault(x => x.DichVuId == dichVu.DichVuId);

            if (existing == null)
                return false;

            bool trungMa = context.DichVus
                .Any(x => x.MaDichVu.ToLower() == maDichVu.ToLower() &&
                          x.DichVuId != dichVu.DichVuId);

            if (trungMa)
                return false;

            bool trungTen = context.DichVus
                .Any(x => x.TenDichVu.ToLower() == tenDichVu.ToLower() &&
                          x.DichVuId != dichVu.DichVuId);

            if (trungTen)
                return false;

            existing.MaDichVu = maDichVu;
            existing.TenDichVu = tenDichVu;
            existing.DonGia = dichVu.DonGia;
            existing.DonViTinh = donViTinh;
            existing.ConSuDung = dichVu.ConSuDung;
            existing.MoTa = string.IsNullOrWhiteSpace(dichVu.MoTa)
                ? null
                : dichVu.MoTa.Trim();

            return context.SaveChanges() > 0;
        }

        public bool CapNhatTrangThaiSuDung(int dichVuId, bool conSuDung)
        {
            var existing = context.DichVus
                .FirstOrDefault(x => x.DichVuId == dichVuId);

            if (existing == null)
                return false;

            existing.ConSuDung = conSuDung;
            return context.SaveChanges() > 0;
        }

        public bool XoaDichVu(int dichVuId)
        {
            var existing = context.DichVus
                .FirstOrDefault(x => x.DichVuId == dichVuId);

            if (existing == null)
                return false;

            bool dangDuocSuDung = context.SuDungDichVus
                .Any(x => x.DichVuId == dichVuId);

            if (dangDuocSuDung)
                return false;

            context.DichVus.Remove(existing);
            return context.SaveChanges() > 0;
        }
    }
}
