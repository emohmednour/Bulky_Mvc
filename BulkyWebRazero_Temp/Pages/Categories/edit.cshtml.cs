using BulkyWebRazero_Temp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazero_Temp.Pages.Categories
{
    [BindProperties]
    public class editModel : PageModel
    {
        private readonly AppDbContext context;
        public Category? category { get; set; }
        public editModel(AppDbContext context)
        {
            this.context = context;
        }
        public void OnGet(int? id)
        {
            if (id != 0 && id!=null)
            {
                category = context.Categories.Find(id);
            }
            
        }
        public IActionResult OnPost() {

            if (ModelState.IsValid)
            {
                context.Categories.Update(category);
                context.SaveChanges();
                TempData["Success"] = "Category Edited Successfuly";
                return RedirectToPage("Index");
            }
            return Page();

        }

    }
}
