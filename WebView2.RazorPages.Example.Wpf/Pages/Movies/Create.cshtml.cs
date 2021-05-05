using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebView2.RazorPages.Example.Wpf.Models;
using System;
using System.Threading.Tasks;

namespace WebView2.RazorPages.Example.Wpf.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly WebView2.RazorPages.Example.Wpf.Data.MvcMovieContext _context;

        public CreateModel(WebView2.RazorPages.Example.Wpf.Data.MvcMovieContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            //Movie = new Movie
            //{
            //    Genre = "Action",
            //    Price = 1.99m,
            //    ReleaseDate = DateTime.Now,
            //    Title = "Conan"
            //    , Rating = "R"
            //};
            return Page();
        }

        [BindProperty]
        public Movie Movie { get; set; }

        #region snippet
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Movie.Add(Movie);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        #endregion
    }
}
