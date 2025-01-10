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
        public IActionResult GetAll()
        {

            List<ApplicationUser> objUserList = context.applicationUsers.Include(u=>u.Company).ToList();

            var userRoles = context.UserRoles.ToList(); // UserRoles 
            var roles = context.Roles.ToList(); // name of roles 
            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;



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
        [HttpDelete]
        public IActionResult Delete(int? id) {

            return Json(new { data = true, Massage = "Delete Successful " });
        
        }

    }
}
