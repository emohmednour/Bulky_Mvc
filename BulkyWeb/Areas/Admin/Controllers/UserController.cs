using Microsoft.AspNetCore.Mvc;

using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;
using Microsoft.EntityFrameworkCore;
using BulkyBook.DataAccess.Repository;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext context;

        public UserController(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }





        [HttpGet]
        public IActionResult GetAll(string status)
        {

            List<ApplicationUser> objUserList = context.applicationUsers.Include(u=>u.Company).ToList();

            var UserRoles = context.UserRoles.ToList(); // UserRoles 
            var Roles = context.Roles.ToList(); // name of roles 
            foreach (var item in objUserList)
            {
                


                if(item.Company  == null)
                {
                    item.Company = new() { Name = "" };
                }
            }








            return Json(new { data = objUserList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id) {

            return Json(new { data = true, Massage = "Delete Successful " });
        
        }

    }
}
