﻿// <auto-generated />
using System;
using GenericDataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GenericDataStore.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241030151740_removefieldname")]
    partial class removefieldname
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GenericDataStore.Models.AppRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("GenericDataStore.Models.AppUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

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
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("HasSub")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("MaxDataCountInMonth")
                        .HasColumnType("int");

                    b.Property<int>("MaxExternalDataCountInMonth")
                        .HasColumnType("int");

                    b.Property<DateTime?>("NextPay")
                        .HasColumnType("datetime2");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool?>("PublicDashboard")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SubStart")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasFilter("[UserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("GenericDataStore.Models.Chart", b =>
                {
                    b.Property<Guid>("ChartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Colorcalculation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Fill")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("GroupOption")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ObjectTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Position")
                        .HasColumnType("int");

                    b.Property<string>("Regression")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RootFilter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Size")
                        .HasColumnType("int");

                    b.Property<bool?>("Stacked")
                        .HasColumnType("bit");

                    b.Property<string>("Step")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Xcalculation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ycalculation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChartId");

                    b.ToTable("Chart");
                });

            modelBuilder.Entity("GenericDataStore.Models.DashboardTable", b =>
                {
                    b.Property<Guid>("DashboardTableId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ObjectTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Position")
                        .HasColumnType("int");

                    b.Property<string>("RootFilter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Size")
                        .HasColumnType("int");

                    b.HasKey("DashboardTableId");

                    b.ToTable("DashboardTable");
                });

            modelBuilder.Entity("GenericDataStore.Models.DatabaseConnectionProperty", b =>
                {
                    b.Property<Guid>("DatabaseConnectionPropertyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConnectionString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DatabaseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DatabaseType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Default")
                        .HasColumnType("bit");

                    b.Property<string>("DefaultIdType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Public")
                        .HasColumnType("bit");

                    b.HasKey("DatabaseConnectionPropertyId");

                    b.HasIndex("AppUserId");

                    b.ToTable("DatabaseConnectionProperty");
                });

            modelBuilder.Entity("GenericDataStore.Models.DatabaseTableRelations", b =>
                {
                    b.Property<Guid>("DatabaseTableRelationsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ChildObjecttypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChildPropertyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChildTable")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ChildTableId")
                        .HasColumnType("int");

                    b.Property<string>("FKName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ParentColumnId")
                        .HasColumnType("int");

                    b.Property<Guid?>("ParentObjecttypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ParentPropertyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ParentTable")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Virtual")
                        .HasColumnType("bit");

                    b.HasKey("DatabaseTableRelationsId");

                    b.ToTable("DatabaseTableRelations");
                });

            modelBuilder.Entity("GenericDataStore.Models.Field", b =>
                {
                    b.Property<Guid>("FieldId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CalculationMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ColorMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("DefaultOrder")
                        .HasColumnType("bit");

                    b.Property<string>("LabelColorMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ObjectTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Position")
                        .HasColumnType("int");

                    b.Property<string>("SizeMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Visible")
                        .HasColumnType("bit");

                    b.HasKey("FieldId");

                    b.HasIndex("ObjectTypeId");

                    b.ToTable("Field");
                });

            modelBuilder.Entity("GenericDataStore.Models.ObjectType", b =>
                {
                    b.Property<Guid>("ObjectTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AllUserFullAccess")
                        .HasColumnType("bit");

                    b.Property<Guid?>("AppUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DatabaseConnectionPropertyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("DenyAdd")
                        .HasColumnType("bit");

                    b.Property<bool>("DenyChart")
                        .HasColumnType("bit");

                    b.Property<bool>("DenyExport")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NoFilterMenu")
                        .HasColumnType("bit");

                    b.Property<bool>("Private")
                        .HasColumnType("bit");

                    b.Property<bool>("Promoted")
                        .HasColumnType("bit");

                    b.Property<string>("TableName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ObjectTypeId");

                    b.HasIndex("AppUserId");

                    b.HasIndex("DatabaseConnectionPropertyId");

                    b.ToTable("ObjectType");
                });

            modelBuilder.Entity("GenericDataStore.Models.Offer", b =>
                {
                    b.Property<Guid>("OfferId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OfferId");

                    b.ToTable("Offer");
                });

            modelBuilder.Entity("GenericDataStore.Models.Option", b =>
                {
                    b.Property<Guid>("OptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FieldId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OptionName")
                        .HasColumnType("nvarchar(max)");

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
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AppUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Css")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Html")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ObjectTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TablePageId");

                    b.ToTable("TablePage");
                });

            modelBuilder.Entity("GenericDataStore.Models.UserMessage", b =>
                {
                    b.Property<Guid?>("UserMessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("LastMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("NoVisibleReceiver")
                        .HasColumnType("bit");

                    b.Property<bool?>("NoVisibleSender")
                        .HasColumnType("bit");

                    b.Property<Guid?>("ObjectTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ReceivUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("SendUserId")
                        .HasColumnType("uniqueidentifier");

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

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

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
