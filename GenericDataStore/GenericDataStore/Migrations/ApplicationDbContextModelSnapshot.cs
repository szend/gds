﻿// <auto-generated />
using System;
using GenericDataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GenericDataStore.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GenericDataStore.Models.AppRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("GenericDataStore.Models.AppUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<int>("AllowedDataCount")
                        .HasColumnType("int");

                    b.Property<int>("AllowedExternalDataCount")
                        .HasColumnType("int");

                    b.Property<int>("AllowedListCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("HasSub")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("MaxDataCountInMonth")
                        .HasColumnType("int");

                    b.Property<int>("MaxExternalDataCountInMonth")
                        .HasColumnType("int");

                    b.Property<DateTime?>("NextPay")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("SubStart")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SubType")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("GenericDataStore.Models.DatabaseConnectionProperty", b =>
                {
                    b.Property<Guid>("DatabaseConnectionPropertyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("AppUserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ConnectionString")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DatabaseName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("DatabaseType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool?>("Default")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("DefaultIdType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool?>("Public")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("DatabaseConnectionPropertyId");

                    b.HasIndex("AppUserId");

                    b.ToTable("DatabaseConnectionProperty");
                });

            modelBuilder.Entity("GenericDataStore.Models.DatabaseTableRelations", b =>
                {
                    b.Property<Guid>("DatabaseTableRelationsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ChildObjecttypeId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ChildPropertyName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ChildTable")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ChildTableId")
                        .HasColumnType("int");

                    b.Property<string>("FKName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("ParentColumnId")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentObjecttypeId")
                        .HasColumnType("char(36)");

                    b.Property<string>("ParentPropertyName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ParentTable")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Virtual")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("DatabaseTableRelationsId");

                    b.ToTable("DatabaseTableRelations");
                });

            modelBuilder.Entity("GenericDataStore.Models.Field", b =>
                {
                    b.Property<Guid>("FieldId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CalculationMethod")
                        .HasColumnType("longtext");

                    b.Property<string>("ColorMethod")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("ObjectTypeId")
                        .HasColumnType("char(36)");

                    b.Property<string>("PropertyName")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("FieldId");

                    b.HasIndex("ObjectTypeId");

                    b.ToTable("Field");
                });

            modelBuilder.Entity("GenericDataStore.Models.ObjectType", b =>
                {
                    b.Property<Guid>("ObjectTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("AllUserFullAccess")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("AppUserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Category")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DatabaseConnectionPropertyId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("DenyAdd")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("DenyChart")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("DenyExport")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<bool>("NoFilterMenu")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Private")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Promoted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("TableName")
                        .HasColumnType("longtext");

                    b.HasKey("ObjectTypeId");

                    b.HasIndex("AppUserId");

                    b.HasIndex("DatabaseConnectionPropertyId");

                    b.ToTable("ObjectType");
                });

            modelBuilder.Entity("GenericDataStore.Models.Offer", b =>
                {
                    b.Property<Guid>("OfferId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("OfferId");

                    b.ToTable("Offer");
                });

            modelBuilder.Entity("GenericDataStore.Models.Option", b =>
                {
                    b.Property<Guid>("OptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FieldId")
                        .HasColumnType("char(36)");

                    b.Property<string>("OptionName")
                        .HasColumnType("longtext");

                    b.Property<int?>("OptionValue")
                        .HasColumnType("int");

                    b.HasKey("OptionId");

                    b.HasIndex("FieldId");

                    b.ToTable("Option");
                });

            modelBuilder.Entity("GenericDataStore.Models.TablePage", b =>
                {
                    b.Property<Guid>("TablePageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("AppUserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Css")
                        .HasColumnType("longtext");

                    b.Property<string>("Html")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid>("ObjectTypeId")
                        .HasColumnType("char(36)");

                    b.HasKey("TablePageId");

                    b.ToTable("TablePage");
                });

            modelBuilder.Entity("GenericDataStore.Models.UserMessage", b =>
                {
                    b.Property<Guid?>("UserMessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("LastMessageId")
                        .HasColumnType("char(36)");

                    b.Property<bool?>("NoVisibleReceiver")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool?>("NoVisibleSender")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid?>("ObjectTypeId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ReceivUserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("SendUserId")
                        .HasColumnType("char(36)");

                    b.HasKey("UserMessageId");

                    b.HasIndex("ReceivUserId");

                    b.HasIndex("SendUserId");

                    b.ToTable("UserMessage");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("char(36)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("GenericDataStore.Models.DatabaseConnectionProperty", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppUser", "AppUser")
                        .WithMany("DatabaseConnectionProperty")
                        .HasForeignKey("AppUserId");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("GenericDataStore.Models.Field", b =>
                {
                    b.HasOne("GenericDataStore.Models.ObjectType", null)
                        .WithMany("Field")
                        .HasForeignKey("ObjectTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GenericDataStore.Models.ObjectType", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppUser", "AppUser")
                        .WithMany("ObjectType")
                        .HasForeignKey("AppUserId");

                    b.HasOne("GenericDataStore.Models.DatabaseConnectionProperty", "DatabaseConnectionProperty")
                        .WithMany("ObjectType")
                        .HasForeignKey("DatabaseConnectionPropertyId");

                    b.Navigation("AppUser");

                    b.Navigation("DatabaseConnectionProperty");
                });

            modelBuilder.Entity("GenericDataStore.Models.Option", b =>
                {
                    b.HasOne("GenericDataStore.Models.Field", null)
                        .WithMany("Option")
                        .HasForeignKey("FieldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GenericDataStore.Models.UserMessage", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppUser", "ReceivUser")
                        .WithMany("Received")
                        .HasForeignKey("ReceivUserId");

                    b.HasOne("GenericDataStore.Models.AppUser", "SendUser")
                        .WithMany("Sent")
                        .HasForeignKey("SendUserId");

                    b.Navigation("ReceivUser");

                    b.Navigation("SendUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GenericDataStore.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("GenericDataStore.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GenericDataStore.Models.AppUser", b =>
                {
                    b.Navigation("DatabaseConnectionProperty");

                    b.Navigation("ObjectType");

                    b.Navigation("Received");

                    b.Navigation("Sent");
                });

            modelBuilder.Entity("GenericDataStore.Models.DatabaseConnectionProperty", b =>
                {
                    b.Navigation("ObjectType");
                });

            modelBuilder.Entity("GenericDataStore.Models.Field", b =>
                {
                    b.Navigation("Option");
                });

            modelBuilder.Entity("GenericDataStore.Models.ObjectType", b =>
                {
                    b.Navigation("Field");
                });
#pragma warning restore 612, 618
        }
    }
}
