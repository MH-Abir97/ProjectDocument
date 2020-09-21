using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Pronali.Data;
using Pronali.Data.Constant;
using Pronali.Data.Helper;
using Pronali.Data.Models;
using Pronali.Web.Extension;
using Pronali.Web.Helper;
using Pronali.Web.Job;
using Pronali.Web.Resources;
using Pronali.Web.Services;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Pronali.Web
{
    public class Startup
    {
        private IHostingEnvironment _env { get; }
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = 500000; // 500000 items max
                options.ValueLengthLimit = 1024 * 1024 * 500; // 500MB max len form data
            });
            // Configure Identity options and password complexity here
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = false;

                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                //// Lockout settings
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                //options.Lockout.MaxFailedAccessAttempts = 10;
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.Cookie.Name = "Pronali";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(200);
                options.LoginPath = "/Auth/Login";
                //ReturnUrlParameter requires
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
                options.SessionStore = new MemoryCacheTicketStore();
            });

            services.AddAutoMapper(typeof(Startup));

            //claims
            services.AddAuthorization(option =>
            {
                ClaimHelper claimHelper = new ClaimHelper();
                
                //registering core claims
                var coreClaimList = claimHelper.GetCoreClaimList();
                foreach (var claim in coreClaimList)
                {
                    option.AddPolicy(claim.Value, policy => policy.RequireClaim(claim.Value));
                }

                //registering hr claims
                var payrollClaimList = claimHelper.GetHrClaimList();
                foreach (var claim in coreClaimList)
                {
                    option.AddPolicy(claim.Value, policy => policy.RequireClaim(claim.Value));
                }

                //registering pos claims
                var posClaimList = claimHelper.GetPosClaimList();
                foreach (var claim in coreClaimList)
                {
                    option.AddPolicy(claim.Value, policy => policy.RequireClaim(claim.Value));
                }
            });


            services.AddSqlServerConnectionService(Configuration);

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IImagePath, ImagePath>();

            services.AddSingleton<LocalizationService>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("en"),
                            new CultureInfo("bn")
                        };

                    options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                    // You can change which providers are configured to determine the culture for requests, or even add a custom
                    // provider with your own logic. The providers will be asked in order to provide a culture for each request,
                    // and the first to provide a non-null result that is in the configured supported cultures list will be used.
                    // By default, the following built-in providers are configured:
                    // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
                    // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
                    // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
                    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
                });

            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(Resource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("Resource", assemblyName.Name);
                    };
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddMvc().AddJsonOptions(
                options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // AddPermissionToRoleAsync(RoleConstants.SuperAdmin, services);


            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddHostedService<QuartzHostedService>();

            ////////// Add our job

            // RAW DATA PROCESS
            services.AddSingleton<JobAttendanceRawDataProcess>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(JobAttendanceRawDataProcess),
                cronExpression: "0/5 * * * * ?")); // run every 5 seconds

            // FILTERED DATA PROCESS FROM RAW MACHINE DATA
            services.AddSingleton<JobAttendanceFilteredDataProcess>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(JobAttendanceFilteredDataProcess),
                cronExpression: "0/15 * * * * ?")); // run every 15 seconds

            // PROCESS ATTENDANCE PROCESSED DATA
            services.AddSingleton<JobAttendanceProcessedDataProcess>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(JobAttendanceProcessedDataProcess),
                cronExpression: "0 0 * ? * *")); // run every hour



            // PROCESS DATA PROCESSING AS PER QUEUE LIST
            services.AddSingleton<JobProcessingQueueProcess>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(JobProcessingQueueProcess),
                cronExpression: "0/15 * * * * ?")); // run every 15 seconds
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }


            var options = app.ApplicationServices
              .GetService<IOptions<RequestLocalizationOptions>>();

            app.UseRequestLocalization(options.Value);

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseCookiePolicy();



            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

          // CreateDefaultUser(services).Wait();

        }


        private async Task CreateDefaultUser(IServiceProvider serviceProvider)
        {
            //initializing custom roles
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "SuperAdmin", "Admin", "HR", "General" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            ApplicationUser SuperAdmin = await UserManager.FindByNameAsync("sa");

            if (SuperAdmin == null)
            {
                SuperAdmin = new ApplicationUser()
                {
                    UserName = "sa",
                    Email = "hk.bd93@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                var result = await UserManager.CreateAsync(SuperAdmin, "123456");
            }
            await UserManager.AddToRoleAsync(SuperAdmin, "SuperAdmin");
        }


        private async void AddPermissionToRoleAsync(string roleName, IServiceCollection services)
        {
            RoleManager<ApplicationRole> roleManager = services.BuildServiceProvider().GetRequiredService<RoleManager<ApplicationRole>>();

            var role = await roleManager.FindByNameAsync(roleName);

            var permittedClaimList = roleManager.GetClaimsAsync(role);

            ClaimHelper claimHelper = new ClaimHelper();
            var claimList = claimHelper.GetAllClaimList();
            foreach (var claim in claimList)
            {
                if (!permittedClaimList.Result.Any(x => x.Type == claim.Value))
                {
                    await roleManager.AddClaimAsync(role, new Claim(claim.Value, role.Id));
                }
            }

        }
    }
}
