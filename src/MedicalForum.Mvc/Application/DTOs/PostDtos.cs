using Microsoft.AspNetCore.Http;
using System;

namespace MedicalForum.Mvc.Application.DTOs
{
    public class CreatePostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public bool IsAnonymous { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class PostDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        
        // Hide author details if post is anonymous and caller is not Admin
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public bool IsAuthorVerifiedDoctor { get; set; }
        
        public bool IsAnonymous { get; set; }
        public string? ImageUrl { get; set; }
        public string MedicalDisclaimer { get; set; } = string.Empty;
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
