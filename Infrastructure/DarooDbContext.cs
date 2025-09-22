using Domain.Entities.Daroo;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    /// <summary>
    /// DbContext updated to work with Bizagi generated tables
    /// </summary>
    public class DarooDbContext : DbContext
    {
        public DarooDbContext(DbContextOptions<DarooDbContext> options) : base(options) { }

        #region DbSets - Bizagi Tables
        public DbSet<Department> Departments { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<MainTitle> MainTitles { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ServiceFeature> ServiceFeatures { get; set; }
        public DbSet<MainTitleServiceFeature> MainTitleServiceFeatures { get; set; }
        public DbSet<BpmType> BpmTypes { get; set; } // اضافه کردن DbSet برای BpmType

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(e => e.Id).HasColumnName("idDepartment");
                entity.Property(e => e.FinalEnt).HasColumnName("finalEnt").HasDefaultValue(10008);
                entity.Property(e => e.BaCreatedTime).HasColumnName("baCreatedTime");
                entity.Property(e => e.BaGuid).HasColumnName("baGuid").HasDefaultValueSql("newid()");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.CreateUserID).HasColumnName("CreateUserID");
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.ModifyDate).HasColumnName("ModifyDate");
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");

                // Configure default for baCreatedTime
                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint],datediff(second,'1970-01-01',getutcdate()))*(1000)");
            });

            // Configure Scopes
            modelBuilder.Entity<Scope>(entity =>
            {
                entity.ToTable("Scopes");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(e => e.Id).HasColumnName("idScopes");
                entity.Property(e => e.FinalEnt).HasColumnName("finalEnt").HasDefaultValue(10009);
                entity.Property(e => e.BaCreatedTime).HasColumnName("baCreatedTime");
                entity.Property(e => e.BaGuid).HasColumnName("baGuid").HasDefaultValueSql("newid()");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.DepartmentId).HasColumnName("Department");
                entity.Property(e => e.CreateUserID).HasColumnName("CreateUserID");
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.ModifyDate).HasColumnName("ModifyDate");
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");

                // Configure default for baCreatedTime
                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint],datediff(second,'1970-01-01',getutcdate()))*(1000)");

                // Configure relationship with Department
                entity.HasOne(s => s.Department)
                      .WithMany(d => d.Scopes)
                      .HasForeignKey(s => s.DepartmentId)
                      .HasConstraintName("FK_Scopes_Department")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure MainTitle
            modelBuilder.Entity<MainTitle>(entity =>
            {
                entity.ToTable("MainTitle");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(e => e.Id).HasColumnName("idMainTitle");
                entity.Property(e => e.FinalEnt).HasColumnName("finalEnt").HasDefaultValue(10012);
                entity.Property(e => e.BaCreatedTime).HasColumnName("baCreatedTime");
                entity.Property(e => e.BaGuid).HasColumnName("baGuid").HasDefaultValueSql("newid()");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(150);
                entity.Property(e => e.Amount).HasColumnName("Amount").HasColumnType("money");
                entity.Property(e => e.ScopesId).HasColumnName("ScopesId");
                entity.Property(e => e.DisplayOrder).HasColumnName("DisplayOrder").HasMaxLength(50);
                entity.Property(e => e.CreateUserID).HasColumnName("CreateUserID");
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.ModifyDate).HasColumnName("ModifyDate");
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                entity.Property(e => e.BpmType).HasColumnName("BpmType");

                // Configure default for baCreatedTime
                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint],datediff(second,'1970-01-01',getutcdate()))*(1000)");

                // Configure relationship with Scope
                entity.HasOne(mt => mt.Scope)
                      .WithMany(s => s.MainTitles)
                      .HasForeignKey(mt => mt.ScopesId)
                      .HasConstraintName("FK_MainTitle_Scopes")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ProductType
            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("ProductType");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(e => e.Id).HasColumnName("idProductType");
                entity.Property(e => e.FinalEnt).HasColumnName("finalEnt").HasDefaultValue(10011);
                entity.Property(e => e.BaCreatedTime).HasColumnName("baCreatedTime");
                entity.Property(e => e.BaGuid).HasColumnName("baGuid").HasDefaultValueSql("newid()");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.ModifyDate).HasColumnName("ModifyDate");
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                entity.Property(e => e.MainTitleID).HasColumnName("MainTitleID");

                // Configure default for baCreatedTime
                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint],datediff(second,'1970-01-01',getutcdate'))*(1000)");

                // Configure relationship with MainTitle
                entity.HasOne(pt => pt.MainTitle)
                      .WithMany()
                      .HasForeignKey(pt => pt.MainTitleID)
                      .HasConstraintName("FK_ProductType_MainTitle")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BpmType>(entity =>
            {
                entity.ToTable("BpmType");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(e => e.Id).HasColumnName("idBpmType");
                entity.Property(e => e.FinalEnt).HasColumnName("finalEnt").HasDefaultValue(10010);
                entity.Property(e => e.BaCreatedTime).HasColumnName("baCreatedTime");
                entity.Property(e => e.BaGuid).HasColumnName("baGuid").HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50);
                entity.Property(e => e.CreateUserID).HasColumnName("CreateUserID");
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.ModifyDate).HasColumnName("ModifyDate");
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");

                // Configure default for baCreatedTime
                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint], DATEDIFF(SECOND, '1970-01-01', GETUTCDATE())) * (1000)");


            });


        }
    }
}