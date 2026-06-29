# Đồ Án Môn Học: Chuyên Đề ASP.NET
## ĐỀ TÀI: WEBSITE DIỄN ĐÀN SỨC KHỎE (MEDFORUM)

---

## 👨‍💻 Thông tin Sinh viên & Giảng viên hướng dẫn
*   **Sinh viên thực hiện:** Trịnh Lê Bá Khánh
*   **Mã số sinh viên (MSSV):** 170124344
*   **Lớp sinh hoạt:** D24TTK3
*   **Giáo viên hướng dẫn (GVHD):** Thầy Đoàn Nguyễn Ngọc Duyên
*   **GitHub Repository:** [ASPNET-D24TTK3-trinhlekhanh-MedForum](https://github.com/trinhlbk121190-cell/-ASPNET-DK24TTK3-TR-NH-L-B-KH-NH-170124344.git)
*   **Tài khoản cộng tác viên GVHD:** `antonio86doan@gmail.com`

---

## 📝 Giới thiệu Đề tài
**MedForum** là một ứng dụng diễn đàn y tế trực tuyến chuyên biệt nhằm kết nối Bệnh nhân và các Bác sĩ chuyên khoa. Khác với các diễn đàn công cộng không có kiểm chứng, MedForum đặt chất lượng chuyên môn y khoa lên hàng đầu:
*   **Bệnh nhân (Patient):** Đăng câu hỏi tư vấn y khoa (ẩn danh hoặc công khai), thảo luận, upvote/downvote nội dung hữu ích, và gửi báo cáo vi phạm.
*   **Bác sĩ (Doctor):** Trả lời tư vấn chuyên môn y tế. Các câu trả lời được ghim lên đầu danh sách phản hồi và có badge "Bác sĩ xác minh" để tạo niềm tin cho người bệnh.
*   **Quản trị viên (Admin):** Duyệt hồ sơ chứng chỉ hành nghề bác sĩ, quản lý phân quyền thành viên, điều phối phân công bác sĩ phụ trách câu hỏi y tế, cấu hình giới thiệu động và theo dõi báo cáo thống kê hoạt động.

---

## 🛠️ Công nghệ sử dụng (Technology Stack)
*   **Backend framework:** ASP.NET Core 10.0 MVC (C#)
*   **Object-Relational Mapping (ORM):** Entity Framework Core (v9.0.2) Code-First
*   **Cơ sở dữ liệu:** Microsoft SQL Server 2019
*   **Xác thực và phân quyền:** ASP.NET Core Identity (Cookie-based Session, Role-based Authorization)
*   **Truyền thông thời gian thực:** ASP.NET Core SignalR (WebSocket / Server-Sent Events / Long Polling)
*   **Frontend UI Framework:** HTML5, CSS3 (phong cách Glassmorphism), Bootstrap v5.3, JavaScript AJAX (jQuery v3.7)

---

## 🏛️ Kiến trúc ứng dụng
Hệ thống được tổ chức theo kiến trúc phân lớp (layered architecture) lấy cảm hứng từ Clean Architecture để tách biệt nghiệp vụ:
1.  **Domain (Core):** Định nghĩa các thực thể nghiệp vụ dữ liệu chính (`User`, `Post`, `Comment`, `Category`, `Vote`, `Report`).
2.  **Infrastructure (Hạ tầng):** Triển khai cấu hình `ApplicationDbContext` sử dụng Fluent API, quản lý các khóa ngoại phức tạp (OnDelete Behavior), các ràng buộc CHECK constraint và seeding dữ liệu mẫu.
3.  **Controllers (Điều phối):** Tiếp nhận HTTP request, gọi các Service và Repository để xử lý nghiệp vụ y tế.
4.  **Views (Giao diện):** Hiển thị Razor templates động kết hợp hiệu ứng Glassmorphism.

---

## 📂 Cấu trúc thư mục Repository trên GitHub
Dự án được cấu trúc theo đúng quy định chung của Học phần Đồ án:
```text
ASPNET-D24TTK3-trinhlekhanh-MedForum/
├── src/                              # Thư mục mã nguồn chính (Source Code)
│   └── MedicalForum.Mvc/             # Dự án ASP.NET Core MVC
│       ├── Controllers/              # Điều hướng & xử lý request
│       ├── Domain/Entities/          # Thực thể dữ liệu
│       ├── Infrastructure/Data/      # DbContext & migrations
│       ├── Views/                    # razor views (.cshtml)
│       └── wwwroot/                  # css, js tĩnh
├── setup/                            # Dữ liệu thử nghiệm và kiểm tra môi trường
│   ├── check-env.ps1                 # Script PowerShell kiểm tra môi trường cài đặt
│   ├── test-accounts.md              # Danh sách tài khoản kiểm thử
│   └── README.md                     # Hướng dẫn chi tiết các bước setup
├── thesis/                           # Tài liệu Đồ án hoàn chỉnh nộp bài
│   ├── doc/                          # Chứa file .doc và .docx báo cáo đồ án
│   ├── pdf/                          # Chứa file .pdf
│   ├── html/                         # Tài liệu dạng web
│   ├── abs/                          # Chứa slide thuyết trình, video demo
│   └── refs/                         # Chứa tài liệu tham khảo gốc
├── progress-report/                  # Nhật ký tiến độ hàng tuần
└── README.md                         # Hướng dẫn tổng quan (file này)
```

---

## 🚀 Hướng dẫn cài đặt và Chạy ứng dụng

### 📋 Yêu cầu tiên quyết
*   Máy tính đã cài đặt **.NET 10 SDK**.
*   Đã cài đặt **Microsoft SQL Server 2019** (hoặc bản LocalDB).
*   Công cụ lập trình **Visual Studio 2022** hoặc VS Code.

### 💻 Các bước cài đặt chi tiết

1.  **Clone mã nguồn từ GitHub:**
    ```bash
    git clone https://github.com/trinhlbk121190-cell/-ASPNET-DK24TTK3-TR-NH-L-B-KH-NH-170124344.git
    cd -ASPNET-DK24TTK3-TR-NH-L-B-KH-NH-170124344
    ```

2.  **Cấu hình kết nối CSDL (Connection String):**
    Mở file `src/MedicalForum.Mvc/appsettings.json`, cập nhật thông tin máy chủ SQL Server của bạn tại mục `DefaultConnection`:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MedForumDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
      }
    }
    ```

3.  **Tạo cơ sở dữ liệu và chạy Migrations:**
    Mở terminal tại thư mục `src/MedicalForum.Mvc/` và thực thi lệnh:
    ```bash
    dotnet ef database update
    ```

4.  **Khởi động ứng dụng:**
    Chạy lệnh dotnet run:
    ```bash
    dotnet run --urls "http://localhost:5050"
    ```

5.  **Truy cập hệ thống:**
    Mở trình duyệt web của bạn và truy cập địa chỉ: [http://localhost:5050](http://localhost:5050)

---

## 👥 Danh sách tài khoản kiểm thử (Demo Accounts)
Hệ thống đã tự động seeding sẵn các dữ liệu mẫu và tài khoản kiểm thử cho các vai trò để giảng viên thuận tiện đánh giá:

| Vai trò | Email đăng nhập | Mật khẩu | Chuyên khoa / Quyền hạn |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@medforum.com` | `Admin@123456` | Toàn quyền kiểm trị, phê duyệt, gán bác sĩ |
| **Bác sĩ** | `doctor1@medforum.com` | `Doctor@123456` | Chuyên khoa Tim mạch (Đã xác minh) |
| **Bác sĩ** | `doctor2@medforum.com` | `Doctor@123456` | Chuyên khoa Da liễu (Đã xác minh) |
| **Bác sĩ** | `doctor3@medforum.com` | `Doctor@123456` | Chuyên khoa Nhi khoa (Đã xác minh) |
| **Bệnh nhân** | `patient@medforum.com` | `Patient@123456` | Tài khoản bệnh nhân mẫu dùng để hỏi đáp |

---

## 🏆 Tính năng đặc sắc / Điểm sáng kỹ thuật
1.  **Thuật toán Tự động gán Bác sĩ phụ trách:** Khi bệnh nhân đăng câu hỏi y tế, hệ thống dựa vào danh mục và từ khóa chuyên môn y khoa để tìm kiếm bác sĩ phù hợp, kết hợp kỹ thuật cân bằng tải (load balancing) để phân chia đều số lượng câu hỏi cần phụ trách cho từng bác sĩ.
2.  **Ràng buộc cơ sở dữ liệu (CHECK Constraints):** Áp dụng CHECK constraints nghiêm ngặt tại mức cơ sở dữ liệu (bảng `Votes` và `Reports`) để đảm bảo một lượt upvote hoặc báo cáo chỉ trỏ đến đúng một bài viết hoặc một bình luận, không bao giờ xảy ra lỗi dữ liệu mồ côi.
3.  **Xử lý cascade paths đa tuyến:** Cấu hình Fluent API `OnDelete(DeleteBehavior.NoAction)` cho khóa ngoại `AssignedDoctorId` để vượt qua giới hạn tự nhiên của SQL Server khi có hai khóa ngoại cùng trỏ về một bảng.
4.  **SignalR & AJAX Real-time:** Bình chọn upvote/downvote cập nhật số liệu ngay lập tức đến mọi client đang kết nối mà không cần tải lại trang.
5.  **Tìm kiếm tương đối (Partial Match):** Dịch chuyển linh hoạt truy vấn LINQ sang toán tử `LIKE` trong SQL Server để tìm kiếm kết quả tương đối chính xác.
