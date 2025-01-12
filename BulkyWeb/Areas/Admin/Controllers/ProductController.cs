
using BulkyBook.DataAccess.Repository;
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
                productVM.Product = unit.Product.Get(u => u.Id == id , includeProperties: "ProductImages");
                return View(productVM);
            }





        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
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

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (var file in files)

                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string ProductPath = @"Images\Products\Product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, ProductPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        ProductImage productImge = new()
                        {

                            ImageUrl = @"\" + ProductPath + @"\" + fileName,
                            ProductId = productVM.Product.Id
                        };

                        using(var filestream = new FileStream(Path.Combine(finalPath , fileName), FileMode.Create))
                        {
                            file.CopyTo(filestream);
                        }


                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImge);

                    }
                    unit.Product.Update(productVM.Product);
                    unit.Save();


                }

                TempData["Success"] = "The Product id updated / Created successfully";
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




        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = unit.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                unit.ProductImage.Remove(imageToBeDeleted);
                unit.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
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

            string ProductPath = @"Images\Products\Product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, ProductPath);

            if (Directory.Exists(finalPath))
         {
                string[] filePahts = Directory.GetFiles(finalPath);
                foreach (var file in filePahts)
                {
                    System.IO.File.Delete(file);
                }
                Directory.Delete(finalPath); 
            }



            unit.Product.Remove(producttobeDeleteed);
            unit.Save();
            return Json(new { success = true, massage = "Delete Successfully" });

        }


        #endregion
    }
}
