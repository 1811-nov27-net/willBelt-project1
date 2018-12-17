using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Library;

namespace PizzaShop.DataAccess
{
    /// <summary>
    /// PizzaShop Data Access Repository
    /// </summary>
    public class PizzaShopRepo : IPizzaShopRepo
    {
        public UserClass User { get; set; }
        public IList<LocationClass> LocationsList { get; set; }
        public ProjectsContext db { get; }
        /// <summary>
        /// Constructor that takes a Database context argument
        /// </summary>
        /// <param name="Db">Database context of type ProjectsContext</param>
        public PizzaShopRepo(ProjectsContext Db)
        {
            db = Db ?? throw new ArgumentNullException(nameof(Db));
        }
        /// <summary>
        /// Adds a new location to the database
        /// </summary>
        /// <param name="location">LocationClass object to be added to the database</param>
        public void AddNewLocation(LocationClass location)
        {
            Locations trackedLocation = new Locations();
            trackedLocation.LocationDescription = location.LocationDescription;
            trackedLocation.Menu = location.GetMenu();
            trackedLocation.Inventory = location.GetInventory();
            db.Add(trackedLocation);
        }
        /// <summary>
        /// Creates a new DataAccess Users object from a UserClass object and adds it to the database to the database
        /// </summary>
        /// <param name="user">UserClass object to be added to the database</param>
        public void AddNewUser(UserClass user)
        {
            Users trackedUser = new Users();
            trackedUser.FirstName = user.FirstName;
            trackedUser.LastName = user.LastName;
            trackedUser.DefaultLocation = db.Locations.First(l => l.LocationDescription == user.DefaultLocation.LocationDescription).LocationId;
            db.Add(trackedUser);
        }

        public bool AddNewUser(Users user)
        {
            if (db.Users.FirstOrDefault(u => u.FirstName == user.FirstName && u.LastName == user.LastName && u.Password == user.Password) == null)
            {
                db.Users.Add(user);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Creates a new DataAccess Orders Object from an OrderClass object and adds it to the database
        /// </summary>
        /// <param name="order">OrderClass object to be added to the database</param>
        public void CreateOrder(OrderClass order)
        {
            Orders trackedOrder = new Orders();
            //ensure the user who placed the order is in the database
            if (db.Users.Where(u => u.FirstName == order.customer.FirstName && u.LastName == order.customer.LastName).ToList().Count == 0)
            {
                AddNewUser(order.customer);
            }
            //copy fields into the DataAccess Orders object
            trackedOrder.UserId = db.Users.First(u => u.FirstName == order.customer.FirstName && u.LastName == order.customer.LastName).UserId;
            trackedOrder.LocationId = db.Locations.First(u => u.LocationDescription == order.location.LocationDescription).LocationId;
            trackedOrder.TotalCost = order.total;
            bool firstLoop = true;
            //iterate through OrderClass objects list of pizzas and copy the internal details of each pizza into a string
            foreach(var pizza in order.pizzas)
            {
                if(!firstLoop)
                    trackedOrder.OrderDescription += "/";
                trackedOrder.OrderDescription += $"{pizza.size},{pizza.crustSelection},";
                foreach(var topping in pizza.toppingSelection)
                {
                    if (topping)
                        trackedOrder.OrderDescription += "1";
                    else
                        trackedOrder.OrderDescription += "0";
                }
                if (firstLoop)
                    firstLoop = false;
            }
            trackedOrder.Time = order.time;
            //Add Orders object to the database
            db.Orders.Add(trackedOrder);
        }
        /// <summary>
        /// Returns All Locations stored in the database as an IList of LocationClass objects
        /// </summary>
        /// <returns></returns>
        public IList<LocationClass> GetAllLocations()
        {
            IList<LocationClass> locationList = new List<LocationClass>();
            foreach (var location in db.Locations.ToList())
            {
                var temporary = BuildLocationFromDBLocations(location);
                BuildLocationOrderHistory(temporary);
                locationList.Add(temporary);
            }
            return locationList;
        }
        /// <summary>
        /// Returns All Orders in Database as an IList of OrderClass objects
        /// </summary>
        /// <returns></returns>
        public IList<OrderClass> GetAllOrders()
        {
            IList<OrderClass> orderList = new List<OrderClass>();
            foreach (var order in db.Orders.ToList())
            {
                orderList.Add(BuildOrderFromDBOrder(order));
            }
            return orderList;
        }
        /// <summary>
        /// Returns all Users in the database as an IList of UserClass objects
        /// </summary>
        /// <returns></returns>
        public IList<UserClass> GetAllUsers()
        {
            IList<UserClass> userList = new List<UserClass>();
            foreach(var user in db.Users.ToList())
            {
                userList.Add(BuildUserFromDBUser(user));
            }
            return userList;
        }
        /// <summary>
        /// Returns all orders in the database placed by a particular user as an IList of OrderClass objects
        /// </summary>
        /// <param name="user">UserClass object representing user whose order history you want to retrieve</param>
        /// <returns></returns>
        public IList<OrderClass> GetOrdersByUser(UserClass user)
        {
            Users DBUser = db.Users.First(u => u.FirstName == user.FirstName && u.LastName == user.LastName);
            IList<OrderClass> orderList = new List<OrderClass>();
            foreach (var order in db.Orders.Where(o => o.UserId == DBUser.UserId).ToList())
            {
                orderList.Add(BuildOrderFromDBOrder(order));
            }
            return orderList;
        }
        /// <summary>
        /// Calls the DB.SaveChanges() Method to save changes made in other methods
        /// </summary>
        public void SaveChanges()
        {
            db.SaveChanges();
        }
        /// <summary>
        /// Checks for a User in the database by name
        /// </summary>
        /// <param name="firstName">First name of the user being searched for</param>
        /// <param name="lastName">Last name of the user being searched for</param>
        /// <returns>true if user exists in database, False if not</returns>
        public bool UserIsInDB(string firstName, string lastName)
        {
            return db.Users.Where(u => u.FirstName == firstName && u.LastName == lastName).ToList().Count == 1;
        }
        /// <summary>
        /// Finds a user in the database by name and builds a UserClass object
        /// </summary>
        /// <param name="firstName">First name of the user being searched for</param>
        /// <param name="lastName">Last name of the user being searched for</param>
        /// <returns>UserClass object Built from the Row in the database corresponding to the queried name</returns>
        public UserClass GetUserByName(string firstName, string lastName, string password)
        {
            var user = BuildUserFromDBUser(db.Users.First(u => u.FirstName == firstName && u.LastName == lastName && u.Password == password));
            BuildLocationOrderHistory(user.DefaultLocation);
            return user;
        }

        public bool CheckLogin(string firstName, string lastName, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName && u.Password == password);
            return user != null;
        }
        /// <summary>
        /// Method to convers from DataAccess Locations object to LocationClass object
        /// </summary>
        /// <param name="location">Locations object from database</param>
        /// <returns>LocationClass Object built from database Locations object</returns>
        private LocationClass BuildLocationFromDBLocations(Locations location)
        {
            List<OrderClass> history = new List<OrderClass>();
            return new LocationClass(location.LocationId, location.LocationDescription, history, location.Menu, location.Inventory);
        }
        /// <summary>
        /// Populates OrderHistory of LocationClass object by retrieving DataAccess Orders with corresponding LocationID from database and sending them to BuildOrderFromDBOrder
        /// </summary>
        /// <param name="location">LocationClass object whose orderHistory has not yet been loaded</param>
        public void BuildLocationOrderHistory(LocationClass location)
        {
            foreach (var order in db.Orders.Where(o => o.LocationId == location.LocationID).ToList())
            {
                location.OrderHistory.Add(BuildOrderFromDBOrder(order));
            }
        }
        /// <summary>
        /// Creates an OrderClass object from a DataAccess Orders object
        /// </summary>
        /// <param name="order">DataAccess Orders Object from database to be converted</param>
        /// <returns>OrderClass object built from DataAccess Orders argument</returns>
        private OrderClass BuildOrderFromDBOrder(Orders order)
        {
            //Declare and instantiate new OrderClass object
            OrderClass newOrder = new OrderClass(order.OrderId, BuildUserFromDBUser(db.Users.Find(order.UserId)), BuildLocationFromDBLocations(db.Locations.Find(order.LocationId)));
            //Set time and total fields to values from DataAccess Orders object
            newOrder.time = order.Time;
            newOrder.total = order.TotalCost;
            //Iterate through and parse DataAccess Orders.OrderDescription field to populate newOrder.pizzas list 
            foreach (var pizza in order.OrderDescription.Split('/'))
            {
                string[] pizzaSubStrings = pizza.Split(',');
                if (pizzaSubStrings.Length == 3)
                {
                    int size, crust, topping;
                    bool[] toppingChoices = new bool[newOrder.location.toppings.Count];
                    int.TryParse(pizzaSubStrings[0], out size);
                    int.TryParse(pizzaSubStrings[1], out crust);
                    for (int i = 0; i < pizzaSubStrings[2].Length; i++)
                    {
                        string temp = "";
                        temp += pizzaSubStrings[2][i];
                        int.TryParse(temp, out topping);
                        toppingChoices[i] = topping == 1;
                    }
                    newOrder.pizzas.Add(
                        new PizzaClass
                        (
                            newOrder.location.sizes,
                            newOrder.location.crustTypes,
                            newOrder.location.toppings,
                            size,
                            crust,
                            toppingChoices
                            ));
                }
            }
            return newOrder;
        }
        /// <summary>
        /// Builds a UserClass object from a DataAccess Users Object
        /// </summary>
        /// <param name="user">DataAccess Users Object from database to be converted</param>
        /// <returns>UserClass object built from DataAccess Users object</returns>
        private UserClass BuildUserFromDBUser(Users user)
        {
            LocationClass location = BuildLocationFromDBLocations(db.Locations.Find(user.DefaultLocation));
            UserClass User = new UserClass(
                user.UserId,
                user.FirstName,
                user.LastName,
                location
                );
            User.Password = user.Password;
            return User;

        }
        /// <summary>
        /// Updates a Locations entry in the database to reflect status of inventory and menu changes
        /// </summary>
        /// <param name="location">LocationClass object representing the location to be updated</param>
        public void UpdateLocation(LocationClass location)
        {
            var trackedLocation = db.Locations.Find(location.LocationID);
            trackedLocation.Inventory = location.GetInventory();
            trackedLocation.Menu = location.GetMenu();
            db.Locations.Update(trackedLocation);
        }
        /// <summary>
        /// Gets a Location from the database by LocationDescription
        /// </summary>
        /// <param name="description">LocationDescription to search for in the database</param>
        /// <returns>LocationClass object representing the Location in the database corresponding to the provided description</returns>
        public LocationClass GetLocationByDescription(string description)
        {
            LocationClass location = BuildLocationFromDBLocations(db.Locations.First(l => l.LocationDescription == description));
            BuildLocationOrderHistory(location);
            return location;
        }
        /// <summary>
        /// Updates User entry in the database to reflect changes to default location
        /// </summary>
        /// <param name="user">UserClass object representing the user to be updated</param>
        public void UpdateUser(UserClass user)
        {
            var trackedUser = db.Users.Find(user.UserID);
            trackedUser.FirstName = user.FirstName;
            trackedUser.LastName = user.LastName;
            trackedUser.DefaultLocation = user.DefaultLocation.LocationID;
            trackedUser.Password = user.Password;
            db.Users.Update(trackedUser);
        }

        public bool IsAdmin(UserClass user)
        {
            if (db.Users.First(u => u.UserId == user.UserID).Admin)
                return true;
            else
                return false;
        }

        public OrderClass GetOrderById(int id)
        {
            OrderClass order = BuildOrderFromDBOrder(db.Orders.Find(id));
            BuildLocationOrderHistory(order.location);
            return order;
        }

        public IList<UserClass> SearchUsersByName(string firstName, string lastName)
        {
            IList<UserClass> list = new List<UserClass>();
            if(firstName != null && lastName != null && firstName != "" && lastName != "")
            {
                foreach(var user in db.Users.Where( u => u.FirstName == firstName && u.LastName == lastName))
                {
                    list.Add(BuildUserFromDBUser(user));
                }
            }
            else if(firstName != null && firstName != "")
            {
                foreach (var user in db.Users.Where(u => u.FirstName == firstName))
                {
                    list.Add(BuildUserFromDBUser(user));
                }
            }
            else if (lastName != null && lastName != "")
            {
                foreach (var user in db.Users.Where(u => u.LastName == lastName))
                {
                    list.Add(BuildUserFromDBUser(user));
                }
            }
            return list;
        }
        public UserClass GetUserById(int id)
        {
            return BuildUserFromDBUser(db.Users.Find(id));
        }
    }
}
