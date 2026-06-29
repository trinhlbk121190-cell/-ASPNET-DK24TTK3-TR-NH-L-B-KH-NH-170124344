using MedicalForum.WebApi.Application.DTOs;
using MedicalForum.WebApi.Application.Interfaces;
using MedicalForum.WebApi.Domain.Entities;
using MedicalForum.WebApi.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly INotificationService _notificationService;

        public CommentsController(ApplicationDbContext dbContext, INotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var post = await _dbContext.Posts.FindAsync(request.PostId);
            if (post == null) return NotFound("Bài viết không tồn tại.");

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return Unauthorized();

            var isDoctor = user.IsVerifiedDoctor;
            string? disclaimer = isDoctor 
                ? "Bác sĩ khuyến cáo: Thông tin tư vấn này chỉ mang tính tham khảo trực quan, không thay thế việc thăm khám trực tiếp tại cơ sở y tế."
                : null;

            var comment = new Comment
            {
                PostId = request.PostId,
                AuthorId = userId,
                Content = request.Content,
                IsAnonymous = request.IsAnonymous,
                IsDoctorResponse = isDoctor,
                MedicalDisclaimer = disclaimer
            };

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            // Emit dynamic post update via SignalR
            await _notificationService.BroadcastToPostGroupAsync(post.Id.ToString(), "NewComment", new
            {
                CommentId = comment.Id,
                Content = comment.Content,
                IsDoctor = comment.IsDoctorResponse,
                AuthorName = comment.IsAnonymous ? "Ẩn danh" : user.FullName,
                MedicalDisclaimer = comment.MedicalDisclaimer,
                CreatedAt = comment.CreatedAt
            });

            // Notify post author (real-time push alert)
            if (post.AuthorId != null && post.AuthorId != userId)
            {
                string alertTitle = isDoctor ? "Một Bác sĩ đã phản hồi câu hỏi của bạn" : "Bạn có bình luận mới";
                string alertMsg = isDoctor 
                    ? $"Bác sĩ {user.FullName} đã bình luận: \"{(comment.Content.Length > 30 ? comment.Content.Substring(0, 30) + "..." : comment.Content)}\""
                    : $"{user.FullName} đã bình luận về bài viết của bạn.";

                await _notificationService.SendNotificationToUserAsync(
                    post.AuthorId.ToString()!, 
                    alertTitle, 
                    alertMsg, 
                    isDoctor ? "DoctorReply" : "GeneralComment", 
                    post.Id.ToString()
                );
            }

            return Ok(comment);
        }

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetCommentsForPost(Guid postId)
        {
            var comments = await _dbContext.Comments
                .Include(c => c.Author)
                .Where(c => c.PostId == postId && !c.IsDeleted)
                // 1. Doctor responses first (IsDoctorResponse == true)
                // 2. Sorted by net upvotes (Upvotes - Downvotes) descending
                // 3. Sorted by oldest creation date first (CreatedAt ascending)
                .OrderByDescending(c => c.IsDoctorResponse)
                .ThenByDescending(c => c.Upvotes - c.Downvotes)
                .ThenBy(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    Content = c.Content,
                    AuthorName = c.IsAnonymous ? "Thành viên ẩn danh" : (c.Author != null ? c.Author.FullName : "Người dùng"),
                    AuthorAvatar = c.IsAnonymous ? null : (c.Author != null ? c.Author.AvatarUrl : null),
                    IsAnonymous = c.IsAnonymous,
                    IsDoctorResponse = c.IsDoctorResponse,
                    MedicalDisclaimer = c.MedicalDisclaimer,
                    Upvotes = c.Upvotes,
                    Downvotes = c.Downvotes,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }
    }

    public class CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
    }
}
