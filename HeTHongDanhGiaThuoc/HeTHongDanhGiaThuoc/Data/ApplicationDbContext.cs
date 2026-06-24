using HeTHongDanhGiaThuoc.Models;
using Microsoft.EntityFrameworkCore;

namespace HeTHongDanhGiaThuoc.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }

        public DbSet<HoSoBenhNhan> HoSoBenhNhans { get; set; }

        public DbSet<BenhNen> BenhNens { get; set; }
        public DbSet<BenhNhanBenhNen> BenhNhanBenhNens { get; set; }

        public DbSet<HoatChat> HoatChats { get; set; }
        public DbSet<DiUngThuoc> DiUngThuocs { get; set; }

        public DbSet<Thuoc> Thuocs { get; set; }
        public DbSet<ThuocHoatChat> ThuocHoatChats { get; set; }
        public DbSet<ThuocDangSuDung> ThuocDangSuDungs { get; set; }

        public DbSet<ChongChiDinh> ChongChiDinhs { get; set; }
        public DbSet<TuongTacThuoc> TuongTacThuocs { get; set; }

        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<ChiTietDanhGia> ChiTietDanhGias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================
            // UNIQUE INDEX
            // ======================

            modelBuilder.Entity<VaiTro>()
                .HasIndex(x => x.TenVaiTro)
                .IsUnique();

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(x => x.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<HoSoBenhNhan>()
                .HasIndex(x => x.MaDinhDanhBenhNhan)
                .IsUnique();

            modelBuilder.Entity<HoatChat>()
                .HasIndex(x => x.TenHoatChat)
                .IsUnique();

            // ======================
            // NGUOIDUNG - VAITRO
            // ======================

            modelBuilder.Entity<NguoiDung>()
                .HasOne(x => x.VaiTro)
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(x => x.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // HOSOBENHNHAN - NGUOITAO
            // ======================

            modelBuilder.Entity<HoSoBenhNhan>()
                .HasOne(x => x.NguoiTao)
                .WithMany(n => n.HoSoBenhNhans)
                .HasForeignKey(x => x.MaNguoiTao)
                .OnDelete(DeleteBehavior.SetNull);

            // ======================
            // BENHNHANBENHNEN
            // ======================

            modelBuilder.Entity<BenhNhanBenhNen>()
                .HasOne(x => x.HoSoBenhNhan)
                .WithMany(h => h.BenhNhanBenhNens)
                .HasForeignKey(x => x.MaBenhNhan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BenhNhanBenhNen>()
                .HasOne(x => x.BenhNen)
                .WithMany(b => b.BenhNhanBenhNens)
                .HasForeignKey(x => x.MaBenhNen)
                .OnDelete(DeleteBehavior.Cascade);

            // ======================
            // DI UNG THUOC
            // ======================

            modelBuilder.Entity<DiUngThuoc>()
                .HasOne(x => x.HoSoBenhNhan)
                .WithMany(h => h.DiUngThuocs)
                .HasForeignKey(x => x.MaBenhNhan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiUngThuoc>()
                .HasOne(x => x.HoatChat)
                .WithMany(h => h.DiUngThuocs)
                .HasForeignKey(x => x.MaHoatChat)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // THUOC - HOATCHAT
            // ======================

            modelBuilder.Entity<ThuocHoatChat>()
                .HasOne(x => x.Thuoc)
                .WithMany(t => t.ThuocHoatChats)
                .HasForeignKey(x => x.MaThuoc)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThuocHoatChat>()
                .HasOne(x => x.HoatChat)
                .WithMany(h => h.ThuocHoatChats)
                .HasForeignKey(x => x.MaHoatChat)
                .OnDelete(DeleteBehavior.Cascade);

            // ======================
            // THUOC DANG SU DUNG
            // ======================

            modelBuilder.Entity<ThuocDangSuDung>()
                .HasOne(x => x.HoSoBenhNhan)
                .WithMany(h => h.ThuocDangSuDungs)
                .HasForeignKey(x => x.MaBenhNhan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThuocDangSuDung>()
                .HasOne(x => x.Thuoc)
                .WithMany()
                .HasForeignKey(x => x.MaThuoc)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // CHONG CHI DINH
            // ======================

            modelBuilder.Entity<ChongChiDinh>()
                .HasOne(x => x.Thuoc)
                .WithMany(t => t.ChongChiDinhs)
                .HasForeignKey(x => x.MaThuoc)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChongChiDinh>()
                .HasOne(x => x.BenhNen)
                .WithMany(b => b.ChongChiDinhs)
                .HasForeignKey(x => x.MaBenhNen)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // TUONG TAC THUOC
            // ======================

            modelBuilder.Entity<TuongTacThuoc>()
                .HasOne(x => x.ThuocA)
                .WithMany(t => t.TuongTacA)
                .HasForeignKey(x => x.MaThuocA)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TuongTacThuoc>()
                .HasOne(x => x.ThuocB)
                .WithMany(t => t.TuongTacB)
                .HasForeignKey(x => x.MaThuocB)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // DANH GIA
            // ======================

            modelBuilder.Entity<DanhGia>()
                .HasOne(x => x.HoSoBenhNhan)
                .WithMany(h => h.DanhGias)
                .HasForeignKey(x => x.MaBenhNhan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DanhGia>()
                .HasOne(x => x.NguoiDanhGia)
                .WithMany(n => n.DanhGias)
                .HasForeignKey(x => x.MaNguoiDanhGia)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // CHI TIET DANH GIA
            // ======================

            modelBuilder.Entity<ChiTietDanhGia>()
                .HasOne(x => x.DanhGia)
                .WithMany(d => d.ChiTietDanhGias)
                .HasForeignKey(x => x.MaDanhGia)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChiTietDanhGia>()
                .HasOne(x => x.Thuoc)
                .WithMany(t => t.ChiTietDanhGias)
                .HasForeignKey(x => x.MaThuoc)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // TABLE NAME
            // ======================

            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");

            modelBuilder.Entity<HoSoBenhNhan>().ToTable("HoSoBenhNhan");

            modelBuilder.Entity<BenhNen>().ToTable("BenhNen");
            modelBuilder.Entity<BenhNhanBenhNen>().ToTable("BenhNhanBenhNen");

            modelBuilder.Entity<HoatChat>().ToTable("HoatChat");
            modelBuilder.Entity<DiUngThuoc>().ToTable("DiUngThuoc");

            modelBuilder.Entity<Thuoc>().ToTable("Thuoc");
            modelBuilder.Entity<ThuocHoatChat>().ToTable("ThuocHoatChat");
            modelBuilder.Entity<ThuocDangSuDung>().ToTable("ThuocDangSuDung");

            modelBuilder.Entity<ChongChiDinh>().ToTable("ChongChiDinh");
            modelBuilder.Entity<TuongTacThuoc>().ToTable("TuongTacThuoc");
      

            modelBuilder.Entity<DanhGia>().ToTable("DanhGia");
            modelBuilder.Entity<ChiTietDanhGia>().ToTable("ChiTietDanhGia");

            modelBuilder.Entity<BenhNhanBenhNen>()
    .HasIndex(x => new
    {
        x.MaBenhNhan,
        x.MaBenhNen
    })
    .IsUnique();

            modelBuilder.Entity<ThuocHoatChat>()
            .HasIndex(x => new
            {
                x.MaThuoc,
                x.MaHoatChat
            })
            .IsUnique();

            modelBuilder.Entity<ThuocDangSuDung>()
    .HasIndex(x => new
    {
        x.MaBenhNhan,
        x.MaThuoc
    })
    .IsUnique();
        }
    }
}
