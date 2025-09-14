using Domain.Entities.Daroo;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class DarooDbContext : DbContext
    {
        public DarooDbContext(DbContextOptions<DarooDbContext> options) : base(options) { }

        #region DbSets
        public DbSet<Department> Departments { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<MainTitle> MainTitles { get; set; }
        public DbSet<ServiceFeature> ServiceFeatures { get; set; }
        public DbSet<MainTitleServiceFeature> MainTitleServiceFeatures { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // پیکربندی Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.CreateUserId)
                      .IsRequired();

                // Index برای بهبود عملکرد
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.IsDelete);
                entity.HasIndex(e => e.CreateUserId);
                entity.HasIndex(e => e.CreateDate);
            });

            // پیکربندی Scope
            modelBuilder.Entity<Scope>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(250)
                      .IsRequired();

                entity.Property(e => e.CreateUserId)
                      .IsRequired();

                // رابطه با Department (One-to-Many)
                entity.HasOne(s => s.Department)
                      .WithMany(d => d.Scopes)
                      .HasForeignKey(s => s.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Index ها برای بهبود عملکرد
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.DepartmentId);
                entity.HasIndex(e => e.IsDelete);
                entity.HasIndex(e => e.CreateUserId);
                entity.HasIndex(e => e.CreateDate);

                // Index ترکیبی
                entity.HasIndex(e => new { e.DepartmentId, e.IsDelete });
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
                entity.HasIndex(e => e.CreateUserId);
                entity.HasIndex(e => e.CreateDate);

                // Index ترکیبی برای بهبود query های متداول
                entity.HasIndex(e => new { e.ScopeId, e.IsDelete, e.DisplayOrder });
                entity.HasIndex(e => new { e.IsDelete, e.Amount });
            });

            // پیکربندی ServiceFeature
            modelBuilder.Entity<ServiceFeature>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasMaxLength(500);

                entity.Property(e => e.Code)
                      .HasMaxLength(50);

                entity.Property(e => e.Icon)
                      .HasMaxLength(100);

                entity.Property(e => e.Color)
                      .HasMaxLength(20);

                entity.Property(e => e.CreateUserId)
                      .IsRequired();

                // Index ها برای بهبود عملکرد
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
                entity.HasIndex(e => e.IsDelete);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.CreateUserId);

                // Index ترکیبی
                entity.HasIndex(e => new { e.IsDelete, e.IsActive, e.DisplayOrder });
            });

            // پیکربندی MainTitleServiceFeature (Junction Table)
            modelBuilder.Entity<MainTitleServiceFeature>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Alternative Composite Key (اختیاری - اگر می‌خواهید از Id جداگانه استفاده نکنید)
                // entity.HasKey(e => new { e.MainTitleId, e.ServiceFeatureId });

                entity.Property(e => e.Notes)
                      .HasMaxLength(500);

                entity.Property(e => e.CreateUserId)
                      .IsRequired();

                // رابطه با MainTitle
                entity.HasOne(mtsf => mtsf.MainTitle)
                      .WithMany(mt => mt.MainTitleServiceFeatures)
                      .HasForeignKey(mtsf => mtsf.MainTitleId)
                      .OnDelete(DeleteBehavior.Cascade); // حذف CASCADE برای junction table

                // رابطه با ServiceFeature
                entity.HasOne(mtsf => mtsf.ServiceFeature)
                      .WithMany(sf => sf.MainTitleServiceFeatures)
                      .HasForeignKey(mtsf => mtsf.ServiceFeatureId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Index ها برای بهبود عملکرد
                entity.HasIndex(e => e.MainTitleId);
                entity.HasIndex(e => e.ServiceFeatureId);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.IsDelete);
                entity.HasIndex(e => e.DisplayOrder);
                entity.HasIndex(e => e.CreateUserId);

                // Index ترکیبی برای query های متداول
                entity.HasIndex(e => new { e.MainTitleId, e.IsDelete, e.IsActive });
                entity.HasIndex(e => new { e.ServiceFeatureId, e.IsDelete, e.IsActive });
                entity.HasIndex(e => new { e.MainTitleId, e.ServiceFeatureId, e.IsDelete });

                // Unique constraint برای جلوگیری از تکرار رابطه
                entity.HasIndex(e => new { e.MainTitleId, e.ServiceFeatureId })
                      .IsUnique()
                      .HasFilter("[IsDelete] = 0"); // فقط برای رکوردهای حذف نشده
            });
        }
    }
}