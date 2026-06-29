# BÁO CÁO ĐỒ ÁN MÔN HỌC: CHUYÊN ĐỀ ASP.NET
## ĐỀ TÀI: XÂY DỰNG WEBSITE DIỄN ĐÀN SỨC KHỎE (MEDFORUM)

---

### THÔNG TIN CÁ NHÂN
* **Họ và tên:** Trịnh Lê Bá Khánh
* **MSSV:** 170124344
* **Lớp:** D24TTK3
* **Giảng viên hướng dẫn (GVHD):** Thầy Đoàn Nguyễn Ngọc Duyên
* **GitHub Repository:** `ASPNET-D24TTK3-trinhlebakhanh-MEDFORUM`

---

## TÓM TẮT ĐỒ ÁN
Đồ án tập trung nghiên cứu, thiết kế và phát triển hệ thống website Diễn đàn Y tế trực tuyến **MedForum** bằng công nghệ **ASP.NET Core MVC** (.NET 10). Hệ thống giải quyết nhu cầu thảo luận, hỏi đáp y khoa an toàn của người bệnh và tư vấn trực tiếp từ các bác sĩ chuyên khoa đã được duyệt chứng chỉ hành nghề. Dự án triển khai kiến trúc phân tầng (Clean Architecture), tích hợp phân quyền phân cấp dựa trên Identity, bảo mật tài khoản bằng Cookie Authentication, tối ưu hóa trải nghiệm người dùng với AJAX Voting, và đảm bảo thông báo thời gian thực bằng SignalR. Đồng thời, hệ thống phát triển cơ chế tự động phân phối câu hỏi cho bác sĩ phù hợp chuyên khoa và cung cấp bộ công cụ quản lý nội dung giới thiệu động cho Quản trị viên (Admin).

---

## MỞ ĐẦU
### 1. Lý do chọn đề tài
Sức khỏe là tài sản quý giá nhất của con người. Tuy nhiên, việc tìm kiếm thông tin y khoa chính thống trên môi trường internet hiện nay gặp nhiều rào cản do thông tin rác, quảng cáo thuốc lậu tràn lan. Người bệnh có xu hướng ngại đi khám trực tiếp với các triệu chứng nhạy cảm, đồng thời khó kiểm chứng được trình độ chuyên môn của những người tư vấn online. Do đó, việc xây dựng một diễn đàn sức khỏe uy tín, nơi các tài khoản bác sĩ được xác thực chặt chẽ bởi đội ngũ Admin và có sự phân công chuyên môn rõ ràng là vô cùng cần thiết.

### 2. Mục đích nghiên cứu
Xây dựng một nền tảng diễn đàn y tế chuyên nghiệp phục vụ nhu cầu:
* Đặt câu hỏi ẩn danh an toàn, đính kèm ảnh lâm sàng.
* Tự động điều phối câu hỏi đến đúng bác sĩ chuyên khoa phụ trách.
* Hỗ trợ bác sĩ giải đáp trực tuyến nhanh chóng với các biểu mẫu ghim bình luận tự động.
* Giúp Admin quản lý phân quyền thành viên, kiểm duyệt bài viết và xử lý báo cáo vi phạm cộng đồng.

### 3. Đối tượng và phạm vi nghiên cứu
* **Đối tượng**: Người bệnh có nhu cầu tư vấn sức khỏe, các bác sĩ chuyên khoa muốn hỗ trợ cộng đồng, và quản trị viên hệ thống.
* **Phạm vi**: Ứng dụng Web chạy trên môi trường ASP.NET Core MVC kết nối cơ sở dữ liệu SQL Server LocalDB, tập trung vào tính năng hỏi đáp, tương tác và quản trị phân quyền.

---

## CHƯƠNG 1. TỔNG QUAN VỀ ĐỀ TÀI
MedForum là mô hình diễn đàn y tế kết nối trực tiếp Patient - Doctor - Admin:
* **Hệ thống chuyên khoa**: Phân loại theo Tim mạch, Da liễu, Nhi khoa, Dinh dưỡng, Tâm lý học.
* **Cơ chế gán bác sĩ phụ trách**: Hỗ trợ hai phương án: tự động gán dựa trên sự tương đồng giữa chuyên ngành bác sĩ và chuyên khoa của câu hỏi; hoặc Admin chỉ định thủ công.
* **Quy trình kiểm duyệt**: Bác sĩ đăng ký phải nộp chứng chỉ hành nghề dạng hình ảnh và chờ Admin phê duyệt trước khi được cấp Badge xác thực.

---

## CHƯƠNG 2. NGHIÊN CỨU LÝ THUYẾT & CÔNG NGHỆ SỬ DỤNG
Đồ án sử dụng các nền tảng công nghệ hiện đại thuộc hệ sinh thái Microsoft:
1. **ASP.NET Core MVC (.NET 10.0)**: Framework mạnh mẽ, hiệu năng cao để phát triển ứng dụng Web phía máy chủ theo mô hình Model-View-Controller.
2. **Entity Framework Core**: Bộ ORM giúp giao tiếp với cơ sở dữ liệu SQL Server thông qua các lớp thực thể C# thuần túy (Code-First).
3. **ASP.NET Core Identity**: Thư viện quản lý thành viên, xử lý mật khẩu, xác thực Cookie-based và phân quyền theo Role.
4. **SignalR**: Thư viện hỗ trợ truyền tải dữ liệu thời gian thực hai chiều giữa Client và Server để gửi thông báo tức thì.
5. **Glassmorphism & Custom CSS**: Thiết kế giao diện cao cấp dạng mờ thủy tinh, bảng màu HSL hài hòa, hỗ trợ chế độ tương phản cao, hạn chế dùng các màu mặc định thô sơ.

---

## CHƯƠNG 3. HIỆN THỰC HÓA NGHIÊN CỨU & THIẾT KẾ HỆ THỐNG

### 3.1. Sơ đồ cơ sở dữ liệu (Database Relationships)
Cơ sở dữ liệu bao gồm các bảng được thiết kế chặt chẽ:
* **AspNetUsers**: Lưu trữ người dùng.
* **Categories**: Chuyên khoa y tế.
* **Posts**: Câu hỏi của bệnh nhân. Liên kết khóa ngoại `AssignedDoctorId` trỏ đến `AspNetUsers` (Bác sĩ phụ trách).
* **Comments**: Phản hồi y tế.
* **Votes**: Lượt bình chọn bài viết/bình luận (Chỉ cho phép mỗi user vote 1 lần duy nhất trên 1 thực thể).
* **Reports**: Báo cáo vi phạm cộng đồng.
* **DoctorVerificationRequests**: Đơn xin duyệt chứng chỉ bác sĩ.

### 3.2. Sơ đồ các luồng xử lý chính

#### Luồng 1: Tự động điều phối câu hỏi cho bác sĩ
```
[Bệnh nhân đặt câu hỏi] 
       │
       ▼
[Chọn Chuyên khoa (Category)] 
       │
       ▼
[Quét tìm Bác sĩ có Specialty phù hợp với Category] 
       │
       ├─► Tìm thấy: Gán AssignedDoctorId = Bác sĩ đó -> Gửi thông báo SignalR Real-time.
       └─► Không tìm thấy: Để trống trạng thái chờ Admin phân phối thủ công.
```

#### Luồng 2: Quy trình Phân quyền Bác sĩ và Duyệt Chứng chỉ
```
[Bác sĩ Đăng ký tài khoản] ──► [Gửi Hồ sơ & Chứng chỉ]
                                       │
                                       ▼
                               [Admin Kiểm duyệt]
                                       │
                ┌──────────────────────┴──────────────────────┐
                ▼ (Duyệt)                                     ▼ (Từ chối)
   [Gán Role "Doctor"]                           [Cập nhật trạng thái Rejected]
   [IsVerifiedDoctor = true]                     [Lưu lý do từ chối vào hồ sơ]
   [Hiển thị Badge trên Diễn đàn]
```

---

## CHƯƠNG 4. KẾT QUẢ NGHIÊN CỨU & GIAO DIỆN CHƯƠNG TRÌNH

### 4.1. Giao diện trang chủ và Diễn đàn
* **Trang chủ**: Cung cấp các số liệu thống kê thời gian thực và điều hướng nhanh đến 5 chuyên khoa chính.
* **Chi tiết câu hỏi**: Hiển thị ảnh đính kèm lâm sàng. Banner thông báo rõ bác sĩ được phân công phụ trách. Câu trả lời của bác sĩ tự động ghim lên đầu và có badge nổi bật.

### 4.2. Giao diện Quản trị viên (Admin Panel)
* **Trang Phân quyền**: Admin chọn bất kỳ user nào để cấp hoặc thu hồi quyền `Admin`, `Doctor`, `Patient` bằng một thao tác chọn nhanh từ Dropdown.
* **Trang Quản lý bài đăng**: Cho phép Admin phân phối/thay đổi bác sĩ phụ trách câu hỏi lâm sàng bằng danh sách Dropdown nạp động toàn bộ các bác sĩ đã xác thực trong hệ thống.
* **Trang Chỉnh sửa Giới thiệu**: Form soạn thảo trực tiếp 4 phần thông tin giới thiệu hệ thống, tự động lưu xuống file cấu hình JSON trên máy chủ.

---

## CHƯƠNG 5. KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

### 5.1. Kết quả đạt được
* Hiện thực hóa thành công sản phẩm đồ án Diễn đàn Y tế đáp ứng đầy đủ tiêu chí kỹ thuật: chạy trên .NET 10.0 MVC, giao diện thẩm mỹ cao, phân quyền rõ ràng, hệ cơ sở dữ liệu tối ưu hóa các ràng buộc tránh lỗi khóa ngoại hoặc cycles path.
* Tích hợp thành công cơ chế gán bác sĩ phụ trách chuyên môn tự động và thủ công.
* Admin dễ dàng cập nhật thông tin hệ thống động qua JSON và phân quyền trực tiếp trên giao diện quản lý.

### 5.2. Hướng phát triển tiếp theo
* Tích hợp tính năng chat riêng tư thời gian thực (1-1) sử dụng SignalR kết hợp mã hóa tin nhắn đầu cuối để nâng cao tính bảo mật khi trao đổi thông tin bệnh án.
* Áp dụng mô hình ngôn ngữ lớn (LLM) để tự động phân tích câu hỏi của bệnh nhân, gắn nhãn chuyên khoa gợi ý chính xác và phát hiện các bài đăng spam hoặc vi phạm tiêu chuẩn cộng đồng trước khi hiển thị công khai.

---

## TÀI LIỆU THAM KHẢO
1. Microsoft, "ASP.NET Core MVC Documentation," Microsoft Learn, 2025.
2. J. Skeet, *C# in Depth, 4th Edition*, Manning Publications, 2019.
3. Microsoft, "SignalR Real-time communication on ASP.NET Core," Microsoft Docs, 2024.
4. Andrew Lock, *ASP.NET Core in Action, 3rd Edition*, Manning Publications, 2023.
