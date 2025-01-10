using Microsoft.AspNetCore.Mvc;

using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unit)
        {
            _unitOfWork = unit;
        }
        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {


            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "the displayOrder Can't exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfuly";
                return RedirectToAction("index");

            }

            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? result = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? result1 = db.categories.FirstOrDefault(i => i.Id == id);
            //Category? result2 = db.categories.Where(i => i.Id == id).FirstOrDefault();
            if (result == null) { return NotFound(); }
            return View(result);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category Editrd Successfuly";

                return RedirectToAction("index");

            }


            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? result = _unitOfWork.Category.Get(u => u.Id == id);

            if (result == null)
            {
                return NotFound();
            }
            return View(result);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? result = _unitOfWork.Category.Get(u => u.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(result);
            _unitOfWork.Save();
            TempData["Success"] = "Category Deleted Successfuly";

            return RedirectToAction("index");




        }


    }
}
