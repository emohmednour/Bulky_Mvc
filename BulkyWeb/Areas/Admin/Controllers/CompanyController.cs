using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unit;

        public CompanyController(IUnitOfWork unit)
        {
           
            _unit = unit;
        }
        public IActionResult Index()
        {
            
            List<Company> companies= _unit.Company.GetAll().ToList();
            return View(companies);
        }



        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //create
                return View(new Company());
                
            }
            else
            {
                //update
                Company company = _unit.Company.Get(u => u.id == id);

                return View(company); 
            }

        }
        [HttpPost]
        public IActionResult Upsert(Company company) {

            if (ModelState.IsValid)
            {
                if (company.id == 0)
                {
                    //create
                    _unit.Company.Add(company);

                }
                else
                {
                    //update

                    _unit.Company.Update(company);


                }
                _unit.Save();
                TempData["Success"] = "The Product is Already Added";
                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
        
        }



        #region call Api
        [HttpGet]
        public IActionResult Getall() {
            List<Company> companies = _unit.Company.GetAll().ToList();
            return Json(new {data =  companies });


        }

        [HttpDelete]
        public IActionResult Delete(int id) {
            Company company = _unit.Company.Get(u =>u.id == id);
            if (company == null)
            {
                return Json (new {success =  false , massage = "Error While Deleteing"});
            }
            _unit.Company.Remove(company);
            _unit.Save();
            return Json(new { success = true, massage = "Delete Successfully" });
        
        
        }
        #endregion

    }
}
