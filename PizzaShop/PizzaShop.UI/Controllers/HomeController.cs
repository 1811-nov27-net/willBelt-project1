using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess;
using PizzaShop.UI.Models;
using PizzaShop.Library;

namespace PizzaShop.UI.Controllers
{
    public class HomeController : Controller
    {
        public IPizzaShopRepo repo;
        public UserClass LoggedInUser;
        public HomeController(IPizzaShopRepo Repo)
        {
            repo = Repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Users user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(repo.CheckLogin(user.FirstName, user.LastName, user.Password))
                    {
                        LoggedInUser = repo.GetUserByName(user.FirstName, user.LastName);
                        RedirectToAction(nameof(Location));
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Id", ex.Message);
                return View();
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Location(UserClass user)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
