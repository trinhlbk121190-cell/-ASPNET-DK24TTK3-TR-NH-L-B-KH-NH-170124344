using MedicalForum.WebApi.Application.DTOs;
using MedicalForum.WebApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var postDetail = await _postService.CreatePostAsync(dto, userId);
                return CreatedAtAction(nameof(GetPostById), new { id = postDetail.Id }, postDetail);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? currentUserId = null;
            if (Guid.TryParse(userIdClaim, out var parsedId))
            {
                currentUserId = parsedId;
            }

            var post = await _postService.GetPostByIdAsync(id, currentUserId);
            if (post == null)
            {
                return NotFound("Bài viết không tồn tại.");
            }

            return Ok(post);
        }
    }
}
