using HotelAPP.Data;
using HotelAPP.Models;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPP.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext Db;
        public AdminController(AppDbContext db)
        {
            Db = db;
        }
        public IActionResult Index()
        {
            List<ResvDto> userReservations = (from reservation in Db.Reservations
                                              join room in Db.Rooms on reservation.RoomId equals room.Id
                                              orderby reservation.ExitTime
                                              select new ResvDto
                                              {
                                                  Id = reservation.Id,
                                                  Name = Db.Users.Where(x => x.Id == reservation.UserId).Select(x => x.Name).First(),
                                                  Surname = Db.Users.Where(x => x.Id == reservation.UserId).Select(x => x.Surname).First(),
                                                  RoomNumber = room.RoomNumber,
                                                  RoomPrice = room.RoomPrice,
                                                  EnterTime = reservation.EnterTime,
                                                  ExitTime = reservation.ExitTime,
                                              }).ToList();
            return View(userReservations);
        }
        public IActionResult DeleteReservation(Guid id)
        {
            var reservation = Db.Reservations.Find(id);
            if (reservation != null)
            {
                Db.Reservations.Remove(reservation);
                Db.SaveChanges();
            }

            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Rooms()
        {
            List<RoomDto> roomList = (from room in Db.Rooms
                                      orderby room.RoomNumber
                                      select new RoomDto
                                      {
                                          Id = room.Id,
                                          RoomNumber = room.RoomNumber,
                                          RoomPrice = room.RoomPrice,
                                          //status = 

                                      }).ToList();
            return View(roomList);
        }
        public IActionResult CreateRoom()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateRoom(Room room)
        {
            if (room != null)
            {
                var roomNumber = Db.Rooms.Where(x => x.RoomNumber == room.RoomNumber).FirstOrDefault();
                if (roomNumber == null)
                {
                    room.Id = Guid.NewGuid();
                    Db.Rooms.Add(room);
                    Db.SaveChanges();
                    return RedirectToAction("Rooms", "Admin");
                }
                else
                {
                    return RedirectToAction("CreateRoom", "Admin");
                }
            }
            else
            {
                return RedirectToAction("CreateRoom", "Admin");
            }
        }
        public IActionResult DeleteRoom(Guid id)
        {
            var room = Db.Rooms.Find(id);
            if (room != null)
            {
                Db.Rooms.Remove(room);
                Db.SaveChanges();
            }

            return RedirectToAction("Rooms", "Admin");
        }
        public IActionResult Users()
        {
            List<User> users = Db.Users.Where(x=> x.IsAdmin == false).ToList();
            return View(users);
        }
        public IActionResult DeleteUser(Guid id)
        {
            var user = Db.Users.Find(id);
            if (user != null)
            {
                Db.Users.Remove(user);
                Db.SaveChanges();
            }

            return RedirectToAction("Users", "Admin");
        }
    }
}
