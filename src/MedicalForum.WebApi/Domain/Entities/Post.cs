using System;
using System.Collections.Generic;

namespace MedicalForum.WebApi.Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        
        public Guid? AuthorId { get; set; }
        public virtual User? Author { get; set; }
        
        public bool IsAnonymous { get; set; } = false;
        public string? ImageUrl { get; set; }
        
        // Auto-attached medical disclaimer to meet medical forum safety compliance
        public string MedicalDisclaimer { get; set; } = string.Empty;
        
        public int Views { get; set; } = 0;
        public int Upvotes { get; set; } = 0;
        public int Downvotes { get; set; } = 0;

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
