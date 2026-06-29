using MedicalForum.Mvc.Domain.Enums;
using MedicalForum.Mvc.Infrastructure.Data;
using MedicalForum.Mvc.Domain.Entities;
using MedicalForum.Mvc.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalForum.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AdminController(ApplicationDbContext db, UserManager<User> um, RoleManager<IdentityRole<Guid>> rm)
        { _db = db; _userManager = um; _roleManager = rm; }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalPosts = await _db.Posts.CountAsync(p => !p.IsDeleted);
            ViewBag.TotalUsers = await _db.Users.CountAsync();
            ViewBag.TotalDoctors = await _db.Users.CountAsync(u => u.IsVerifiedDoctor);
            ViewBag.PendingVerifications = await _db.DoctorVerificationRequests.CountAsync(r => r.Status == VerificationStatus.Pending);
            ViewBag.PendingReports = await _db.Reports.CountAsync(r => r.Status == ReportStatus.Pending);
            ViewBag.RecentPosts = await _db.Posts.Include(p => p.Author).Include(p => p.Category)
                .Where(p => !p.IsDeleted).OrderByDescending(p => p.CreatedAt).Take(5).ToListAsync();

            // Load about.json content
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "about.json");
            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                ViewBag.AboutData = data;
            }

            return View();
        }

        public async Task<IActionResult> Users(string? search)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.FullName.Contains(search) || (u.Email != null && u.Email.Contains(search)));
            var users = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
            var userRoles = new Dictionary<Guid, IList<string>>();
            foreach (var u in users)
                userRoles[u.Id] = await _userManager.GetRolesAsync(u);
            ViewBag.UserRoles = userRoles;
            ViewBag.Search = search;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BanUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null) { user.IsBanned = !user.IsBanned; await _userManager.UpdateAsync(user); }
            TempData["Success"] = user?.IsBanned == true ? "Đã khóa tài khoản." : "Đã mở khóa tài khoản.";
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Verifications()
        {
            var requests = await _db.DoctorVerificationRequests
                .Include(r => r.Doctor)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVerification(Guid id)
        {
            var req = await _db.DoctorVerificationRequests.Include(r => r.Doctor).FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound();
            req.Status = VerificationStatus.Approved;
            req.ReviewedAt = DateTime.UtcNow;
            req.Doctor.IsVerifiedDoctor = true;
            req.Doctor.DoctorBadgeApprovedAt = DateTime.UtcNow;
            req.Doctor.Specialty = req.Specialty;
            req.Doctor.Hospital = req.Hospital;
            if (!await _userManager.IsInRoleAsync(req.Doctor, "Doctor"))
                await _userManager.AddToRoleAsync(req.Doctor, "Doctor");
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã cấp badge Bác sĩ cho {req.Doctor.FullName}.";
            return RedirectToAction("Verifications");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectVerification(Guid id, string? notes)
        {
            var req = await _db.DoctorVerificationRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound();
            req.Status = VerificationStatus.Rejected;
            req.ReviewerNotes = notes;
            req.ReviewedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã từ chối yêu cầu.";
            return RedirectToAction("Verifications");
        }

        public async Task<IActionResult> Reports()
        {
            var reports = await _db.Reports
                .Include(r => r.Reporter)
                .Include(r => r.Post)
                .Include(r => r.Comment)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(reports);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveReport(Guid id, string action)
        {
            var report = await _db.Reports.FirstOrDefaultAsync(r => r.Id == id);
            if (report == null) return NotFound();
            report.Status = action == "resolve" ? ReportStatus.Resolved : ReportStatus.Dismissed;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xử lý báo cáo.";
            return RedirectToAction("Reports");
        }

        public async Task<IActionResult> Posts()
        {
            var posts = await _db.Posts
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.AssignedDoctor)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.VerifiedDoctors = await _db.Users
                .Where(u => u.IsVerifiedDoctor)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return View(posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post != null) { post.IsDeleted = true; await _db.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa bài viết.";
            return RedirectToAction("Posts");
        }

        // ====== PHÂN QUYỀN ======

        public async Task<IActionResult> Roles(string? search)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.FullName.Contains(search) || (u.Email != null && u.Email.Contains(search)));

            var users = await query.OrderBy(u => u.FullName).ToListAsync();

            // Build user → roles map
            var userRolesMap = new Dictionary<Guid, IList<string>>();
            foreach (var u in users)
                userRolesMap[u.Id] = await _userManager.GetRolesAsync(u);

            // All available roles
            var allRoles = await _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToListAsync();

            ViewBag.UserRolesMap = userRolesMap;
            ViewBag.AllRoles = allRoles;
            ViewBag.Search = search;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { TempData["Error"] = "Không tìm thấy người dùng."; return RedirectToAction("Roles"); }

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);

                // If assigning Doctor role, check if they need IsVerifiedDoctor flag
                if (role == "Doctor" && !user.IsVerifiedDoctor)
                {
                    user.IsVerifiedDoctor = true;
                    user.DoctorBadgeApprovedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
                TempData["Success"] = $"Đã cấp quyền '{role}' cho {user.FullName}.";
            }
            else
            {
                TempData["Error"] = $"{user.FullName} đã có quyền '{role}' rồi.";
            }
            return RedirectToAction("Roles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) { TempData["Error"] = "Không tìm thấy người dùng."; return RedirectToAction("Roles"); }

            // Prevent removing Admin role from the last Admin
            if (role == "Admin")
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                {
                    TempData["Error"] = "Không thể xóa quyền Admin của người dùng duy nhất còn lại.";
                    return RedirectToAction("Roles");
                }
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);

                // If removing Doctor role, revoke badge
                if (role == "Doctor" && user.IsVerifiedDoctor)
                {
                    user.IsVerifiedDoctor = false;
                    user.DoctorBadgeApprovedAt = null;
                    await _userManager.UpdateAsync(user);
                }
                TempData["Success"] = $"Đã thu hồi quyền '{role}' của {user.FullName}.";
            }
            else
            {
                TempData["Error"] = $"{user.FullName} không có quyền '{role}'.";
            }
            return RedirectToAction("Roles");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            { TempData["Error"] = "Tên quyền không được để trống."; return RedirectToAction("Roles"); }

            roleName = roleName.Trim();
            if (await _roleManager.RoleExistsAsync(roleName))
            { TempData["Error"] = $"Quyền '{roleName}' đã tồn tại."; return RedirectToAction("Roles"); }

            await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            TempData["Success"] = $"Đã tạo quyền '{roleName}' thành công.";
            return RedirectToAction("Roles");
        }

        // ====== CHỈNH SỬA GIỚI THIỆU ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAbout(string mission, string vision, string values, string verification)
        {
            var data = new Dictionary<string, string>
            {
                { "Mission", mission ?? "" },
                { "Vision", vision ?? "" },
                { "Values", values ?? "" },
                { "Verification", verification ?? "" }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "about.json");
            System.IO.File.WriteAllText(path, json);

            TempData["Success"] = "Đã cập nhật nội dung giới thiệu thành công.";
            return RedirectToAction("Index");
        }

        // ====== ĐIỀU PHỐI BÁC SĨ THỦ CÔNG ======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManualAssignDoctor(Guid postId, Guid doctorId)
        {
            var post = await _db.Posts.FindAsync(postId);
            if (post == null) return NotFound();

            var doctor = await _userManager.FindByIdAsync(doctorId.ToString());
            if (doctor == null || !doctor.IsVerifiedDoctor)
            {
                TempData["Error"] = "Bác sĩ không hợp lệ hoặc chưa được xác thực.";
                return RedirectToAction("Posts");
            }

            post.AssignedDoctorId = doctorId;
            await _db.SaveChangesAsync();

            // Send Real-time notification
            try
            {
                var notificationService = HttpContext.RequestServices.GetService<INotificationService>();
                if (notificationService != null)
                {
                    await notificationService.SendNotificationToUserAsync(
                        doctorId.ToString(),
                        "Bạn được phân công trả lời câu hỏi",
                        $"Admin đã chỉ định bạn phụ trách giải đáp câu hỏi: \"{post.Title}\"",
                        "AssignedPost",
                        post.Id.ToString()
                    );
                }
            }
            catch { }

            TempData["Success"] = $"Đã chỉ định Bác sĩ {doctor.FullName} phụ trách câu hỏi này.";
            return RedirectToAction("Posts");
        }
    }
}
