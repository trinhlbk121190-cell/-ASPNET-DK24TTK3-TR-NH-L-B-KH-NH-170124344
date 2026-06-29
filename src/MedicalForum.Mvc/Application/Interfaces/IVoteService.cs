using MedicalForum.Mvc.Application.DTOs;
using MedicalForum.Mvc.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace MedicalForum.Mvc.Application.Interfaces
{
    public interface IVoteService
    {
        Task<VoteResultDto> VotePostAsync(Guid postId, Guid userId, VoteType type);
        Task<VoteResultDto> VoteCommentAsync(Guid commentId, Guid userId, VoteType type);
    }
}
