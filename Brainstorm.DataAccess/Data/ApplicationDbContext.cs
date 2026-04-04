using Brainstorm.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<React> Reacts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Đừng quên gọi base.OnModelCreating vì chúng ta đang kế thừa từ IdentityDbContext
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<View>()
                .HasOne(v => v.Idea)
                .WithMany()
                .HasForeignKey(v => v.IdeaId)
                .OnDelete(DeleteBehavior.NoAction); // Tắt tính năng tự động xóa (Cascade Delete) ở nhánh này
            modelBuilder.Entity<React>()
                .HasOne(v => v.Idea)
                .WithMany()
                .HasForeignKey(v => v.IdeaId)
                .OnDelete(DeleteBehavior.NoAction); // Tắt tính năng tự động xóa (Cascade Delete) ở nhánh này
            modelBuilder.Entity<Comment>()
                .HasOne(v => v.Idea)
                .WithMany()
                .HasForeignKey(v => v.IdeaId)
                .OnDelete(DeleteBehavior.NoAction); // Tắt tính năng tự động xóa (Cascade Delete) ở nhánh này
        }
    }
}
