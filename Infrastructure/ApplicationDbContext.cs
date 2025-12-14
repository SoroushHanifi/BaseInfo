using Domain.Entities.Daroo;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    /// <summary>
    /// DbContext updated to work with Bizagi generated tables
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        #region DbSets - Bizagi Tables
        public DbSet<Department> Departments { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<MainTitle> MainTitles { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ServiceFeature> ServiceFeature { get; set; }
        public DbSet<MainTitleServiceFeature> MainTitleServiceFeature { get; set; }
        public DbSet<BpmType> BpmTypes { get; set; }
        public DbSet<ProduceType> ProduceTypes { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Payment> Payments { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configure Payments
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(e => e.Id).HasColumnName("idPayments");
                entity.Property(e => e.MainTitleId).HasColumnName("MainTitleId");
                entity.Property(e => e.NationalCode).HasColumnName("NationalCode");
                entity.Property(e => e.PaymentAmount).HasColumnName("PaymentAmount");
                entity.Property(e => e.PaymentResult).HasColumnName("PaymentResult");
                entity.Property(e => e.PaymentDate).HasColumnName("PaymentDate");
                entity.Property(e => e.Founder).HasColumnName("Founder");


                // Configure relationship with MainTitle
                entity.HasOne(e => e.MainTitle)
                    .WithMany(e => e.Payments)
                    .HasForeignKey(e => e.MainTitleId)
                    .HasConstraintName("FK_MainTitle_Payments")
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure default for baCreatedTime
                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint],datediff(second,'1970-01-01',getutcdate()))*(1000)");

            });

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
                entity.Property(e => e.ScopesId).HasColumnName("Scopes");
                entity.Property(e => e.DisplayOrder).HasColumnName("DisplayOrder").HasMaxLength(50);
                entity.Property(e => e.CreateUserID).HasColumnName("CreateUserID");
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.ModifyDate).HasColumnName("ModifyDate");
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");
                entity.Property(e => e.BpmType).HasColumnName("BpmType");
                entity.Property(e => e.ProduceType).HasColumnName("ProduceType");


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

            modelBuilder.Entity<ProduceType>(entity =>
            {
                entity.ToTable("ProduceType");
                entity.HasKey(e => e.Id);

                // Configure propertiesi
                entity.Property(e => e.Id).HasColumnName("idProduceType");
                entity.Property(e => e.FinalEnt).HasColumnName("finalEnt").HasDefaultValue(10020);
                entity.Property(e => e.BaCreatedTime).HasColumnName("baCreatedTime");
                entity.Property(e => e.BaGuid).HasColumnName("baGuid").HasDefaultValueSql("newid()");
                entity.Property(e => e.Title).HasColumnName("Title").HasMaxLength(50);
                entity.Property(e => e.Code).HasColumnName("Code");
                entity.Property(e => e.IsActive).HasColumnName("IsActive");
                entity.Property(e => e.CreateDate).HasColumnName("CreateDate");
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted");

                // Configure default for baCreatedTime
                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint],datediff(second,'1970-01-01',getutcdate()))*(1000)");
            });

            modelBuilder.Entity<ServiceFeature>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("idServiceFeature");
            });

            modelBuilder.Entity<MainTitleServiceFeature>(entity =>
            {
                entity.ToTable("MainTitleServiceFeature"); // بهتره صریح بگی

                entity.Property(e => e.Id).HasColumnName("idMainTitleServiceFeature");

                // اینا FK هستن، نه navigation
                entity.Property(e => e.ServiceFeature).HasColumnName("ServiceFeature");
                entity.Property(e => e.MainTitle).HasColumnName("MainTitle");

                // رابطه با MainTitle
                entity.HasOne(a => a.MainTitleVirtual)
                      .WithMany()
                      .HasForeignKey(a => a.MainTitle)  // ← درست: به پراپرتی long اشاره می‌کنه
                      .HasConstraintName("FK_MainTitleServiceFeature_MainTitle")
                      .OnDelete(DeleteBehavior.Restrict);

                // رابطه با ServiceFeature
                entity.HasOne(a => a.ServiceFeatureVirtual)
                      .WithMany()
                      .HasForeignKey(a => a.ServiceFeature)  // ← درست: چون ServiceFeature یک long هست
                      .HasConstraintName("FK_MainTitleServiceFeature_ServiceFeature")
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Approval>(entity =>
            {
                entity.ToTable("Approval");
                entity.HasKey(e => e.Id);

                // Configure properties
                entity.Property(e => e.Id)
                      .HasColumnName("idApproval");

                entity.Property(e => e.FinalEnt)
                      .HasColumnName("finalEnt")
                      .HasDefaultValue(10030);

                entity.Property(e => e.BaCreatedTime)
                      .HasColumnName("baCreatedTime");

                entity.Property(e => e.BaGuid)
                      .HasColumnName("baGuid")
                      .HasDefaultValueSql("NEWID()");

                entity.Property(e => e.ExecutionDate)
                      .HasColumnName("ExecutionDate")
                      .HasColumnType("datetime");

                entity.Property(e => e.TariffStartDate)
                      .HasColumnName("TariffStartDate")
                      .HasColumnType("datetime");

                entity.Property(e => e.TariffEndDate)
                      .HasColumnName("TariffEndDate")
                      .HasColumnType("datetime");

                entity.Property(e => e.MainTitleId)
                      .HasColumnName("MainTitle");

                entity.Property(e => e.Amount)
                      .HasColumnName("Amount")
                      .HasColumnType("money");

                entity.Property(e => e.IsActive)
                      .HasColumnName("IsActive");

                entity.Property(e => e.BaCreatedTime)
                      .HasDefaultValueSql("CONVERT([bigint], DATEDIFF(SECOND, '1970-01-01', GETUTCDATE())) * (1000)");

                // Relationship with MainTitle
                entity.HasOne(a => a.MainTitle)
                      .WithMany() // اگر MainTitle لیستی از Approval ندارد، WithMany() بدون پارامتر
                      .HasForeignKey(a => a.MainTitleId)
                      .HasConstraintName("FK_Approval_MainTitle")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(a => a.MainTitleId);
                entity.HasIndex(a => new { a.MainTitleId, a.TariffStartDate });
            });


        }

        public long GetLastId<TEntity>(string idPropertyName = "Id") where TEntity : class
        {
            var lastItem = Set<TEntity>().OrderBy(e => EF.Property<long>(e, idPropertyName)).LastOrDefault();
            if (lastItem == null)
                return 0;

            var propertyInfo = typeof(TEntity).GetProperty(idPropertyName);
            if (propertyInfo == null)
                throw new InvalidOperationException($"Property {idPropertyName} not found on type {typeof(TEntity).Name}");

            return (long)propertyInfo.GetValue(lastItem);
        }
    }
}