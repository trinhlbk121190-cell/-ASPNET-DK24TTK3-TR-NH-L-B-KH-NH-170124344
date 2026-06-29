using System;
using System.Collections.Generic;

namespace MedicalForum.WebApi.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
        
        public Guid? AuthorId { get; set; }
        public virtual User? Author { get; set; }
        
        public string Content { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; } = false;
        
        // True if author was a verified doctor when commenting
        public bool IsDoctorResponse { get; set; } = false;
        public string? MedicalDisclaimer { get; set; }
        
        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;

        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
