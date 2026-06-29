using MedicalForum.WebApi.Application.DTOs;
using MedicalForum.WebApi.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Application.Interfaces
{
    public interface IVoteService
    {
        Task<VoteResultDto> VotePostAsync(Guid postId, Guid userId, VoteType type);
        Task<VoteResultDto> VoteCommentAsync(Guid commentId, Guid userId, VoteType type);
    }
}
