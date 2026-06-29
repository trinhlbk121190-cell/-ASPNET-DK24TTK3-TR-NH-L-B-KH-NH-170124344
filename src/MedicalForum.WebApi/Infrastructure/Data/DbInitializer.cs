using MedicalForum.WebApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            ApplicationDbContext context, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            // 1. Seed Roles
            string[] roles = { "Admin", "Doctor", "Patient" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }

            // 2. Seed Admin User
            string adminEmail = "admin@medicalforum.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    IsVerifiedDoctor = false,
                    IsBanned = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(adminUser, "Admin@12345");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // 3. Seed default medical Categories if empty
            if (!context.Categories.Any())
            {
                var categories = new[]
                {
                    new Category { Name = "Tim mạch", Slug = "tim-mach", Description = "Chuyên khoa Tim mạch và huyết áp", Icon = "heartbeat" },
                    new Category { Name = "Da liễu", Slug = "da-lieu", Description = "Bệnh lý về da, dị ứng và thẩm mỹ da", Icon = "allergies" },
                    new Category { Name = "Nhi khoa", Slug = "nhi-khoa", Description = "Sức khỏe trẻ em và nhi sơ sinh", Icon = "baby" },
                    new Category { Name = "Dinh dưỡng", Slug = "dinh-duong", Description = "Chế độ ăn uống, quản lý cân nặng và dinh dưỡng y học", Icon = "apple-alt" },
                    new Category { Name = "Tâm lý học", Slug = "tam-ly-hoc", Description = "Tư vấn tâm lý, stress, trầm cảm và sức khỏe tinh thần", Icon = "brain" }
                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}
