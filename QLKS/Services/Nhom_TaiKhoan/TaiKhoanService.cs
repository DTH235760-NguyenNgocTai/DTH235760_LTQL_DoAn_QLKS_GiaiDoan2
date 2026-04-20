using Microsoft.EntityFrameworkCore;
using QLKS.Data.Entities.Nhom_TaiKhoan;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using QLKS.Services.Nhom_NhanVien;

namespace QLKS.Services.Nhom_TaiKhoan
{
    public class TaiKhoanService
    {
        private const int BCryptWorkFactor = 12;
        private readonly QLKSDbContext context;

        public TaiKhoanService()
        {
            context = new QLKSDbContext();
        }

        public List<TaiKhoan> LayDanhSach()
        {
            KhoiTaoTaiKhoanMacDinhNeuCan();
            return context.TaiKhoans
                .Include(x => x.NhanVien)
                    .ThenInclude(x => x.ChucVu)
                .Include(x => x.VaiTro)
                .OrderBy(x => x.TenDangNhap)
                .ToList();
        }

        public List<TaiKhoan> LayDanhSachHoatDong()
        {
            KhoiTaoTaiKhoanMacDinhNeuCan();
            return context.TaiKhoans
                .Include(x => x.NhanVien)
                    .ThenInclude(x => x.ChucVu)
                .Include(x => x.VaiTro)
                .Where(x => x.TrangThai == TrangThaiTaiKhoan.HoatDong)
                .OrderBy(x => x.TenDangNhap)
                .ToList();
        }

        public TaiKhoan? LayTheoId(int taiKhoanId)
        {
            return context.TaiKhoans
                .Include(x => x.NhanVien)
                    .ThenInclude(x => x.ChucVu)
                .Include(x => x.VaiTro)
                .FirstOrDefault(x => x.TaiKhoanId == taiKhoanId);
        }

        public TaiKhoan? LayTheoTenDangNhap(string tenDangNhap)
        {
            tenDangNhap = tenDangNhap?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenDangNhap))
                return null;

            return context.TaiKhoans
                .Include(x => x.NhanVien)
                .Include(x => x.VaiTro)
                .FirstOrDefault(x => x.TenDangNhap.ToLower() == tenDangNhap.ToLower());
        }

        public List<TaiKhoan> TimKiemTaiKhoan(
            string? tuKhoa = null,
            int? vaiTroId = null,
            TrangThaiTaiKhoan? trangThai = null)
        {
            var query = context.TaiKhoans
                .Include(x => x.NhanVien)
                    .ThenInclude(x => x.ChucVu)
                .Include(x => x.VaiTro)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                query = query.Where(x =>
                    x.TenDangNhap.Contains(tuKhoa) ||
                    x.NhanVien.MaNhanVien.Contains(tuKhoa) ||
                    x.NhanVien.HoTen.Contains(tuKhoa) ||
                    x.VaiTro.TenVaiTro.Contains(tuKhoa));
            }

            if (vaiTroId.HasValue)
            {
                query = query.Where(x => x.VaiTroId == vaiTroId.Value);
            }

            if (trangThai.HasValue)
            {
                query = query.Where(x => x.TrangThai == trangThai.Value);
            }

            return query
                .OrderBy(x => x.TenDangNhap)
                .ToList();
        }

        public bool ThemTaiKhoan(TaiKhoan taiKhoan)
        {
            if (taiKhoan == null)
                return false;

            string tenDangNhap = taiKhoan.TenDangNhap?.Trim() ?? string.Empty;
            string matKhau = taiKhoan.MatKhau?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
                return false;

            bool tenDangNhapBiTrung = context.TaiKhoans
                .Any(x => x.TenDangNhap.ToLower() == tenDangNhap.ToLower());

            if (tenDangNhapBiTrung)
                return false;

            bool nhanVienHopLe = context.NhanViens
                .Any(x => x.NhanVienId == taiKhoan.NhanVienId &&
                          x.TrangThaiLamViec == TrangThaiLamViec.DangLam);

            if (!nhanVienHopLe)
                return false;

            bool nhanVienDaCoTaiKhoan = context.TaiKhoans
                .Any(x => x.NhanVienId == taiKhoan.NhanVienId);

            if (nhanVienDaCoTaiKhoan)
                return false;

            bool vaiTroHopLe = context.VaiTros
                .Any(x => x.VaiTroId == taiKhoan.VaiTroId);

            if (!vaiTroHopLe)
                return false;

            taiKhoan.TenDangNhap = tenDangNhap;
            taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhau, workFactor: BCryptWorkFactor);
            taiKhoan.GhiChu = string.IsNullOrWhiteSpace(taiKhoan.GhiChu)
                ? null
                : taiKhoan.GhiChu.Trim();

            context.TaiKhoans.Add(taiKhoan);
            return context.SaveChanges() > 0;
        }

        public bool SuaTaiKhoan(TaiKhoan taiKhoan)
        {
            if (taiKhoan == null)
                return false;

            string tenDangNhap = taiKhoan.TenDangNhap?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenDangNhap))
                return false;

            var existing = context.TaiKhoans
                .FirstOrDefault(x => x.TaiKhoanId == taiKhoan.TaiKhoanId);

            if (existing == null)
                return false;

            bool tenDangNhapBiTrung = context.TaiKhoans
                .Any(x => x.TenDangNhap.ToLower() == tenDangNhap.ToLower() &&
                          x.TaiKhoanId != taiKhoan.TaiKhoanId);

            if (tenDangNhapBiTrung)
                return false;

            bool nhanVienHopLe = context.NhanViens
                .Any(x => x.NhanVienId == taiKhoan.NhanVienId &&
                          x.TrangThaiLamViec == TrangThaiLamViec.DangLam);

            if (!nhanVienHopLe)
                return false;

            bool nhanVienDaCoTaiKhoanKhac = context.TaiKhoans
                .Any(x => x.NhanVienId == taiKhoan.NhanVienId &&
                          x.TaiKhoanId != taiKhoan.TaiKhoanId);

            if (nhanVienDaCoTaiKhoanKhac)
                return false;

            bool vaiTroHopLe = context.VaiTros
                .Any(x => x.VaiTroId == taiKhoan.VaiTroId);

            if (!vaiTroHopLe)
                return false;

            existing.TenDangNhap = tenDangNhap;
            existing.MatKhau = string.IsNullOrWhiteSpace(taiKhoan.MatKhau)
                ? existing.MatKhau
                : BCrypt.Net.BCrypt.HashPassword(taiKhoan.MatKhau.Trim(), workFactor: BCryptWorkFactor);
            existing.NhanVienId = taiKhoan.NhanVienId;
            existing.VaiTroId = taiKhoan.VaiTroId;
            existing.TrangThai = taiKhoan.TrangThai;
            existing.GhiChu = string.IsNullOrWhiteSpace(taiKhoan.GhiChu)
                ? null
                : taiKhoan.GhiChu.Trim();

            return context.SaveChanges() > 0;
        }

        public bool DoiMatKhau(int taiKhoanId, string matKhauMoi)
        {
            matKhauMoi = matKhauMoi?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(matKhauMoi))
                return false;

            var existing = context.TaiKhoans
                .FirstOrDefault(x => x.TaiKhoanId == taiKhoanId);

            if (existing == null)
                return false;

            existing.MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhauMoi, workFactor: BCryptWorkFactor);
            return context.SaveChanges() > 0;
        }

        public bool CapNhatTrangThaiTaiKhoan(int taiKhoanId, TrangThaiTaiKhoan trangThaiMoi)
        {
            var existing = context.TaiKhoans
                .FirstOrDefault(x => x.TaiKhoanId == taiKhoanId);

            if (existing == null)
                return false;

            existing.TrangThai = trangThaiMoi;
            return context.SaveChanges() > 0;
        }

        public TaiKhoan? DangNhap(string tenDangNhap, string matKhau)
        {
            KhoiTaoTaiKhoanMacDinhNeuCan();

            tenDangNhap = tenDangNhap?.Trim() ?? string.Empty;
            matKhau = matKhau?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
                return null;

            var taiKhoan = context.TaiKhoans
                .Include(x => x.NhanVien)
                    .ThenInclude(x => x.ChucVu)
                .Include(x => x.VaiTro)
                .FirstOrDefault(x =>
                    x.TenDangNhap.ToLower() == tenDangNhap.ToLower() &&
                    x.TrangThai == TrangThaiTaiKhoan.HoatDong &&
                    x.NhanVien.TrangThaiLamViec == TrangThaiLamViec.DangLam);

            if (taiKhoan == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(matKhau, taiKhoan.MatKhau))
                return null;

            taiKhoan.LanDangNhapCuoi = DateTime.Now;
            context.SaveChanges();
            return taiKhoan;
        }

        public bool XoaTaiKhoan(int taiKhoanId)
        {
            var existing = context.TaiKhoans
                .FirstOrDefault(x => x.TaiKhoanId == taiKhoanId);

            if (existing == null)
                return false;

            context.TaiKhoans.Remove(existing);
            return context.SaveChanges() > 0;
        }

        public void KhoiTaoTaiKhoanMacDinhNeuCan()
        {
            new VaiTroService().DamBaoVaiTroMacDinh();

            if (context.TaiKhoans.Any())
            {
                return;
            }

            NhanVienService nhanVienService = new NhanVienService();
            var nhanVienMacDinh = nhanVienService.LayDanhSachDangLam()
                .OrderBy(x => x.NhanVienId)
                .FirstOrDefault();

            if (nhanVienMacDinh == null)
            {
                return;
            }

            int adminRoleId = context.VaiTros
                .OrderBy(x => x.VaiTroId)
                .First(x => x.TenVaiTro.ToLower() == "admin")
                .VaiTroId;

            TaiKhoan taiKhoanMacDinh = new TaiKhoan
            {
                TenDangNhap = "admin",
                MatKhau = BCrypt.Net.BCrypt.HashPassword("123456", workFactor: BCryptWorkFactor),
                NhanVienId = nhanVienMacDinh.NhanVienId,
                VaiTroId = adminRoleId,
                TrangThai = TrangThaiTaiKhoan.HoatDong,
                GhiChu = "Tài khoản quản trị được tạo tự động cho lần sử dụng đầu tiên."
            };

            context.TaiKhoans.Add(taiKhoanMacDinh);
            context.SaveChanges();
        }
    }
}
