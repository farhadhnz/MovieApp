using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MovieApp.API.Filters.ExceptionFilters;
using MovieApp.API.Infrastructure;
using MovieApp.API.Services;
using MovieApp.Repository;
using MovieApp.Repository.Models;
using MovieApp.Repository.Repositories;

namespace MovieApp.API
{
    public class Startup
    {

        private readonly int? _httpsPort;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var launchJsonComfig = new ConfigurationBuilder().
                AddJsonFile("Properties\\launchSettings.json").Build();
            _httpsPort = launchJsonComfig.GetValue<int>("iisSettings:iisExpress:sslPort");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            

            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(JsonExceptionFilter));
                opt.Filters.Add(typeof(LinkRewritingFilter));

                opt.SslPort = _httpsPort;
                opt.Filters.Add(typeof(RequireHttpsAttribute));

                var jsonFormatter = opt.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().Single();
                opt.OutputFormatters.Remove(jsonFormatter);
                opt.OutputFormatters.Add(new IonOutputFormatter(jsonFormatter));

                opt.CacheProfiles.Add("Static", new CacheProfile
                {
                    Duration = 86400,

                });
            });

            services.AddRouting(opt => opt.LowercaseUrls = true);

            services.AddApiVersioning(opt =>
            {
                opt.ApiVersionReader = new MediaTypeApiVersionReader();
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            });

            services.AddDbContext<MovieAppContext>(opt =>
            {
                opt.UseInMemoryDatabase("MovieDb");
                //opt.UseOpenIddict<Guid>();

            });
            
            services.AddAutoMapper(typeof(Startup));


            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<MovieRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserRepository>();

            services.AddIdentity<UserEntity, UserRoleIdentity>()
                .AddEntityFrameworkStores<MovieAppContext>()
                .AddDefaultTokenProviders();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        //ValidateLifetime = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        //ValidIssuer = Configuration["Jwt:Issuer"],
                        //ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });
            //services.AddOpenIddict()
            //    .AddCore(opt =>
            //    {
            //        opt.UseEntityFrameworkCore()
            //        .UseDbContext<MovieAppContext>()
            //        .ReplaceDefaultEntities<Guid>();
            //    })
            //    .AddServer(opt =>
            //    {
            //        opt.SetTokenEndpointUris("/token");

            //        opt.AllowPasswordFlow();

            //        opt.AcceptAnonymousClients();

            //        opt.AddDevelopmentEncryptionCertificate().
            //            AddDevelopmentSigningCertificate();

            //        opt.UseAspNetCore()
            //          //.EnableAuthorizationEndpointPassthrough()
            //          .EnableTokenEndpointPassthrough();
            //          //.DisableTransportSecurityRequirement();

            //    })
            //    .AddValidation(opt =>
            //    {
            //        //opt.UseLocalServer();
            //        //opt.UseAspNetCore();
            //    });

            //services.Configure<IdentityOptions>(opt =>
            //{
            //    opt.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
            //    opt.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
            //    opt.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            //});

            //services.AddAuthentication(opt =>
            //{
            //    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            //});

            AddIdentityCoreServices(services);
        } 

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

               
            }

            app.UseRouting();


            if (env.IsDevelopment())
            {
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<MovieAppContext>();

                    AddTestData(context);

                    var roleManager = serviceScope.ServiceProvider
                        .GetService<RoleManager<UserRoleIdentity>>();
                    var userManager = serviceScope.ServiceProvider
                        .GetService<UserManager<UserEntity>>();

                    AddTestUsers(roleManager, userManager).Wait();
                }
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }

        private static void AddIdentityCoreServices(IServiceCollection services)
        {
            var builder = services.AddIdentityCore<UserEntity>();
            builder = new IdentityBuilder(
                builder.UserType,
                typeof(UserRoleIdentity),
                builder.Services);

            builder.AddRoles<UserRoleIdentity>()
                .AddEntityFrameworkStores<MovieAppContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager<SignInManager<UserEntity>>();
        }

        private static async Task AddTestUsers(RoleManager<UserRoleIdentity> roleManager, UserManager<UserEntity> userManager)
        {
            await roleManager.CreateAsync(new UserRoleIdentity("Admin"));

            var user = new UserEntity
            {
                Email = "farhad.hajnoruzi@gmail.com",
                UserName = "farhad.hajnoruzi@gmail.com",
                FirstName = "Farhad",
                LastName = "Hajnoruzi",
                CreatedAt = DateTimeOffset.Now
            };

            user.SecurityStamp = Guid.NewGuid().ToString();

            await userManager.CreateAsync(user, "Far123456789");

            await userManager.AddToRoleAsync(user, "Admin");
            //await userManager.UpdateAsync(user);
        }

        private static void AddTestData(MovieAppContext context)
        {
            context.Movies.Add(new Repository.Models.MovieEntity()
            {
                Id = Guid.Parse("db40a58b-86d2-4678-aa3e-e9617dfedc1d"),
                Title = "Inception"
            });

            context.Movies.Add(new Repository.Models.MovieEntity()
            {
                Id = Guid.Parse("e901de71-d555-40b4-88f5-cdf7d30c50a4"),
                Title = "Titanic"
            });

            context.SaveChanges();
        }
    }
}
