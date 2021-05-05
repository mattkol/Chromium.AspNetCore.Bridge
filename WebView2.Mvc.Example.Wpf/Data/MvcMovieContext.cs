using Microsoft.EntityFrameworkCore;
using WebView2.Mvc.Example.Wpf.Models;

namespace WebView2.Mvc.Example.Wpf.Data
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
    }
}
