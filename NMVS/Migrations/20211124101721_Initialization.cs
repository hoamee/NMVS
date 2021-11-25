using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NMVS.Migrations
{
    public partial class Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AgentNo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CustName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Addr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ctry = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustCode);
                });

            migrationBuilder.CreateTable(
                name: "GeneralizedCodes",
                columns: table => new
                {
                    CodeNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeFldName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeDesc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralizedCodes", x => x.CodeNo);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransacs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovementTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastId = table.Column<int>(type: "int", nullable: false),
                    NewId = table.Column<int>(type: "int", nullable: true),
                    IsDisposed = table.Column<bool>(type: "bit", nullable: false),
                    OrderNo = table.Column<int>(type: "int", nullable: false),
                    IsAllocate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransacs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvRequests",
                columns: table => new
                {
                    RqID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RqType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RqBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RqDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ref = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoConfirm = table.Column<bool>(type: "bit", nullable: false),
                    RqCmt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Confirmed = table.Column<bool>(type: "bit", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmationNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reported = table.Column<bool>(type: "bit", nullable: false),
                    ReportedNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Closed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvRequests", x => x.RqID);
                });

            migrationBuilder.CreateTable(
                name: "IssueNoteDets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsNId = table.Column<int>(type: "int", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    PtId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueNoteDets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IssueOrders",
                columns: table => new
                {
                    ExpOrdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpOrdQty = table.Column<double>(type: "float", nullable: false),
                    MovedQty = table.Column<double>(type: "float", nullable: false),
                    Reported = table.Column<double>(type: "float", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromLoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    ToLoc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToVehicle = table.Column<int>(type: "int", nullable: true),
                    IssueToDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Confirm = table.Column<bool>(type: "bit", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RqID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetId = table.Column<int>(type: "int", nullable: false),
                    OrderBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovementTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueOrders", x => x.ExpOrdId);
                });

            migrationBuilder.CreateTable(
                name: "ItemDatas",
                columns: table => new
                {
                    ItemNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ItemUm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemPkg = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ItemPkgQty = table.Column<double>(type: "float", nullable: false),
                    Flammable = table.Column<bool>(type: "bit", nullable: false),
                    ItemWhUnit = table.Column<double>(type: "float", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDatas", x => x.ItemNo);
                });

            migrationBuilder.CreateTable(
                name: "MfgIssueNotes",
                columns: table => new
                {
                    IsNId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RqId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MfgIssueNotes", x => x.IsNId);
                });

            migrationBuilder.CreateTable(
                name: "RequestDets",
                columns: table => new
                {
                    DetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Picked = table.Column<double>(type: "float", nullable: false),
                    Ready = table.Column<double>(type: "float", nullable: false),
                    Closed = table.Column<bool>(type: "bit", nullable: true),
                    Issued = table.Column<double>(type: "float", nullable: true),
                    RequireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Arranged = table.Column<double>(type: "float", nullable: false),
                    Shipped = table.Column<bool>(type: "bit", nullable: true),
                    Report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SpecDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MovementNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RqID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SodId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDets", x => x.DetId);
                });

            migrationBuilder.CreateTable(
                name: "ShipperDets",
                columns: table => new
                {
                    SpDetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShpId = table.Column<int>(type: "int", nullable: false),
                    DetId = table.Column<int>(type: "int", nullable: false),
                    RqId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipToAddr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldToName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    PackCount = table.Column<int>(type: "int", nullable: false),
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipperDets", x => x.SpDetId);
                });

            migrationBuilder.CreateTable(
                name: "Shippers",
                columns: table => new
                {
                    ShpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShpDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Driver = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RememberMe = table.Column<bool>(type: "bit", nullable: false),
                    ShpTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShpVia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckInBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckOutBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Loc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IssueConfirmedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shippers", x => x.ShpId);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    SiCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    SiCmmt = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.SiCode);
                });

            migrationBuilder.CreateTable(
                name: "SoIssueNoteDets",
                columns: table => new
                {
                    IndID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    PackCount = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    InId = table.Column<int>(type: "int", nullable: false),
                    InType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoIssueNoteDets", x => x.IndID);
                });

            migrationBuilder.CreateTable(
                name: "SoIssueNotes",
                columns: table => new
                {
                    InId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipper = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoIssueNotes", x => x.InId);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupDesc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Addr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Ctry = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupCode);
                });

            migrationBuilder.CreateTable(
                name: "SystemMessages",
                columns: table => new
                {
                    MsgNo = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMessages", x => x.MsgNo);
                });

            migrationBuilder.CreateTable(
                name: "Unqualifieds",
                columns: table => new
                {
                    UqId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    RecycleQty = table.Column<double>(type: "float", nullable: false),
                    DisposedQty = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unqualifieds", x => x.UqId);
                });

            migrationBuilder.CreateTable(
                name: "UnqualifiedTransacs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnqId = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<double>(type: "float", nullable: false),
                    IsDisposed = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransantionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ByUser = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnqualifiedTransacs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UploadId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadReports",
                columns: table => new
                {
                    UploadId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadFunction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalRecord = table.Column<int>(type: "int", nullable: false),
                    Inserted = table.Column<int>(type: "int", nullable: false),
                    Updated = table.Column<int>(type: "int", nullable: false),
                    Errors = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadReports", x => x.UploadId);
                });

            migrationBuilder.CreateTable(
                name: "WrIssueNotes",
                columns: table => new
                {
                    InId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipper = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WrIssueNotes", x => x.InId);
                });

            migrationBuilder.CreateTable(
                name: "WtIssueNotes",
                columns: table => new
                {
                    InId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoldTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shipper = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WtIssueNotes", x => x.InId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrders",
                columns: table => new
                {
                    SoNbr = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoType = table.Column<int>(type: "int", nullable: false),
                    CustCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerCustCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ShipTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReqDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoCurr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipVia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Confirm = table.Column<bool>(type: "bit", nullable: true),
                    ConfirmBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Warning = table.Column<bool>(type: "bit", nullable: false),
                    ReqReported = table.Column<bool>(type: "bit", nullable: false),
                    ReqReportedNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    ConfirmationNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovalNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Closed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrders", x => x.SoNbr);
                    table.ForeignKey(
                        name: "FK_SalesOrders_Customers_CustomerCustCode",
                        column: x => x.CustomerCustCode,
                        principalTable: "Customers",
                        principalColumn: "CustCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProdLines",
                columns: table => new
                {
                    PrLnId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SiCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteSiCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdLines", x => x.PrLnId);
                    table.ForeignKey(
                        name: "FK_ProdLines_Sites_SiteSiCode",
                        column: x => x.SiteSiCode,
                        principalTable: "Sites",
                        principalColumn: "SiCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WhCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WhDesc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WhStatus = table.Column<bool>(type: "bit", nullable: false),
                    WhCmmt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SiCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SiteSiCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WhCode);
                    table.ForeignKey(
                        name: "FK_Warehouses_Sites_SiteSiCode",
                        column: x => x.SiteSiCode,
                        principalTable: "Sites",
                        principalColumn: "SiCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomingLists",
                columns: table => new
                {
                    IcId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierSupCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Po = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Vehicle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Driver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsWarranty = table.Column<bool>(type: "bit", nullable: false),
                    IsRecycle = table.Column<bool>(type: "bit", nullable: false),
                    RecycleId = table.Column<int>(type: "int", nullable: true),
                    Closed = table.Column<bool>(type: "bit", nullable: false),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    Checked = table.Column<int>(type: "int", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingLists", x => x.IcId);
                    table.ForeignKey(
                        name: "FK_IncomingLists_Suppliers_SupplierSupCode",
                        column: x => x.SupplierSupCode,
                        principalTable: "Suppliers",
                        principalColumn: "SupCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoDetails",
                columns: table => new
                {
                    SodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesOrderSoNbr = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetPrice = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Shipped = table.Column<double>(type: "float", nullable: false),
                    RqDetId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoDetails", x => x.SodId);
                    table.ForeignKey(
                        name: "FK_SoDetails_SalesOrders_SalesOrderSoNbr",
                        column: x => x.SalesOrderSoNbr,
                        principalTable: "SalesOrders",
                        principalColumn: "SoNbr",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    WoNbr = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemDataItemNo = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    QtyOrd = table.Column<double>(type: "float", nullable: false),
                    QtyCom = table.Column<double>(type: "float", nullable: false),
                    SoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrdBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrLnId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdLinePrLnId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Closed = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.WoNbr);
                    table.ForeignKey(
                        name: "FK_WorkOrders_ItemDatas_ItemDataItemNo",
                        column: x => x.ItemDataItemNo,
                        principalTable: "ItemDatas",
                        principalColumn: "ItemNo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrders_ProdLines_ProdLinePrLnId",
                        column: x => x.ProdLinePrLnId,
                        principalTable: "ProdLines",
                        principalColumn: "PrLnId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locs",
                columns: table => new
                {
                    LocCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LocDesc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocStatus = table.Column<bool>(type: "bit", nullable: false),
                    LocCmmt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LocType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCap = table.Column<double>(type: "float", nullable: false),
                    LocRemain = table.Column<double>(type: "float", nullable: false),
                    LocHolding = table.Column<double>(type: "float", nullable: false),
                    LocOutgo = table.Column<double>(type: "float", nullable: false),
                    Direct = table.Column<bool>(type: "bit", nullable: false),
                    WhCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flammable = table.Column<bool>(type: "bit", nullable: false),
                    WarehouseWhCode = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locs", x => x.LocCode);
                    table.ForeignKey(
                        name: "FK_Locs_Warehouses_WarehouseWhCode",
                        column: x => x.WarehouseWhCode,
                        principalTable: "Warehouses",
                        principalColumn: "WhCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemMasters",
                columns: table => new
                {
                    PtId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PtLotNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PtQty = table.Column<double>(type: "float", nullable: false),
                    Accepted = table.Column<double>(type: "float", nullable: false),
                    Qc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtHold = table.Column<double>(type: "float", nullable: false),
                    LocCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtDateIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierSupCode = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    RecQty = table.Column<double>(type: "float", nullable: false),
                    RecBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IcId = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PtCmt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRecycled = table.Column<bool>(type: "bit", nullable: true),
                    RecycleDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnqualifiedId = table.Column<int>(type: "int", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    MovementNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemMasters", x => x.PtId);
                    table.ForeignKey(
                        name: "FK_ItemMasters_IncomingLists_IcId",
                        column: x => x.IcId,
                        principalTable: "IncomingLists",
                        principalColumn: "IcId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemMasters_Suppliers_SupplierSupCode",
                        column: x => x.SupplierSupCode,
                        principalTable: "Suppliers",
                        principalColumn: "SupCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WoBills",
                columns: table => new
                {
                    WoBillNbr = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WoNbr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkOrderWoNbr = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OrdQty = table.Column<double>(type: "float", nullable: false),
                    ComQty = table.Column<double>(type: "float", nullable: false),
                    Assignee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reporter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WoBills", x => x.WoBillNbr);
                    table.ForeignKey(
                        name: "FK_WoBills_WorkOrders_WorkOrderWoNbr",
                        column: x => x.WorkOrderWoNbr,
                        principalTable: "WorkOrders",
                        principalColumn: "WoNbr",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllocateOrders",
                columns: table => new
                {
                    AlcOrdId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    AlcOrdQty = table.Column<double>(type: "float", nullable: false),
                    MovedQty = table.Column<double>(type: "float", nullable: false),
                    Reported = table.Column<double>(type: "float", nullable: false),
                    AlcOrdFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlcOrdFDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Confirm = table.Column<bool>(type: "bit", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    OrderBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MovementTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MovementNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocateOrders", x => x.AlcOrdId);
                    table.ForeignKey(
                        name: "FK_AllocateOrders_Locs_LocCode1",
                        column: x => x.LocCode1,
                        principalTable: "Locs",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AllocateRequests",
                columns: table => new
                {
                    AlcId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PtId = table.Column<int>(type: "int", nullable: false),
                    ItemMasterPtId = table.Column<int>(type: "int", nullable: true),
                    AlcQty = table.Column<double>(type: "float", nullable: false),
                    Reported = table.Column<double>(type: "float", nullable: false),
                    AlcFrom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AlcFromDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocCode1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AlcCmmt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: true),
                    MovementTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocateRequests", x => x.AlcId);
                    table.ForeignKey(
                        name: "FK_AllocateRequests_ItemMasters_ItemMasterPtId",
                        column: x => x.ItemMasterPtId,
                        principalTable: "ItemMasters",
                        principalColumn: "PtId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AllocateRequests_Locs_LocCode1",
                        column: x => x.LocCode1,
                        principalTable: "Locs",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllocateOrders_LocCode1",
                table: "AllocateOrders",
                column: "LocCode1");

            migrationBuilder.CreateIndex(
                name: "IX_AllocateRequests_ItemMasterPtId",
                table: "AllocateRequests",
                column: "ItemMasterPtId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocateRequests_LocCode1",
                table: "AllocateRequests",
                column: "LocCode1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingLists_SupplierSupCode",
                table: "IncomingLists",
                column: "SupplierSupCode");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasters_IcId",
                table: "ItemMasters",
                column: "IcId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemMasters_SupplierSupCode",
                table: "ItemMasters",
                column: "SupplierSupCode");

            migrationBuilder.CreateIndex(
                name: "IX_Locs_WarehouseWhCode",
                table: "Locs",
                column: "WarehouseWhCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProdLines_SiteSiCode",
                table: "ProdLines",
                column: "SiteSiCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrders_CustomerCustCode",
                table: "SalesOrders",
                column: "CustomerCustCode");

            migrationBuilder.CreateIndex(
                name: "IX_SoDetails_SalesOrderSoNbr",
                table: "SoDetails",
                column: "SalesOrderSoNbr");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_SiteSiCode",
                table: "Warehouses",
                column: "SiteSiCode");

            migrationBuilder.CreateIndex(
                name: "IX_WoBills_WorkOrderWoNbr",
                table: "WoBills",
                column: "WorkOrderWoNbr");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ItemDataItemNo",
                table: "WorkOrders",
                column: "ItemDataItemNo");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_ProdLinePrLnId",
                table: "WorkOrders",
                column: "ProdLinePrLnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllocateOrders");

            migrationBuilder.DropTable(
                name: "AllocateRequests");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "GeneralizedCodes");

            migrationBuilder.DropTable(
                name: "InventoryTransacs");

            migrationBuilder.DropTable(
                name: "InvRequests");

            migrationBuilder.DropTable(
                name: "IssueNoteDets");

            migrationBuilder.DropTable(
                name: "IssueOrders");

            migrationBuilder.DropTable(
                name: "MfgIssueNotes");

            migrationBuilder.DropTable(
                name: "RequestDets");

            migrationBuilder.DropTable(
                name: "ShipperDets");

            migrationBuilder.DropTable(
                name: "Shippers");

            migrationBuilder.DropTable(
                name: "SoDetails");

            migrationBuilder.DropTable(
                name: "SoIssueNoteDets");

            migrationBuilder.DropTable(
                name: "SoIssueNotes");

            migrationBuilder.DropTable(
                name: "SystemMessages");

            migrationBuilder.DropTable(
                name: "Unqualifieds");

            migrationBuilder.DropTable(
                name: "UnqualifiedTransacs");

            migrationBuilder.DropTable(
                name: "UploadErrors");

            migrationBuilder.DropTable(
                name: "UploadReports");

            migrationBuilder.DropTable(
                name: "WoBills");

            migrationBuilder.DropTable(
                name: "WrIssueNotes");

            migrationBuilder.DropTable(
                name: "WtIssueNotes");

            migrationBuilder.DropTable(
                name: "ItemMasters");

            migrationBuilder.DropTable(
                name: "Locs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SalesOrders");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "IncomingLists");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ItemDatas");

            migrationBuilder.DropTable(
                name: "ProdLines");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Sites");
        }
    }
}
