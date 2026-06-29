using MedicalForum.WebApi.Domain.Enums;
using System;

namespace MedicalForum.WebApi.Domain.Entities
{
    public class Report : BaseEntity
    {
        public Guid ReporterId { get; set; }
        public virtual User Reporter { get; set; } = null!;
        
        public Guid? PostId { get; set; }
        public virtual Post? Post { get; set; }
        
        public Guid? CommentId { get; set; }
        public virtual Comment? Comment { get; set; }
        
        public string Reason { get; set; } = string.Empty; // e.g. "Misinformation", "Unapproved Sales"
        public string Details { get; set; } = string.Empty;
        public ReportStatus Status { get; set; } = ReportStatus.Pending;
        public string? AdminNotes { get; set; }
    }
}
