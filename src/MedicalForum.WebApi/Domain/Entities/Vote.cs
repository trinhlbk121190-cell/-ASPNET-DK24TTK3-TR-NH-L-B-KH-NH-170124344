using MedicalForum.WebApi.Domain.Enums;
using System;

namespace MedicalForum.WebApi.Domain.Entities
{
    public class Vote
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        
        public Guid? PostId { get; set; }
        public virtual Post? Post { get; set; }
        
        public Guid? CommentId { get; set; }
        public virtual Comment? Comment { get; set; }
        
        public VoteType Type { get; set; } // Upvote or Downvote
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    }
}
