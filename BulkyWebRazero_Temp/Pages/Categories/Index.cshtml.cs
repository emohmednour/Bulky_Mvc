using BulkyWebRazero_Temp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazero_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext db;
        public List<Category> CategoriesList { get; set; }
        public IndexModel(AppDbContext db)
        {
            this.db = db;
        }
        public void OnGet()
        {
            CategoriesList = db.Categories.ToList();

        }
    }
}
