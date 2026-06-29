using MedicalForum.Mvc.Domain.Enums;
using System;

namespace MedicalForum.Mvc.Domain.Entities
{
    public class DoctorVerificationRequest : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public virtual User Doctor { get; set; } = null!;
        
        public string Specialty { get; set; } = string.Empty;
        public string Hospital { get; set; } = string.Empty;
        public string CertificateImageUrl { get; set; } = string.Empty;
        
        public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
        public string? ReviewerNotes { get; set; }
        public Guid? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
