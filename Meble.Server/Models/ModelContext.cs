using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Meble.Server.Models;

public partial class ModelContext : IdentityDbContext<User>
{
    private readonly IConfiguration _configuration;
   
    public ModelContext(DbContextOptions<ModelContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public virtual DbSet<ClientOrder> ClientOrders { get; set; }
    public virtual DbSet<Furniture> Furnitures { get; set; }
    public virtual DbSet<FurnitureCategory> FurnitureCategories { get; set; }
    public virtual DbSet<FurniturePhoto> FurniturePhotos { get; set; }
    public virtual DbSet<OrderFurniture> OrderFurnitures { get; set; }
    public DbSet<UserData> UserDatas { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration == null)
        {
            throw new Exception("IConfiguration is null. Make sure it is properly configured.");
        }

        string? connectionString = _configuration.GetConnectionString("ConnectionString");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Connection string from secrets.json is empty or whitespace.");
        }

        optionsBuilder.UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.Entity<IdentityUserLogin<string>>()
            .ToTable("UserLogins");

        modelBuilder.Entity<IdentityUserRole<string>>()
            .ToTable("UserRoles");

        modelBuilder.Entity<IdentityUserToken<string>>()
            .ToTable("UserTokens");

        modelBuilder.Entity<IdentityRole>()
            .ToTable("Roles");

        modelBuilder.Entity<IdentityUserClaim<string>>()
            .ToTable("UserClaims");

        modelBuilder.Entity<IdentityRoleClaim<string>>()
            .ToTable("RoleClaims");
 
        modelBuilder.Entity<ClientOrder>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_ClientOrder");

            entity.ToTable("ClientOrders");

            entity.Property(e => e.OrderId).HasColumnName("OrderId");
            entity.Property(e => e.OrderDateOfOrder).HasColumnName("DateOfOrder");
            entity.Property(e => e.OrderDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ClientOrderDateOfUpdate");
            entity.Property(e => e.OrderUserId).HasColumnName("OrderUserId");

            entity.HasOne(d => d.OrderUser).WithMany(p => p.ClientOrders)
                .HasForeignKey(d => d.OrderUserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ClientOrderToUserId");
        });

        modelBuilder.Entity<Furniture>(entity =>
        {
            entity.HasKey(e => e.FurnitureId).HasName("PK_Furniture");

            entity.ToTable("Furnitures");

            entity.Property(e => e.FurnitureId).HasColumnName("FurnitureId");
            entity.Property(e => e.FurnitureDateOfAddition).HasColumnName("DateOfAddition");
            entity.Property(e => e.FurnitureDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FurnitureDateOfUpdate");
            entity.Property(e => e.FurnitureDescription)
                .IsUnicode(true)
                .HasColumnName("FurnitureDescription");
            entity.Property(e => e.FurnitureName)
                .HasMaxLength(100)
                .IsUnicode(true)
                .HasColumnName("FurnitureName");
            entity.Property(e => e.FurniturePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Price");
        });

        modelBuilder.Entity<FurnitureCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_FurnitureCategory");

            entity.ToTable("FurnitureCategories");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryId");
            entity.Property(e => e.CategoryDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CategoryDateOfUpdate");
            entity.Property(e => e.CategoryFurnitureId).HasColumnName("category_furniture_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(true)
                .HasColumnName("CategoryName");

            entity.HasOne(d => d.CategoryFurniture).WithMany(p => p.FurnitureCategories)
                .HasForeignKey(d => d.CategoryFurnitureId)
                .HasConstraintName("FK_CategoryToFurniture");
        });

        modelBuilder.Entity<FurniturePhoto>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK_FurniturePhoto");

            entity.ToTable("FurniturePhotos");

            entity.Property(e => e.PhotoId).HasColumnName("PhotoId");
            entity.Property(e => e.PhotoDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("PhotoDateOfUpdate");
            entity.Property(e => e.PhotoDescription)
                .HasMaxLength(200)
                .IsUnicode(true)
                .HasColumnName("PhotoDescription");
            entity.Property(e => e.PhotoFurnitureId).HasColumnName("PhotoFurnitureId");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(200)
                .IsUnicode(true)
                .HasColumnName("PhotoUrl");

            entity.HasOne(d => d.PhotoFurniture).WithMany(p => p.FurniturePhotos)
                .HasForeignKey(d => d.PhotoFurnitureId)
                .HasConstraintName("FK_PhotoToFurniture");
        });

        modelBuilder.Entity<OrderFurniture>(entity =>
        {
            entity.HasKey(e => e.OrderFurnitureId).HasName("PK_OrderFurniture");

            entity.ToTable("OrderFurnitures");

            entity.Property(e => e.OrderFurnitureId).HasColumnName("OrderFurnitureId");
            entity.Property(e => e.FurnitureId).HasColumnName("FurnitureId");
            entity.Property(e => e.OrderId).HasColumnName("OrderId");

            entity.HasOne(d => d.Furniture).WithMany(p => p.OrderFurnitures)
                .HasForeignKey(d => d.FurnitureId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ToFurniture");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderFurnitures)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ToCLientOrder");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.Property(e => e.UserDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("DateOfUpdate");

            entity.HasOne(u => u.UserDatas)
                .WithOne(d => d.User)
                .HasForeignKey<UserData>(d => d.UserDataId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_User_UserData");

            entity.HasMany(u => u.ClientOrders)
                .WithOne(co => co.OrderUser)
                .HasForeignKey(co => co.OrderUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<UserData>(entity =>
        {
            entity.ToTable("UserDatas");

            entity.HasKey(e => e.UserDataId);
            entity.Property(e => e.UserFirstName)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(true)
                .HasColumnName("UserdataFirstName");

            entity.Property(e => e.UserSurname)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(true)
                .HasColumnName("UserdataSurname");

            entity.Property(e => e.UserCountry)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(true)
                .HasColumnName("UserdataCountry");

            entity.Property(e => e.UserTown)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(true)
                .HasColumnName("UserdataTown");

            entity.Property(e => e.UserStreet)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(true)
                .HasColumnName("UserdataStreet");

            entity.Property(e => e.UserHomeNumber)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(true)
                .HasColumnName("UserdataHomeNumber");

            entity.Property(e => e.UserFlatNumber)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(true)
                .HasColumnName("UserdataFlatNumber");


            // Relacja User - UserData (1:1)
            entity.HasOne(e => e.User)
                .WithOne(u => u.UserDatas)
                .HasForeignKey<UserData>(e => e.UserDataId)
                .HasConstraintName("FK_UserData_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
