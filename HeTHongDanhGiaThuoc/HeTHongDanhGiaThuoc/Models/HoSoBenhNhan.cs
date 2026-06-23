using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class HoSoBenhNhan
    {
        [Key]
        public int MaBenhNhan { get; set; } //Khóa chính database 

        [Required]
        [MaxLength(50)]
        [Display(Name = "Mã định danh")]
        public string MaDinhDanhBenhNhan { get; set; } = string.Empty; //Do user tự nhập

        [Required]
        [MaxLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [MaxLength(10)]
        [Display(Name = "Nhóm máu")]
        public string? NhomMau { get; set; }

        [Display(Name = "Chiều cao (cm)")]
        public float? ChieuCao { get; set; }

        [Display(Name = "Cân nặng (kg)")]
        public float? CanNang { get; set; }

        [MaxLength(255)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Bệnh hiện tại")]
        public string? BenhHienTai { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Người tạo")]
        public int? MaNguoiTao { get; set; }

        [Display(Name = "Ngày tiếp nhận")]
        public DateTime NgayTiepNhan { get; set; } = DateTime.Now;

        //Khóa Ngoại

        [ForeignKey(nameof(MaNguoiTao))]
        public NguoiDung? NguoiTao { get; set; }  //Khóa ngoại

        // =========================
        // Navigation Properties
        // =========================

        public ICollection<BenhNhanBenhNen> BenhNhanBenhNens { get; set; }= new List<BenhNhanBenhNen>();

        public ICollection<DiUngThuoc> DiUngThuocs { get; set; } = new List<DiUngThuoc>();

        public ICollection<ThuocDangSuDung> ThuocDangSuDungs { get; set; } = new List<ThuocDangSuDung>();

        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();

    //Tuổi do hệ thống tự động tính
        [NotMapped]
        public int? Tuoi
        {
            get
            {
                if (!NgaySinh.HasValue)
                    return null;

                var tuoi = DateTime.Now.Year - NgaySinh.Value.Year;

                if (NgaySinh.Value.Date > DateTime.Now.AddYears(-tuoi))
                    tuoi--;

                return tuoi;
            }
        }
    }
}