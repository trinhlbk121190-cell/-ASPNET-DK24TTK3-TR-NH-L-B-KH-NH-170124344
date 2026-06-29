using MedicalForum.Mvc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MedicalForum.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var recentPosts = await _db.Posts
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.Comments)
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            var categories = await _db.Categories.ToListAsync();

            ViewBag.RecentPosts = recentPosts;
            ViewBag.Categories = categories;
            ViewBag.TotalPosts = await _db.Posts.CountAsync(p => !p.IsDeleted);
            ViewBag.TotalUsers = await _db.Users.CountAsync();
            ViewBag.TotalDoctors = await _db.Users.CountAsync(u => u.IsVerifiedDoctor);

            return View();
        }

        public IActionResult About()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "about.json");
            if (System.IO.File.Exists(path))
            {
                var json = System.IO.File.ReadAllText(path);
                var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                ViewBag.AboutData = data;
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
