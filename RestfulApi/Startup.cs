using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

namespace RestfulApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public static ILoggerRepository LoggerRepository { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            LoggerRepository = LogManager.CreateRepository("NETCoreRepository");

            XmlConfigurator.Configure(LoggerRepository, new FileInfo("log4net.config"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelAttribute));
                options.Filters.Add(typeof(CustomizedExceptionFilterAttribute));
            });

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = Configuration.GetSection("Redis")["InstanceName"];
                options.Configuration = Configuration.GetSection("Redis")["ConnectionString"];
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = GetJwtTokenValidationParameters();
                });

            services.Configure<JwtOptions>(Configuration.GetSection("Colipu-ERP"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }

        private TokenValidationParameters GetJwtTokenValidationParameters()
        {
            IConfigurationSection jwtSection = Configuration.GetSection("JsonWebToken");

            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["SecretKey"]));

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,

                ValidateIssuer = true,
                ValidIssuer = jwtSection["IssUser"],

                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            return tokenValidationParameters;
        }
    }
}
