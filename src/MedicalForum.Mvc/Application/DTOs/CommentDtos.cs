using System;

namespace MedicalForum.Mvc.Application.DTOs
{
    public class CreateCommentDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
    }

    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsDoctorResponse { get; set; }
        public string? MedicalDisclaimer { get; set; }
        
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
