using Microsoft.EntityFrameworkCore;
using QLKS.Data.Entities.Nhom_NhanVien;
using QLKS.Data.Entities.Nhom_TaiKhoan;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using QLKS.Services.Nhom_TaiKhoan;
using System.Globalization;
using System.Text;

namespace QLKS.Services.Nhom_NhanVien
{
    internal class NhanVienService
    {
        private const int BCryptWorkFactor = 12;
        private readonly QLKSDbContext context;

        public NhanVienService()
        {
            context = new QLKSDbContext();
        }

        public List<NhanVien> LayDanhSach()
        {
            return context.NhanViens
                .Include(x => x.ChucVu)
                .OrderBy(x => x.MaNhanVien)
                .ToList();
        }

        public List<NhanVien> LayDanhSachDangLam()
        {
            return context.NhanViens
                .Include(x => x.ChucVu)
                .Include(x => x.TaiKhoan)
                .Where(x => x.TrangThaiLamViec == TrangThaiLamViec.DangLam)
                .OrderBy(x => x.MaNhanVien)
                .ToList();
        }

        public List<NhanVien> LayDanhSachChoComboBox()
        {
            return context.NhanViens
                .Include(x => x.ChucVu)
                .Include(x => x.TaiKhoan)
                .Where(x => x.TrangThaiLamViec == TrangThaiLamViec.DangLam)
                .OrderBy(x => x.HoTen)
                .ToList();
        }

        public NhanVien? LayTheoId(int nhanVienId)
        {
            return context.NhanViens
                .Include(x => x.ChucVu)
                .Include(x => x.TaiKhoan)
                .FirstOrDefault(x => x.NhanVienId == nhanVienId);
        }

        public List<NhanVien> TimKiemNhanVien(
            string? tuKhoa = null,
            int? chucVuId = null,
            TrangThaiLamViec? trangThaiLamViec = null)
        {
            var query = context.NhanViens
                .Include(x => x.ChucVu)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();

                query = query.Where(x =>
                    x.MaNhanVien.Contains(tuKhoa) ||
                    x.HoTen.Contains(tuKhoa) ||
                    x.SoDienThoai.Contains(tuKhoa) ||
                    x.Email.Contains(tuKhoa) ||
                    x.CCCD.Contains(tuKhoa) ||
                    x.ChucVu.TenChucVu.Contains(tuKhoa));
            }

            if (chucVuId.HasValue)
            {
                query = query.Where(x => x.ChucVuId == chucVuId.Value);
            }

            if (trangThaiLamViec.HasValue)
            {
                query = query.Where(x => x.TrangThaiLamViec == trangThaiLamViec.Value);
            }

            return query
                .OrderBy(x => x.MaNhanVien)
                .ToList();
        }

        public bool ThemNhanVien(NhanVien nhanVien)
        {
            return ThemNhanVien(nhanVien, out _);
        }

        public bool ThemNhanVien(NhanVien nhanVien, out TaiKhoan? taiKhoanDuocTao)
        {
            taiKhoanDuocTao = null;

            if (nhanVien == null)
                return false;

            string hoTen = nhanVien.HoTen?.Trim() ?? string.Empty;
            string soDienThoai = nhanVien.SoDienThoai?.Trim() ?? string.Empty;
            string email = nhanVien.Email?.Trim() ?? string.Empty;
            string diaChi = nhanVien.DiaChi?.Trim() ?? string.Empty;
            string cccd = nhanVien.CCCD?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(hoTen) ||
                string.IsNullOrWhiteSpace(soDienThoai) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(diaChi) ||
                string.IsNullOrWhiteSpace(cccd))
            {
                return false;
            }

            if (nhanVien.NgaySinh > DateTime.Now ||
                nhanVien.NgayVaoLam > DateTime.Now ||
                nhanVien.LuongCoBan < 0)
            {
                return false;
            }

            ChucVu? chucVu = context.ChucVus
                .FirstOrDefault(x => x.ChucVuId == nhanVien.ChucVuId);

            if (chucVu == null)
                return false;

            bool trungCccd = context.NhanViens
                .Any(x => x.CCCD == cccd);

            if (trungCccd)
                return false;

            bool trungEmail = context.NhanViens
                .Any(x => x.Email.ToLower() == email.ToLower());

            if (trungEmail)
                return false;

            bool laNhanSuQuanTri = LaNhanSuQuanTri(chucVu.TenChucVu);
            nhanVien.MaNhanVien = TaoMaNhanVienMoi(laNhanSuQuanTri ? "AD" : "NV");
            nhanVien.HoTen = hoTen;
            nhanVien.SoDienThoai = soDienThoai;
            nhanVien.Email = email;
            nhanVien.DiaChi = diaChi;
            nhanVien.CCCD = cccd;
            nhanVien.GhiChu = string.IsNullOrWhiteSpace(nhanVien.GhiChu)
                ? null
                : nhanVien.GhiChu.Trim();

            string tenDangNhap = nhanVien.MaNhanVien;
            string matKhauMacDinh = TaoMatKhauMacDinh(nhanVien.NgaySinh);
            int vaiTroId = LayVaiTroMacDinhId(laNhanSuQuanTri);

            using var transaction = context.Database.BeginTransaction();

            try
            {
                context.NhanViens.Add(nhanVien);
                context.SaveChanges();

                TaiKhoan taiKhoan = new TaiKhoan
                {
                    TenDangNhap = tenDangNhap,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(matKhauMacDinh, workFactor: BCryptWorkFactor),
                    NhanVienId = nhanVien.NhanVienId,
                    VaiTroId = vaiTroId,
                    TrangThai = nhanVien.TrangThaiLamViec == TrangThaiLamViec.DangLam
                        ? TrangThaiTaiKhoan.HoatDong
                        : TrangThaiTaiKhoan.TamKhoa,
                    GhiChu = $"Tai khoan tu dong cho nhan vien {nhanVien.MaNhanVien}."
                };

                context.TaiKhoans.Add(taiKhoan);
                context.SaveChanges();

                transaction.Commit();
                taiKhoanDuocTao = taiKhoan;
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool SuaNhanVien(NhanVien nhanVien)
        {
            if (nhanVien == null)
                return false;

            string maNhanVien = nhanVien.MaNhanVien?.Trim() ?? string.Empty;
            string hoTen = nhanVien.HoTen?.Trim() ?? string.Empty;
            string soDienThoai = nhanVien.SoDienThoai?.Trim() ?? string.Empty;
            string email = nhanVien.Email?.Trim() ?? string.Empty;
            string diaChi = nhanVien.DiaChi?.Trim() ?? string.Empty;
            string cccd = nhanVien.CCCD?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(maNhanVien) ||
                string.IsNullOrWhiteSpace(hoTen) ||
                string.IsNullOrWhiteSpace(soDienThoai) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(diaChi) ||
                string.IsNullOrWhiteSpace(cccd))
            {
                return false;
            }

            if (nhanVien.NgaySinh > DateTime.Now ||
                nhanVien.NgayVaoLam > DateTime.Now ||
                nhanVien.LuongCoBan < 0)
            {
                return false;
            }

            var existing = context.NhanViens
                .FirstOrDefault(x => x.NhanVienId == nhanVien.NhanVienId);

            if (existing == null)
                return false;

            bool chucVuHopLe = context.ChucVus
                .Any(x => x.ChucVuId == nhanVien.ChucVuId);

            if (!chucVuHopLe)
                return false;

            bool trungMaNhanVien = context.NhanViens
                .Any(x => x.MaNhanVien.ToLower() == maNhanVien.ToLower()
                       && x.NhanVienId != nhanVien.NhanVienId);

            if (trungMaNhanVien)
                return false;

            bool trungCccd = context.NhanViens
                .Any(x => x.CCCD == cccd
                       && x.NhanVienId != nhanVien.NhanVienId);

            if (trungCccd)
                return false;

            bool trungEmail = context.NhanViens
                .Any(x => x.Email.ToLower() == email.ToLower()
                       && x.NhanVienId != nhanVien.NhanVienId);

            if (trungEmail)
                return false;

            existing.MaNhanVien = maNhanVien;
            existing.HoTen = hoTen;
            existing.NgaySinh = nhanVien.NgaySinh;
            existing.GioiTinh = nhanVien.GioiTinh;
            existing.SoDienThoai = soDienThoai;
            existing.Email = email;
            existing.DiaChi = diaChi;
            existing.CCCD = cccd;
            existing.ChucVuId = nhanVien.ChucVuId;
            existing.NgayVaoLam = nhanVien.NgayVaoLam;
            existing.LuongCoBan = nhanVien.LuongCoBan;
            existing.TrangThaiLamViec = nhanVien.TrangThaiLamViec;
            existing.GhiChu = string.IsNullOrWhiteSpace(nhanVien.GhiChu)
                ? null
                : nhanVien.GhiChu.Trim();

            return context.SaveChanges() > 0;
        }

        public bool CapNhatTrangThaiLamViec(int nhanVienId, TrangThaiLamViec trangThaiMoi)
        {
            var existing = context.NhanViens
                .FirstOrDefault(x => x.NhanVienId == nhanVienId);

            if (existing == null)
                return false;

            existing.TrangThaiLamViec = trangThaiMoi;
            return context.SaveChanges() > 0;
        }

        public bool XoaNhanVien(int nhanVienId)
        {
            var existing = context.NhanViens
                .Include(x => x.TaiKhoan)
                .FirstOrDefault(x => x.NhanVienId == nhanVienId);

            if (existing == null)
                return false;

            bool daCoPhieuNhan = context.NhanPhongs
                .Any(x => x.NhanVienId == nhanVienId);

            if (daCoPhieuNhan)
                return false;

            bool daCoPhieuTra = context.TraPhongs
                .Any(x => x.NhanVienId == nhanVienId);

            if (daCoPhieuTra)
                return false;

            bool daCoHoaDon = context.HoaDons
                .Any(x => x.NhanVienId == nhanVienId);

            if (daCoHoaDon)
                return false;

            if (existing.TaiKhoan != null)
                return false;

            context.NhanViens.Remove(existing);
            return context.SaveChanges() > 0;
        }

        public static string TaoMatKhauMacDinh(DateTime ngaySinh)
        {
            return $"{ngaySinh:ddMMyyyy}nsnv";
        }

        private string TaoMaNhanVienMoi(string tienTo)
        {
            int soThuTuMoi = context.NhanViens
                .Select(x => x.MaNhanVien)
                .AsEnumerable()
                .Concat(context.TaiKhoans.Select(x => x.TenDangNhap).AsEnumerable())
                .Select(x => LaySoThuTu(x, tienTo))
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .DefaultIfEmpty(0)
                .Max() + 1;

            return $"{tienTo}{soThuTuMoi:000}";
        }

        private int LayVaiTroMacDinhId(bool laNhanSuQuanTri)
        {
            new VaiTroService().DamBaoVaiTroMacDinh();

            string khoaVaiTro = laNhanSuQuanTri ? "admin" : "nhanvien";
            VaiTro? vaiTro = context.VaiTros
                .AsEnumerable()
                .FirstOrDefault(x => ChuanHoaKhoaSoSanh(x.TenVaiTro) == khoaVaiTro);

            if (vaiTro == null)
            {
                throw new InvalidOperationException("Khong tim thay vai tro mac dinh.");
            }

            return vaiTro.VaiTroId;
        }

        private static bool LaNhanSuQuanTri(string tenChucVu)
        {
            string khoa = ChuanHoaKhoaSoSanh(tenChucVu);

            return khoa.Contains("admin") ||
                   khoa.Contains("quanly") ||
                   khoa.Contains("quantri") ||
                   khoa.Contains("giamdoc");
        }

        private static int? LaySoThuTu(string? ma, string tienTo)
        {
            if (string.IsNullOrWhiteSpace(ma))
            {
                return null;
            }

            string giaTri = ma.Trim().ToUpperInvariant();
            if (!giaTri.StartsWith(tienTo, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string phanSo = giaTri.Substring(tienTo.Length);
            return int.TryParse(phanSo, out int soThuTu) ? soThuTu : null;
        }

        private static string ChuanHoaKhoaSoSanh(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.Trim().Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder(normalized.Length);

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
