﻿using System;
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
        public static UserClass LoggedInUser;
        public static OrderClass order, SuggestedOrder;
        public static bool SuggestedAnOrder;
        public static PizzaClass pizza;
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
                        order = new OrderClass();
                        order.customer = repo.GetUserByName(user.FirstName, user.LastName, user.Password);
                        return RedirectToAction(nameof(DefaultLocation));
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

        public IActionResult CreateNewAccount()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewAccount(Users user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repo.AddNewUser(user);
                    order = new OrderClass();
                    order.customer = repo.GetUserByName(user.FirstName, user.LastName, user.Password);
                    return RedirectToAction(nameof(ChooseDefaultLocation));   
                }
                else
                {
                    return View();
                }
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

        public IActionResult ChooseDefaultLocation()
        {
            LocationsList list = new LocationsList();
            list.InitializeList(repo.GetAllLocations());
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewAccount(LocationsList list)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.customer.DefaultLocation = repo.GetLocationByDescription(list.locationDescription);
                    repo.UpdateUser(order.customer);
                    return RedirectToAction(nameof(DefaultLocation));
                }
                else
                {
                    return View();
                }
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

        public IActionResult DefaultLocation()
        {
            return View(order.customer.DefaultLocation);
        }

        public IActionResult Location()
        {
            LocationsList list = new LocationsList();
            list.InitializeList(repo.GetAllLocations());
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Location(LocationsList locationList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.location = repo.GetLocationByDescription(locationList.locationDescription);
                    return RedirectToAction(nameof(PlaceOrder));
                }
                else
                {
                    return View();
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Id", ex.Message);
                return View(repo.GetAllLocations());
            }
            catch
            {
                return View(repo.GetAllLocations());
            }
        }

        public IActionResult SuggestOrder()
        {
            SuggestedOrder = new OrderClass();
            SuggestedOrder = order.location.SuggestFromHistory(order.location.OrderHistory, order.customer);
            repo.BuildLocationOrderHistory(SuggestedOrder.location);
            return View(SuggestedOrder);
        }

        public IActionResult PlaceOrder()
        {
            if (order.location == null)
            {
                order.location = order.customer.DefaultLocation;
            }
            if(order.location.OrderHistory.Any(o => o.customer.UserID == order.customer.UserID) && !order.location.TimeCheck(order.customer))
            {
                return RedirectToAction(nameof(TimeCheckFailure));
            }
            if (order.location.OrderHistory.Any(o => o.customer.UserID == order.customer.UserID) && SuggestedOrder == null)
            {
                return RedirectToAction(nameof(SuggestOrder));
            }
            return View(order);
        }

        public IActionResult AcceptedSuggestedOrder()
        {
            order = SuggestedOrder;
            return RedirectToAction(nameof(PlaceOrder));
        }

        public IActionResult MakePizza()
        {
            PizzaOrder pizza = new PizzaOrder();
            pizza.InitalizeMenus(order.location.sizes, order.location.crustTypes, order.location.toppings, order.location.inventory);
            return View(pizza);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakePizza(PizzaOrder pizzaOrder)
        {
            pizza = new PizzaClass(
                order.location.sizes,
                order.location.crustTypes,
                order.location.toppings,
                pizzaOrder.size,
                pizzaOrder.crust,
                pizzaOrder.toppings);
            if(order.pizzas.Count < 12 && order.total + pizza.price <= 500.00m && order.location.CheckInventory(pizza))
            {
                order.AddPizza(pizza);
                order.location.DecrementInventory(pizza);
                return RedirectToAction(nameof(PlaceOrder));
            }
            else if (order.pizzas.Count == 12)
            {
                return RedirectToAction(nameof(OrderIsFull));
            }
            else if (order.total + pizza.price > 500.00m)
            {
                return RedirectToAction(nameof(MaximumPrice));
            }
            else
            {
                return RedirectToAction(nameof(InventoryUnavailable));
            }
        }

        public IActionResult OrderIsFull()
        {
            return View();
        }

        public IActionResult MaximumPrice()
        {
            return View();
        }

        public IActionResult InventoryUnavailable()
        {
            InventoryCheckFailure items = new InventoryCheckFailure();
            items.BuildList(order.location.toppings, order.location.inventory, pizza);
            return View(items);
        }

        public IActionResult TimeCheckFailure()
        {
            OrderClass previousOrder = order.location.OrderHistory
                .FindAll(o => o.customer.UserID == order.customer.UserID)
                .OrderByDescending(o => o.time).First();
            return View(previousOrder);
        }

        public IActionResult OrderPlaced()
        {
            order.time = DateTime.Now;
            repo.CreateOrder(order);
            repo.SaveChanges();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
