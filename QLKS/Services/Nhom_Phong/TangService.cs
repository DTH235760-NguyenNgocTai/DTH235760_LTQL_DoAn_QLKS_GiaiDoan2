using QLKS.Data.Entities.Nhom_Phong;
using QLKS.Data.QLKSDbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLKS.Services.Nhom_Phong
{
    public class TangService
    {
        private readonly QLKSDbContext context;

        public TangService()
        {
            context = new QLKSDbContext();
        }

        public List<Tang> LayDanhSach()
        {
            return context.Tangs
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SoTang)
                .ToList();
        }

        public List<Tang> LayDanhSachChoComboBox()
        {
            return context.Tangs
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SoTang)
                .ToList();
        }

        public Tang? LayTheoId(int tangId)
        {
            return context.Tangs
                .FirstOrDefault(x => x.TangId == tangId && !x.IsDeleted);
        }

        public bool ThemTang(Tang tang)
        {
            if (tang == null || tang.SoTang <= 0)
                return false;

            bool daTonTai = context.Tangs
                .Any(x => x.SoTang == tang.SoTang && !x.IsDeleted);

            if (daTonTai)
                return false;

            context.Tangs.Add(tang);
            return context.SaveChanges() > 0;
        }

        public bool SuaTang(Tang tang)
        {
            if (tang == null || tang.SoTang <= 0)
                return false;

            var existing = context.Tangs
                .FirstOrDefault(x => x.TangId == tang.TangId && !x.IsDeleted);

            if (existing == null)
                return false;

            bool biTrung = context.Tangs
                .Any(x => x.SoTang == tang.SoTang &&
                          x.TangId != tang.TangId &&
                          !x.IsDeleted);

            if (biTrung)
                return false;

            existing.SoTang = tang.SoTang;

            return context.SaveChanges() > 0;
        }

        public bool XoaTang(int tangId)
        {
            var existing = context.Tangs
                .FirstOrDefault(x => x.TangId == tangId && !x.IsDeleted);

            if (existing == null)
                return false;

            bool dangDuocSuDung = context.Phongs
                .Any(x => x.TangId == tangId && !x.IsDeleted);

            if (dangDuocSuDung)
                return false;

            existing.IsDeleted = true;
            return context.SaveChanges() > 0;
        }
    }
}
