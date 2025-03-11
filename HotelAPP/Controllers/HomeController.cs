using HotelAPP.Data;
using HotelAPP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HotelAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext Db;
        public HomeController(AppDbContext db, ILogger<HomeController> logger)
        {
            Db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Room> rooms = (from room in Db.Rooms
                                join reservation in Db.Reservations
                                on room.Id equals reservation.RoomId into roomReservations
                                from reservation in roomReservations.DefaultIfEmpty()
                                where reservation == null || reservation.ExitTime < DateTime.Now || reservation.ExitTime == null
                                select new Room
                                {
                                    Id = room.Id,
                                    RoomNumber = room.RoomNumber,
                                    RoomPrice = room.RoomPrice
                                }).Distinct().OrderBy(x=> x.RoomNumber).ToList();
            ViewData["RoomList"] = rooms;
            return View();
        }
        public IActionResult ViewResv()
        {
            var userId = new Guid( HttpContext.Session.GetString("UserId"));
            List<ResvDto> userReservations = (from reservation in Db.Reservations
                                              join room in Db.Rooms on reservation.RoomId equals room.Id
                                              where reservation.UserId == userId
                                              orderby reservation.ExitTime
                                              select new ResvDto
                                              {
                                                  Id = reservation.Id,
                                                  RoomNumber = room.RoomNumber,
                                                  RoomPrice = room.RoomPrice,
                                                  EnterTime = reservation.EnterTime,
                                                  ExitTime=reservation.ExitTime,
                                              }).ToList();
            //ViewData["ResvList"] = userReservations;
            return View(userReservations);
        }
        [HttpPost]
        public IActionResult Index(int RoomId, DateTime EnterTime, DateTime ExitTime)
        {

            if (RoomId != null && EnterTime != null && ExitTime != null)
            {
                var userId = new Guid(HttpContext.Session.GetString("UserId"));
                var userReservations = new Reservation
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoomId = Db.Rooms.Where(x => x.RoomNumber == RoomId).Select(x => x.Id).First(),
                    EnterTime = EnterTime,
                    ExitTime = ExitTime
                };
                Db.Reservations.Add(userReservations);
                Db.SaveChanges();
                return RedirectToAction("ViewResv", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public IActionResult DeleteReservation(Guid id)
        {
            var reservation = Db.Reservations.Find(id);
            if (reservation != null)
            {
                Db.Reservations.Remove(reservation);
                Db.SaveChanges();
            }

            return RedirectToAction("ViewResv", "Home");
        }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
