using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace gym.Data;

public partial class GymContext : DbContext
{
    public GymContext()
    {
    }

    public GymContext(DbContextOptions<GymContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberPakage> MemberPakages { get; set; }

    public virtual DbSet<MemberPayment> MemberPayments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Package> Packages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    public virtual DbSet<TrainingSchedule> TrainingSchedules { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserNotification> UserNotifications { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=haiit;Initial Catalog=GYM;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId)
                .HasName("PK_MEMBER")
                .IsClustered(false);

            entity.ToTable("Member");

            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("createDate");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("dateOfBirth");
            entity.Property(e => e.FullName)
                .HasColumnType("text")
                .HasColumnName("fullName");
            entity.Property(e => e.Phone)
                .HasColumnType("text")
                .HasColumnName("phone");
            entity.Property(e => e.Sex).HasColumnName("sex");
        });

        modelBuilder.Entity<MemberPakage>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.PackageId }).HasName("PK_MEMBERPAKAGE");

            entity.ToTable("MemberPakage");

            entity.HasIndex(e => e.PackageId, "MemberPakage2_FK");

            entity.HasIndex(e => e.MemberId, "MemberPakage_FK");

            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.PackageId).HasColumnName("packageId");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("endDate");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.IsPaid).HasColumnName("isPaid");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("startDate");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberPakages)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAK_MEMBER");

            entity.HasOne(d => d.Package).WithMany(p => p.MemberPakages)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAK_PACKAGE");
        });

        modelBuilder.Entity<MemberPayment>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.PaymentId, e.StaffId }).HasName("PK_MEMBERPAYMENT");

            entity.ToTable("MemberPayment");

            entity.HasIndex(e => e.PaymentId, "MemberPayment2_FK");

            entity.HasIndex(e => e.StaffId, "MemberPayment3_FK");

            entity.HasIndex(e => e.MemberId, "MemberPayment_FK");

            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("datetime")
                .HasColumnName("paymentDate");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberPayments)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAY_MEMBER");

            entity.HasOne(d => d.Payment).WithMany(p => p.MemberPayments)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAY_PAYMENT");

            entity.HasOne(d => d.Staff).WithMany(p => p.MemberPayments)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBERPA_MEMBERPAY_STAFF");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId)
                .HasName("PK_NOTIFICATION")
                .IsClustered(false);

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("notificationId");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.SendRole)
                .HasColumnType("text")
                .HasColumnName("sendRole");
            entity.Property(e => e.Title)
                .HasColumnType("text")
                .HasColumnName("title");
        });

        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.PackageId)
                .HasName("PK_PACKAGE")
                .IsClustered(false);

            entity.ToTable("Package");

            entity.Property(e => e.PackageId).HasColumnName("packageId");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.DurationInDays).HasColumnName("durationInDays");
            entity.Property(e => e.Name)
                .HasColumnType("text")
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Type)
                .HasColumnType("text")
                .HasColumnName("type");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId)
                .HasName("PK_PAYMENT")
                .IsClustered(false);

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Method)
                .HasColumnType("text")
                .HasColumnName("method");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId)
                .HasName("PK_ROLE")
                .IsClustered(false);

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasColumnType("text")
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId)
                .HasName("PK_STAFF")
                .IsClustered(false);

            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.Email)
                .HasColumnType("text")
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasColumnType("text")
                .HasColumnName("fullName");
            entity.Property(e => e.Phone)
                .HasColumnType("text")
                .HasColumnName("phone");
            entity.Property(e => e.WorkingSince)
                .HasColumnType("datetime")
                .HasColumnName("workingSince");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId)
                .HasName("PK_TRAINER")
                .IsClustered(false);

            entity.ToTable("Trainer");

            entity.Property(e => e.TrainerId).HasColumnName("trainerId");
            entity.Property(e => e.FullName)
                .HasColumnType("text")
                .HasColumnName("fullName");
            entity.Property(e => e.Phone)
                .HasColumnType("text")
                .HasColumnName("phone");
            entity.Property(e => e.ScheduleNote)
                .HasColumnType("text")
                .HasColumnName("scheduleNote");
            entity.Property(e => e.Specialty)
                .HasColumnType("text")
                .HasColumnName("specialty");
        });

        modelBuilder.Entity<TrainingSchedule>(entity =>
        {
            entity.HasKey(e => new { e.TrainerId, e.MemberId }).HasName("PK_TRAININGSCHEDULE");

            entity.ToTable("TrainingSchedule");

            entity.HasIndex(e => e.MemberId, "TrainingSchedule2_FK");

            entity.HasIndex(e => e.TrainerId, "TrainingSchedule_FK");

            entity.Property(e => e.TrainerId).HasColumnName("trainerId");
            entity.Property(e => e.MemberId).HasColumnName("memberId");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.Node)
                .HasColumnType("text")
                .HasColumnName("node");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("startTime");
            entity.Property(e => e.TrainingDate)
                .HasColumnType("datetime")
                .HasColumnName("trainingDate");

            entity.HasOne(d => d.Member).WithMany(p => p.TrainingSchedules)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRAINING_TRAININGS_MEMBER");

            entity.HasOne(d => d.Trainer).WithMany(p => p.TrainingSchedules)
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRAINING_TRAININGS_TRAINER");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("PK_USER")
                .IsClustered(false);

            entity.ToTable("User");

            entity.HasIndex(e => e.RoleId, "include_FK");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Email)
                .HasColumnType("text")
                .HasColumnName("email");
            entity.Property(e => e.IsAtive).HasColumnName("isAtive");
            entity.Property(e => e.Password)
                .HasColumnType("text")
                .HasColumnName("password");
            entity.Property(e => e.ReferenceId).HasColumnName("referenceId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasColumnType("text")
                .HasColumnName("userName");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USER_INCLUDE_ROLE");
        });

        modelBuilder.Entity<UserNotification>(entity =>
        {
            entity.HasKey(e => new { e.NotificationId, e.UserId }).HasName("PK_USERNOTIFICATION");

            entity.ToTable("UserNotification");

            entity.HasIndex(e => e.UserId, "UserNotification2_FK");

            entity.HasIndex(e => e.NotificationId, "UserNotification_FK");

            entity.Property(e => e.NotificationId).HasColumnName("notificationId");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Seen).HasColumnName("seen");
            entity.Property(e => e.TimeSend)
                .HasColumnType("datetime")
                .HasColumnName("timeSend");

            entity.HasOne(d => d.Notification).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERNOTI_USERNOTIF_NOTIFICA");

            entity.HasOne(d => d.User).WithMany(p => p.UserNotifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USERNOTI_USERNOTIF_USER");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
