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
        public DbSet<MainTitle> MainTitles { get; set; }

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

            // پیکربندی MainTitle
            modelBuilder.Entity<MainTitle>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(300)
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasMaxLength(1000);


                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.CreateUserId)
                      .IsRequired();

                // رابطه با Scope (One-to-Many)
                entity.HasOne(mt => mt.Scope)
                      .WithMany(s => s.MainTitles)
                      .HasForeignKey(mt => mt.ScopeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Index ها برای بهبود عملکرد
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.ScopeId);
                entity.HasIndex(e => e.IsDelete);
                entity.HasIndex(e => e.DisplayOrder);

                // Index ترکیبی برای بهبود query های متداول
                entity.HasIndex(e => new { e.ScopeId, e.IsDelete, e.DisplayOrder });
            });

        }
    }
}
