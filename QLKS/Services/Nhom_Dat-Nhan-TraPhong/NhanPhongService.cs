using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using Microsoft.EntityFrameworkCore;

namespace QLKS.Services.Nhom_Dat_Nhan_TraPhong
{
    public class NhanPhongService
    {
        private readonly QLKSDbContext context;

        public NhanPhongService()
        {
            context = new QLKSDbContext();
        }

        public List<NhanPhong> LayDanhSach()
        {
            return context.NhanPhongs
                .Include(x => x.NhanVien)
                .Include(x => x.ChiTietNhanPhongs)
                    .ThenInclude(x => x.ChiTietDatPhong)
                        .ThenInclude(x => x.Phong)
                .OrderByDescending(x => x.NgayLap)
                .ToList();
        }

        public NhanPhong? LayTheoId(int nhanPhongId)
        {
            return context.NhanPhongs
                .Include(x => x.NhanVien)
                .Include(x => x.ChiTietNhanPhongs)
                    .ThenInclude(x => x.ChiTietDatPhong)
                        .ThenInclude(x => x.DatPhong)
                            .ThenInclude(x => x.KhachHang)
                .Include(x => x.ChiTietNhanPhongs)
                    .ThenInclude(x => x.ChiTietDatPhong)
                        .ThenInclude(x => x.Phong)
                            .ThenInclude(x => x.LoaiPhong)
                .FirstOrDefault(x => x.NhanPhongId == nhanPhongId);
        }

        public bool TaoPhieuNhanPhong(int nhanVienId, List<int> danhSachChiTietDatPhongId, DateTime? thoiGianNhanThucTe = null)
        {
            if (danhSachChiTietDatPhongId == null || danhSachChiTietDatPhongId.Count == 0)
                return false;

            if (danhSachChiTietDatPhongId.Distinct().Count() != danhSachChiTietDatPhongId.Count)
                return false;

            bool nhanVienHopLe = context.NhanViens
                .Any(x => x.NhanVienId == nhanVienId && x.TrangThaiLamViec == TrangThaiLamViec.DangLam);

            if (!nhanVienHopLe)
                return false;

            DateTime tgNhan = thoiGianNhanThucTe ?? DateTime.Now;

            var danhSachChiTiet = context.ChiTietDatPhongs
                .Include(x => x.DatPhong)
                .Include(x => x.Phong)
                .Where(x => danhSachChiTietDatPhongId.Contains(x.ChiTietDatPhongId))
                .ToList();

            if (danhSachChiTiet.Count != danhSachChiTietDatPhongId.Count)
                return false;

            foreach (var chiTiet in danhSachChiTiet)
            {
                if (chiTiet.TrangThai == TrangThaiChiTietDatPhong.DaHuy ||
                    chiTiet.TrangThai == TrangThaiChiTietDatPhong.DaTra ||
                    chiTiet.TrangThai == TrangThaiChiTietDatPhong.DangO)
                {
                    return false;
                }

                bool daNhan = context.ChiTietNhanPhongs
                    .Any(x => x.ChiTietDatPhongId == chiTiet.ChiTietDatPhongId);

                if (daNhan)
                    return false;

                if (tgNhan >= chiTiet.ThoiGianTraDuKien)
                    return false;

                if (chiTiet.Phong.IsDeleted ||
                    chiTiet.Phong.TrangThai == TrangThaiPhong.BaoTri ||
                    chiTiet.Phong.TrangThai == TrangThaiPhong.DangDon)
                {
                    return false;
                }
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                var phieuNhan = new NhanPhong
                {
                    MaNhanPhong = TaoMaNhanPhong(),
                    NhanVienId = nhanVienId,
                    NgayLap = tgNhan
                };

                context.NhanPhongs.Add(phieuNhan);
                context.SaveChanges();

                foreach (var chiTiet in danhSachChiTiet)
                {
                    var chiTietNhan = new ChiTietNhanPhong
                    {
                        NhanPhongId = phieuNhan.NhanPhongId,
                        ChiTietDatPhongId = chiTiet.ChiTietDatPhongId,
                        ThoiGianNhanThucTe = tgNhan
                    };

                    context.ChiTietNhanPhongs.Add(chiTietNhan);

                    chiTiet.TrangThai = TrangThaiChiTietDatPhong.DangO;
                    chiTiet.Phong.TrangThai = TrangThaiPhong.DangO;
                }

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

        private string TaoMaNhanPhong()
        {
            return $"NP{DateTime.Now:yyyyMMddHHmmssfff}";
        }
    }
}
