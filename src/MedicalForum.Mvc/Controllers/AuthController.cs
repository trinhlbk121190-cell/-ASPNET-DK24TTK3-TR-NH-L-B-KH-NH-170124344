using MedicalForum.Mvc.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MedicalForum.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthController(UserManager<User> um, SignInManager<User> sim, RoleManager<IdentityRole<Guid>> rm)
        { _userManager = um; _signInManager = sim; _roleManager = rm; }

        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe, string? returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) { TempData["Error"] = "Email hoặc mật khẩu không đúng."; return View(); }
            if (user.IsBanned) { TempData["Error"] = "Tài khoản đã bị khóa do vi phạm cộng đồng."; return View(); }

            var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Chào mừng {user.FullName}!";
                return LocalRedirect(returnUrl ?? "/");
            }
            TempData["Error"] = "Email hoặc mật khẩu không đúng.";
            return View();
        }

        public IActionResult Register() => User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Home") : View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string fullName, string role)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
            { TempData["Error"] = "Email này đã được sử dụng."; return View(); }

            if (role != "Patient" && role != "Doctor") role = "Patient";

            var user = new User { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true, CreatedAt = DateTime.UtcNow };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            { TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description)); return View(); }

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            await _userManager.AddToRoleAsync(user, role);

            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Success"] = "Đăng ký thành công! Chào mừng bạn đến với MedForum.";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();
    }
}
