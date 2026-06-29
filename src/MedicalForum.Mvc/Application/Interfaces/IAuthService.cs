using MedicalForum.Mvc.Application.DTOs;
using System.Threading.Tasks;

namespace MedicalForum.Mvc.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
