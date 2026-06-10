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
        public DbSet<HoatChat> HoatChats { get; set; }
        public DbSet<DiUngThuoc> DiUngThuocs { get; set; }
        public DbSet<Thuoc> Thuocs { get; set; }
        public DbSet<ThuocHoatChat> ThuocHoatChats { get; set; }
        public DbSet<TuongTacThuoc> TuongTacThuocs { get; set; }
        public DbSet<ChongChiDinh> ChongChiDinhs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<ChiTietDanhGia> ChiTietDanhGias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // VaiTro - unique TenVaiTro
            modelBuilder.Entity<VaiTro>()
                .HasIndex(v => v.TenVaiTro).IsUnique();

            // NguoiDung - unique TenDangNhap
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(n => n.TenDangNhap).IsUnique();
            modelBuilder.Entity<NguoiDung>()
                .HasOne(n => n.VaiTro)
                .WithMany(v => v.NguoiDungs)
                .HasForeignKey(n => n.MaVaiTro)
                .OnDelete(DeleteBehavior.Restrict);

            // HoSoBenhNhan - unique MaDinhDanhBenhNhan
            modelBuilder.Entity<HoSoBenhNhan>()
                .HasIndex(h => h.MaDinhDanhBenhNhan).IsUnique();
            modelBuilder.Entity<HoSoBenhNhan>()
                .HasOne(h => h.NguoiTao)
                .WithMany(n => n.HoSoBenhNhans)
                .HasForeignKey(h => h.MaNguoiTao)
                .OnDelete(DeleteBehavior.SetNull);

            // BenhNen
            modelBuilder.Entity<BenhNen>()
                .HasOne(b => b.HoSoBenhNhan)
                .WithMany(h => h.BenhNens)
                .HasForeignKey(b => b.MaBenhNhan)
                .OnDelete(DeleteBehavior.Cascade);

            // DiUngThuoc
            modelBuilder.Entity<DiUngThuoc>()
                .HasOne(d => d.HoSoBenhNhan)
                .WithMany(h => h.DiUngThuocs)
                .HasForeignKey(d => d.MaBenhNhan)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DiUngThuoc>()
                .HasOne(d => d.HoatChat)
                .WithMany(h => h.DiUngThuocs)
                .HasForeignKey(d => d.MaHoatChat)
                .OnDelete(DeleteBehavior.Restrict);

            // HoatChat - unique TenHoatChat
            modelBuilder.Entity<HoatChat>()
                .HasIndex(h => h.TenHoatChat).IsUnique();

            // ThuocHoatChat
            modelBuilder.Entity<ThuocHoatChat>()
                .HasOne(t => t.Thuoc)
                .WithMany(th => th.ThuocHoatChats)
                .HasForeignKey(t => t.MaThuoc)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ThuocHoatChat>()
                .HasOne(t => t.HoatChat)
                .WithMany(h => h.ThuocHoatChats)
                .HasForeignKey(t => t.MaHoatChat)
                .OnDelete(DeleteBehavior.Cascade);

            // TuongTacThuoc - two FK to Thuoc
            modelBuilder.Entity<TuongTacThuoc>()
                .HasOne(t => t.ThuocA)
                .WithMany(th => th.TuongTacA)
                .HasForeignKey(t => t.MaThuoc_A)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TuongTacThuoc>()
                .HasOne(t => t.ThuocB)
                .WithMany(th => th.TuongTacB)
                .HasForeignKey(t => t.MaThuoc_B)
                .OnDelete(DeleteBehavior.Restrict);

            // ChongChiDinh
            modelBuilder.Entity<ChongChiDinh>()
                .HasOne(c => c.Thuoc)
                .WithMany(t => t.ChongChiDinhs)
                .HasForeignKey(c => c.MaThuoc)
                .OnDelete(DeleteBehavior.Cascade);

            // DanhGia
            modelBuilder.Entity<DanhGia>()
                .HasOne(d => d.HoSoBenhNhan)
                .WithMany(h => h.DanhGias)
                .HasForeignKey(d => d.MaBenhNhan)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DanhGia>()
                .HasOne(d => d.NguoiDanhGia)
                .WithMany(n => n.DanhGias)
                .HasForeignKey(d => d.MaNguoiDanhGia)
                .OnDelete(DeleteBehavior.SetNull);

            // ChiTietDanhGia
            modelBuilder.Entity<ChiTietDanhGia>()
                .HasOne(c => c.DanhGia)
                .WithMany(d => d.ChiTietDanhGias)
                .HasForeignKey(c => c.MaDanhGia)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ChiTietDanhGia>()
                .HasOne(c => c.Thuoc)
                .WithMany(t => t.ChiTietDanhGias)
                .HasForeignKey(c => c.MaThuoc)
                .OnDelete(DeleteBehavior.Restrict);

            // Table mapping (match SQL naming)
            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<HoSoBenhNhan>().ToTable("HoSoBenhNhan");
            modelBuilder.Entity<BenhNen>().ToTable("BenhNen");
            modelBuilder.Entity<HoatChat>().ToTable("HoatChat");
            modelBuilder.Entity<DiUngThuoc>().ToTable("DiUngThuoc");
            modelBuilder.Entity<Thuoc>().ToTable("Thuoc");
            modelBuilder.Entity<ThuocHoatChat>().ToTable("Thuoc_HoatChat");
            modelBuilder.Entity<TuongTacThuoc>().ToTable("TuongTacThuoc");
            modelBuilder.Entity<ChongChiDinh>().ToTable("ChongChiDinh");
            modelBuilder.Entity<DanhGia>().ToTable("DanhGia");
            modelBuilder.Entity<ChiTietDanhGia>().ToTable("ChiTietDanhGia");

            // Seed data
            modelBuilder.Entity<VaiTro>().HasData(
                new VaiTro { MaVaiTro = 1, TenVaiTro = "Admin" },
                new VaiTro { MaVaiTro = 2, TenVaiTro = "Bác sĩ" },
                new VaiTro { MaVaiTro = 3, TenVaiTro = "Dược sĩ" },
                new VaiTro { MaVaiTro = 4, TenVaiTro = "Y tá" }
            );

            // Seed admin user (password: Admin@123)
            modelBuilder.Entity<NguoiDung>().HasData(
                new NguoiDung
                {
                    MaNguoiDung = 1,
                    TenDangNhap = "admin",
                    MatKhauMaHoa = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    HoTen = "Quản trị viên",
                    Email = "admin@hethong.com",
                    MaVaiTro = 1,
                    TrangThaiHoatDong = true,
                    NgayTao = new DateTime(2024, 1, 1)
                }
            );
        }
    }
}
