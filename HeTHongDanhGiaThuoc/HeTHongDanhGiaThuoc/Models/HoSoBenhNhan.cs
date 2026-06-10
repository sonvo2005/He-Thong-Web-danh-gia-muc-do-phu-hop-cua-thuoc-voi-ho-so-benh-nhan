using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeTHongDanhGiaThuoc.Models
{
    public class HoSoBenhNhan
    {
        [Key]
        public int MaBenhNhan { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Mã định danh")]
        public string MaDinhDanhBenhNhan { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [MaxLength(255)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Người tạo")]
        public int? MaNguoiTao { get; set; }

        [Display(Name = "Ngày tiếp nhận")]
        public DateTime NgayTiepNhan { get; set; } = DateTime.Now;

        [ForeignKey("MaNguoiTao")]
        public virtual NguoiDung? NguoiTao { get; set; }

        public virtual ICollection<BenhNen> BenhNens { get; set; } = new List<BenhNen>();
        public virtual ICollection<DiUngThuoc> DiUngThuocs { get; set; } = new List<DiUngThuoc>();
        public virtual ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}