using masterCore.Interfaces;
using masterShared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace masterShared
{
    public static class Startup
    {
        public static IServiceCollection AddDependenciesShared(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITwoFactorAuthService, TwoFactorAuthService>();


            return services;
        }
    }
}
