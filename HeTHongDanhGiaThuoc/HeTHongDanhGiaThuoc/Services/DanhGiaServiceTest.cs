// File: DanhGiaServiceTests.cs

public class DanhGiaServiceTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task TaoDanhGia_ThuocChongChiDinh_DiemBang0()
    {
        // Arrange
        // Tạo bệnh nhân có bệnh nền suy thận
        // Tạo thuốc có chống chỉ định suy thận

        // Act
        // Gọi hàm TaoDanhGiaAsync()

        // Assert
        // Kiểm tra điểm = 0 và thuốc bị loại
    }

    [Fact]
    public async Task TaoDanhGia_ThuocPhuHop_TyLe100Phan()
    {
        // Arrange
        // Bệnh nhân không có bệnh nền và dị ứng
        // Thuốc không có chống chỉ định hoặc tương tác

        // Act
        // Thực hiện đánh giá

        // Assert
        // Tỷ lệ phù hợp = 100%
    }
}