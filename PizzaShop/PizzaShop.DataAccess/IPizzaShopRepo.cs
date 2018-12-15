using PizzaShop.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaShop.DataAccess
{
    /// <summary>
    /// PizzaShopRepo Interface
    /// </summary>
    public interface IPizzaShopRepo
    {
        UserClass User { get; set; }
        IList<LocationClass> LocationsList { get; set; }
        IList<LocationClass> GetAllLocations();
        IList<UserClass> GetAllUsers();
        IList<OrderClass> GetAllOrders();
        void CreateOrder(OrderClass order);
        IList<OrderClass> GetOrdersByUser(UserClass user);
        void SaveChanges();
        void AddNewLocation(LocationClass location);
        void AddNewUser(UserClass user);
        bool AddNewUser(Users user);
        bool UserIsInDB(string firstName, string lastName);
        UserClass GetUserByName(string firstName, string lastName, string password);
        void BuildLocationOrderHistory(LocationClass location);
        void UpdateLocation(LocationClass location);
        LocationClass GetLocationByDescription(string description);
        void UpdateUser(UserClass user);
        bool CheckLogin(string firstName, string lastName, string password);
    }
}
