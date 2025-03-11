using HotelAPP.Data;
using HotelAPP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPP.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext Db;
        public AuthController(AppDbContext db)
        {
            Db = db;
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if(user != null)
            {
                user.Id = Guid.NewGuid();
                user.IsAdmin = false;
                Db.Users.Add(user);
                Db.SaveChanges();
                return RedirectToAction("Login", "Auth");
            }
            else 
            {
                return RedirectToAction("Login", "Auth");
            }
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            var human = Db.Users.FirstOrDefault(User => User.Email == user.Email && User.Password == user.Password) ;
            if (human != null)
            {
                HttpContext.Session.SetString("UserId", human.Id.ToString());
                if (human.IsAdmin)
                {
                    HttpContext.Session.SetString("AdminId", human.Id.ToString());
                    return RedirectToAction("Index","Admin");
                }
                else
                {
                    HttpContext.Session.SetString("UserId", human.Id.ToString());
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(user);
            }
        }
    }
}
