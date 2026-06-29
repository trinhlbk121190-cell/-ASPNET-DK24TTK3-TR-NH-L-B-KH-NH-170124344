using MedicalForum.WebApi.Application.Interfaces;
using MedicalForum.WebApi.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VotesController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VotesController(IVoteService voteService)
        {
            _voteService = voteService;
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> VotePost(Guid postId, [FromBody] VoteRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            try
            {
                var result = await _voteService.VotePostAsync(postId, userId, request.Type);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("comment/{commentId}")]
        public async Task<IActionResult> VoteComment(Guid commentId, [FromBody] VoteRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

            try
            {
                var result = await _voteService.VoteCommentAsync(commentId, userId, request.Type);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class VoteRequest
    {
        public VoteType Type { get; set; } // Upvote = 1, Downvote = -1
    }
}
