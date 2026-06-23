using HeTHongDanhGiaThuoc.Data;
using HeTHongDanhGiaThuoc.Models;
using Microsoft.EntityFrameworkCore;
using HeTHongDanhGiaThuoc.Models.Requests;
namespace HeTHongDanhGiaThuoc.Services
{
    public class DanhGiaService : IDanhGiaService
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaService(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Thực hiện đánh giá mức độ phù hợp của phác đồ thuốc đối với bệnh nhân.
        /// Quy trình:
        /// 1. Kiểm tra chống chỉ định với bệnh nền.
        /// 2. Kiểm tra dị ứng hoạt chất(Xét mức độ từ nhẹ -> nghiêm trọng)
        /// 3. Kiểm tra tương tác giữa các thuốc(Xét mức độ từ thấp -> nghiêm trọng)
        /// 4. Kiểm tra yếu tố bệnh nhân (tuổi, cân nặng).
        /// 5. Kiểm tra liều lượng và tần suất sử dụng.
        /// 6. Tính tỷ lệ phù hợp và sinh khuyến nghị.   
        public async Task<int> TaoDanhGiaAsync(TaoDanhGiaRequest request)
        {
            // ======================================
            // 1. LẤY BỆNH NHÂN
            // ======================================
            var benhNhan = await _context.HoSoBenhNhans //Truy vấn database
                .Include(h => h.BenhNhanBenhNens).ThenInclude(x => x.BenhNen)//Lấy bệnh nền 
                .Include(h => h.DiUngThuocs).ThenInclude(d => d.HoatChat)//Lấy hoạt chất
                .Include(h => h.ThuocDangSuDungs).ThenInclude(t => t.Thuoc)
                .FirstOrDefaultAsync(h => h.MaBenhNhan == request.MaBenhNhan);//Tìm bệnh nhân theo ID

            if (benhNhan == null)
                throw new Exception("Không tìm thấy bệnh nhân");

            // ======================================
            // 2. LẤY THUỐC
            // ======================================
            var maThuocList = request.Thuocs.Select(x => x.MaThuoc).ToList();//Lấy danh sách mã thuốc

            var thuocDachon = await _context.Thuocs
                .Include(t => t.ThuocHoatChats).ThenInclude(th => th.HoatChat)//Lấy hoạt chất của thuốc
                .Include(t => t.ChongChiDinhs).ThenInclude(c => c.BenhNen)//Lấy bệnh nền chống chỉ định của thuốc
                                                                          //Các tương tác thuốc
                .Include(t => t.TuongTacA).ThenInclude(tt => tt.ThuocB)
                .Include(t => t.TuongTacB).ThenInclude(tt => tt.ThuocA)

                .Where(t => maThuocList.Contains(t.MaThuoc))
                .ToListAsync();

            // ======================================
            // INIT
            // ======================================
            var chiTiets = new List<ChiTietDanhGia>(); //Tạo danh sách chi tiết danh giá
            var canhBaos = new List<string>();//Tạo danh sách cảnh báo

            int tongThuocDanhGia = thuocDachon.Count; //Tạo tổng thuốc phù hợpf
            int soThuocPhuHop = 0; //Mặc định sẽ là = 0

            var hoatChatDiUngIds = benhNhan.DiUngThuocs//Lấy ds dị ứng (dị ứng với những hoạt chất nào)
                .Select(x => x.MaHoatChat)
                .ToHashSet();

            int? tuoi = benhNhan.Tuoi; //Tuổi của bệnh nhân
            decimal? canNang = benhNhan.CanNang.HasValue //Cân nặng của bệnh nhân
                ? (decimal?)benhNhan.CanNang.Value
                : null;

            var thuocBenhNhanDangDung = benhNhan.ThuocDangSuDungs
                .Select(x => x.Thuoc)
                .Where(x => x != null)
                .ToList();

            var tatCaThuocLienQuan = thuocDachon
                .Concat(thuocBenhNhanDangDung)
                .DistinctBy(x => x.MaThuoc)
                .ToList();
            // ======================================
            // 3. XỬ LÝ TỪNG THUỐC
            // ======================================
            foreach (var input in request.Thuocs) //Tiến hành xét từng loại thuốc trong ds thuốc chọn
            {
                var thuoc = thuocDachon.FirstOrDefault(x => x.MaThuoc == input.MaThuoc);//Lấy thuốc theo mã thuốc
                if (thuoc == null) continue;

                var lyDos = new List<string>();//Tạo ds lý do để tiến hành đánh giá
                int score = 100;//Điểm phù hợp mặc định là 100 (100% phù hợp)
                bool biLoai = false;//Mặc định thuốc xét sẽ là phù hợp 

                decimal? lieuMg = input.LieuMg; //Liều dùng trong ngày
                int? soLan = input.SoLanMoiNgay;//Số lần dùng trong ngày

                // ======================================
                // 3.1 CHỐNG CHỈ ĐỊNH (LOẠI LUÔN)
                // ======================================
                foreach (var ccd in thuoc.ChongChiDinhs)//Xét thuốc với từng Chống chỉ định của bệnh nhân
                {
                    var tenBenh = ccd.BenhNen?.TenBenhNen?.ToLower();//Lấy tên bệnh nền

                    if (!string.IsNullOrEmpty(tenBenh) && //Nếu kiểm tra thấy bệnh nền bệnh nhân = chống chỉ định của thuốc
                        benhNhan.BenhNhanBenhNens.Any(b =>
                            b.BenhNen!.TenBenhNen.ToLower().Contains(tenBenh) ||
                            tenBenh.Contains(b.BenhNen!.TenBenhNen.ToLower())))
                    {
                        score = 0; //Điểm sẽ là 0 (0% phù hợp)
                        biLoai = true;//Loại luôn!

                        //Hiển thị lý do ra view
                        lyDos.Add($"CHỐNG CHỈ ĐỊNH: {ccd.BenhNen?.TenBenhNen}");
                        canhBaos.Add($"[{thuoc.TenThuoc}] ⛔ CHỐNG CHỈ ĐỊNH");
                        break;
                    }
                }

                if (biLoai) //Nếu không bị trùng (ko bị lỗi) => (biLoai = false) thì tiếp tục 
                {
                    chiTiets.Add(new ChiTietDanhGia //Thêm vào danh sách đánh giá và tiếp tục xét tiếp
                    {
                        MaThuoc = thuoc.MaThuoc,
                        LieuDungChiDinh = input.LieuDung,
                        LieuMoiLanMg = lieuMg,
                        SoLanMoiNgay = soLan,
                        CoPhuHopKhong = false,
                        LyDoKhongPhuHop = string.Join("; ", lyDos),
                        DiemPhuHop = 0
                    });

                    continue;
                }

                // ======================================
                // 3.2 DỊ ỨNG HOẠT CHẤT (THEO MỨC ĐỘ CAO NHẤT)
                // ======================================

                // Lấy danh sách mã hoạt chất của thuốc hiện tại
                var hoatChatThuoc = thuoc.ThuocHoatChats
                    .Select(x => x.MaHoatChat)
                    .ToHashSet();

                // Tìm các hoạt chất của thuốc mà bệnh nhân bị dị ứng
                var dsDiUngTrung = benhNhan.DiUngThuocs
                    .Where(d => hoatChatThuoc.Contains(d.MaHoatChat))
                    .ToList();

                // Nếu tồn tại dị ứng
                if (dsDiUngTrung.Any())
                {
                    // Lấy dị ứng có mức độ nghiêm trọng nhất
                    var diUngNangNhat = dsDiUngTrung
                        .OrderByDescending(x => MapDiUngScore(x.MucDoDiUng))
                        .First();

                    // Tính số điểm bị trừ
                    int truDiem = MapDiUngScore(diUngNangNhat.MucDoDiUng);

                    score -= truDiem; //Vd Dị ứng hoạt chất X: Nặng!
                                      // 100 - 80 = 20 (Còn 20% phù hợp)

                    // Ghi nhận lý do đánh giá
                    lyDos.Add(
                        $"DỊ ỨNG HOẠT CHẤT: {diUngNangNhat.HoatChat?.TenHoatChat} ({diUngNangNhat.MucDoDiUng})");

                    // Ghi cảnh báo
                    canhBaos.Add(
                        $"[{thuoc.TenThuoc}] ⚠ Dị ứng {diUngNangNhat.HoatChat?.TenHoatChat} ({diUngNangNhat.MucDoDiUng})");
                }
                // ======================================
                // 3.3 TƯƠNG TÁC THUỐC
                // ======================================
                //Xét ở trường hợp thuốc trong danh sách đánh giá
                //Xét ở trường hợp thuốc đang sử dụng của bệnh nhân <---> Thuốc đang xét ở đánh giá
                // Duyệt toàn bộ thuốc còn lại trong đơn

                // ======================================
                // 3.3 TƯƠNG TÁC THUỐC
                // ======================================
                //Xét ở trường hợp thuốc trong danh sách đánh giá
                //Xét ở trường hợp thuốc đang sử dụng của bệnh nhân <---> Thuốc đang xét ở đánh giá

                // 1. Thuốc trong đơn ↔ thuốc trong đơn
                foreach (var thuocKhac in thuocDachon)
                {
                    // Bỏ qua chính nó
                    if (thuocKhac.MaThuoc == thuoc.MaThuoc)
                        continue;

                    // Tìm tương tác giữa 2 thuốc
                    // Có thể nằm ở chiều A->B hoặc B->A
                    var tuongTac =
                        thuoc.TuongTacA.FirstOrDefault(x => x.MaThuocB == thuocKhac.MaThuoc)
                        ?? thuoc.TuongTacB.FirstOrDefault(x => x.MaThuocA == thuocKhac.MaThuoc);

                    // Không có tương tác thì bỏ qua
                    if (tuongTac == null)
                        continue;

                    // Lấy mức độ tương tác từ database
                    string mucDo = tuongTac.MucDoNghiemTrong ?? "Thấp";

                    // Quy đổi mức độ sang số điểm bị trừ
                    int truDiem = MapTuongTacThuocScore(mucDo);

                    // Trừ điểm phù hợp
                    score -= truDiem; //vd Thuốc A <--> Thuốc B => Nghiêm trọng
                                      //100 - 100 = 0 (Còn 0% phù hợp)

                    // Ghi lại lý do để hiển thị trong kết quả đánh giá
                    lyDos.Add(
                        $"TƯƠNG TÁC VỚI THUỐC TRONG ĐƠN: {thuocKhac.TenThuoc} ({mucDo})");

                    // Ghi cảnh báo y tế
                    canhBaos.Add(
                        $"[{thuoc.TenThuoc}] ↔ {thuocKhac.TenThuoc} ({mucDo})");
                }

                // 2. Thuốc trong đơn ↔ thuốc bệnh nhân đang dùng
                foreach (var thuocDangDung in thuocBenhNhanDangDung)
                {
                    // Bỏ qua nếu thuốc đang dùng trùng với thuốc đang xét trong đơn mới
                    if (thuocDangDung.MaThuoc == thuoc.MaThuoc)
                        continue;

                    var tuongTac =
                        thuoc.TuongTacA.FirstOrDefault(x => x.MaThuocB == thuocDangDung.MaThuoc)
                        ?? thuoc.TuongTacB.FirstOrDefault(x => x.MaThuocA == thuocDangDung.MaThuoc);

                    if (tuongTac == null) continue;

                    string mucDo = tuongTac.MucDoNghiemTrong ?? "Thấp";
                    int truDiem = MapTuongTacThuocScore(mucDo);

                    score -= truDiem;

                    lyDos.Add($"TƯƠNG TÁC VỚI THUỐC ĐANG DÙNG: {thuocDangDung.TenThuoc} ({mucDo})");
                    canhBaos.Add($"[{thuoc.TenThuoc}] ↔ {thuocDangDung.TenThuoc} ({mucDo})");
                }

                // ======================================
                // 3.4 YẾU TỐ BỆNH NHÂN
                // ======================================

                //Xét Theo Tuổi
                // Tuổi nhỏ hơn tuổi tối thiểu của thuốc
                if (tuoi.HasValue && thuoc.TuoiToiThieu.HasValue) //Kiểm tra tuổi tối thiểu ở bệnh nhân và thuốc có tồn tại ko?
                {
                    if (tuoi.Value < thuoc.TuoiToiThieu.Value)//Xét tuổi bệnh nhân < tuổi tối thieu của thuốc
                    {
                        score -= 40;

                        lyDos.Add(
                            $"TUỔI THẤP HƠN MỨC CHO PHÉP ({thuoc.TuoiToiThieu})");
                    }
                }

                // Tuổi lớn hơn tuổi tối đa của thuốc
                if (tuoi.HasValue && thuoc.TuoiToiDa.HasValue)//Kiểm tra cân tuổi thiểu ở bệnh nhân và thuốc có tồn tại ko?
                {
                    if (tuoi.Value > thuoc.TuoiToiDa.Value) //Xét tuổi  bệnh nhân > cân nặng tối thieu của thuốc?
                    {
                        score -= 20;

                        lyDos.Add(
                            $"TUỔI CAO HƠN MỨC KHUYẾN NGHỊ ({thuoc.TuoiToiDa})");
                    }
                }

                //Xét Theo Cân Nặng
                // Cân nặng thấp hơn mức tối thiểu
                if (canNang.HasValue && thuoc.CanNangToiThieu.HasValue)//Kiểm tra cân nặng tối thiểu ở bệnh nhân và thuốc có tồn tại ko?
                {
                    if (canNang.Value < thuoc.CanNangToiThieu.Value)//Xét cân nặng bệnh nhân < cân nặng tối thieu của thuốc?
                    {
                        score -= 30;

                        lyDos.Add(
                            $"CÂN NẶNG THẤP HƠN MỨC CHO PHÉP ({thuoc.CanNangToiThieu}kg)");
                    }
                }

                // Cân nặng cao hơn mức tối đa
                if (canNang.HasValue && thuoc.CanNangToiDa.HasValue)//Kiểm tra cân nặng tối thiểu ở bệnh nhân và thuốc có tồn tại ko?
                {
                    if (canNang.Value > thuoc.CanNangToiDa.Value)//Xét cân nặng bệnh nhân > cân nặng tối thieu của thuốc?
                    {
                        score -= 10;

                        lyDos.Add(
                            $"CÂN NẶNG CAO HƠN MỨC KHUYẾN NGHỊ ({thuoc.CanNangToiDa}kg)");
                    }
                }
                // ======================================
                // 3.5 LIỀU DÙNG
                // ======================================

                // Tính tổng liều sử dụng trong ngày
                //
                // Công thức:
                // Tổng liều/ngày = Liều mỗi lần × Tần suất/ngày
                //
                // Ví dụ:
                // 500mg × 3 lần = 1500mg/ngày
                //
                decimal? tongLieuNgay = null;

                if (lieuMg.HasValue && soLan.HasValue)
                {
                    tongLieuNgay = lieuMg.Value * soLan.Value; //Công thức tính Tổng Liều(đây là tổng liều do user nhập lúc đánh giá)
                    //Dùng để xét với thông tin Liều khuyến nghị của thuốc trong hệ thống
                    //Không phải liều mặc định của thuốc trong hệ thống
                }

                // ======================================
                // KIỂM TRA LIỀU MỖI LẦN
                // ======================================
                //
                // Ví dụ thuốc:
                //Đây là Liều được ghi ở mỗi Thuốc
                // Liều tối thiểu : 250mg/lần
                // Liều khuyến nghị: 500mg/lần
                // Liều tối đa    : 1000mg/lần
                //
                    if (lieuMg.HasValue) // Dùng liều mỗi lần do người dùng nhập Xét với liều mỗi lần dùng thực tế của thuốc
                {
                    // ======================================
                    // THẤP HƠN LIỀU TỐI THIỂU (NGUY CƠ THIẾU LIỀU)
                    // ======================================
                    if (thuoc.LieuToiThieuMg.HasValue &&
                        lieuMg.Value < thuoc.LieuToiThieuMg.Value)
                    // Vd: user nhập 100mg < 250mg
                    {
                        score -= 10; // Trừ điểm nhẹ và đưa cảnh báo

                        lyDos.Add(
                            $"LIỀU MỖI LẦN THẤP ({lieuMg}mg < {thuoc.LieuToiThieuMg}mg)"
                        );
                    }

                    // ======================================
                    // VƯỢT LIỀU KHUYẾN NGHỊ (CHẤP NHẬN ĐƯỢC NHƯNG KHÔNG TỐI ƯU)
                    // ======================================
                    if (thuoc.LieuKhuyenNghiMg.HasValue &&
                        lieuMg.Value > thuoc.LieuKhuyenNghiMg.Value)
                    // Vd: user nhập 750mg > 500mg
                    {
                        score -= 15; // Trừ trung bình và đưa cảnh báo

                        lyDos.Add(
                            $"VƯỢT LIỀU KHUYẾN NGHỊ MỖI LẦN ({lieuMg}mg > {thuoc.LieuKhuyenNghiMg}mg)"
                        );
                    }
                    // ======================================
                    // VƯỢT LIỀU TỐI ĐA (NGUY HIỂM)
                    // ======================================
                    if (thuoc.LieuToiDaMg.HasValue &&
                        lieuMg.Value > thuoc.LieuToiDaMg.Value)
                    // Vd: user nhập 1200mg > 1000mg
                    {
                        score -= 30; // Trừ mạnh và đưa cảnh báo

                        lyDos.Add(
                            $"VƯỢT LIỀU TỐI ĐA MỖI LẦN ({lieuMg}mg > {thuoc.LieuToiDaMg}mg)"
                        );
                    }
                }

                // ======================================
                // KIỂM TRA TẦN SUẤT DÙNG THUỐC
                // ======================================
                //
                // Ví dụ:
                // Thuốc cho phép tối đa 3 lần/ngày
                //
                // Người dùng nhập cho bệnh nhân dùng 4 lần/ngày (Lúc thực hiện đánh giá!)
                //
                if (soLan.HasValue &&
                    thuoc.TanSuatToiDaMoiNgay.HasValue &&
                    soLan.Value > thuoc.TanSuatToiDaMoiNgay.Value)
                {// Vd: User nhập khi đánh giá 4 lần/ngày > 3 lần/ngày của thuốc trong hệ thống
                    score -= 20;

                    lyDos.Add(
                        $"VƯỢT TẦN SUẤT ({soLan} lần > {thuoc.TanSuatToiDaMoiNgay} lần/ngày)"
                    );
                }

                // ======================================
                // KIỂM TRA TỔNG LIỀU TRONG NGÀY
                // ======================================
                //
                // Ví dụ dữ liệu thuốc trong hệ thống:
                //
                // Liều khuyến nghị = 500mg/lần
                // Liều tối đa = 1000mg/lần
                // Tần suất tối đa = 3 lần/ngày

                // => Tổng khuyến nghị: (Đã được hệ thống tính)
                // 500 × 3 = 1500mg/ngày
                //
                // => Tổng tối đa: (Đã được hệ thống tính)
                // 1000 × 3 = 3000mg/ngày
                //
                // --------------------------------------
                // Ví dụ Khi thực hiện đánh giá thuốc:
                //Khi thực hiện đánh giá: User sẽ nhập Liều dùng/Lần và Tần suất dùng/ngày cho mỗi loại thuốc đánh giá
                //Tổng liều ngày (user nhập) = Liều dùng/Lần × Tần suất dùng/ngày
                //=> Dùng KQ đó để Xét với Tổng Liều của hệ thống (Basic của Thuốc)

                // <= 1500mg
                //      => Tốt, không trừ điểm
                //
                // > 1500mg và <= 3000mg
                //      => Chưa nguy hiểm nhưng cao hơn khuyến nghị
                //
                // > 3000mg
                //      => Vượt giới hạn an toàn
                // --------------------------------------
                //
                if (tongLieuNgay.HasValue)
                {
                    // Vượt tổng liều tối đa/ngày
                    if (thuoc.TongLieuToiDaNgay > 0 &&
                        tongLieuNgay.Value > thuoc.TongLieuToiDaNgay)
                    { //Tổng liều ngày do User nhập khi đánh giá > Tổng Liều Tối Đa ngày trong hệ thống
                        score -= 40;

                        lyDos.Add(
                            $"VƯỢT TỔNG LIỀU TỐI ĐA ({tongLieuNgay}mg > {thuoc.TongLieuToiDaNgay}mg/ngày)"
                        );
                    }

                    // Cao hơn mức khuyến nghị nhưng vẫn trong ngưỡng an toàn
                    else if (thuoc.TongLieuKhuyenNghiNgay > 0 &&
                             tongLieuNgay.Value > thuoc.TongLieuKhuyenNghiNgay)
                    {//Tổng liều ngày do User nhập khi đánh giá > Tổng Liều Khuyến Nghị ngày trong hệ thống
                        score -= 15;

                        lyDos.Add(
                            $"CAO HƠN TỔNG LIỀU KHUYẾN NGHỊ ({tongLieuNgay}mg > {thuoc.TongLieuKhuyenNghiNgay}mg/ngày)"
                        );
                    }
                }


                // ======================================
                // 3.7. TỔNG HỢP KẾT QUẢ ĐÁNH GIÁ TỪNG THUỐC
                // ======================================
                //
                // Sau khi đã tính score cho từng thuốc (3.x),
                // hệ thống chuẩn hoá điểm và xác định thuốc phù hợp
                // để đưa vào kết quả tổng thể.
                //

                // Giới hạn điểm trong khoảng 0 - 100
                // => đảm bảo không âm hoặc vượt 100 do cộng/trừ nhiều yếu tố
                if (score < 0) score = 0;
                if (score > 100) score = 100;

                // Xác định thuốc có phù hợp hay không
                // Ngưỡng >= 60 được xem là đạt yêu cầu sử dụng
                bool phuHop = score >= 60;

                // Đếm số thuốc đạt yêu cầu trong toàn đơn
                if (phuHop)
                    soThuocPhuHop++;

                // Lưu chi tiết đánh giá của từng thuốc
                // Bao gồm:
                // - liều dùng
                // - số lần dùng
                // - tổng liều/ngày
                // - kết luận phù hợp hay không
                // - danh sách lý do bị trừ điểm
                chiTiets.Add(new ChiTietDanhGia
                {
                    MaThuoc = thuoc.MaThuoc,

                    // Liều bệnh nhân nhập cho 1 lần dùng
                    LieuDungChiDinh = input.LieuDung,

                    // Liều mỗi lần (mg)
                    LieuMoiLanMg = lieuMg,

                    // Số lần dùng trong ngày
                    SoLanMoiNgay = soLan,

                    // Tổng liều/ngày = liều mỗi lần × số lần/ngày
                    TongLieuNgayMg = tongLieuNgay,

                    // Kết luận thuốc có phù hợp hay không
                    CoPhuHopKhong = phuHop,

                    // Chuỗi lý do không phù hợp (nếu có)
                    LyDoKhongPhuHop = lyDos.Any()
                        ? string.Join("; ", lyDos)
                        : null,

                    // Điểm đánh giá cuối cùng sau khi tổng hợp tất cả yếu tố
                    DiemPhuHop = score
                });
            }

            // ======================================
            // 4. TÍNH TỶ LỆ PHÙ HỢP TOÀN PHIÊN ĐÁNH GIÁ
            // ======================================
            //
            // Công thức:
            // Tỷ lệ (%) = (Số thuốc phù hợp / Tổng số thuốc) × 100
            //
            decimal tyLe = tongThuocDanhGia > 0
                ? Math.Round((decimal)soThuocPhuHop / tongThuocDanhGia * 100, 1)
                : 0;
        

            // ======================================
            // 5. LƯU PHIÊN ĐÁNH GIÁ VÀO DATABASE
            // ======================================
            //
            // Tổng hợp toàn bộ kết quả đánh giá của bệnh nhân
            // trong một phiên làm việc.
            //
            var danhGia = new DanhGia
            {
                MaBenhNhan = request.MaBenhNhan,
                MaNguoiDanhGia = request.MaNguoiDung,

                // Thời điểm thực hiện đánh giá
                NgayDanhGia = DateTime.Now,

                // % thuốc phù hợp trong toàn đơn
                TyLePhuHop = tyLe,

                // Cảnh báo tổng hợp từ tất cả thuốc
                CanhBaoYTe = canhBaos.Any()
                    ? string.Join("\n", canhBaos)
                    : "Không có cảnh báo",

                // Kết luận tự động dựa trên tỷ lệ phù hợp
                KhuyenNghiSuDung =
                    tyLe >= 80 ? "An toàn" :
                    tyLe >= 50 ? "Cần xem xét" :
                    "Nguy cơ cao",

                // Ghi chú từ người đánh giá
                GhiChuPhienDanhGia = request.GhiChu,

                // Chi tiết từng thuốc trong phiên đánh giá
                ChiTietDanhGias = chiTiets
            };

            // Lưu vào database
            _context.DanhGias.Add(danhGia);
            await _context.SaveChangesAsync();

            // Trả về mã phiên đánh giá
            return danhGia.MaDanhGia;
        }



        private int MapDiUngScore(string? mucDo)
        {
            return mucDo switch
            {
                "Nhẹ" => 20,
                "Trung bình" => 50,
                "Nặng" => 80,
                "Nghiêm trọng" => 100,
                _ => 20
            };
        }

        private int MapTuongTacThuocScore(string? mucDo)
        {
            return mucDo switch
            {
                "Nghiêm trọng" => 80,
                "Cao" => 50,
                "Trung bình" => 25,
                "Thấp" => 10,
                _ => 10
            };
        }
    }
}
