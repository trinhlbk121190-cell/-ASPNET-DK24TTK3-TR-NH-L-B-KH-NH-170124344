using MedicalForum.WebApi.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Application.Interfaces
{
    public interface IPostService
    {
        Task<PostDetailDto> CreatePostAsync(CreatePostDto dto, Guid userId);
        Task<PostDetailDto?> GetPostByIdAsync(Guid id, Guid? currentUserId);
    }
}
