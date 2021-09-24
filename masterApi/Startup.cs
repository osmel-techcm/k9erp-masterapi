using HealthChecks.UI.Client;
using masterApi.Validation;
using masterInfrastructure;
using masterInfrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace masterApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static string idTenant = string.Empty;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed( _ => true)
                        .AllowCredentials();
            }));

            services.AddInfrastructure(Configuration);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JwtIssuer"],
                        ValidAudiences = new List<string> {
                                        Configuration["JwtAudienceTenant"],
                                        Configuration["JwtAudience"]
                        },
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                        {
                            var keys = new List<SecurityKey>();
                            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"]));
                            keys.Add(signingKey);

                            if (!string.IsNullOrEmpty(kid) && kid == idTenant)
                            {
                                signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKeyTenant"] + "-" + kid));
                                keys.Add(signingKey);
                            }

                            return keys;
                        }
                    };
                    cfg.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }

                            idTenant = context.Request.Headers["x-tenant-id"];

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddDependencies(Configuration);

            services
                .AddControllers()
                .AddNewtonsoftJson()
                .ConfigureApiBehaviorOptions(options => {
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddMvcOptions(options => {
                    options.Filters.Add<ValidationFilter>();
                });

            services.AddHealthChecks()
                .AddCheck("Success", () => HealthCheckResult.Healthy("Success from " + Environment.MachineName));
            
        }

        private string getConnectionStringRedis()
        {
            return Environment.GetEnvironmentVariable("SERVER-REDIS");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext appDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UpdateDatabase(appDbContext);

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/", new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        
    }
}
