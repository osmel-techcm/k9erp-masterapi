using masterCore.Interfaces;
using masterCore.Services;
using masterInfrastructure.Data;
using masterInfrastructure.Models;
using masterInfrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace masterInfrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(getConnectionString(configuration)));
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(getConnectionString(configuration)));

            services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddDefaultTokenProviders();

            services.AddTransient<SignInManager<ApplicationUser>>();
            services.AddTransient<UserManager<ApplicationUser>>();

            return services;
        }

        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddTransient<IConfigRepo, ConfigRepo>();
            services.AddTransient<IConfigService, ConfigService>();

            services.AddTransient<IAspNetUsersGroupsRepo, AspNetUsersGroupRepo>();
            services.AddTransient<IAspNetUsersGroupsService, AspNetUsersGroupsService>();

            services.AddTransient<ICustomerRepo, CustomerRepo>();
            services.AddTransient<ICustomerService, CustomerService>();

            services.AddTransient<ITenantRepo, TenantRepo>();
            services.AddTransient<ITenantService, TenantService>();

            services.AddTransient<IUsersTenantsRelationRepo, UsersTenantsRelationRepo>();
            services.AddTransient<IUsersTenantsRelationService, UsersTenantsRelationService>();

            services.AddTransient<IAspNetUserRepo, AspNetUserRepo>();
            services.AddTransient<IAspNetUserService, AspNetUserService>();

            services.AddTransient<IMenuItemService, MenuItemService>();
            services.AddTransient<IMenuItemRepo, MenuItemRepo>();

            services.AddTransient<IMenuItemUserGroupService, MenuItemUserGroupService>();
            services.AddTransient<IMenuItemUserGroupRepo, MenuItemUserGroupRepo>();

            return services;
        }

        public static IApplicationBuilder UpdateDatabase(this IApplicationBuilder applicationBuilder, AppDbContext appDbContext) 
        {
            appDbContext.Database.Migrate();
            return applicationBuilder;
        }

        private static string getConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AppTenantCS");

            var server = Environment.GetEnvironmentVariable("SERVER");
            var database = Environment.GetEnvironmentVariable("DATABASE");
            var user = Environment.GetEnvironmentVariable("USER");
            var password = Environment.GetEnvironmentVariable("PASSWORD");

            connectionString = connectionString.Replace(@"{{SERVER}}", server).Replace(@"{{DATABASE}}", database).Replace(@"{{USER}}", user).Replace(@"{{PASSWORD}}", password);

            return connectionString;
        }
    }
}
