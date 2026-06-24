# Hệ Thống Đánh Giá Mức Độ Phù Hợp Thuốc

Web ASP.NET Core MVC (.NET 8) — đánh giá mức độ phù hợp của phác đồ thuốc với hồ sơ bệnh nhân.

---

## ⚙️ Cài đặt & Chạy dự án

### Bước 1 — Cấu hình chuỗi kết nối database

Mở file `appsettings.json`, sửa `DefaultConnection`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TEN_SERVER;Database=HeTHongDanhGiaThuocDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

**Ví dụ theo từng trường hợp:**
| Trường hợp | Connection String |
|---|---|
| SQL Server local (Windows Auth) | `Server=.;Database=HeTHongDanhGiaThuocDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server với tên instance | `Server=.\SQLEXPRESS;Database=HeTHongDanhGiaThuocDB;Trusted_Connection=True;TrustServerCertificate=True;` |
| SQL Server với user/password | `Server=.;Database=HeTHongDanhGiaThuocDB;User Id=sa;Password=Matkhau123;TrustServerCertificate=True;` |

### Bước 2 — Khôi phục database từ file SQL

Chạy file `SQL.sql` trong SQL Server Management Studio (SSMS):
1. Mở SSMS → New Query
2. Mở file `SQL.sql` → Execute

### Bước 3 — Restore packages & chạy migration

Mở Terminal trong VS Code:

```bash
# Khôi phục packages
dotnet restore

# Tạo migration ban đầu (nếu chưa có database, dùng lệnh này)
dotnet ef migrations add InitialCreate

# Cập nhật database (tạo bảng + seed data)
dotnet ef database update

# Chạy ứng dụng
dotnet run
```

> **Lưu ý:** Nếu đã chạy file SQL.sql thì **không cần** chạy `ef migrations add` và `ef database update`. Chỉ cần `dotnet run`.

### Bước 4 — Đăng nhập

Truy cập: `https://localhost:5001` hoặc `http://localhost:5000`

| Tài khoản | Mật khẩu |
|---|---|
| `admin` | `Admin@123` |

---

## 🏗️ Cấu trúc MVC

```
HeTHongDanhGiaThuoc/
├── Controllers/
│   ├── AccountController.cs    # Đăng nhập / đăng xuất
│   ├── HomeController.cs       # Dashboard tổng quan
│   ├── HoSoBenhNhanController.cs # CRUD hồ sơ bệnh nhân
│   ├── ThuocController.cs      # CRUD danh mục thuốc
│   ├── DanhGiaController.cs    # Tạo & xem kết quả đánh giá
│   └── AdminController.cs      # Quản trị: user, hoạt chất, tương tác
│
├── Models/
│   ├── VaiTro.cs
│   ├── NguoiDung.cs
│   ├── HoSoBenhNhan.cs
│   ├── BenhNen.cs
│   ├── HoatChat.cs
│   ├── DiUngThuoc.cs
│   ├── Thuoc.cs
│   ├── ThuocHoatChat.cs
│   ├── TuongTacThuoc.cs
│   ├── ChongChiDinh.cs
│   ├── DanhGia.cs
│   ├── ChiTietDanhGia.cs
│   └── ViewModels/ViewModels.cs
│
├── Data/
│   └── ApplicationDbContext.cs # EF Core DbContext + cấu hình quan hệ
│
├── Views/
│   ├── Shared/_Layout.cshtml   # Layout chính với navbar
│   ├── Account/                # Login, AccessDenied
│   ├── Home/                   # Dashboard
│   ├── HoSoBenhNhan/           # Index, Create, Edit, Details
│   ├── Thuoc/                  # Index, Create, Edit, Details
│   ├── DanhGia/                # Index, Create, KetQua
│   └── Admin/                  # NguoiDung, HoatChat, TuongTacThuoc
│
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
│
├── appsettings.json            # ← SỬA CONNECTION STRING TẠI ĐÂY
└── Program.cs
```

---

## 🔑 Phân quyền

| Vai trò | Quyền |
|---|---|
| **Admin** | Toàn quyền: CRUD tất cả + quản lý user, hoạt chất, tương tác |
| **Bác sĩ** | Xem/thêm/sửa hồ sơ bệnh nhân, tạo đánh giá, xóa đánh giá |
| **Dược sĩ** | Xem/thêm/sửa danh mục thuốc, tạo đánh giá |
| **Y tá** | Xem hồ sơ, tạo đánh giá |

---

## 🧠 Logic đánh giá

Khi tạo phiên đánh giá, hệ thống tự động kiểm tra:

1. **Dị ứng hoạt chất** — thuốc có chứa hoạt chất bệnh nhân dị ứng không?
2. **Chống chỉ định bệnh nền** — bệnh nền của bệnh nhân có nằm trong danh sách chống chỉ định của thuốc không?
3. **Tương tác thuốc** — trong danh sách thuốc được chỉ định, có cặp thuốc nào tương tác nghiêm trọng không?

Kết quả trả về:
- **Tỷ lệ phù hợp (%)** = số thuốc phù hợp / tổng số thuốc × 100
- **Cảnh báo y tế** chi tiết theo từng vấn đề
- **Khuyến nghị lâm sàng** tự động

---

## 📦 Packages sử dụng

| Package | Mục đích |
|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | ORM kết nối SQL Server |
| `BCrypt.Net-Next` | Mã hóa mật khẩu |
| `Microsoft.AspNetCore.Authentication.Cookies` | Xác thực cookie |
| Bootstrap 5 (CDN) | UI framework |
| Font Awesome 6 (CDN) | Icons |
