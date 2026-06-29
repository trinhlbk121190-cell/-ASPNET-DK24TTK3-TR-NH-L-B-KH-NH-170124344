using MedicalForum.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<Post> Posts { get; }
        DbSet<Comment> Comments { get; }
        DbSet<Vote> Votes { get; }
        DbSet<Report> Reports { get; }
        DbSet<DoctorVerificationRequest> DoctorVerificationRequests { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
