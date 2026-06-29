using System.Collections.Generic;

namespace MedicalForum.Mvc.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Icon { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
