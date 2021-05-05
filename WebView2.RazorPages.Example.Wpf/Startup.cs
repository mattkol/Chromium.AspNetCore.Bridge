using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebView2.RazorPages.Example.Wpf.Data;

namespace WebView2.RazorPages.Example.Wpf
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
           .AddRazorPagesOptions(options =>
           {
               options.Conventions
                      .ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
           });

            services.AddDbContext<MvcMovieContext>(options =>
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                var connectionString = "Data Source=MvcMovie.db";// Configuration.GetConnectionString("MvcMovieContext");
                options.UseSqlite(connectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
