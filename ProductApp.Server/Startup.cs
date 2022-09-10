using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
//using ProductApp.Server.BindModels;
using ProductApp.Server.Localized;
using ProductApp.Server.Models;
using ProductApp.Server.Services;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace ProductApp.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private const string ConnectionString = "PgConnection";

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString(ConnectionString));
            //});
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString(ConnectionString));
            });


            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 5;
                options.User.AllowedUserNameCharacters = null;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddRoles<IdentityRole>().AddErrorDescriber<RussianIdentityErrorDescriber>();
            //TODO: нужно ли использовать этот логгер ?
            services.AddSingleton(Log.Logger);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = false,// !!!!!!!!!!!!! Не работает если включить издателя токена
                    ValidateAudience = true,
                    ValidAudience = Configuration["AuthSettings:Audience"],
                    ValidIssuer = Configuration["AuthSettings:Issuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddAuthorization(options =>
             options.AddPolicy("Role",
                 policy => policy.RequireClaim(claimType: ClaimTypes.Role, "admin")));

            services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
            services.AddTransient<IMailService, SendGridMailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            //TODO : AddScoped или AddTransient
            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<IUserDataService, UserDataService>();
            services.AddTransient<IApplicationStartupService, ApplicationStartupService>();
            services.AddControllers();
            //services.AddControllers(options =>
            //{
            //    options.ModelBinderProviders.Insert(0, new StreamBinderProvider());
            //});

            services.AddCors(options =>
            {
                options.AddPolicy("DevCorsPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            services.AddRazorPages();
            services.AddSwaggerGen();
            //services.AddLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseRequestLocalization("ru-RU");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("DevCorsPolicy");

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
