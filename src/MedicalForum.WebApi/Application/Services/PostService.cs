using MedicalForum.WebApi.Application.DTOs;
using MedicalForum.WebApi.Application.Interfaces;
using MedicalForum.WebApi.Domain.Entities;
using MedicalForum.WebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Application.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IFileStorageService _fileStorageService;
        private const string StandardMedicalDisclaimer = "Thông tin chỉ mang tính chất tham khảo, không thay thế chẩn đoán hoặc điều trị y khoa chuyên nghiệp.";

        public PostService(ApplicationDbContext dbContext, IFileStorageService fileStorageService)
        {
            _dbContext = dbContext;
            _fileStorageService = fileStorageService;
        }

        public async Task<PostDetailDto> CreatePostAsync(CreatePostDto dto, Guid userId)
        {
            // 1. Validate category
            var category = await _dbContext.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("Chuyên khoa y tế không hợp lệ.");
            }

            // 2. Handle image upload (e.g. lab results or skin symptoms)
            string? uploadedImageUrl = null;
            if (dto.Image != null && dto.Image.Length > 0)
            {
                // Verify image extension/size
                var ext = Path.GetExtension(dto.Image.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
                {
                    throw new InvalidOperationException("Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .webp).");
                }
                
                uploadedImageUrl = await _fileStorageService.UploadFileAsync(dto.Image, "posts");
            }

            // 3. Create core post entity & attach standard medical disclaimer
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                CategoryId = dto.CategoryId,
                AuthorId = userId,
                IsAnonymous = dto.IsAnonymous,
                ImageUrl = uploadedImageUrl,
                MedicalDisclaimer = StandardMedicalDisclaimer
            };

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            // 4. Load post author info
            var author = await _dbContext.Users.FindAsync(userId);

            // 5. Build DTO output (Respecting anonymity)
            return new PostDetailDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId,
                CategoryName = category.Name,
                AuthorName = post.IsAnonymous ? "Thành viên ẩn danh" : (author?.FullName ?? "Người dùng"),
                AuthorAvatar = post.IsAnonymous ? null : author?.AvatarUrl,
                IsAuthorVerifiedDoctor = !post.IsAnonymous && (author?.IsVerifiedDoctor ?? false),
                IsAnonymous = post.IsAnonymous,
                ImageUrl = post.ImageUrl,
                MedicalDisclaimer = post.MedicalDisclaimer,
                Upvotes = post.Upvotes,
                Downvotes = post.Downvotes,
                CreatedAt = post.CreatedAt
            };
        }

        public async Task<PostDetailDto?> GetPostByIdAsync(Guid id, Guid? currentUserId)
        {
            var post = await _dbContext.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (post == null) return null;

            // Determine if the current viewer can see the author profile details (e.g. author themselves or Admins)
            bool revealIdentity = !post.IsAnonymous || post.AuthorId == currentUserId;

            return new PostDetailDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId,
                CategoryName = post.Category.Name,
                AuthorName = revealIdentity ? (post.Author?.FullName ?? "Người dùng") : "Thành viên ẩn danh",
                AuthorAvatar = revealIdentity ? post.Author?.AvatarUrl : null,
                IsAuthorVerifiedDoctor = revealIdentity && (post.Author?.IsVerifiedDoctor ?? false),
                IsAnonymous = post.IsAnonymous,
                ImageUrl = post.ImageUrl,
                MedicalDisclaimer = post.MedicalDisclaimer,
                Upvotes = post.Upvotes,
                Downvotes = post.Downvotes,
                CreatedAt = post.CreatedAt
            };
        }
    }
}
