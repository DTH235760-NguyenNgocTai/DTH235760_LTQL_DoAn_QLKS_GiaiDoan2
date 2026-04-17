using Microsoft.EntityFrameworkCore;
using QLKS.Data.Entities.Nhom_DatNhanTraPhong;
using QLKS.Data.Entities.Nhom_DichVu;
using QLKS.Data.Entities.Nhom_HoaDon_ThanhToan;
using QLKS.Data.Entities.Nhom_KhachHang;
using QLKS.Data.Entities.Nhom_NhanVien;
using QLKS.Data.Entities.Nhom_Phong;
using QLKS.Data.Entities.Nhom_TaiKhoan;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace QLKS.Data.QLKSDbContext
{
    public class QLKSDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=NOONE\\SQLEXPRESS;Database=QLKS_DB;Trusted_Connection=True;TrustServerCertificate=True");
        }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<Tang> Tangs { get; set; }
        public DbSet<LoaiPhong> LoaiPhongs { get; set; }

        public DbSet<LoaiKhachHang> LoaiKhachHangs { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }

        public DbSet<DatPhong> DatPhongs { get; set; }
        public DbSet<ChiTietDatPhong> ChiTietDatPhongs { get; set; }
        public DbSet<NhanPhong> NhanPhongs { get; set; }
        public DbSet<ChiTietNhanPhong> ChiTietNhanPhongs { get; set; }
        public DbSet<TraPhong> TraPhongs { get; set; }
        public DbSet<ChiTietTraPhong> ChiTietTraPhongs { get; set; }

        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<SuDungDichVu> SuDungDichVus { get; set; }

        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }

        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }

        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Phong>()
                .HasIndex(x => x.SoPhong)
                .IsUnique();

            modelBuilder.Entity<DatPhong>()
                .HasIndex(x => x.MaDatPhong)
                .IsUnique();

            modelBuilder.Entity<NhanPhong>()
                .HasIndex(x => x.MaNhanPhong)
                .IsUnique();

            modelBuilder.Entity<TraPhong>()
                .HasIndex(x => x.MaTraPhong)
                .IsUnique();

            modelBuilder.Entity<DichVu>()
                .HasIndex(x => x.MaDichVu)
                .IsUnique();

            modelBuilder.Entity<NhanVien>()
                .HasIndex(x => x.MaNhanVien)
                .IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(x => x.TenDangNhap)
                .IsUnique();
            
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(x => x.NhanVienId)
                .IsUnique();
            modelBuilder.Entity<ChiTietNhanPhong>()
                .HasIndex(x => x.ChiTietDatPhongId)
                .IsUnique();

            modelBuilder.Entity<ChiTietTraPhong>()
                .HasIndex(x => x.ChiTietNhanPhongId)
                .IsUnique();

            modelBuilder.Entity<ChiTietDatPhong>()
                .Property(x => x.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DichVu>()
                .Property(x => x.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SuDungDichVu>()
                .Property(x => x.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<NhanVien>()
                .Property(x => x.LuongCoBan)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DatPhong>()
                .Property(x => x.TienCoc)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(x => x.TongThanhToan)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ChiTietHoaDon>()
                .Property(x => x.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ChiTietHoaDon>()
                .Property(x => x.ThanhTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ThanhToan>()
                .Property(x => x.SoTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Phong>()
                .HasOne(x => x.Tang)
                .WithMany(x => x.Phongs)
                .HasForeignKey(x => x.TangId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Phong>()
                .HasOne(x => x.LoaiPhong)
                .WithMany(x => x.Phongs)
                .HasForeignKey(x => x.LoaiPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KhachHang>()
                .HasOne(x => x.LoaiKhachHang)
                .WithMany(x => x.KhachHangs)
                .HasForeignKey(x => x.LoaiKhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatPhong>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.DatPhongs)
                .HasForeignKey(x => x.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietDatPhong>()
                .HasOne(x => x.DatPhong)
                .WithMany(x => x.ChiTietDatPhongs)
                .HasForeignKey(x => x.DatPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietDatPhong>()
                .HasOne(x => x.Phong)
                .WithMany(x => x.ChiTietDatPhongs)
                .HasForeignKey(x => x.PhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NhanPhong>()
                .HasOne(x => x.NhanVien)
                .WithMany(x => x.NhanPhongs)
                .HasForeignKey(x => x.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietNhanPhong>()
                .HasOne(x => x.NhanPhong)
                .WithMany(x => x.ChiTietNhanPhongs)
                .HasForeignKey(x => x.NhanPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietNhanPhong>()
                .HasOne(x => x.ChiTietDatPhong)
                .WithMany(x => x.ChiTietNhanPhongs)
                .HasForeignKey(x => x.ChiTietDatPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TraPhong>()
                .HasOne(x => x.NhanVien)
                .WithMany(x => x.TraPhongs)
                .HasForeignKey(x => x.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietTraPhong>()
                .HasOne(x => x.TraPhong)
                .WithMany(x => x.ChiTietTraPhongs)
                .HasForeignKey(x => x.TraPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietTraPhong>()
                .HasOne(x => x.ChiTietNhanPhong)
                .WithMany(x => x.ChiTietTraPhongs)
                .HasForeignKey(x => x.ChiTietNhanPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NhanVien>()
                .HasOne(x => x.ChucVu)
                .WithMany(x => x.NhanViens)
                .HasForeignKey(x => x.ChucVuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaiKhoan>()
                .HasOne(x => x.NhanVien)
                .WithOne(x => x.TaiKhoan)
                .HasForeignKey<TaiKhoan>(x => x.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TaiKhoan>()
                .HasOne(x => x.VaiTro)
                .WithMany(x => x.TaiKhoans)
                .HasForeignKey(x => x.VaiTroId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SuDungDichVu>()
                .HasOne(x => x.ChiTietDatPhong)
                .WithMany(x => x.SuDungDichVus)
                .HasForeignKey(x => x.ChiTietDatPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SuDungDichVu>()
                .HasOne(x => x.DichVu)
                .WithMany(x => x.SuDungDichVus)
                .HasForeignKey(x => x.DichVuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.DatPhong)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.DatPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.NhanVien)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(x => x.HoaDon)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.HoaDonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(x => x.ChiTietDatPhong)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.ChiTietDatPhongId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(x => x.SuDungDichVu)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.SuDungDichVuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThanhToan>()
                .HasOne(x => x.HoaDon)
                .WithMany(x => x.ThanhToans)
                .HasForeignKey(x => x.HoaDonId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
