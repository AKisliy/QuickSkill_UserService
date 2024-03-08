using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess.Entities;

namespace UserService.DataAccess
{
    public partial class UserServiceContext : DbContext
    {
        public UserServiceContext()
        {
        }

        public UserServiceContext(DbContextOptions<UserServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BadgeEntity> Badges { get; set; }

        public virtual DbSet<UserEntity> Users { get; set; }

        public virtual DbSet<UserBadgeEntity> UserBadges { get; set; }

        public virtual DbSet<UserActivityEntity> UsersActivities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=userservice;Username=alexeykiselev;Password=kisliy");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BadgeEntity>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("badges_pkey");

                entity.ToTable("badges");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.GrayPhoto).HasColumnName("grayphoto");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Photo).HasColumnName("photo");
                entity.Property(e => e.Required).HasColumnName("required");
                entity.Property(e => e.TaskToAchieve).HasColumnName("tasktoachieve");
            });

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.FirstName).HasColumnName("firstname");
                entity.Property(e => e.GoalDays).HasColumnName("goaldays");
                entity.Property(e => e.GoalText)
                    .HasMaxLength(200)
                    .HasColumnName("goaltext");
                entity.Property(e => e.LastName).HasColumnName("lastname");
                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password");
                entity.Property(e => e.Photo).HasColumnName("photo");
                entity.Property(e => e.Status)
                    .HasDefaultValue("Default")
                    .HasMaxLength(10)
                    .HasColumnName("status");
                entity.Property(e => e.UserLevel)
                    .HasDefaultValue(1)
                    .HasColumnName("userlevel");
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.Xp)
                    .HasDefaultValue(0)
                    .HasColumnName("xp");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdat")
                    .HasDefaultValue(DateTime.UtcNow);

                entity.Property(e => e.VerifiedAt)
                    .HasColumnName("verifiedat");

                entity.Property(e => e.VerificationToken)
                    .HasColumnName("verificationtoken")
                    .HasMaxLength(128);

                entity.Property(e => e.VerificationTokenExpires)
                    .HasColumnName("verificationtokenexpires");

                entity.Property(e => e.ResetToken)
                    .HasColumnName("resettoken")
                    .HasMaxLength(128);
                entity.Property(e => e.ResetTokenExpires)
                    .HasColumnName("resettokenexpires");
            });

            modelBuilder.Entity<UserBadgeEntity>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.BadgeId }).HasName("userbadges_pkey");

                entity.ToTable("userbadges");

                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("userid");
                entity.Property(e => e.BadgeId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("badgeid");
                entity.Property(e => e.Achieved).HasColumnName("achieved");
                entity.Property(e => e.Progress)
                    .HasDefaultValue(0)
                    .HasColumnName("progress");

                entity.HasOne(d => d.Badge).WithMany(p => p.UserBadges)
                    .HasForeignKey(d => d.BadgeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("userbadges_badgeid_fkey");

                entity.HasOne(d => d.User).WithMany(p => p.UserBadges)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("userbadges_userid_fkey");
            });

            modelBuilder.Entity<UserActivityEntity>(entity =>
            {
                entity.ToTable("usersactivity");
                entity.Property(e => e.ActivityDate).HasColumnName("activitydate");
                entity.Property(e => e.ActivityType)
                    .HasMaxLength(10)
                    .HasColumnName("activitytype");
                entity.Property(e => e.UserId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("userid");

                entity.HasOne(d => d.User).WithMany(p => p.UserActivities)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("usersactivity_userid_fkey");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}

