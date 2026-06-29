using MedicalForum.WebApi.Application.Interfaces;
using MedicalForum.WebApi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalForum.WebApi.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Vote> Votes { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<DoctorVerificationRequest> DoctorVerificationRequests { get; set; } = null!;

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Category configuration
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // Post configuration
            builder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(250);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.MedicalDisclaimer).IsRequired().HasMaxLength(500);

                // Relationship with User (Author)
                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relationship with Category
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Comment configuration
            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();

                // Relationship with Post
                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with User (Author)
                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Vote configuration
            builder.Entity<Vote>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique vote constraint: A user can vote only once per Post or Comment
                entity.HasIndex(e => new { e.UserId, e.PostId }).IsUnique().HasFilter("[PostId] IS NOT NULL");
                entity.HasIndex(e => new { e.UserId, e.CommentId }).IsUnique().HasFilter("[CommentId] IS NOT NULL");
            });

            // Report configuration
            builder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Reason).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Details).IsRequired().HasMaxLength(1000);

                entity.HasOne(d => d.Reporter)
                    .WithMany(p => p.ReportsSubmitted)
                    .HasForeignKey(d => d.ReporterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Doctor Verification Request configuration
            builder.Entity<DoctorVerificationRequest>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.VerificationRequests)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
