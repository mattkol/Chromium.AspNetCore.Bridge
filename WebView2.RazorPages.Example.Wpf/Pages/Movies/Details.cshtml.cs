using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebView2.RazorPages.Example.Wpf.Data;
using WebView2.RazorPages.Example.Wpf.Models;

namespace WebView2.RazorPages.Example.Wpf.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly WebView2.RazorPages.Example.Wpf.Data.MvcMovieContext _context;

        public DetailsModel(WebView2.RazorPages.Example.Wpf.Data.MvcMovieContext context)
        {
            _context = context;
        }

        public Movie Movie { get; set; }

        #region snippet1
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == id);

            if (Movie == null)
            {
                return NotFound();
            }
            return Page();
        }
        #endregion
    }
}
