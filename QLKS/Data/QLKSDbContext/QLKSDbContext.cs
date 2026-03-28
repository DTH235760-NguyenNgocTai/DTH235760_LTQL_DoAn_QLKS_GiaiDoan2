using QLKS.Data.Entities.Nhom_Phong;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace QLKS.Data.QLKSDbContext
{
    internal class QLKSDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=NOONE\\SQLEXPRESS;Database=QLKS_DB;Trusted_Connection=True;TrustServerCertificate=True");
        }
        public DbSet<Phong> Phongs { get; set; }
        public DbSet<Tang> Tangs { get; set; }
        public DbSet<LoaiPhong> LoaiPhongs { get; set; }
        public DbSet<LoaiPhongTienNghi> LoaiPhongTienNghis { get; set; }
        public DbSet<TienNghi> TienNghis { get; set; }
        public DbSet<LoaiPhongThietBi> LoaiPhongThietBis { get; set; }
        public DbSet<ThietBi> ThietBis { get; set; }
    }
}
