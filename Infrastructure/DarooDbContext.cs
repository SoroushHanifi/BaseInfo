using Domain.Entities.Daroo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class DarooDbContext : DbContext
    {
        public DarooDbContext(DbContextOptions<DarooDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Scope> Scopes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.HasIndex(e => e.Name);
                
            });

            // پیکربندی Scope
            modelBuilder.Entity<Scope>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(250)
                      .IsRequired();

                // رابطه با Department (One-to-Many)
                entity.HasOne(s => s.Department)
                      .WithMany(d => d.Scopes)
                      .HasForeignKey(s => s.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف CASCADE

                // Index ها برای بهبود عملکرد
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.DepartmentId);
            });
        }
    }
}
