using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using NLog.Extensions.Logging;
using System.IO;
using Contracts;
using Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace server
{
    public class Startup
    {
        public string _SecretApiKey = null;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            // Load configuration from nlog.config
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            // Configure cors
            services.ConfigureCors();
            
            // Configure IIS to handle HTTP requests
            services.ConfigureIISIntegration();

            // Logger logs to file
            services.ConfigureLoggerService();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Indetify IRepositoryWrapper as RepositoryWrapper object
            services.AddTransient<IRepositoryWrapper, RepositoryWrapper>();

            // Configure MySql Context
            services.ConfigureMySqlContext(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            // Enable access from other networks
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
