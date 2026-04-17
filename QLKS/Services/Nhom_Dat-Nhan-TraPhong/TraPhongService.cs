using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.QLKSDbContext;
using QLKS.Enums;
using Microsoft.EntityFrameworkCore;
namespace QLKS.Services.Nhom_Dat_Nhan_TraPhong
{
    internal class TraPhongService
    {
        private readonly QLKSDbContext context;

        public TraPhongService()
        {
            context = new QLKSDbContext();
        }

        public List<TraPhong> LayDanhSach()
        {
            return context.TraPhongs
                .Include(x => x.NhanVien)
                .Include(x => x.ChiTietTraPhongs)
                    .ThenInclude(x => x.ChiTietNhanPhong)
                        .ThenInclude(x => x.ChiTietDatPhong)
                            .ThenInclude(x => x.Phong)
                .OrderByDescending(x => x.NgayLap)
                .ToList();
        }

        public TraPhong? LayTheoId(int traPhongId)
        {
            return context.TraPhongs
                .Include(x => x.NhanVien)
                .Include(x => x.ChiTietTraPhongs)
                    .ThenInclude(x => x.ChiTietNhanPhong)
                        .ThenInclude(x => x.ChiTietDatPhong)
                            .ThenInclude(x => x.DatPhong)
                                .ThenInclude(x => x.KhachHang)
                .Include(x => x.ChiTietTraPhongs)
                    .ThenInclude(x => x.ChiTietNhanPhong)
                        .ThenInclude(x => x.ChiTietDatPhong)
                            .ThenInclude(x => x.Phong)
                                .ThenInclude(x => x.LoaiPhong)
                .FirstOrDefault(x => x.TraPhongId == traPhongId);
        }

        public bool TaoPhieuTraPhong(int nhanVienId, List<int> danhSachChiTietNhanPhongId, DateTime? thoiGianTraThucTe = null)
        {
            if (danhSachChiTietNhanPhongId == null || danhSachChiTietNhanPhongId.Count == 0)
                return false;

            if (danhSachChiTietNhanPhongId.Distinct().Count() != danhSachChiTietNhanPhongId.Count)
                return false;

            bool nhanVienHopLe = context.NhanViens
                .Any(x => x.NhanVienId == nhanVienId && x.TrangThaiLamViec == TrangThaiLamViec.DangLam);

            if (!nhanVienHopLe)
                return false;

            DateTime tgTra = thoiGianTraThucTe ?? DateTime.Now;

            var danhSachChiTietNhan = context.ChiTietNhanPhongs
                .Include(x => x.ChiTietDatPhong)
                    .ThenInclude(x => x.Phong)
                .Where(x => danhSachChiTietNhanPhongId.Contains(x.ChiTietNhanPhongId))
                .ToList();

            if (danhSachChiTietNhan.Count != danhSachChiTietNhanPhongId.Count)
                return false;

            foreach (var chiTietNhan in danhSachChiTietNhan)
            {
                bool daTra = context.ChiTietTraPhongs
                    .Any(x => x.ChiTietNhanPhongId == chiTietNhan.ChiTietNhanPhongId);

                if (daTra)
                    return false;

                if (tgTra < chiTietNhan.ThoiGianNhanThucTe)
                    return false;

                if (chiTietNhan.ChiTietDatPhong.TrangThai == TrangThaiChiTietDatPhong.DaHuy ||
                    chiTietNhan.ChiTietDatPhong.TrangThai == TrangThaiChiTietDatPhong.DaTra)
                {
                    return false;
                }
            }

            using var transaction = context.Database.BeginTransaction();

            try
            {
                var phieuTra = new TraPhong
                {
                    MaTraPhong = TaoMaTraPhong(),
                    NhanVienId = nhanVienId,
                    NgayLap = tgTra
                };

                context.TraPhongs.Add(phieuTra);
                context.SaveChanges();

                foreach (var chiTietNhan in danhSachChiTietNhan)
                {
                    var chiTietTra = new ChiTietTraPhong
                    {
                        TraPhongId = phieuTra.TraPhongId,
                        ChiTietNhanPhongId = chiTietNhan.ChiTietNhanPhongId,
                        ThoiGianTraThucTe = tgTra
                    };

                    context.ChiTietTraPhongs.Add(chiTietTra);

                    chiTietNhan.ChiTietDatPhong.TrangThai = TrangThaiChiTietDatPhong.DaTra;
                    chiTietNhan.ChiTietDatPhong.Phong.TrangThai = TrangThaiPhong.DangDon;
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
        private string TaoMaTraPhong()
        {
            return $"TP{DateTime.Now:yyyyMMddHHmmssfff}";
        }
    }
}
