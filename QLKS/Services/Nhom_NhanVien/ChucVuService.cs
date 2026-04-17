using QLKS.Data.Entities.Nhom_NhanVien;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using Microsoft.EntityFrameworkCore;
namespace QLKS.Services.Nhom_NhanVien
{
    public class ChucVuService
    {
        private readonly QLKSDbContext context;

        public ChucVuService()
        {
            context = new QLKSDbContext();
        }

        public List<ChucVu> LayDanhSach()
        {
            return context.ChucVus
                .OrderBy(x => x.TenChucVu)
                .ToList();
        }

        public List<ChucVu> LayDanhSachChoComboBox()
        {
            return context.ChucVus
                .OrderBy(x => x.TenChucVu)
                .ToList();
        }

        public ChucVu? LayTheoId(int chucVuId)
        {
            return context.ChucVus
                .FirstOrDefault(x => x.ChucVuId == chucVuId);
        }

        public bool ThemChucVu(ChucVu chucVu)
        {
            if (chucVu == null)
                return false;

            string tenChucVu = chucVu.TenChucVu?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tenChucVu))
                return false;

            bool daTonTai = context.ChucVus
                .Any(x => x.TenChucVu.ToLower() == tenChucVu.ToLower());

            if (daTonTai)
                return false;

            chucVu.TenChucVu = tenChucVu;

            if (!string.IsNullOrWhiteSpace(chucVu.MoTa))
                chucVu.MoTa = chucVu.MoTa.Trim();

            context.ChucVus.Add(chucVu);
            return context.SaveChanges() > 0;
        }

        public bool SuaChucVu(ChucVu chucVu)
        {
            if (chucVu == null)
                return false;

            string tenChucVu = chucVu.TenChucVu?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tenChucVu))
                return false;

            var existing = context.ChucVus
                .FirstOrDefault(x => x.ChucVuId == chucVu.ChucVuId);

            if (existing == null)
                return false;

            bool biTrung = context.ChucVus
                .Any(x => x.TenChucVu.ToLower() == tenChucVu.ToLower()
                       && x.ChucVuId != chucVu.ChucVuId);

            if (biTrung)
                return false;

            existing.TenChucVu = tenChucVu;
            existing.MoTa = string.IsNullOrWhiteSpace(chucVu.MoTa)
                ? null
                : chucVu.MoTa.Trim();

            return context.SaveChanges() > 0;
        }

        public bool XoaChucVu(int chucVuId)
        {
            var existing = context.ChucVus
                .FirstOrDefault(x => x.ChucVuId == chucVuId);

            if (existing == null)
                return false;

            bool dangDuocSuDung = context.NhanViens
                .Any(x => x.ChucVuId == chucVuId);

            if (dangDuocSuDung)
                return false;

            context.ChucVus.Remove(existing);
            return context.SaveChanges() > 0;
        }
    }
}
