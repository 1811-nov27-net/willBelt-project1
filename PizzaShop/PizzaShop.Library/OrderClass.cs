using System;
using System.Collections.Generic;

namespace PizzaShop.Library
{
    public class OrderClass
    {
        public UserClass customer { get; set; }
        public List<PizzaClass> pizzas = new List<PizzaClass>();
        public DateTime time { get; set; }
        public LocationClass location { get; set; }
        public decimal total { get; set; }
        public int OrderID { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderID">integer orderID that identifies this order in the database</param>
        /// <param name="user">UserClass representing the user who placed the order</param>
        /// <param name="location">LocationClass representing the location to which the order was placed</param>
        public OrderClass(int orderID, UserClass user, LocationClass location)
        {
            OrderID = orderID;
            customer = user;
            this.location = location;
            total = 0.0m;
        }
        /// <summary>
        /// Check cost and pizza count constraints, and add a PizzaClass object to the pizzas list
        /// </summary>
        /// <param name="newPizza">PizzaClass object to be added to the order</param>
        /// <returns>string indicating whether the pizza was successfully added or not</returns>
        public string AddPizza(PizzaClass newPizza)
        {
            //check pizza count constraint and if pizza was built successfully
            if (pizzas.Count < 12 && newPizza.price != 0.0m)
            {
                //check price limit constraint
                if (total + newPizza.price < 500.00m)
                {
                    //add pizza to pizzas list, add price of pizza to order total, and return string indicating success
                    this.pizzas.Add(newPizza);
                    total += newPizza.price;
                    return newPizza.ToString() + "\nhas been added to your order.";
                }
                else
                    //return string indicating failure of price constraint
                    return "New pizza not added to your order, total may not exceed $500.00";
            }
            else if(pizzas.Count == 12)
            {
                //return string indicating failure of pizza count constraint
                return "No more than 12 pizzas may be added to a single order";
            }
            else
                //return string indicating pizza was not built successfully
                return "Invalid Selection, please try again.";
        }
        /// <summary>
        /// creates a string describing the order
        /// </summary>
        /// <returns>a string describing the order</returns>
        public override string ToString()
        {
            //declare orderString and instantiateit with users name and location desription
            var orderString = $"Customer: {customer.FirstName} {customer.LastName}\nLocation: {location.LocationDescription}\n";
            //iterate through pizzas list and call each pizzas ToString method and add that string to the order string
            foreach (var pizza in pizzas)
            {
                orderString += $"{pizza.ToString()}\n";
            }
            //add total price and time to orderString
            orderString += $"Total: ${total}\n";
            orderString += time.ToString();
            //return completed orderString
            return orderString;
        }
    }
}