using QLKS.Data.Entities.Nhom_NhanVien;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using Microsoft.EntityFrameworkCore;

namespace QLKS.Services.Nhom_NhanVien
{
    internal class NhanVienService
    {
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
                .Where(x => x.TrangThaiLamViec == TrangThaiLamViec.DangLam)
                .OrderBy(x => x.MaNhanVien)
                .ToList();
        }

        public List<NhanVien> LayDanhSachChoComboBox()
        {
            return context.NhanViens
                .Include(x => x.ChucVu)
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

            bool chucVuHopLe = context.ChucVus
                .Any(x => x.ChucVuId == nhanVien.ChucVuId);

            if (!chucVuHopLe)
                return false;

            bool trungMaNhanVien = context.NhanViens
                .Any(x => x.MaNhanVien.ToLower() == maNhanVien.ToLower());

            if (trungMaNhanVien)
                return false;

            bool trungCccd = context.NhanViens
                .Any(x => x.CCCD == cccd);

            if (trungCccd)
                return false;

            bool trungEmail = context.NhanViens
                .Any(x => x.Email.ToLower() == email.ToLower());

            if (trungEmail)
                return false;

            nhanVien.MaNhanVien = maNhanVien;
            nhanVien.HoTen = hoTen;
            nhanVien.SoDienThoai = soDienThoai;
            nhanVien.Email = email;
            nhanVien.DiaChi = diaChi;
            nhanVien.CCCD = cccd;
            nhanVien.GhiChu = string.IsNullOrWhiteSpace(nhanVien.GhiChu)
                ? null
                : nhanVien.GhiChu.Trim();

            context.NhanViens.Add(nhanVien);
            return context.SaveChanges() > 0;
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
    }
}
