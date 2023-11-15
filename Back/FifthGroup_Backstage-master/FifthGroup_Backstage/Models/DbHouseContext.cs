using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FifthGroup_Backstage.Models;

public partial class DbHouseContext : DbContext
{
    public DbHouseContext()
    {
    }

    public DbHouseContext(DbContextOptions<DbHouseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<Community> Communities { get; set; }

    public virtual DbSet<CommunityBuilding> CommunityBuildings { get; set; }

    public virtual DbSet<EcpayOrder> EcpayOrders { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentItem> PaymentItems { get; set; }

    public virtual DbSet<PaymentItemsName> PaymentItemsNames { get; set; }

    public virtual DbSet<Periodoftime> Periodoftimes { get; set; }

    public virtual DbSet<PublicSpaceDetail> PublicSpaceDetails { get; set; }

    public virtual DbSet<Registration> Registrations { get; set; }

    public virtual DbSet<Repair> Repairs { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationPlace> ReservationPlaces { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    public virtual DbSet<ResidentManager> ResidentManagers { get; set; }

    public virtual DbSet<ResidentRegister> ResidentRegisters { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=dbhouse05.database.windows.net;Initial Catalog=dbHouse;Persist Security Info=True;User ID=Showshow306;Password=WaveF0219306");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Chinese_Taiwan_Stroke_CS_AS");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Member");

            entity.ToTable("Admin");

            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UserID");
            entity.Property(e => e.UserAddress).HasMaxLength(255);
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(20);
            entity.Property(e => e.UserPhone)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.UserPhoto).HasMaxLength(255);
            entity.Property(e => e.UserPwd)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.VerificationCode).HasMaxLength(128);

            entity.HasOne(d => d.Community).WithMany(p => p.Admins)
                .HasForeignKey(d => d.CommunityId)
                .HasConstraintName("FK_Admin_Community");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplyCode).HasName("PK__Applicat__E76F63EAFB55CF45");

            entity.Property(e => e.Activities).HasMaxLength(20);
            entity.Property(e => e.ActivityName).HasMaxLength(50);
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.HouseholdCode).HasMaxLength(20);
            entity.Property(e => e.Image).HasMaxLength(50);
            entity.Property(e => e.Introduce).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(10);
            entity.Property(e => e.Phone).HasMaxLength(10);

            entity.HasOne(d => d.HouseholdCodeNavigation).WithMany(p => p.Applications)
                .HasForeignKey(d => d.HouseholdCode)
                .HasConstraintName("FK__Applicati__House__4316F928");

            entity.HasOne(d => d.ReserveCodeNavigation).WithMany(p => p.Applications)
                .HasForeignKey(d => d.ReserveCode)
                .HasConstraintName("FK__Applicati__Reser__440B1D61");
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.Content).HasMaxLength(1500);
            entity.Property(e => e.FeacturedImageUrl).HasMaxLength(500);
            entity.Property(e => e.Heading).HasMaxLength(200);
            entity.Property(e => e.PageTitle).HasMaxLength(100);
            entity.Property(e => e.ShortDescription).HasMaxLength(500);
            entity.Property(e => e.UrlHandle).HasMaxLength(500);

            entity.HasMany(d => d.Tags).WithMany(p => p.BlogPosts)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogPostTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<BlogPost>().WithMany()
                        .HasForeignKey("BlogPostsId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("BlogPostsId", "TagsId");
                        j.ToTable("BlogPostTag");
                    });
        });

        modelBuilder.Entity<Community>(entity =>
        {
            entity.ToTable("Community");

            entity.Property(e => e.Address).HasMaxLength(1000);
            entity.Property(e => e.CommunityName).HasMaxLength(100);
            entity.Property(e => e.VerificationCode).HasMaxLength(100);
        });

        modelBuilder.Entity<CommunityBuilding>(entity =>
        {
            entity.ToTable("CommunityBuilding");

            entity.Property(e => e.BuildingName).HasMaxLength(100);

            entity.HasOne(d => d.Community).WithMany(p => p.CommunityBuildings)
                .HasForeignKey(d => d.CommunityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommunityBuilding_Community");
        });

        modelBuilder.Entity<EcpayOrder>(entity =>
        {
            entity.HasKey(e => e.MerchantTradeNo);

            entity.Property(e => e.MerchantTradeNo).HasMaxLength(50);
            entity.Property(e => e.MemberId)
                .HasMaxLength(50)
                .HasColumnName("MemberID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentType).HasMaxLength(50);
            entity.Property(e => e.PaymentTypeChargeFee).HasMaxLength(50);
            entity.Property(e => e.RtnMsg).HasMaxLength(50);
            entity.Property(e => e.TradeDate).HasMaxLength(50);
            entity.Property(e => e.TradeNo).HasMaxLength(50);
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.EmailCode);

            entity.ToTable("Email");

            entity.Property(e => e.Body).HasMaxLength(4000);
            entity.Property(e => e.FromEmail).HasMaxLength(200);
            entity.Property(e => e.HouseholdCode).HasMaxLength(20);
            entity.Property(e => e.Subject).HasMaxLength(1000);
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.ToEmail).HasMaxLength(200);

            entity.HasOne(d => d.HouseholdCodeNavigation).WithMany(p => p.Emails)
                .HasForeignKey(d => d.HouseholdCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Email_Residents");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentCode).HasName("PK__Payments__106D3BA9FF1FF5DB");

            entity.Property(e => e.HouseholdCode).HasMaxLength(20);
            entity.Property(e => e.MerchantTradeNo).HasMaxLength(50);
            entity.Property(e => e.PayDay).HasColumnType("datetime");

            entity.HasOne(d => d.HouseholdCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.HouseholdCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__Househ__5535A963");

            entity.HasOne(d => d.MerchantTradeNoNavigation).WithMany(p => p.Payments).HasForeignKey(d => d.MerchantTradeNo);

            entity.HasOne(d => d.PaymentItemCodeNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentItemCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_PaymentItems");
        });

        modelBuilder.Entity<PaymentItem>(entity =>
        {
            entity.HasKey(e => e.PaymentItemCode).HasName("PK__PaymentI__AD77CD8708540BAA");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.PaymentName).HasMaxLength(20);
            entity.Property(e => e.Remark).HasMaxLength(1000);

            entity.HasOne(d => d.CommunityBuilding).WithMany(p => p.PaymentItems)
                .HasForeignKey(d => d.CommunityBuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PaymentItems_CommunityBuilding");

            entity.HasOne(d => d.ItemClassificationCodeNavigation).WithMany(p => p.PaymentItems)
                .HasForeignKey(d => d.ItemClassificationCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentIt__ItemC__59FA5E80");
        });

        modelBuilder.Entity<PaymentItemsName>(entity =>
        {
            entity.HasKey(e => e.ItemClassificationCode).HasName("PK__PaymentI__2973A7BA78E89F0C");

            entity.ToTable("PaymentItemsName");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Periodoftime>(entity =>
        {
            entity.HasKey(e => e.PeriodoftimeCode);

            entity.ToTable("Periodoftime");

            entity.Property(e => e.PeriodoftimeCode).ValueGeneratedNever();
            entity.Property(e => e.Periodoftime1)
                .HasMaxLength(50)
                .HasColumnName("Periodoftime");
        });

        modelBuilder.Entity<PublicSpaceDetail>(entity =>
        {
            entity.HasKey(e => e.PlaceCode).HasName("PK__PublicSp__4742E30576E3AACA");

            entity.ToTable("PublicSpaceDetail");

            entity.Property(e => e.AreaCode).HasMaxLength(20);
            entity.Property(e => e.Pid)
                .HasMaxLength(20)
                .HasColumnName("PId");
            entity.Property(e => e.PlaceName).HasMaxLength(20);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.HasKey(e => e.AutoCode).HasName("PK__Registra__B61EA5C902209FBD");

            entity.Property(e => e.HouseholdCode).HasMaxLength(20);

            entity.HasOne(d => d.ApplyCodeNavigation).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.ApplyCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__Apply__4F7CD00D");

            entity.HasOne(d => d.HouseholdCodeNavigation).WithMany(p => p.Registrations)
                .HasForeignKey(d => d.HouseholdCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Registrat__House__5070F446");
        });

        modelBuilder.Entity<Repair>(entity =>
        {
            entity.HasKey(e => e.RepairCode).HasName("PK__Repairs__615611833F0BDDE2");

            entity.Property(e => e.Detail).HasMaxLength(1000);
            entity.Property(e => e.HouseholdCode).HasMaxLength(20);
            entity.Property(e => e.ManufacturerCode).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Pic).HasMaxLength(200);
            entity.Property(e => e.ProcessingStatus).HasMaxLength(50);
            entity.Property(e => e.ProcessingStatusDetail).HasMaxLength(2000);
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(10);

            entity.HasOne(d => d.HouseholdCodeNavigation).WithMany(p => p.Repairs)
                .HasForeignKey(d => d.HouseholdCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Repairs__Househo__398D8EEE");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReserveCode).HasName("PK__Reservat__2A06DE04E9BF889F");

            entity.Property(e => e.HEmail)
                .HasMaxLength(50)
                .HasColumnName("hEmail");
            entity.Property(e => e.HName)
                .HasMaxLength(50)
                .HasColumnName("hName");
            entity.Property(e => e.HOrdertime)
                .HasColumnType("datetime")
                .HasColumnName("hOrdertime");
            entity.Property(e => e.HPhone)
                .HasMaxLength(10)
                .HasColumnName("hPhone");
            entity.Property(e => e.HState).HasColumnName("hState");
            entity.Property(e => e.HouseholdCode).HasMaxLength(20);

            entity.HasOne(d => d.HouseholdCodeNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.HouseholdCode)
                .HasConstraintName("FK__Reservati__House__403A8C7D");
        });

        modelBuilder.Entity<ReservationPlace>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__Reservat__A25C5AA6F6146789");

            entity.Property(e => e.FDate)
                .HasColumnType("date")
                .HasColumnName("fDate");
            entity.Property(e => e.FDateEnd)
                .HasColumnType("date")
                .HasColumnName("fDateEnd");
            entity.Property(e => e.FDateStart)
                .HasColumnType("date")
                .HasColumnName("fDateStart");
            entity.Property(e => e.FState).HasColumnName("fState");

            entity.HasOne(d => d.PeriodoftimeCodeNavigation).WithMany(p => p.ReservationPlaces)
                .HasForeignKey(d => d.PeriodoftimeCode)
                .HasConstraintName("FK_ReservationPlaces_Periodoftime");

            entity.HasOne(d => d.PlaceCodeNavigation).WithMany(p => p.ReservationPlaces)
                .HasForeignKey(d => d.PlaceCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__Place__4BAC3F29");

            entity.HasOne(d => d.ReserveCodeNavigation).WithMany(p => p.ReservationPlaces)
                .HasForeignKey(d => d.ReserveCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__Reser__4AB81AF0");
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasKey(e => e.HouseholdCode).HasName("PK__Resident__886067A472A02067");

            entity.Property(e => e.HouseholdCode).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Headshot).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(10);

            entity.HasOne(d => d.CommunityBuilding).WithMany(p => p.Residents)
                .HasForeignKey(d => d.CommunityBuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Residents_CommunityBuilding");
        });

        modelBuilder.Entity<ResidentManager>(entity =>
        {
            entity.HasKey(e => e.AdminCode);

            entity.Property(e => e.Account).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Headshot).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(1000);
            entity.Property(e => e.Phone).HasMaxLength(10);
        });

        modelBuilder.Entity<ResidentRegister>(entity =>
        {
            entity.HasKey(e => e.RegisterCode);

            entity.ToTable("ResidentRegister");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Headshot).HasMaxLength(1000);
            entity.Property(e => e.HouseholdCode).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(1000);
            entity.Property(e => e.PersonId)
                .HasMaxLength(20)
                .HasColumnName("PersonID");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.VerifyCode).HasMaxLength(20);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
