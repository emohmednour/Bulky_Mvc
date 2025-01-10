
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork unit;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unit, IWebHostEnvironment webHostEnvironment)
        {
            this.unit = unit;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = unit.Product.GetAll(includeProperties: "Category").ToList();

            return View(products);
        }

        //Create
        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = unit.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = unit.Product.Get(u => u.Id == id);
                return View(productVM);
            }





        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string ProductPath = Path.Combine(wwwRootPath, @"Images\Product");

                    // حذف الصورة القديمة إذا كانت موجودة
                    if (!string.IsNullOrEmpty(productVM.Product.ImageURl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageURl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }


                    using (var filestream = new FileStream(Path.Combine(ProductPath, fileName), FileMode.Create))
                    {

                        file.CopyTo(filestream);
                    }
                    productVM.Product.ImageURl = @"\Images\Product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    //create
                    unit.Product.Add(productVM.Product);

                }
                else
                {
                    //update

                    unit.Product.Update(productVM.Product);

                }
                unit.Save();
                TempData["Success"] = "The Product is Already Added";
                return RedirectToAction("Index");
            }
            else
            {

                productVM.CategoryList = unit.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }


        }











        #region API Call

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> products = unit.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = products });

        }


        //delete
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var producttobeDeleteed = unit.Product.Get(u => u.Id == id);
            if (producttobeDeleteed == null)
            {
                return Json(new { success = false, massage = "Error While Deleteing" });
            }
            var oldimagePath = Path.Combine(_webHostEnvironment.WebRootPath, producttobeDeleteed.ImageURl.TrimStart('\\'));

            if (System.IO.File.Exists(oldimagePath))
            {
                System.IO.File.Delete(oldimagePath);
            }

            unit.Product.Remove(producttobeDeleteed);
            unit.Save();
            return Json(new { success = true, massage = "Delete Successfully" });

        }


        #endregion
    }
}
