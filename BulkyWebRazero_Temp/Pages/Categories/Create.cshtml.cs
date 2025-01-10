using BulkyWebRazero_Temp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazero_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext db;
        [BindProperty]
        public Category category { get; set; }
        public CreateModel(AppDbContext db)
        {
            this.db = db;
        }
        public void OnGet()
        {
        }
        public  IActionResult OnPost()
        {
            db.Categories.Add(category);
            db.SaveChanges();
            TempData["Success"] = "Category Added Successfuly";
            return RedirectToPage("Index");
        }
    }
}
