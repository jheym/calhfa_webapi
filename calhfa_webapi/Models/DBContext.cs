using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace calhfa_webapi.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanCategory> LoanCategories { get; set; }
        public virtual DbSet<LoanStatus> LoanStatuses { get; set; }
        public virtual DbSet<LoanType> LoanTypes { get; set; }
        public virtual DbSet<StatusCode> StatusCodes { get; set; }

        public virtual DbSet<ReviewQueue> ReviewQueue { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=cal_haf_dummy");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ReviewQueue>().HasNoKey();

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.ToTable("Loan");

                entity.Property(e => e.LoanId).HasColumnName("LoanID");

                entity.Property(e => e.AnnualIncome).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.Balance).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.DeliveryDate).HasColumnType("date");

                entity.Property(e => e.InsurerNum)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InterestRate).HasColumnType("decimal(6, 5)");

                entity.Property(e => e.LoanAmount).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.LoanTypeId).HasColumnName("LoanTypeID");

                entity.Property(e => e.Lvratio)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("LVRatio");

                entity.Property(e => e.ReservDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.LoanType)
                    .WithMany(p => p.Loans)
                    .HasForeignKey(d => d.LoanTypeId)
                    .HasConstraintName("FK_Loan_LoanType");
            });

            modelBuilder.Entity<LoanCategory>(entity =>
            {
                entity.ToTable("LoanCategory");

                entity.Property(e => e.LoanCategoryId).HasColumnName("LoanCategoryID");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LoanStatus>(entity =>
            {
                entity.ToTable("LoanStatus");

                entity.Property(e => e.LoanStatusId).HasColumnName("LoanStatusID");

                entity.Property(e => e.LoanId).HasColumnName("LoanID");

                entity.Property(e => e.LoginDate).HasColumnType("datetime");

                entity.Property(e => e.LoginName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StatusDate).HasColumnType("datetime");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanStatuses)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoanStatus_Loan");

                entity.HasOne(d => d.StatusCodeNavigation)
                    .WithMany(p => p.LoanStatuses)
                    .HasForeignKey(d => d.StatusCode)
                    .HasConstraintName("FK_LoanStatus_StatusCode");
            });

            modelBuilder.Entity<LoanType>(entity =>
            {
                entity.ToTable("LoanType");

                entity.Property(e => e.LoanTypeId).HasColumnName("LoanTypeID");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ActiveEndDate).HasColumnType("date");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LoanCategoryId).HasColumnName("LoanCategoryID");

                entity.Property(e => e.LoanTypeCategoryId).HasColumnName("LoanTypeCategoryID");

                entity.Property(e => e.LongDescription)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ProductTypeId).HasColumnName("ProductTypeID");

                entity.Property(e => e.SortOrder).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.LoanCategory)
                    .WithMany(p => p.LoanTypes)
                    .HasForeignKey(d => d.LoanCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LoanType_LoanCategory");
            });

            modelBuilder.Entity<StatusCode>(entity =>
            {
                entity.HasKey(e => e.StatusCode1);

                entity.ToTable("StatusCode");

                entity.Property(e => e.StatusCode1)
                    .ValueGeneratedNever()
                    .HasColumnName("StatusCode");

                entity.Property(e => e.BusinessUnit)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConversationCategoryId).HasColumnName("ConversationCategoryID");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NotesAndAssumptions)
                    .HasMaxLength(640)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
