using DemoEntityCodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoEntityCodeFirst.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            using (var manager = new ManagerDbContext())
            {
                User stud = new User() { ID = 1, Name = "TanLD", Division = ".Net", Experencde = 1 };
                User stud2 = new User() { ID = 2, Name = "TinLVV", Division = ".Net", Experencde = 2 };
                User stud3 = new User() { ID = 3, Name = "ThangDQ", Division = ".Net", Experencde = 3 };
                User stud4 = new User() { ID = 4, Name = "PhongTV", Division = ".Net", Experencde = 2 };
                User stud5 = new User() { ID = 5, Name = "BaoNQ", Division = ".Net", Experencde = 4 };
                manager.Users.Add(stud);
                manager.Users.Add(stud2);
                manager.Users.Add(stud3);
                manager.Users.Add(stud4);
                manager.Users.Add(stud5);
                manager.SaveChanges();
            }
            return View();
        }
    }
}