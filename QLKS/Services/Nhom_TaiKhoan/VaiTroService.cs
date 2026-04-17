using QLKS.Data.Entities.Nhom_TaiKhoan;
using QLKS.Data.QLKSDbContext;

namespace QLKS.Services.Nhom_TaiKhoan
{
    public class VaiTroService
    {
        private readonly QLKSDbContext context;

        public VaiTroService()
        {
            context = new QLKSDbContext();
        }

        public List<VaiTro> LayDanhSach()
        {
            return context.VaiTros
                .OrderBy(x => x.TenVaiTro)
                .ToList();
        }

        public List<VaiTro> LayDanhSachChoComboBox()
        {
            return context.VaiTros
                .OrderBy(x => x.TenVaiTro)
                .ToList();
        }

        public VaiTro? LayTheoId(int vaiTroId)
        {
            return context.VaiTros
                .FirstOrDefault(x => x.VaiTroId == vaiTroId);
        }

        public bool ThemVaiTro(VaiTro vaiTro)
        {
            if (vaiTro == null)
                return false;

            string tenVaiTro = vaiTro.TenVaiTro?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenVaiTro))
                return false;

            bool daTonTai = context.VaiTros
                .Any(x => x.TenVaiTro.ToLower() == tenVaiTro.ToLower());

            if (daTonTai)
                return false;

            vaiTro.TenVaiTro = tenVaiTro;
            vaiTro.MoTa = string.IsNullOrWhiteSpace(vaiTro.MoTa)
                ? null
                : vaiTro.MoTa.Trim();

            context.VaiTros.Add(vaiTro);
            return context.SaveChanges() > 0;
        }

        public bool SuaVaiTro(VaiTro vaiTro)
        {
            if (vaiTro == null)
                return false;

            string tenVaiTro = vaiTro.TenVaiTro?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenVaiTro))
                return false;

            var existing = context.VaiTros
                .FirstOrDefault(x => x.VaiTroId == vaiTro.VaiTroId);

            if (existing == null)
                return false;

            bool biTrung = context.VaiTros
                .Any(x => x.TenVaiTro.ToLower() == tenVaiTro.ToLower() &&
                          x.VaiTroId != vaiTro.VaiTroId);

            if (biTrung)
                return false;

            existing.TenVaiTro = tenVaiTro;
            existing.MoTa = string.IsNullOrWhiteSpace(vaiTro.MoTa)
                ? null
                : vaiTro.MoTa.Trim();

            return context.SaveChanges() > 0;
        }

        public bool XoaVaiTro(int vaiTroId)
        {
            var existing = context.VaiTros
                .FirstOrDefault(x => x.VaiTroId == vaiTroId);

            if (existing == null)
                return false;

            bool dangDuocSuDung = context.TaiKhoans
                .Any(x => x.VaiTroId == vaiTroId);

            if (dangDuocSuDung)
                return false;

            context.VaiTros.Remove(existing);
            return context.SaveChanges() > 0;
        }
    }
}
