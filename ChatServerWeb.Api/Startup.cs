using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerWeb.Api.Infrastructure;
using ChatServerWeb.Api.Infrastructure.Configuration;
using ChatServerWeb.BusinessLogic;
using ChatServerWeb.BusinessLogic.Service;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;
using ChatServerWeb.Model.Identity;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChatServerWeb.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(p =>
            {
                p.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Chat Server API", Version = "v1" });
            });

            //Sql Server
            services.AddDbContext<ChatServerDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("ChatServerWeb.Api")));

           
            services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly(("ChatServerWeb.Api"))));


            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()

            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;

                
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
                // User settings
                options.User.RequireUniqueEmail = true;
            });

           


            JWTConfig jwtConfig = new JWTConfig();

            Configuration.GetSection("JWT").Bind(jwtConfig);


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();




            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })

                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.ServerSecret));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = serverSecret,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                    };
                });

            //HangFire

            services.AddHangfire(prop =>
                {
                    prop.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
                }


            );

            //Automapper
            services.AddAutoMapper();

            //D.I
            services.AddScoped<IRepository, Repository>();
            services.AddTransient<JWTAUthenticator>();
            services.AddSingleton(jwtConfig);
            services.AddSingleton<ChatMessageSingletonService>();
            services.AddTransient<ChatMessageLogic>();
            services.AddTransient<ChatApplicationsLogic>();
            services.AddTransient<ChatApplicationUserLogic>();
            services.AddTransient<ApplicationInitializer>();
            services.AddTransient<HangfireBackgroundService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IRecurringJobManager recurringJobManager)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatServer API V1");
            });

            //Enable Hangfire
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new List<IDashboardAuthorizationFilter>()
                {
                    new HangfireAuthorization()
                }
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();

            recurringJobManager.AddOrUpdate("SaveGeneralChatMessageFromSingleton",Job.FromExpression<HangfireBackgroundService>(p=>p.EnqueueChatMessageJobs()),Cron.MinuteInterval(2));
        }
    }
}
