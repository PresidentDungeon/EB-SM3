using System;
using EB.Infrastructure.Data;
using EB.Infrastructure.DataInitializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace EB.RestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Byte[] secretBytes = new byte[40];
            Random rand = new Random();
            rand.NextBytes(secretBytes);

            services.AddScoped<InitStaticData>();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.MaxDepth = 3;
            });

            if (Env.IsDevelopment())
            {
                services.AddDbContext<EBContext>(opt =>
                {
                    opt.UseSqlite("Data Source=EBApp.db");
                    opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                },
                    ServiceLifetime.Transient);
            }

            else if (Env.IsProduction())
            {
                services.AddDbContext<EBContext>(opt =>
                {
                    opt.UseSqlServer(Configuration.GetConnectionString("defaultConnection"));
                    opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                },
                    ServiceLifetime.Transient);
            }

            services.AddCors(options => options.AddDefaultPolicy(
                builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }
                ));

            services.AddSwaggerGen((options) => {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Esbjerg Bryghus",
                    Description = "A RestAPI for a brewery back-end application",
                    Version = "v1"
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetService<EBContext>();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    ctx.Database.EnsureDeleted();
                    ctx.Database.EnsureCreated();

                    InitStaticData dataInitilizer = scope.ServiceProvider.GetRequiredService<InitStaticData>();
                    //dataInitilizer.InitData();
                }
                else
                {
                    {
                        ctx.Database.EnsureCreated();
                    }
                }
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Brewery back-end API");
            });

            app.UseCors();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}