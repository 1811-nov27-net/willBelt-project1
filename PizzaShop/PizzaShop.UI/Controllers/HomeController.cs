using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.DataAccess;
using PizzaShop.UI.Models;
using PizzaShop.Library;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace PizzaShop.UI.Controllers
{
    public class HomeController : Controller
    {
        public IPizzaShopRepo repo;
        public OrderClass order;
        public int SuggestedOrder;
        public PizzaClass pizza;
        public IList<UserClass> UserList;
        public UserClass selectedUser;
        public IEnumerable<OrderClass> history;
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
                        TempData.Put("order", order);
                        
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
            order = TempData.Peek<OrderClass>("order");
            if (repo.IsAdmin(order.customer))
            {
                
                return RedirectToAction(nameof(AdminOptions));
            }
            else
            {
                
                return RedirectToAction(nameof(UserOptions));
            }
        }

        public IActionResult UserOptions()
        {
            order = TempData.Peek<OrderClass>("order");
            
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
                    order = TempData.Peek<OrderClass>("order");
                    if (repo.CheckLogin(order.customer.FirstName, order.customer.LastName, password.CurrentPassword))
                    {
                        order.customer.Password = password.NewPassword;
                        repo.UpdateUser(order.customer);
                        repo.SaveChanges();
                        order.customer = repo.GetUserByName(order.customer.FirstName, order.customer.LastName, order.customer.Password);
                        TempData.Put("order", order);
                        
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

        public IActionResult AdminOptions()
        {
            order = TempData.Peek<OrderClass>("order");
            if (repo.IsAdmin(order.customer))
            {
                
                return View(order.customer);
            }
            else
            {
                
                return RedirectToAction(nameof(UserOptions));
            }
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
            order = TempData.Peek<OrderClass>("order");
            Inventory inventory = new Inventory();
            inventory.BuildInventory(order.location);
            return View(inventory);
        }

        public IActionResult RestockedInventory()
        {
            order = TempData.Peek<OrderClass>("order");
            for (int i = 0; i < order.location.inventory.Count; i++)
            {
                if (order.location.inventory[i] != -1)
                {
                    order.location.inventory[i] = 50;
                }
            }
            repo.UpdateLocation(order.location);
            repo.SaveChanges();
            TempData.Put("order", order);
            
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
                    user.DefaultLocation = 1;
                    repo.AddNewUser(user);
                    repo.SaveChanges();
                    order = new OrderClass();
                    order.customer = repo.GetUserByName(user.FirstName, user.LastName, user.Password);
                    TempData.Put("order", order);
                    
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
                    order = TempData.Peek<OrderClass>("order");
                    order.location = repo.GetLocationByDescription(locationList.locationDescription);
                    TempData.Put("order", order);
                    
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
            return View();
        }

        public IActionResult SortOrderHistoryByEarliest()
        {
            
            TempData["sortBy"] = 1;
            
            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult SortOrderHistoryByLatest()
        {

            TempData["sortBy"] = 2;

            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult SortOrderHistoryByCheapest()
        {

            TempData["sortBy"] = 3;

            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult SortOrderHistoryByMostExpensive()
        {

            TempData["sortBy"] = 4;

            return RedirectToAction(nameof(ViewOrderHistory));
        }

        public IActionResult ViewOrderHistory()
        {
            order = TempData.Peek<OrderClass>("order");
            repo.BuildLocationOrderHistory(order.location);
            history = order.location.OrderHistory;
            switch (TempData["sortBy"])
                {
                case 1:
                    history = history.OrderBy(o => o.time);
                    break;
                case 2:
                    history = history.OrderByDescending(o => o.time);
                    break;
                case 3:
                    history = history.OrderBy(o => o.total);
                    break;
                case 4:
                    history = history.OrderByDescending(o => o.total);
                    break;
            }
            return View(history);
        }

        public IActionResult OrderDetails(int id)
        {
            OrderClass Order = repo.GetOrderById(id);
            
            return View(Order);
        }

        public IActionResult AddTopping()
        {
            order = TempData.Peek<OrderClass>("order");
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
                order = TempData.Peek<OrderClass>("order");
                if (ModelState.IsValid)
                {
                    order.location.AddToppingToMenu(topping.NewTopping);
                    repo.UpdateLocation(order.location);
                    repo.SaveChanges();
                    TempData.Put("order", order);
                    
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
            order = TempData.Peek<OrderClass>("order");
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
                order = TempData.Peek<OrderClass>("order");
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
                    TempData.Put("order", order);
                    
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
                order = TempData.Peek<OrderClass>("order");
                if (ModelState.IsValid)
                {
                    order.customer.DefaultLocation = repo.GetLocationByDescription(Locationlist.locationDescription);
                    repo.UpdateUser(order.customer);
                    repo.SaveChanges();
                    TempData.Put("order", order);
                    
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
            order = TempData.Peek<OrderClass>("order");
            
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
                order = TempData.Peek<OrderClass>("order");
                if (ModelState.IsValid)
                {
                    order.location = repo.GetLocationByDescription(locationList.locationDescription);
                    TempData.Put("order", order);
                    
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
            order = TempData.Peek<OrderClass>("order");
            repo.BuildLocationOrderHistory(order.location);
            SuggestedOrder = order.location.SuggestFromHistory(order.location.OrderHistory, order.customer).OrderID;
            //repo.BuildLocationOrderHistory(SuggestedOrder.location);
            TempData["SuggestedOrder"] = SuggestedOrder;
            
            return View(repo.GetOrderById(SuggestedOrder));
        }

        public IActionResult PlaceOrder()
        {
            order = TempData.Peek<OrderClass>("order");
            if (TempData.Peek("SuggestedOrder") != null)
            {
                SuggestedOrder = (int)TempData.Peek("SuggestedOrder");
            }
            if (order.location == null)
            {
                order.location = order.customer.DefaultLocation;
                TempData.Put("order", order);
            }
            repo.BuildLocationOrderHistory(order.location);
            if(order.location.OrderHistory.Any(o => o.customer.UserID == order.customer.UserID) && !order.location.TimeCheck(order.customer))
            {   
                return RedirectToAction(nameof(TimeCheckFailure));
            }
            if (order.location.OrderHistory.Any(o => o.customer.UserID == order.customer.UserID) && SuggestedOrder == 0)
            {
                return RedirectToAction(nameof(SuggestOrder));
            }
            
            return View(order);
        }

        public IActionResult AcceptedSuggestedOrder()
        {
            TempData.Put("order", repo.GetOrderById((int)TempData["SuggestedOrder"]));
            
            return RedirectToAction(nameof(PlaceOrder));
        }

        public IActionResult MakePizza()
        {
            order = TempData.Peek<OrderClass>("order");
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
                order = TempData.Peek<OrderClass>("order");
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
                    TempData.Put("order", order);
                    
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
            order = TempData.Peek<OrderClass>("order");
            InventoryCheckFailure items = new InventoryCheckFailure();
            items.BuildList(order.location.toppings, order.location.inventory, pizza);
            
            return View(items);
        }

        public IActionResult TimeCheckFailure()
        {
            order = TempData.Peek<OrderClass>("order");
            repo.BuildLocationOrderHistory(order.location);
            OrderClass previousOrder = order.location.OrderHistory
                .FindAll(o => o.customer.UserID == order.customer.UserID)
                .OrderByDescending(o => o.time).First();
            
            return View(previousOrder);
        }

        public IActionResult OrderPlaced()
        {
            order = TempData.Peek<OrderClass>("order");
            order.time = DateTime.Now;
            repo.CreateOrder(order);
            repo.SaveChanges();
            TempData.Put("order", order);
            
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
                    TempData.Put("UserList", UserList);
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
            UserList = TempData.Get<IList<UserClass>>("UserList");
            
            return View(UserList);
        }

        public IActionResult EmptyUserSearchResult()
        {
            
            return View();
        }

        public IActionResult UserDetails(int id)
        {
            TempData["UserID"] = id;
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
            TempData["sortBy"] = 1;

            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult SortUserHistoryByLatest()
        {
            TempData["sortBy"] = 2;

            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult SortUserHistoryByCheapest()
        {
            TempData["sortBy"] = 3;
            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult SortUserHistoryByMostExpensive()
        {
            TempData["sortBy"] = 4;
            return RedirectToAction(nameof(UserHistory));
        }

        public IActionResult UserHistory()
        {
            history = repo.GetOrdersByUser(repo.GetUserById((int)TempData.Peek("UserID")));
            switch (TempData["sortBy"])
            {
                case 1:
                    history = history.OrderBy(o => o.time);
                    break;
                case 2:
                    history = history.OrderByDescending(o => o.time);
                    break;
                case 3:
                    history = history.OrderBy(o => o.total);
                    break;
                case 4:
                    history = history.OrderByDescending(o => o.total);
                    break;
            }
            return View(history);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static T Peek<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o = tempData.Peek(key);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }

}
