﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NMVS.Models;

namespace NMVS.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20211021122021_AllocateORder")]
    partial class AllocateORder
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("NMVS.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

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

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("NMVS.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

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

                    b.Property<string>("SecurityStamp")
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

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("NMVS.Models.ApplicationUserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.AllocateOrder", b =>
                {
                    b.Property<int>("AlcOrdId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AlcOrdFDesc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AlcOrdFrom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("AlcOrdQty")
                        .HasColumnType("float");

                    b.Property<int?>("AllocateRequestAlcId")
                        .HasColumnType("int");

                    b.Property<string>("CodeValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Confirm")
                        .HasColumnType("bit");

                    b.Property<string>("ConfirmedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GeneralizedCodeCodeNo")
                        .HasColumnType("int");

                    b.Property<int?>("ItemMasterPtId")
                        .HasColumnType("int");

                    b.Property<string>("LocCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LocCode1")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("MovementTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("OrderBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PtId")
                        .HasColumnType("int");

                    b.Property<int>("RequestID")
                        .HasColumnType("int");

                    b.HasKey("AlcOrdId");

                    b.HasIndex("AllocateRequestAlcId");

                    b.HasIndex("GeneralizedCodeCodeNo");

                    b.HasIndex("ItemMasterPtId");

                    b.HasIndex("LocCode1");

                    b.ToTable("AllocateOrders");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.AllocateRequest", b =>
                {
                    b.Property<int>("AlcId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AlcCmmt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AlcFrom")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("AlcFromDesc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("AlcQty")
                        .HasColumnType("float");

                    b.Property<bool?>("IsClosed")
                        .HasColumnType("bit");

                    b.Property<int?>("ItemMasterPtId")
                        .HasColumnType("int");

                    b.Property<string>("LocCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LocCode1")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("MovementTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("PtId")
                        .HasColumnType("int");

                    b.HasKey("AlcId");

                    b.HasIndex("ItemMasterPtId");

                    b.HasIndex("LocCode1");

                    b.ToTable("AllocateRequests");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Customer", b =>
                {
                    b.Property<string>("CustCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Addr")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("AgentNo")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("ApCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("City")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Ctry")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CustName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email1")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email2")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone1")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Phone2")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("TaxCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CustCode");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.GeneralizedCode", b =>
                {
                    b.Property<int>("CodeNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CodeDesc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CodeFldName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CodeValue")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CodeNo");

                    b.ToTable("GeneralizedCodes");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.IncomingList", b =>
                {
                    b.Property<int>("IcId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Checked")
                        .HasColumnType("int");

                    b.Property<bool>("Closed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Driver")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsWarranty")
                        .HasColumnType("bit");

                    b.Property<int>("ItemCount")
                        .HasColumnType("int");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Po")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PoDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SupCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SupplierSupCode")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Vehicle")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IcId");

                    b.HasIndex("SupplierSupCode");

                    b.ToTable("IncomingLists");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.InvRequest", b =>
                {
                    b.Property<string>("RqID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CodeValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GeneralizedCodeCodeNo")
                        .HasColumnType("int");

                    b.Property<string>("Ref")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RqBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RqCmt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RqDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("SoConfirm")
                        .HasColumnType("bit");

                    b.HasKey("RqID");

                    b.HasIndex("GeneralizedCodeCodeNo");

                    b.ToTable("InvRequests");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.IssueOrder", b =>
                {
                    b.Property<int>("ExpOrdId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Confirm")
                        .HasColumnType("bit");

                    b.Property<string>("ConfirmedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ExpOrdQty")
                        .HasColumnType("float");

                    b.Property<string>("InvRequestRqID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IssueType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LocCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("MovementTime")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("OrderBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PtId")
                        .HasColumnType("int");

                    b.Property<string>("RqID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ToLoc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ToLocDesc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WhlCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ExpOrdId");

                    b.HasIndex("InvRequestRqID");

                    b.HasIndex("LocCode");

                    b.ToTable("IssueOrders");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.ItemData", b =>
                {
                    b.Property<string>("ItemNo")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<bool>("Flammable")
                        .HasColumnType("bit");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ItemPkg")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("ItemPkgQty")
                        .HasColumnType("float");

                    b.Property<string>("ItemType")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ItemUm")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("ItemWhUnit")
                        .HasColumnType("float");

                    b.HasKey("ItemNo");

                    b.ToTable("ItemDatas");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.ItemMaster", b =>
                {
                    b.Property<int>("PtId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Accepted")
                        .HasColumnType("float");

                    b.Property<int>("IcId")
                        .HasColumnType("int");

                    b.Property<string>("ItemNo")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LocCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PtCmt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PtDateIn")
                        .HasColumnType("datetime2");

                    b.Property<double>("PtHold")
                        .HasColumnType("float");

                    b.Property<string>("PtLotNo")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("PtQty")
                        .HasColumnType("float");

                    b.Property<string>("Qc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("RecQty")
                        .HasColumnType("float");

                    b.Property<DateTime>("RefDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RefNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SupCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SupplierSupCode")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("PtId");

                    b.HasIndex("IcId");

                    b.HasIndex("SupplierSupCode");

                    b.ToTable("ItemMasters");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Loc", b =>
                {
                    b.Property<string>("LocCode")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Direct")
                        .HasColumnType("bit");

                    b.Property<bool>("Flammable")
                        .HasColumnType("bit");

                    b.Property<double>("LocCap")
                        .HasColumnType("float");

                    b.Property<string>("LocCmmt")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("LocDesc")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<double>("LocHolding")
                        .HasColumnType("float");

                    b.Property<double>("LocOutgo")
                        .HasColumnType("float");

                    b.Property<double>("LocRemain")
                        .HasColumnType("float");

                    b.Property<bool>("LocStatus")
                        .HasColumnType("bit");

                    b.Property<string>("LocType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WarehouseWhCode")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("WhCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LocCode");

                    b.HasIndex("WarehouseWhCode");

                    b.ToTable("Locs");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.RequestDet", b =>
                {
                    b.Property<int>("DetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Arranged")
                        .HasColumnType("float");

                    b.Property<bool?>("Closed")
                        .HasColumnType("bit");

                    b.Property<string>("InvRequestRqID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("IssueOn")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Issued")
                        .HasColumnType("float");

                    b.Property<string>("ItemDataItemNo")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ItemNo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("Picked")
                        .HasColumnType("float");

                    b.Property<double>("Quantity")
                        .HasColumnType("float");

                    b.Property<double>("Ready")
                        .HasColumnType("float");

                    b.Property<string>("Report")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RequireDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RqID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Shipped")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("SpecDate")
                        .HasColumnType("datetime2");

                    b.HasKey("DetId");

                    b.HasIndex("InvRequestRqID");

                    b.HasIndex("ItemDataItemNo");

                    b.ToTable("RequestDets");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Site", b =>
                {
                    b.Property<string>("SiCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("SiCmmt")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("SiName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SiCode");

                    b.ToTable("Sites");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Supplier", b =>
                {
                    b.Property<string>("SupCode")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Addr")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("City")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Ctry")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email1")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email2")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone1")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Phone2")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("SupDesc")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("TaxCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("SupCode");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Warehouse", b =>
                {
                    b.Property<string>("WhCode")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SiCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SiteSiCode")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WhCmmt")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("WhDesc")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("WhStatus")
                        .HasColumnType("bit");

                    b.HasKey("WhCode");

                    b.HasIndex("SiteSiCode");

                    b.ToTable("Warehouses");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("NMVS.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("NMVS.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("NMVS.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("NMVS.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("NMVS.Models.ApplicationUserRole", b =>
                {
                    b.HasOne("NMVS.Models.ApplicationRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NMVS.Models.ApplicationUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.AllocateOrder", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.AllocateRequest", "AllocateRequest")
                        .WithMany()
                        .HasForeignKey("AllocateRequestAlcId");

                    b.HasOne("NMVS.Models.DbModels.GeneralizedCode", "GeneralizedCode")
                        .WithMany()
                        .HasForeignKey("GeneralizedCodeCodeNo");

                    b.HasOne("NMVS.Models.DbModels.ItemMaster", "ItemMaster")
                        .WithMany()
                        .HasForeignKey("ItemMasterPtId");

                    b.HasOne("NMVS.Models.DbModels.Loc", "Loc")
                        .WithMany()
                        .HasForeignKey("LocCode1");

                    b.Navigation("AllocateRequest");

                    b.Navigation("GeneralizedCode");

                    b.Navigation("ItemMaster");

                    b.Navigation("Loc");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.AllocateRequest", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.ItemMaster", "ItemMaster")
                        .WithMany()
                        .HasForeignKey("ItemMasterPtId");

                    b.HasOne("NMVS.Models.DbModels.Loc", "Loc")
                        .WithMany()
                        .HasForeignKey("LocCode1");

                    b.Navigation("ItemMaster");

                    b.Navigation("Loc");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.IncomingList", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierSupCode");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.InvRequest", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.GeneralizedCode", "GeneralizedCode")
                        .WithMany()
                        .HasForeignKey("GeneralizedCodeCodeNo");

                    b.Navigation("GeneralizedCode");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.IssueOrder", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.InvRequest", "InvRequest")
                        .WithMany()
                        .HasForeignKey("InvRequestRqID");

                    b.HasOne("NMVS.Models.DbModels.Loc", "Loc")
                        .WithMany()
                        .HasForeignKey("LocCode");

                    b.Navigation("InvRequest");

                    b.Navigation("Loc");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.ItemMaster", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.IncomingList", "Ic")
                        .WithMany("ItemMasters")
                        .HasForeignKey("IcId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NMVS.Models.DbModels.Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierSupCode");

                    b.Navigation("Ic");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Loc", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.Warehouse", "Warehouse")
                        .WithMany()
                        .HasForeignKey("WarehouseWhCode");

                    b.Navigation("Warehouse");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.RequestDet", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.InvRequest", "InvRequest")
                        .WithMany()
                        .HasForeignKey("InvRequestRqID");

                    b.HasOne("NMVS.Models.DbModels.ItemData", "ItemData")
                        .WithMany()
                        .HasForeignKey("ItemDataItemNo");

                    b.Navigation("InvRequest");

                    b.Navigation("ItemData");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Warehouse", b =>
                {
                    b.HasOne("NMVS.Models.DbModels.Site", "Site")
                        .WithMany("Warehouses")
                        .HasForeignKey("SiteSiCode");

                    b.Navigation("Site");
                });

            modelBuilder.Entity("NMVS.Models.ApplicationRole", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("NMVS.Models.ApplicationUser", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.IncomingList", b =>
                {
                    b.Navigation("ItemMasters");
                });

            modelBuilder.Entity("NMVS.Models.DbModels.Site", b =>
                {
                    b.Navigation("Warehouses");
                });
#pragma warning restore 612, 618
        }
    }
}
