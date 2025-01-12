using Microsoft.AspNetCore.Mvc;

using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;
using Microsoft.EntityFrameworkCore;
using BulkyBook.DataAccess.Repository;
using System.Security.Claims;
using BulkyBook.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork unit;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public UserController(IUnitOfWork unit , RoleManager<IdentityRole> roleManager,UserManager<IdentityUser> userManager)
        {
            this.unit = unit;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }





        [HttpGet]
        public IActionResult GetAll()
        {

            List<ApplicationUser> objUserList = unit.ApplicationUser.GetAll(includeProperties : "Company").ToList();

           
            foreach (var user in objUserList)
            {
                user.Role = userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if(user.Company  == null)
                {
                    user.Company = new Company() 
                    {
                        Name = "" 
                    };
                }
            }


            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = unit.ApplicationUser.Get(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
          unit.ApplicationUser.Update(objFromDb);
            unit.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }



        [HttpGet]
        public IActionResult RoleManagment( string userId)
        {
            RoleManagmentVM roleManagmentvm = new()
            {

                applicationUser = unit.ApplicationUser.Get(u => u.Id == userId , includeProperties : "Company"),
                RoleList = roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = unit.Company.GetAll().Select(i=> new SelectListItem
                {
                    Text = i.Name,
                    Value = i.id.ToString()
                })

            };
            roleManagmentvm.applicationUser.Role = userManager.GetRolesAsync(unit.ApplicationUser.Get(u => u.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();





            return View(roleManagmentvm);


        }
        [HttpPost]

        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            string oldRole = userManager.GetRolesAsync(unit.ApplicationUser.Get(u => u.Id == roleManagmentVM.applicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = unit.ApplicationUser.Get(u => u.Id == roleManagmentVM.applicationUser.Id);
            if (!(oldRole == roleManagmentVM.applicationUser.Role))
            {
                if (roleManagmentVM.applicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyID = roleManagmentVM.applicationUser.CompanyID;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyID = null;
                }
                unit.ApplicationUser.Update(applicationUser);
                unit.Save();

                userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(applicationUser, roleManagmentVM.applicationUser.Role).GetAwaiter().GetResult();


            }
            else
            {
                //if user not select any role (company )           and want to change name company  
                if (oldRole == SD.Role_Company && applicationUser.CompanyID != roleManagmentVM.applicationUser.CompanyID)
                {
                    applicationUser.CompanyID = roleManagmentVM.applicationUser.CompanyID;
                    unit.ApplicationUser.Update(applicationUser);
                    unit.Save();
                }
            }

            return RedirectToAction("Index");

        }








    }
}
