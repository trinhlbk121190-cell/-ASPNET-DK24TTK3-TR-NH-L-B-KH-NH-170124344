using MedicalForum.Mvc.Application.DTOs;
using MedicalForum.Mvc.Application.Interfaces;
using MedicalForum.Mvc.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicalForum.Mvc.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // 1. Validate requested role
            var roleName = dto.Role.Trim();
            if (roleName != "Patient" && roleName != "Doctor")
            {
                throw new ArgumentException("Vai trò đăng ký không hợp lệ. Chỉ được phép chọn Patient hoặc Doctor.");
            }

            // 2. Check if email exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email này đã được sử dụng.");
            }

            // 3. Instantiate user
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                IsVerifiedDoctor = false, // Must be audited/verified by admin later
                IsBanned = false,
                CreatedAt = DateTime.UtcNow
            };

            // If registering as a doctor, save initial application metadata
            if (roleName == "Doctor")
            {
                user.Specialty = dto.Specialty;
                user.Hospital = dto.Hospital;
                user.CertificateImageUrl = dto.CertificateImageUrl;
            }

            // 4. Save user
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Lỗi tạo tài khoản: {errors}");
            }

            // 5. Ensure security role exists and add user
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);

            // 6. Generate token and return
            return await GenerateAuthResponseAsync(user, roleName);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // 1. Fetch user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác.");
            }

            // 2. Check ban status
            if (user.IsBanned)
            {
                throw new UnauthorizedAccessException("Tài khoản này đã bị khóa do vi phạm tiêu chuẩn cộng đồng.");
            }

            // 3. Verify password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác.");
            }

            // 4. Get role
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Patient";

            // 5. Generate token and return
            return await GenerateAuthResponseAsync(user, primaryRole);
        }

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"] ?? "SuperSecretKeyExampleForMedicalForumLongEnoughSecurity";
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, role)
            };

            var expiry = DateTime.UtcNow.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                Issuer = _configuration["Jwt:Issuer"] ?? "MedicalForumIssuer",
                Audience = _configuration["Jwt:Audience"] ?? "MedicalForumAudience",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new AuthResponseDto
            {
                Token = tokenString,
                Expiry = expiry,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    Role = role,
                    IsVerifiedDoctor = user.IsVerifiedDoctor
                }
            };
        }
    }
}
