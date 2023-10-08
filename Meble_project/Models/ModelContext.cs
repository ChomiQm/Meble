using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace inzynierka_geska.Models;

public partial class ModelContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ModelContext(DbContextOptions<ModelContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<ClientOrder> ClientOrders { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Furniture> Furnitures { get; set; }

    public virtual DbSet<FurnitureCategory> FurnitureCategories { get; set; }

    public virtual DbSet<FurnitureOpinion> FurnitureOpinions { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<Photo> Photos { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<StoreLevel> StoreLevels { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierOrder> SupplierOrders { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? connectionString = _configuration.GetConnectionString("ConnectionString");

        if (connectionString == null)
        {
            throw new Exception("Unable to retrieve the connection string from secrets.json");
        }

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        else
        {
            throw new Exception("Connection string from secrets.json is empty or whitespace.");
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientOrder>(entity =>
        {
            entity.HasKey(e => e.ClientOrderId).HasName("PK__client_o__AB146BD83089753D");

            entity.ToTable("client_orders", tb => tb.HasTrigger("CheckIsClientAfterInsert"));

            entity.Property(e => e.ClientOrderId).HasColumnName("client_order_id");
            entity.Property(e => e.ClientOrderDateOfOrder)
                .HasColumnType("date")
                .HasColumnName("client_order_date_of_order");
            entity.Property(e => e.ClientOrderDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("client_order_date_of_update");
            entity.Property(e => e.ClientOrderFurnitureId).HasColumnName("client_order_furniture_id");
            entity.Property(e => e.ClientOrderPersonId).HasColumnName("client_order_person_id");
            entity.Property(e => e.ClientOrderQuantity).HasColumnName("client_order_quantity");
            entity.Property(e => e.ClientOrderUnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("client_order_unit_price");

            entity.HasOne(d => d.ClientOrderFurniture).WithMany(p => p.ClientOrders)
                .HasForeignKey(d => d.ClientOrderFurnitureId)
                .HasConstraintName("FK__client_or__clien__7D439ABD");

            entity.HasOne(d => d.ClientOrderPerson).WithMany(p => p.ClientOrders)
                .HasForeignKey(d => d.ClientOrderPersonId)
                .HasConstraintName("FK__client_or__clien__7C4F7684");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__discount__BDBE9EF93A70E31C");

            entity.ToTable("discounts");

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.DiscountDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("discount_date_of_update");
            entity.Property(e => e.DiscountEndDate)
                .HasColumnType("date")
                .HasColumnName("discount_end_date");
            entity.Property(e => e.DiscountName)
                .HasMaxLength(50)
                .HasColumnName("discount_name");
            entity.Property(e => e.DiscountPercentageValue)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percentage_value");
            entity.Property(e => e.DiscountStartDate)
                .HasColumnType("date")
                .HasColumnName("discount_start_date");
            entity.Property(e => e.DiscountUserPrimaryId).HasColumnName("discount_user_primary_id");

            entity.HasOne(d => d.DiscountUserPrimary).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountUserPrimaryId)
                .HasConstraintName("FK__discounts__disco__17036CC0");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__employee__C52E0BA833A776B5");

            entity.ToTable("employees", tb => tb.HasTrigger("CheckIsEmployeeAfterInsert"));

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EmployeeDateOfEnployment)
                .HasColumnType("date")
                .HasColumnName("employee_date_of_enployment");
            entity.Property(e => e.EmployeePersonId).HasColumnName("employee_person_id");
            entity.Property(e => e.EmployeePosition)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("employee_position");
            entity.Property(e => e.EmployeeStoreId).HasColumnName("employee_store_id");

            entity.HasOne(d => d.EmployeePerson).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeePersonId)
                .HasConstraintName("FK__employees__emplo__01142BA1");

            entity.HasOne(d => d.EmployeeStore).WithMany(p => p.Employees)
                .HasForeignKey(d => d.EmployeeStoreId)
                .HasConstraintName("FK__employees__emplo__00200768");
        });

        modelBuilder.Entity<Furniture>(entity =>
        {
            entity.HasKey(e => e.FurnitureId).HasName("PK__furnitur__61F2B4534FE2801E");

            entity.ToTable("furnitures");

            entity.Property(e => e.FurnitureId).HasColumnName("furniture_id");
            entity.Property(e => e.FurnitureDateOfAddition)
                .HasColumnType("date")
                .HasColumnName("furniture_date_of_addition");
            entity.Property(e => e.FurnitureDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("furniture_date_of_update");
            entity.Property(e => e.FurnitureDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("furniture_description");
            entity.Property(e => e.FurnitureManufacturerId).HasColumnName("furniture_manufacturer_id");
            entity.Property(e => e.FurnitureName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("furniture_name");
            entity.Property(e => e.FurniturePrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("furniture_price");
            entity.Property(e => e.FurnitureStoreId).HasColumnName("furniture_store_id");

            entity.HasOne(d => d.FurnitureManufacturer).WithMany(p => p.Furnitures)
                .HasForeignKey(d => d.FurnitureManufacturerId)
                .HasConstraintName("FK__furniture__furni__656C112C");

            entity.HasOne(d => d.FurnitureStore).WithMany(p => p.Furnitures)
                .HasForeignKey(d => d.FurnitureStoreId)
                .HasConstraintName("FK__furniture__furni__66603565");
        });

        modelBuilder.Entity<FurnitureCategory>(entity =>
        {
            entity.HasKey(e => e.FurnitureCategoryId).HasName("PK__furnitur__6D09129D2FF3BFEB");

            entity.ToTable("furniture_categories");

            entity.Property(e => e.FurnitureCategoryId).HasColumnName("furniture_category_id");
            entity.Property(e => e.FurnitureCategoryDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("furniture_category_date_of_update");
            entity.Property(e => e.FurnitureCategoryFurnitureId).HasColumnName("furniture_category_furniture_id");
            entity.Property(e => e.FurnitureCategoryName)
                .HasMaxLength(50)
                .HasColumnName("furniture_category_name");

            entity.HasOne(d => d.FurnitureCategoryFurniture).WithMany(p => p.FurnitureCategories)
                .HasForeignKey(d => d.FurnitureCategoryFurnitureId)
                .HasConstraintName("FK__furniture__furni__08B54D69");
        });

        modelBuilder.Entity<FurnitureOpinion>(entity =>
        {
            entity.HasKey(e => e.FurnitureOpinionId).HasName("PK__furnitur__E2D22B8B017C8F5E");

            entity.ToTable("furniture_opinions");

            entity.Property(e => e.FurnitureOpinionId).HasColumnName("furniture_opinion_id");
            entity.Property(e => e.FurnitureOpinionComment)
                .HasMaxLength(500)
                .HasColumnName("furniture_opinion_comment");
            entity.Property(e => e.FurnitureOpinionDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("furniture_opinion_date_of_update");
            entity.Property(e => e.FurnitureOpinionFurnitureId).HasColumnName("furniture_opinion_furniture_id");
            entity.Property(e => e.FurnitureOpinionRating).HasColumnName("furniture_opinion_rating");
            entity.Property(e => e.FurnitureOpinionUserPrimaryId).HasColumnName("furniture_opinion_user_primary_id");

            entity.HasOne(d => d.FurnitureOpinionFurniture).WithMany(p => p.FurnitureOpinions)
                .HasForeignKey(d => d.FurnitureOpinionFurnitureId)
                .HasConstraintName("FK__furniture__furni__123EB7A3");

            entity.HasOne(d => d.FurnitureOpinionUserPrimary).WithMany(p => p.FurnitureOpinions)
                .HasForeignKey(d => d.FurnitureOpinionUserPrimaryId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__furniture__furni__1332DBDC");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PK__manufact__163F69B0A8A67B83");

            entity.ToTable("manufacturers");

            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.ManufactureDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("manufacture_date_of_update");
            entity.Property(e => e.ManufacturerAddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("manufacturer_address");
            entity.Property(e => e.ManufacturerCompanyName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("manufacturer_company_name");
            entity.Property(e => e.ManufacturerMail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("manufacturer_mail");
            entity.Property(e => e.ManufacturerPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("manufacturer_phone");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__persons__543848DF247A9782");

            entity.ToTable("persons");

            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.PersonAddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("person_address");
            entity.Property(e => e.PersonDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("person_date_of_update");
            entity.Property(e => e.PersonFirstName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("person_first_name");
            entity.Property(e => e.PersonMail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("person_mail");
            entity.Property(e => e.PersonPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("person_phone");
            entity.Property(e => e.PersonSurname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("person_surname");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.PhotoId).HasName("PK__photos__CB48C83D35FA2378");

            entity.ToTable("photos");

            entity.Property(e => e.PhotoId).HasColumnName("photo_id");
            entity.Property(e => e.PhotoDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("photo_date_of_update");
            entity.Property(e => e.PhotoDescription)
                .HasMaxLength(200)
                .HasColumnName("photo_description");
            entity.Property(e => e.PhotoFurnitureId).HasColumnName("photo_furniture_id");
            entity.Property(e => e.PhotoPhoto).HasColumnName("photo_photo");

            entity.HasOne(d => d.PhotoFurniture).WithMany(p => p.Photos)
                .HasForeignKey(d => d.PhotoFurnitureId)
                .HasConstraintName("FK__photos__photo_fu__1AD3FDA4");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId).HasName("PK__stores__A2F2A30C477C52FF");

            entity.ToTable("stores");

            entity.Property(e => e.StoreId).HasColumnName("store_id");
            entity.Property(e => e.StoreAddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("store_address");
            entity.Property(e => e.StoreDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("store_date_of_update");
            entity.Property(e => e.StoreName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("store_name");
            entity.Property(e => e.StorePhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("store_phone");
        });

        modelBuilder.Entity<StoreLevel>(entity =>
        {
            entity.HasKey(e => e.StoreLevelId).HasName("PK__store_le__EC1D6CEE82C765D8");

            entity.ToTable("store_levels");

            entity.Property(e => e.StoreLevelId).HasColumnName("store_level_id");
            entity.Property(e => e.StoreLevel1).HasColumnName("store_level");
            entity.Property(e => e.StoreLevelDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("store_level_date_of_update");
            entity.Property(e => e.StoreLevelFurnitureId).HasColumnName("store_level_furniture_id");
            entity.Property(e => e.StoreLevelStoreId).HasColumnName("store_level_store_id");

            entity.HasOne(d => d.StoreLevelFurniture).WithMany(p => p.StoreLevels)
                .HasForeignKey(d => d.StoreLevelFurnitureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__store_lev__store__0C85DE4D");

            entity.HasOne(d => d.StoreLevelStore).WithMany(p => p.StoreLevels)
                .HasForeignKey(d => d.StoreLevelStoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__store_lev__store__0D7A0286");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__supplier__6EE594E82B53B571");

            entity.ToTable("suppliers", tb => tb.HasTrigger("CheckIsSupplierAfterInsert"));

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierCompanyAddress)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("supplier_company_address");
            entity.Property(e => e.SupplierCompanyMail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("supplier_company_mail");
            entity.Property(e => e.SupplierCompanyName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("supplier_company_name");
            entity.Property(e => e.SupplierCompanyPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("supplier_company_phone");
            entity.Property(e => e.SupplierDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("supplier_date_of_update");
            entity.Property(e => e.SupplierPersonId).HasColumnName("supplier_person_id");

            entity.HasOne(d => d.SupplierPerson).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.SupplierPersonId)
                .HasConstraintName("FK__suppliers__suppl__71D1E811");
        });

        modelBuilder.Entity<SupplierOrder>(entity =>
        {
            entity.HasKey(e => e.SupplierOrderId).HasName("PK__supplier__5EFAB7DB1BFF12E8");

            entity.ToTable("supplier_orders");

            entity.Property(e => e.SupplierOrderId).HasColumnName("supplier_order_id");
            entity.Property(e => e.SupplierOrderDateOfOrder)
                .HasColumnType("date")
                .HasColumnName("supplier_order_date_of_order");
            entity.Property(e => e.SupplierOrderDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("supplier_order_date_of_update");
            entity.Property(e => e.SupplierOrderFurnitureId).HasColumnName("supplier_order_furniture_id");
            entity.Property(e => e.SupplierOrderQuantity).HasColumnName("supplier_order_quantity");
            entity.Property(e => e.SupplierOrderSupplierId).HasColumnName("supplier_order_supplier_id");
            entity.Property(e => e.SupplierOrderUnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("supplier_order_unit_price");

            entity.HasOne(d => d.SupplierOrderFurniture).WithMany(p => p.SupplierOrders)
                .HasForeignKey(d => d.SupplierOrderFurnitureId)
                .HasConstraintName("FK__supplier___suppl__787EE5A0");

            entity.HasOne(d => d.SupplierOrderSupplier).WithMany(p => p.SupplierOrders)
                .HasForeignKey(d => d.SupplierOrderSupplierId)
                .HasConstraintName("FK__supplier___suppl__778AC167");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__tasks__0492148D66ABE48D");

            entity.ToTable("tasks");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.TaskAssignmentDate)
                .HasColumnType("date")
                .HasColumnName("task_assignment_date");
            entity.Property(e => e.TaskDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("task_date_of_update");
            entity.Property(e => e.TaskDescription)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("task_description");
            entity.Property(e => e.TaskEmployeeId).HasColumnName("task_employee_id");

            entity.HasOne(d => d.TaskEmployee).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TaskEmployeeId)
                .HasConstraintName("FK__tasks__task_empl__04E4BC85");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserPrimaryId).HasName("PK__users__0B70CBDFB8F5DF49");

            entity.ToTable("users", tb => tb.HasTrigger("CheckUserRolesAfterInsert"));

            entity.HasIndex(e => e.UserLogin, "UQ__users__9EA1B5AF73C87711").IsUnique();

            entity.Property(e => e.UserPrimaryId).HasColumnName("user_primary_id");
            entity.Property(e => e.UserDateOfUpdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("user_date_of_update");
            entity.Property(e => e.UserIsEmployee).HasColumnName("user_is_employee");
            entity.Property(e => e.UserIsSupplier).HasColumnName("user_is_supplier");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_login");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasColumnName("user_password");
            entity.Property(e => e.UserPersonId).HasColumnName("user_person_id");

            entity.HasOne(d => d.UserPerson).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserPersonId)
                .HasConstraintName("FK__users__user_pers__6E01572D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
