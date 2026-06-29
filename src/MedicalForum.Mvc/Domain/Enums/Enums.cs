namespace MedicalForum.Mvc.Domain.Enums
{
    public enum VoteType
    {
        Downvote = -1,
        Upvote = 1
    }

    public enum ReportStatus
    {
        Pending = 0,
        Resolved = 1,
        Dismissed = 2
    }

    public enum VerificationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
