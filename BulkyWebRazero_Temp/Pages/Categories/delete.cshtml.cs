using BulkyWebRazero_Temp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace BulkyWebRazero_Temp.Pages.Categories
{
    [BindProperties]
    public class deleteModel : PageModel
    {
        private readonly AppDbContext db;

        public Category category {  get; set; } 
        public deleteModel(AppDbContext db)
        {
            this.db = db;
        }
        public void OnGet(int? id)
        {
            if (id != 0 && id != null)
            {
                category = db.Categories.Find(id);
            }
        }
        public IActionResult OnPost()
        {
            Category? obj= db.Categories.Find(category.Id);
            if (obj == null)
               { return NotFound(); }
           
                db.Categories.Remove(obj);
                db.SaveChanges();
                TempData["Success"] = "Category Deleted Successfuly";
            return RedirectToPage("Index");
            
        }
    }
}
