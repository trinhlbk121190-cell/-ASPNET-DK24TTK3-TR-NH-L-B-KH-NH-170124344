using MedicalForum.Mvc.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace MedicalForum.Mvc.Application.Interfaces
{
    public interface IPostService
    {
        Task<PostDetailDto> CreatePostAsync(CreatePostDto dto, Guid userId);
        Task<PostDetailDto?> GetPostByIdAsync(Guid id, Guid? currentUserId);
    }
}
