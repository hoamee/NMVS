using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Models
{
    public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
    ApplicationUserRole, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }

        public DbSet<Customer> Customers { set; get; }
        public DbSet<Supplier> Suppliers { set; get; }
        public DbSet<Site> Sites { set; get; }
        public DbSet<Warehouse> Warehouses { set; get; }
        public DbSet<Loc> Locs { set; get; }
        public DbSet<ItemData> ItemDatas { set; get; }
        public DbSet<GeneralizedCode> GeneralizedCodes { set; get; }
        public DbSet<ItemMaster> ItemMasters { set; get; }
        public DbSet<IncomingList> IncomingLists { set; get; }
        public DbSet<InvRequest> InvRequests { set; get; }
        public DbSet<RequestDet> RequestDets { set; get; }
        public DbSet<IssueOrder> IssueOrders { set; get; }
        public DbSet<AllocateRequest> AllocateRequests { set; get; }
        public DbSet<AllocateOrder> AllocateOrders { set; get; }

    }
}
