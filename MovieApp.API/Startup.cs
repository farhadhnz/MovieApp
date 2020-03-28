using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieApp.API.Filters.ExceptionFilters;
using MovieApp.API.Infrastructure;
using MovieApp.API.Services;
using MovieApp.Repository;
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

            services.AddDbContext<MovieAppContext>(opt => opt.UseInMemoryDatabase("MovieDb"));
            services.AddAutoMapper(typeof(Startup));


            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<MovieRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

               
            }

            app.UseRouting();

            app.UseAuthorization();

            if (env.IsDevelopment())
            {
                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<MovieAppContext>();

                    AddTestData(context);
                }
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            
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
