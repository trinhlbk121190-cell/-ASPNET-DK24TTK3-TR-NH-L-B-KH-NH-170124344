namespace MedicalForum.Mvc.Application.DTOs
{
    public class VoteResultDto
    {
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public string UserVoteStatus { get; set; } = "None"; // "Upvoted", "Downvoted", "None"
    }
}
