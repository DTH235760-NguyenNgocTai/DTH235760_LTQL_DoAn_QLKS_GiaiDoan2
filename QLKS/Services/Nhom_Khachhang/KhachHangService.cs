using QLKS.Data.Entities.Nhom_KhachHang;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using Microsoft.EntityFrameworkCore;

namespace QLKS.Services.Nhom_Khachhang
{
     public class KhachHangService
        {
            private readonly QLKSDbContext context;

            public KhachHangService()
            {
                context = new QLKSDbContext();
            }

            public List<KhachHang> LayDanhSach()
            {
                return context.KhachHangs
                    .Include(x => x.LoaiKhachHang)
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.HoTen)
                    .ToList();
            }

            public List<KhachHang> LayDanhSachChoComboBox()
            {
                return context.KhachHangs
                    .Include(x => x.LoaiKhachHang)
                    .Where(x => !x.IsDeleted)
                    .OrderBy(x => x.HoTen)
                    .ToList();
            }

            public KhachHang? LayTheoId(int khachHangId)
            {
                return context.KhachHangs
                    .Include(x => x.LoaiKhachHang)
                    .FirstOrDefault(x => x.KhachHangId == khachHangId && !x.IsDeleted);
            }

            public List<KhachHang> TimKiemKhachHang(
                string? tuKhoa = null,
                int? loaiKhachHangId = null,
                GioiTinh? gioiTinh = null)
            {
                var query = context.KhachHangs
                    .Include(x => x.LoaiKhachHang)
                    .Where(x => !x.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    tuKhoa = tuKhoa.Trim();

                    query = query.Where(x =>
                        x.HoTen.Contains(tuKhoa) ||
                        (x.CCCD_Passport != null && x.CCCD_Passport.Contains(tuKhoa)) ||
                        (x.SoDienThoai != null && x.SoDienThoai.Contains(tuKhoa)) ||
                        (x.Email != null && x.Email.Contains(tuKhoa)) ||
                        (x.QuocTich != null && x.QuocTich.Contains(tuKhoa)) ||
                        x.LoaiKhachHang.TenLoaiKhachHang.Contains(tuKhoa));
                }

                if (loaiKhachHangId.HasValue)
                {
                    query = query.Where(x => x.LoaiKhachHangId == loaiKhachHangId.Value);
                }

                if (gioiTinh.HasValue)
                {
                    query = query.Where(x => x.GioiTinh == gioiTinh.Value);
                }

                return query
                    .OrderBy(x => x.HoTen)
                    .ToList();
            }

            public KhachHang? TimTheoSoDienThoai(string soDienThoai)
            {
                soDienThoai = soDienThoai?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(soDienThoai))
                    return null;

                return context.KhachHangs
                    .Include(x => x.LoaiKhachHang)
                    .FirstOrDefault(x =>
                        !x.IsDeleted &&
                        x.SoDienThoai != null &&
                        x.SoDienThoai == soDienThoai);
            }

            public KhachHang? TimTheoCCCDPassport(string cccdPassport)
            {
                cccdPassport = cccdPassport?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(cccdPassport))
                    return null;

                return context.KhachHangs
                    .Include(x => x.LoaiKhachHang)
                    .FirstOrDefault(x =>
                        !x.IsDeleted &&
                        x.CCCD_Passport != null &&
                        x.CCCD_Passport == cccdPassport);
            }

            public KhachHang? TimTheoSoDienThoaiHoacCCCD(string? soDienThoai, string? cccdPassport)
            {
                soDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai.Trim();
                cccdPassport = string.IsNullOrWhiteSpace(cccdPassport) ? null : cccdPassport.Trim();

                if (soDienThoai == null && cccdPassport == null)
                    return null;

                return context.KhachHangs
                    .Include(x => x.LoaiKhachHang)
                    .FirstOrDefault(x =>
                        !x.IsDeleted &&
                        ((soDienThoai != null && x.SoDienThoai == soDienThoai) ||
                         (cccdPassport != null && x.CCCD_Passport == cccdPassport)));
            }

            public bool ThemKhachHang(KhachHang khachHang)
            {
                if (khachHang == null)
                    return false;

                string hoTen = khachHang.HoTen?.Trim() ?? string.Empty;
                string? cccdPassport = string.IsNullOrWhiteSpace(khachHang.CCCD_Passport)
                    ? null
                    : khachHang.CCCD_Passport.Trim();
                string? soDienThoai = string.IsNullOrWhiteSpace(khachHang.SoDienThoai)
                    ? null
                    : khachHang.SoDienThoai.Trim();
                string? email = string.IsNullOrWhiteSpace(khachHang.Email)
                    ? null
                    : khachHang.Email.Trim();
                string? diaChi = string.IsNullOrWhiteSpace(khachHang.DiaChi)
                    ? null
                    : khachHang.DiaChi.Trim();
                string? quocTich = string.IsNullOrWhiteSpace(khachHang.QuocTich)
                    ? null
                    : khachHang.QuocTich.Trim();

                if (string.IsNullOrWhiteSpace(hoTen))
                    return false;

                if (khachHang.NgaySinh.HasValue && khachHang.NgaySinh.Value > DateTime.Now)
                    return false;

                bool loaiKhachHopLe = context.LoaiKhachHangs
                    .Any(x => x.LoaiKhachHangId == khachHang.LoaiKhachHangId);

                if (!loaiKhachHopLe)
                    return false;

                if (!string.IsNullOrWhiteSpace(cccdPassport))
                {
                    bool trungCccdPassport = context.KhachHangs
                        .Any(x => !x.IsDeleted &&
                                  x.CCCD_Passport != null &&
                                  x.CCCD_Passport == cccdPassport);

                    if (trungCccdPassport)
                        return false;
                }

                if (!string.IsNullOrWhiteSpace(soDienThoai))
                {
                    bool trungSoDienThoai = context.KhachHangs
                        .Any(x => !x.IsDeleted &&
                                  x.SoDienThoai != null &&
                                  x.SoDienThoai == soDienThoai);

                    if (trungSoDienThoai)
                        return false;
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    bool trungEmail = context.KhachHangs
                        .Any(x => !x.IsDeleted &&
                                  x.Email != null &&
                                  x.Email.ToLower() == email.ToLower());

                    if (trungEmail)
                        return false;
                }

                khachHang.HoTen = hoTen;
                khachHang.CCCD_Passport = cccdPassport;
                khachHang.SoDienThoai = soDienThoai;
                khachHang.Email = email;
                khachHang.DiaChi = diaChi;
                khachHang.QuocTich = quocTich;

                context.KhachHangs.Add(khachHang);
                return context.SaveChanges() > 0;
            }

            public bool SuaKhachHang(KhachHang khachHang)
            {
                if (khachHang == null)
                    return false;

                string hoTen = khachHang.HoTen?.Trim() ?? string.Empty;
                string? cccdPassport = string.IsNullOrWhiteSpace(khachHang.CCCD_Passport)
                    ? null
                    : khachHang.CCCD_Passport.Trim();
                string? soDienThoai = string.IsNullOrWhiteSpace(khachHang.SoDienThoai)
                    ? null
                    : khachHang.SoDienThoai.Trim();
                string? email = string.IsNullOrWhiteSpace(khachHang.Email)
                    ? null
                    : khachHang.Email.Trim();
                string? diaChi = string.IsNullOrWhiteSpace(khachHang.DiaChi)
                    ? null
                    : khachHang.DiaChi.Trim();
                string? quocTich = string.IsNullOrWhiteSpace(khachHang.QuocTich)
                    ? null
                    : khachHang.QuocTich.Trim();

                if (string.IsNullOrWhiteSpace(hoTen))
                    return false;

                if (khachHang.NgaySinh.HasValue && khachHang.NgaySinh.Value > DateTime.Now)
                    return false;

                var existing = context.KhachHangs
                    .FirstOrDefault(x => x.KhachHangId == khachHang.KhachHangId && !x.IsDeleted);

                if (existing == null)
                    return false;

                bool loaiKhachHopLe = context.LoaiKhachHangs
                    .Any(x => x.LoaiKhachHangId == khachHang.LoaiKhachHangId);

                if (!loaiKhachHopLe)
                    return false;

                if (!string.IsNullOrWhiteSpace(cccdPassport))
                {
                    bool trungCccdPassport = context.KhachHangs
                        .Any(x => !x.IsDeleted &&
                                  x.CCCD_Passport != null &&
                                  x.CCCD_Passport == cccdPassport &&
                                  x.KhachHangId != khachHang.KhachHangId);

                    if (trungCccdPassport)
                        return false;
                }

                if (!string.IsNullOrWhiteSpace(soDienThoai))
                {
                    bool trungSoDienThoai = context.KhachHangs
                        .Any(x => !x.IsDeleted &&
                                  x.SoDienThoai != null &&
                                  x.SoDienThoai == soDienThoai &&
                                  x.KhachHangId != khachHang.KhachHangId);

                    if (trungSoDienThoai)
                        return false;
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    bool trungEmail = context.KhachHangs
                        .Any(x => !x.IsDeleted &&
                                  x.Email != null &&
                                  x.Email.ToLower() == email.ToLower() &&
                                  x.KhachHangId != khachHang.KhachHangId);

                    if (trungEmail)
                        return false;
                }

                existing.HoTen = hoTen;
                existing.NgaySinh = khachHang.NgaySinh;
                existing.GioiTinh = khachHang.GioiTinh;
                existing.CCCD_Passport = cccdPassport;
                existing.SoDienThoai = soDienThoai;
                existing.Email = email;
                existing.DiaChi = diaChi;
                existing.QuocTich = quocTich;
                existing.LoaiKhachHangId = khachHang.LoaiKhachHangId;

                return context.SaveChanges() > 0;
            }

            public bool XoaKhachHang(int khachHangId)
            {
                var existing = context.KhachHangs
                    .FirstOrDefault(x => x.KhachHangId == khachHangId && !x.IsDeleted);

                if (existing == null)
                    return false;

                existing.IsDeleted = true;
                return context.SaveChanges() > 0;
            }

            public bool KhoiPhucKhachHang(int khachHangId)
            {
                var existing = context.KhachHangs
                    .FirstOrDefault(x => x.KhachHangId == khachHangId && x.IsDeleted);

                if (existing == null)
                    return false;

                existing.IsDeleted = false;
                return context.SaveChanges() > 0;
            }
        }
    }


