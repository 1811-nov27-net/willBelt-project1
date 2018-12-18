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
        public static OrderClass order, SuggestedOrder;
        public static bool SuggestedAnOrder;
        public static PizzaClass pizza;
        public static IList<UserClass> UserList;
        public static UserClass selectedUser;
        public static IEnumerable<OrderClass> history;
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
                        return RedirectToAction(nameof(Options));
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

        public IActionResult Options()
        {
            if (repo.IsAdmin(order.customer))
                return RedirectToAction(nameof(AdminOptions));
            else
                return RedirectToAction(nameof(UserOptions));
        }

        public IActionResult UserOptions()
        {
            return View(order.customer);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(PasswordChange password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (repo.CheckLogin(order.customer.FirstName, order.customer.LastName, password.CurrentPassword))
                    {
                        order.customer.Password = password.NewPassword;
                        repo.UpdateUser(order.customer);
                        repo.SaveChanges();
                        order.customer = repo.GetUserByName(order.customer.FirstName, order.customer.LastName, order.customer.Password);
                        return RedirectToAction(nameof(Options));
                    }
                    else
                        return View();
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

        public IActionResult AdminOptions()
        {
            return View(order.customer);
        }

        public IActionResult NewLocation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewLocation(LocationClass location)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repo.AddNewLocation(location);
                    repo.SaveChanges();
                    return RedirectToAction(nameof(AdminOptions));
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

        public IActionResult ViewInventory()
        {
            Inventory inventory = new Inventory();
            inventory.BuildInventory(order.location);
            return View(inventory);
        }

        public IActionResult RestockedInventory()
        {
            for (int i = 0; i < order.location.inventory.Count; i++)
            {
                if (order.location.inventory[i] != -1)
                {
                    order.location.inventory[i] = 50;
                }
            }
            repo.UpdateLocation(order.location);
            repo.SaveChanges();
            return View();
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

        public IActionResult AdminLocationPicker()
        {
            LocationsList locationList = new LocationsList();
            locationList.InitializeList(repo.GetAllLocations());
            return View(locationList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLocationPicker(LocationsList locationList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.location = repo.GetLocationByDescription(locationList.locationDescription);
                    return RedirectToAction(nameof(AdminActions));
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

        public IActionResult AdminActions()
        {
            return View();
        }

        public IActionResult SortOrderHistory()
        {
            history = order.location.OrderHistory;
            return View();
        }

        public IActionResult SortOrderHistoryByEarliest()
        {
            history = history.OrderBy(o => o.time);
            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult SortOrderHistoryByLatest()
        {
            history = history.OrderByDescending(o => o.time);
            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult SortOrderHistoryByCheapest()
        {
            history = history.OrderBy(o => o.total);
            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult SortOrderHistoryByMostExpensive()
        {
            history = history.OrderByDescending(o => o.total);
            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult ViewOrderHistory()
        {
            return View(history);
        }

        public IActionResult OrderDetails(int id)
        {
            OrderClass Order = repo.GetOrderById(id);
            return View(Order);
        }

        public IActionResult AddTopping()
        {
            Topping topping = new Topping();
            topping.BuildMenus(order.location);
            return View(topping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTopping(Topping topping)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.location.AddToppingToMenu(topping.NewTopping);
                    repo.UpdateLocation(order.location);
                    repo.SaveChanges();
                    return RedirectToAction(nameof(ToppingAdded));
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

        public IActionResult RemoveTopping()
        {
            Topping topping = new Topping();
            topping.BuildMenus(order.location);
            return View(topping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveTopping(Topping topping)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    for(int i = 0; i < topping.toppings.Length; i++)
                    {
                        if (topping.toppings[i])
                        {
                            order.location.RemoveToppingFromMenu(i);
                        }
                    }
                    repo.UpdateLocation(order.location);
                    repo.SaveChanges();
                    return RedirectToAction(nameof(ToppingsRemoved));
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

        public IActionResult ToppingAdded()
        {
            return View();
        }

        public IActionResult ToppingsRemoved()
        {
            return View();
        }

        public IActionResult ChooseDefaultLocation()
        {
            LocationsList list = new LocationsList();
            list.InitializeList(repo.GetAllLocations());
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseDefaultLocation(LocationsList Locationlist)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    order.customer.DefaultLocation = repo.GetLocationByDescription(Locationlist.locationDescription);
                    repo.UpdateUser(order.customer);
                    repo.SaveChanges();
                    return RedirectToAction(nameof(Options));
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
            try
            {
                pizza = new PizzaClass(
                    order.location.sizes,
                    order.location.crustTypes,
                    order.location.toppings,
                    pizzaOrder.size,
                    pizzaOrder.crust,
                    pizzaOrder.toppings);
                if (order.pizzas.Count < 12 && order.total + pizza.price <= 500.00m && order.location.CheckInventory(pizza))
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

        public IActionResult SearchUsers()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchUsers(UserClass user)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    UserList = repo.SearchUsersByName(user.FirstName, user.LastName);
                    if (UserList.Count > 0)
                    {
                        return RedirectToAction(nameof(UserSearchResults));
                    }
                    else
                    {
                        return RedirectToAction(nameof(EmptyUserSearchResult));
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

        public IActionResult UserSearchResults()
        {
            return View(UserList);
        }

        public IActionResult EmptyUserSearchResult()
        {
            return View();
        }

        public IActionResult UserDetails(int id)
        {
            selectedUser = repo.GetUserById(id);
            history = repo.GetOrdersByUser(selectedUser);
            return RedirectToAction(nameof(SortUserHistory));
        }

        public IActionResult UserOrderDetails(int id)
        {
            OrderClass Order = repo.GetOrderById(id);
            return View(Order);
        }

        public IActionResult SortUserHistory()
        {
            return View();
        }

        public IActionResult SortUserHistoryByEarliest()
        {
            history = history.OrderBy(o => o.time);
            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult SortUserHistoryByLatest()
        {
            history = history.OrderByDescending(o => o.time);
            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult SortUserHistoryByCheapest()
        {
            history = history.OrderBy(o => o.total);
            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult SortUserHistoryByMostExpensive()
        {
            history = history.OrderByDescending(o => o.total);
            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult UserHistory()
        {
            return View(history);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
