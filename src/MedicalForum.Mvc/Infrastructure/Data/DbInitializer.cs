using MedicalForum.Mvc.Domain.Entities;
using MedicalForum.Mvc.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalForum.Mvc.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        // Seed Roles
        string[] roles = { "Admin", "Doctor", "Patient" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        // Seed Users
        if (!await context.Users.AnyAsync())
        {
            // Admin
            var admin = new User { UserName = "admin@medicalforum.com", Email = "admin@medicalforum.com", FullName = "Quản trị viên", EmailConfirmed = true, CreatedAt = DateTime.UtcNow };
            await userManager.CreateAsync(admin, "Admin@12345");
            await userManager.AddToRoleAsync(admin, "Admin");

            // Doctor 1
            var doctor1 = new User
            {
                UserName = "bsnguyenvana@medicalforum.com", Email = "bsnguyenvana@medicalforum.com",
                FullName = "BS. Nguyễn Văn An", EmailConfirmed = true,
                IsVerifiedDoctor = true, Specialty = "Tim mạch học", Hospital = "Bệnh viện Bạch Mai",
                DoctorBadgeApprovedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(doctor1, "Doctor@12345");
            await userManager.AddToRoleAsync(doctor1, "Doctor");

            // Doctor 2
            var doctor2 = new User
            {
                UserName = "bstranlethib@medicalforum.com", Email = "bstranlethib@medicalforum.com",
                FullName = "BS. Trần Lê Thị Bích", EmailConfirmed = true,
                IsVerifiedDoctor = true, Specialty = "Da liễu học", Hospital = "Bệnh viện Da liễu TP.HCM",
                DoctorBadgeApprovedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(doctor2, "Doctor@12345");
            await userManager.AddToRoleAsync(doctor2, "Doctor");

            // Patient 1
            var patient1 = new User { UserName = "benhnhan1@medicalforum.com", Email = "benhnhan1@medicalforum.com", FullName = "Lê Thị Hoa", EmailConfirmed = true, CreatedAt = DateTime.UtcNow };
            await userManager.CreateAsync(patient1, "Patient@12345");
            await userManager.AddToRoleAsync(patient1, "Patient");

            // Patient 2
            var patient2 = new User { UserName = "benhnhan2@medicalforum.com", Email = "benhnhan2@medicalforum.com", FullName = "Phạm Văn Hùng", EmailConfirmed = true, CreatedAt = DateTime.UtcNow };
            await userManager.CreateAsync(patient2, "Patient@12345");
            await userManager.AddToRoleAsync(patient2, "Patient");

            await context.SaveChangesAsync();
        }

        // Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Tim mạch", Slug = "tim-mach", Description = "Bệnh tim, mạch máu, huyết áp" },
                new Category { Name = "Da liễu", Slug = "da-lieu", Description = "Bệnh da, mụn, viêm nhiễm da" },
                new Category { Name = "Nhi khoa", Slug = "nhi-khoa", Description = "Sức khoẻ trẻ em, trẻ sơ sinh" },
                new Category { Name = "Dinh dưỡng", Slug = "dinh-duong", Description = "Chế độ ăn, thực phẩm, bổ sung" },
                new Category { Name = "Tâm lý học", Slug = "tam-ly-hoc", Description = "Stress, lo âu, trầm cảm, giấc ngủ" }
            );
            await context.SaveChangesAsync();
        }

        // Seed Posts with comments
        if (!await context.Posts.AnyAsync())
        {
            var doctor1 = await userManager.FindByEmailAsync("bsnguyenvana@medicalforum.com");
            var doctor2 = await userManager.FindByEmailAsync("bstranlethib@medicalforum.com");
            var patient1 = await userManager.FindByEmailAsync("benhnhan1@medicalforum.com");
            var patient2 = await userManager.FindByEmailAsync("benhnhan2@medicalforum.com");
            var catHeartId = (await context.Categories.FirstAsync(c => c.Slug == "tim-mach")).Id;
            var catSkinId = (await context.Categories.FirstAsync(c => c.Slug == "da-lieu")).Id;
            var catKidsId = (await context.Categories.FirstAsync(c => c.Slug == "nhi-khoa")).Id;
            var catNutrId = (await context.Categories.FirstAsync(c => c.Slug == "dinh-duong")).Id;
            var catMindId = (await context.Categories.FirstAsync(c => c.Slug == "tam-ly-hoc")).Id;

            var post1 = new Post
            {
                Title = "Đau ngực thoáng qua khi leo cầu thang — có cần lo không?",
                Content = "Chào bác sĩ, em 38 tuổi, hay bị đau tức ngực trái thoáng qua khi leo hơn 2 tầng cầu thang hoặc đi bộ nhanh. Đau khoảng 1-2 phút rồi tự hết, không lan ra cánh tay. Huyết áp đo ở nhà thường 130/85. Em có nên đi khám không? Và khám ở khoa nào là phù hợp?",
                CategoryId = catHeartId, AuthorId = patient1!.Id, IsAnonymous = false, Views = 342, Upvotes = 24,
                MedicalDisclaimer = "Thông tin chỉ mang tính chất tham khảo.", CreatedAt = DateTime.UtcNow.AddDays(-5)
            };
            var post2 = new Post
            {
                Title = "Da mặt nổi mụn li ti sau khi dùng kem dưỡng mới — dị ứng hay gì?",
                Content = "Mình 25 tuổi, da thường. Tuần trước mình bắt đầu dùng kem dưỡng ẩm mới (chứa niacinamide 10% và hyaluronic acid). Sau 3 ngày da mình nổi nhiều mụn li ti nhỏ trắng ở vùng má và trán, hơi ngứa. Mình có nên dừng dùng sản phẩm đó không? Hay là phản ứng purging bình thường?",
                CategoryId = catSkinId, AuthorId = patient2!.Id, IsAnonymous = true, Views = 516, Upvotes = 31,
                MedicalDisclaimer = "Thông tin chỉ mang tính chất tham khảo.", CreatedAt = DateTime.UtcNow.AddDays(-3)
            };
            var post3 = new Post
            {
                Title = "Bé 8 tháng tuổi sốt 38.5°C — khi nào cần đưa đi cấp cứu?",
                Content = "Con mình 8 tháng tuổi, buổi sáng tự nhiên sốt 38.5°C. Bé vẫn bú bình thường, không có biểu hiện khó thở hay co giật. Mình đã lau mát và cho uống thuốc hạ sốt paracetamol liều phù hợp cân nặng. Xin hỏi khi nào cần đưa đi cấp cứu ngay? Sốt bao nhiêu độ mới nguy hiểm?",
                CategoryId = catKidsId, AuthorId = patient1!.Id, IsAnonymous = false, Views = 783, Upvotes = 48,
                MedicalDisclaimer = "Thông tin chỉ mang tính chất tham khảo.", CreatedAt = DateTime.UtcNow.AddDays(-2)
            };
            var post4 = new Post
            {
                Title = "Thường xuyên lo âu, khó ngủ từ khi đi làm — làm sao cải thiện?",
                Content = "Mình 28 tuổi, đi làm văn phòng được 2 năm. Gần đây deadline nhiều, mình thấy lo lắng liên tục ngay cả khi không có việc gì cụ thể. Tối nằm mãi không ngủ được dù rất mệt, khoảng 1-2 giờ sáng mới ngủ. Sáng dậy vẫn mệt. Mình có đang bị lo âu mạn tính không? Và có cần uống thuốc không?",
                CategoryId = catMindId, AuthorId = patient2!.Id, IsAnonymous = true, Views = 425, Upvotes = 37,
                MedicalDisclaimer = "Thông tin chỉ mang tính chất tham khảo.", CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            context.Posts.AddRange(post1, post2, post3, post4);
            await context.SaveChangesAsync();

            // Comments
            context.Comments.AddRange(
                new Comment
                {
                    PostId = post1.Id, AuthorId = doctor1!.Id, Content = "Chào bạn! Dựa trên mô tả của bạn, triệu chứng đau ngực khi gắng sức kết hợp với huyết áp 130/85 là những dấu hiệu cần được đánh giá bởi bác sĩ tim mạch. Đây có thể là biểu hiện của thiếu máu cơ tim khi gắng sức (Stable Angina).\n\nBạn nên:\n1. Đặt lịch khám chuyên khoa Tim mạch sớm, không cần cấp cứu.\n2. Yêu cầu bác sĩ chỉ định điện tim (ECG) và xét nghiệm máu (lipid máu, đường huyết).\n3. Tránh gắng sức mạnh đột ngột trong lúc chờ khám.\n4. Nếu đau kéo dài hơn 5 phút, lan ra cánh tay, hàm hoặc kèm khó thở, toát mồ hôi — gọi 115 ngay.",
                    IsDoctorResponse = true, IsAnonymous = false,
                    MedicalDisclaimer = "Bác sĩ khuyến cáo: Thông tin này chỉ mang tính tham khảo, không thay thế việc thăm khám trực tiếp.",
                    Upvotes = 18, CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new Comment
                {
                    PostId = post1.Id, AuthorId = patient2!.Id, Content = "Mình trước cũng bị tương tự, đi khám ra bị tăng huyết áp giai đoạn 1. Bác sĩ cho uống thuốc và theo dõi, giờ đã ổn hơn rồi. Bạn nên đi khám ngay đi nhé!",
                    IsAnonymous = false, Upvotes = 7, CreatedAt = DateTime.UtcNow.AddDays(-4).AddHours(2)
                },
                new Comment
                {
                    PostId = post2.Id, AuthorId = doctor2!.Id, Content = "Đây có thể là phản ứng kích ứng (irritation) với nồng độ niacinamide 10% — khá cao cho người mới bắt đầu dùng.\n\n**Purging vs Kích ứng:**\n- Purging thường xảy ra với các sản phẩm tăng tốc chu kỳ da (retinol, AHA/BHA) — KHÔNG phải niacinamide.\n- Những gì bạn mô tả nhiều khả năng là kích ứng.\n\n**Khuyến nghị:**\n1. Dừng sản phẩm đó trong 7-14 ngày.\n2. Chỉ dùng sữa rửa mặt nhẹ và kem dưỡng ẩm đơn giản.\n3. Nếu muốn thử lại, chọn niacinamide 5% và dùng cách ngày lúc đầu.\n4. Nếu da viêm đỏ, ngứa nhiều — đến khám da liễu để được kê thuốc phù hợp.",
                    IsDoctorResponse = true, IsAnonymous = false,
                    MedicalDisclaimer = "Bác sĩ khuyến cáo: Thông tin này chỉ mang tính tham khảo, không thay thế việc thăm khám trực tiếp.",
                    Upvotes = 25, CreatedAt = DateTime.UtcNow.AddDays(-2).AddHours(-5)
                },
                new Comment
                {
                    PostId = post3.Id, AuthorId = doctor1!.Id, Content = "Chào bạn! Đây là những dấu hiệu CẦN ĐƯA BÉ ĐI CẤP CỨU NGAY:\n\n🚨 **Cấp cứu ngay khi bé:**\n- Sốt > 39.5°C và không hạ sau thuốc 30 phút\n- Thở nhanh, thở rít, rút lõm ngực\n- Co giật\n- Người tím tái, li bì, không phản ứng\n- Bỏ bú hoàn toàn, nôn ói nhiều\n- Phát ban đột ngột\n\n✅ **Với trường hợp của bé:** Sốt 38.5°C, bú bình thường, không co giật — bạn đang xử lý đúng. Theo dõi sát mỗi 2-4 giờ, đo thân nhiệt thường xuyên. Đưa đi khám nhi khoa trong ngày hôm nay để tìm nguyên nhân.",
                    IsDoctorResponse = true, IsAnonymous = false,
                    MedicalDisclaimer = "Bác sĩ khuyến cáo: Thông tin này chỉ mang tính tham khảo, không thay thế việc thăm khám trực tiếp.",
                    Upvotes = 42, CreatedAt = DateTime.UtcNow.AddDays(-1).AddHours(-10)
                }
            );
            await context.SaveChangesAsync();
        }
    }
}
