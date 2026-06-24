namespace HeTHongDanhGiaThuoc.Models.Requests
{
    public class TaoDanhGiaRequest
    {
        public int MaBenhNhan { get; set; }
        public int MaNguoiDung { get; set; }
        public string? GhiChu { get; set; }

        public List<ThuocDanhGiaInput> Thuocs { get; set; } = new();
    }

    public class ThuocDanhGiaInput
    {
        public int MaThuoc { get; set; }
        public decimal? LieuMg { get; set; }
        public int? SoLanMoiNgay { get; set; }
        public string? LieuDung { get; set; }
    }
}