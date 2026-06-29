using MedicalForum.Mvc.Application.Interfaces;
using MedicalForum.Mvc.Domain.Entities;
using MedicalForum.Mvc.Domain.Enums;
using MedicalForum.Mvc.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalForum.Mvc.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IPostService _postService;
        private readonly IVoteService _voteService;
        private readonly INotificationService _notificationService;
        private readonly IFileStorageService _fileService;

        public ForumController(ApplicationDbContext db, UserManager<User> userManager,
            IPostService postService, IVoteService voteService,
            INotificationService notificationService, IFileStorageService fileService)
        {
            _db = db; _userManager = userManager; _postService = postService;
            _voteService = voteService; _notificationService = notificationService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(string? categorySlug, string? search, int page = 1)
        {
            int pageSize = 10;
            var query = _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(categorySlug))
            {
                query = query.Where(p => p.Category.Slug == categorySlug);
                ViewBag.ActiveCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlug);
            }

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search) || p.Content.Contains(search));

            var total = await query.CountAsync();
            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Categories = await _db.Categories.ToListAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Search = search;
            return View(posts);
        }

        public async Task<IActionResult> Post(Guid id)
        {
            var post = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.AssignedDoctor)
                .Include(p => p.Comments.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (post == null) return NotFound();

            // Increment views
            post.Views++;
            await _db.SaveChangesAsync();

            // Sort: Doctor responses first, then by upvotes, then by date
            var sortedComments = post.Comments
                .OrderByDescending(c => c.IsDoctorResponse)
                .ThenByDescending(c => c.Upvotes - c.Downvotes)
                .ThenBy(c => c.CreatedAt)
                .ToList();

            ViewBag.SortedComments = sortedComments;
            ViewBag.Categories = await _db.Categories.ToListAsync();

            // Get current user vote status
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(_userManager.GetUserId(User)!);
                var userVotes = await _db.Votes
                    .Where(v => v.UserId == userId && (v.PostId == id || sortedComments.Select(c => c.Id).Contains(v.CommentId ?? Guid.Empty)))
                    .ToListAsync();
                ViewBag.UserVotes = userVotes;
            }

            return View(post);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string content, Guid categoryId, bool isAnonymous, IFormFile? image)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();
            if (user.IsBanned) { TempData["Error"] = "Tài khoản của bạn đã bị khóa."; return RedirectToAction("Index"); }

            string? imageUrl = null;
            if (image != null && image.Length > 0)
            {
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
                { TempData["Error"] = "Chỉ chấp nhận ảnh .jpg, .jpeg, .png, .webp"; ViewBag.Categories = await _db.Categories.ToListAsync(); return View(); }
                imageUrl = await _fileService.UploadFileAsync(image, "posts");
            }

            // Auto-assign doctor based on category/specialty
            var category = await _db.Categories.FindAsync(categoryId);
            Guid? assignedDoctorId = null;
            if (category != null)
            {
                // Find verified doctor whose specialty matches category name
                var matchingDoctor = await _db.Users
                    .Where(u => u.IsVerifiedDoctor && u.Specialty != null && u.Specialty.Contains(category.Name))
                    .FirstOrDefaultAsync();
                
                if (matchingDoctor != null)
                {
                    assignedDoctorId = matchingDoctor.Id;
                }
            }

            var post = new Post
            {
                Title = title,
                Content = content,
                CategoryId = categoryId,
                AuthorId = user.Id,
                IsAnonymous = isAnonymous,
                ImageUrl = imageUrl,
                AssignedDoctorId = assignedDoctorId,
                MedicalDisclaimer = "Thông tin chỉ mang tính chất tham khảo, không thay thế chẩn đoán hoặc điều trị y khoa chuyên nghiệp."
            };
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            // Notify the assigned doctor
            if (assignedDoctorId.HasValue)
            {
                try
                {
                    await _notificationService.SendNotificationToUserAsync(
                        assignedDoctorId.Value.ToString(),
                        "Có câu hỏi mới thuộc chuyên khoa của bạn",
                        $"Hệ thống đã tự động điều phối câu hỏi: \"{title}\" cho bạn giải đáp.",
                        "AssignedPost",
                        post.Id.ToString()
                    );
                }
                catch { }
            }

            TempData["Success"] = "Bài viết đã được đăng thành công!" + (assignedDoctorId.HasValue ? " Câu hỏi đã được chuyển đến bác sĩ chuyên khoa tương ứng." : "");
            return RedirectToAction("Post", new { id = post.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(Guid postId, string content, bool isAnonymous)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.IsBanned) return Forbid();

            var post = await _db.Posts.FindAsync(postId);
            if (post == null) return NotFound();

            bool isDoctor = user.IsVerifiedDoctor;
            var comment = new Comment
            {
                PostId = postId,
                AuthorId = user.Id,
                Content = content,
                IsAnonymous = isAnonymous,
                IsDoctorResponse = isDoctor,
                MedicalDisclaimer = isDoctor
                    ? "Bác sĩ khuyến cáo: Thông tin này chỉ mang tính tham khảo, không thay thế việc thăm khám trực tiếp."
                    : null
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // Notify post author
            if (post.AuthorId.HasValue && post.AuthorId != user.Id)
            {
                await _notificationService.SendNotificationToUserAsync(
                    post.AuthorId.Value.ToString(),
                    isDoctor ? $"Bác sĩ {user.FullName} đã phản hồi!" : "Bạn có bình luận mới",
                    content.Length > 50 ? content[..50] + "..." : content,
                    isDoctor ? "DoctorReply" : "GeneralComment",
                    post.Id.ToString()
                );
            }

            TempData["Success"] = "Bình luận của bạn đã được đăng!";
            return RedirectToAction("Post", new { id = postId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Vote(Guid? postId, Guid? commentId, int type)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var voteType = (VoteType)type;

            try
            {
                if (postId.HasValue)
                {
                    var result = await _voteService.VotePostAsync(postId.Value, userId, voteType);
                    return Json(result);
                }
                else if (commentId.HasValue)
                {
                    var result = await _voteService.VoteCommentAsync(commentId.Value, userId, voteType);
                    return Json(result);
                }
            }
            catch { }
            return Json(new { error = true });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(Guid? postId, Guid? commentId, string reason, string details)
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var report = new Report
            {
                ReporterId = userId,
                PostId = postId,
                CommentId = commentId,
                Reason = reason,
                Details = details,
                Status = ReportStatus.Pending
            };
            _db.Reports.Add(report);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Báo cáo của bạn đã được ghi nhận. Cảm ơn bạn!";
            var redirectId = postId ?? (commentId.HasValue
                ? (await _db.Comments.FindAsync(commentId.Value))?.PostId
                : null);
            return redirectId.HasValue ? RedirectToAction("Post", new { id = redirectId }) : RedirectToAction("Index");
        }
    }
}
