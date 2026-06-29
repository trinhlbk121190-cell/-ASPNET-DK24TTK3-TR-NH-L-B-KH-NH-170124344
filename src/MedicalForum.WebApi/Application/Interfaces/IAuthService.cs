using MedicalForum.WebApi.Application.DTOs;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
