using masterCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace masterInfrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<UsersTenantsRelation> UsersTenantsRelations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<AspNetUsersGroup> AspNetUsersGroup { get; set; }
        public DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemUserGroup> MenuItemUserGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
