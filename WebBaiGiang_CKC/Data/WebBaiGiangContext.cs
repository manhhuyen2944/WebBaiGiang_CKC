using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using WebBaiGiang_CKC.Models;

namespace WebBaiGiang_CKC.Data
{
    public class WebBaiGiangContext:DbContext
    {
        public WebBaiGiangContext(DbContextOptions<WebBaiGiangContext> options) : base(options)
        {

        }
        public DbSet<Muc> Muc { get; set; }
        public DbSet<Bai> Bai { get; set; }
        public DbSet<Chuong> Chuong { get; set; }
        public DbSet<KyKiemTra> KyKiemTra { get; set; }
        public DbSet<MonHoc> MonHoc { get; set; }
        public DbSet<CauHoi> CauHoi { get; set; }
        public DbSet<TaiKhoan> TaiKhoan { get; set; }
        public DbSet<De> De { get; set; }
        public DbSet<DanhSachThi> DanhSachThi { get; set; }
        public DbSet<GiaoVien> GiaoVien { get; set; }
        public DbSet<CauHoi_De> CauHoi_De { get; set; }
        public DbSet<BaiLam> BaiLam { get; set; }
        public DbSet<CauHoi_BaiLam> CauHoi_BaiLam { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chuong>()
                .HasIndex(c => c.ChuongId)
                .IsUnique();
        }
    }
}
