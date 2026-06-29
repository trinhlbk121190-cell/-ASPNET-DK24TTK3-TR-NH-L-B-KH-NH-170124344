using MedicalForum.Mvc.Application.DTOs;
using MedicalForum.Mvc.Application.Interfaces;
using MedicalForum.Mvc.Domain.Entities;
using MedicalForum.Mvc.Domain.Enums;
using MedicalForum.Mvc.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MedicalForum.Mvc.Application.Services
{
    public class VoteService : IVoteService
    {
        private readonly ApplicationDbContext _dbContext;

        public VoteService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VoteResultDto> VotePostAsync(Guid postId, Guid userId, VoteType type)
        {
            // 1. Fetch the target post
            var post = await _dbContext.Posts.FindAsync(postId);
            if (post == null)
            {
                throw new ArgumentException("Bài viết không tồn tại.");
            }

            // 2. Check if user already voted on this post
            var existingVote = await _dbContext.Votes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.PostId == postId);

            string voteStatus = "None";

            if (existingVote == null)
            {
                // Scenario A: First time voting on this post
                var newVote = new Vote
                {
                    UserId = userId,
                    PostId = postId,
                    Type = type
                };
                _dbContext.Votes.Add(newVote);

                // Update counters
                if (type == VoteType.Upvote) post.Upvotes++;
                else post.Downvotes++;

                voteStatus = type == VoteType.Upvote ? "Upvoted" : "Downvoted";
            }
            else
            {
                // Scenario B: User is changing or toggling their vote
                if (existingVote.Type == type)
                {
                    // Retract vote (Toggle off if they click the same vote type again)
                    _dbContext.Votes.Remove(existingVote);

                    if (type == VoteType.Upvote) post.Upvotes = Math.Max(0, post.Upvotes - 1);
                    else post.Downvotes = Math.Max(0, post.Downvotes - 1);

                    voteStatus = "None";
                }
                else
                {
                    // Switch vote (e.g. from Upvote to Downvote)
                    existingVote.Type = type;
                    _dbContext.Votes.Update(existingVote);

                    if (type == VoteType.Upvote)
                    {
                        post.Upvotes++;
                        post.Downvotes = Math.Max(0, post.Downvotes - 1);
                        voteStatus = "Upvoted";
                    }
                    else
                    {
                        post.Downvotes++;
                        post.Upvotes = Math.Max(0, post.Upvotes - 1);
                        voteStatus = "Downvoted";
                    }
                }
            }

            // 3. Save changes
            await _dbContext.SaveChangesAsync();

            return new VoteResultDto
            {
                Upvotes = post.Upvotes,
                Downvotes = post.Downvotes,
                UserVoteStatus = voteStatus
            };
        }

        public async Task<VoteResultDto> VoteCommentAsync(Guid commentId, Guid userId, VoteType type)
        {
            // 1. Fetch the target comment
            var comment = await _dbContext.Comments.FindAsync(commentId);
            if (comment == null)
            {
                throw new ArgumentException("Bình luận không tồn tại.");
            }

            // 2. Check if user already voted on this comment
            var existingVote = await _dbContext.Votes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.CommentId == commentId);

            string voteStatus = "None";

            if (existingVote == null)
            {
                // Scenario A: First time voting on this comment
                var newVote = new Vote
                {
                    UserId = userId,
                    CommentId = commentId,
                    Type = type
                };
                _dbContext.Votes.Add(newVote);

                // Update counters
                if (type == VoteType.Upvote) comment.Upvotes++;
                else comment.Downvotes++;

                voteStatus = type == VoteType.Upvote ? "Upvoted" : "Downvoted";
            }
            else
            {
                // Scenario B: User is changing or toggling their vote
                if (existingVote.Type == type)
                {
                    // Retract vote
                    _dbContext.Votes.Remove(existingVote);

                    if (type == VoteType.Upvote) comment.Upvotes = Math.Max(0, comment.Upvotes - 1);
                    else comment.Downvotes = Math.Max(0, comment.Downvotes - 1);

                    voteStatus = "None";
                }
                else
                {
                    // Switch vote
                    existingVote.Type = type;
                    _dbContext.Votes.Update(existingVote);

                    if (type == VoteType.Upvote)
                    {
                        comment.Upvotes++;
                        comment.Downvotes = Math.Max(0, comment.Downvotes - 1);
                        voteStatus = "Upvoted";
                    }
                    else
                    {
                        comment.Downvotes++;
                        comment.Upvotes = Math.Max(0, comment.Upvotes - 1);
                        voteStatus = "Downvoted";
                    }
                }
            }

            // 3. Save changes
            await _dbContext.SaveChangesAsync();

            return new VoteResultDto
            {
                Upvotes = comment.Upvotes,
                Downvotes = comment.Downvotes,
                UserVoteStatus = voteStatus
            };
        }
    }
}
