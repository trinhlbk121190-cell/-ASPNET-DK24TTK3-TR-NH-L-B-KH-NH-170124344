using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MedicalForum.WebApi.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsVerifiedDoctor { get; set; } = false;
        public DateTime? DoctorBadgeApprovedAt { get; set; }
        
        // Doctor Metadata (Null if user is a normal patient)
        public string? Specialty { get; set; }
        public string? Hospital { get; set; }
        public string? CertificateImageUrl { get; set; }
        
        public bool IsBanned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<Report> ReportsSubmitted { get; set; } = new List<Report>();
        public virtual ICollection<DoctorVerificationRequest> VerificationRequests { get; set; } = new List<DoctorVerificationRequest>();
    }
}
