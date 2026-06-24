using HeTHongDanhGiaThuoc.Models.Requests;

namespace HeTHongDanhGiaThuoc.Services
{
    public interface IDanhGiaService
    {
        Task<int> TaoDanhGiaAsync(TaoDanhGiaRequest request);
    }
}