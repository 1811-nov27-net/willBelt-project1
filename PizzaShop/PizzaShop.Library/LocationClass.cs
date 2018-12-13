using System;
using System.Collections.Generic;
using System.Linq;

namespace PizzaShop.Library
{
    public class LocationClass
    {
        public Dictionary<int,int> inventory = new Dictionary<int,int> {
            { 0,50 },
            {1, 50 },
            {2, 50 },
            {3, 50 },
            {4, 50 },
            {5, 50 },
            {6, 50 } };
        public Dictionary<int, string> sizes = new Dictionary<int, string> {
            { 0, "Small" },
            { 1, "Medium" },
            { 2, "Large" } };
        public Dictionary<int, string> crustTypes = new Dictionary<int, string> {
            { 0, "Hand-Tossed" },
            { 1, "Deep-Dish" },
            { 2, "Thin Crust" } };
        public Dictionary<int, string> toppings = new Dictionary<int, string> {
            { 0, "Pepperoni" },
            { 1, "Canadian Bacon" },
            { 2, "Sausage" },
            { 3, "Mushrooms" },
            { 4, "Black Olives" },
            { 5, "Green Peppers" },
            { 6, "Onions" }
        };
        public List<OrderClass> OrderHistory;
        List<string> OrderRequestStrings;
        private string input;
        private bool[] toppingChoices;
        private int size, crust;
        public string LocationDescription;
        public int LocationID { get; set; }
        /// <summary>
        /// Constructor that uses default menu and inventory settings
        /// </summary>
        /// <param name="description">Location name/description</param>
        /// <param name="history">List of OrderClass objects representing the orderHistory of the location</param>
        public LocationClass(string description, List<OrderClass> history)
        {
            LocationDescription = description;
            OrderHistory = history;
        }
        /// <summary>
        /// Constructor that takes Menu and inventory from database
        /// </summary>
        /// <param name="locationID">int LocationID from database</param>
        /// <param name="description">string describing the location</param>
        /// <param name="history">List of OrderClass objects representing the Order History of the location</param>
        /// <param name="menu">string defining the menu for the location</param>
        /// <param name="inventory">string containing the inventory values for the location</param>
        public LocationClass(int locationID, string description, List<OrderClass> history, string menu, string inventory)
        {
            //set LocationID, LocationDescription and OrderHistory to the arguments sent to the constructor
            LocationID = locationID;
            LocationDescription = description;
            OrderHistory = history;
            //Parse the Menu string into section to build the dictionaries that represent the menu
            string[] menuSubstrings = menu.Split('/');
            //Remove default values from menu dictionaries
            this.sizes = new Dictionary<int, string>();
            this.crustTypes = new Dictionary<int, string>();
            this.toppings = new Dictionary<int, string>();
            //Use BuildMenu method to repopulate the Menu dictionaries with information from the database
            BuildMenu(menuSubstrings[0], this.sizes);
            BuildMenu(menuSubstrings[1], this.crustTypes);
            BuildMenu(menuSubstrings[2], this.toppings);
            //use BuildInventory method to set the inventory values to those found in the database
            BuildInventory(inventory);
            
        }
        /// <summary>
        /// Takes input from the User and Generates an OrderClass object, then adds it to the order history of the location
        /// </summary>
        /// <param name="user">UserClass object representing the user placing the order</param>
        /// <returns>OrderClass object to be returned to the console app so it can then be added to the database</returns>
        public OrderClass TakeOrder(UserClass user)
        {
            //Declare a new OrderClass object and instantiate it with an OrderID of 0, user that is placing the order 
            //and location where the order is being placed
            OrderClass newOrder = new OrderClass(0, user, this);
            //retrieve and check users order history from locations order history and check time constraint
            IList<OrderClass> customerHistory = OrderHistory.FindAll(o => o.customer.UserID == user.UserID);
            if (customerHistory.Count == 0 || TimeCheck(customerHistory.OrderByDescending(o => o.time).First().time))
            {
                //make suggestion to user based on order history
                if (customerHistory.Count > 0 && SuggestFromHistory(customerHistory, newOrder))
                {

                }
                else
                {
                    //Ask user to define what kind of pizza they want to order
                    bool OrderCompleted = true;
                    do
                    {
                        //build strings to print to console to ask user for order input
                        BuildOrderRequestStrings();
                        //ask user for order input
                        GetSizeOrder();
                        GetCrustOrder();
                        GetToppigsOrder();
                        //check inventory to ensure order can be fulfilled
                        if (CheckInventory())
                        {
                            //Decrement inventory values of selected ingredients
                            DecrementInventory();
                            //instantiate PizzaClass object , add it to newOrder.pizzas and print to console
                            PizzaClass newPizza = new PizzaClass(sizes, crustTypes, toppings, size, crust, toppingChoices);
                            Console.WriteLine(newOrder.AddPizza(newPizza));
                            Console.WriteLine($"Total: ${newOrder.total}");
                        }
                        //ask user if they want to order any additional pizzas
                        Console.WriteLine("Would you like to order anything else? y/n");
                        input = Console.ReadLine();
                        if (input.ToLower() == "y" || input.ToLower() == "yes")
                            OrderCompleted = false;
                        else
                            OrderCompleted = true;

                    } while (!OrderCompleted);
                }
                //set newOrder.time to the time the order was completed
                newOrder.time = DateTime.Now;
                //add order to locations OrderHistory
                OrderHistory.Add(newOrder);
                //return newOrder to console app so it can be added to the database
                return newOrder;
            }
            else
            {
                //inform user of failure of time constraint
                Console.WriteLine("You must wait at least 2 hours before placing another order to this location");
                return null;
            }
        }
        /// <summary>
        /// Builds order request strings from menu dictionaries
        /// </summary>
        private void BuildOrderRequestStrings()
        {
            //instantiate OrderRequestStrings as a list of strings
            OrderRequestStrings = new List<string>();
            //Build String to request Size from user
            OrderRequestStrings.Add("What Size Pizza would you like?\n");
            //iterate through sizes dictionary and add each option to the string
            foreach (KeyValuePair<int,string> pair in sizes)
            {
                OrderRequestStrings[0] += ($"{pair.Key+1}. {pair.Value}\n");
            }
            //Build string to request Crust Type from user
            OrderRequestStrings.Add("What type of crust would you like?\n");
            //iterate through crustTypes dictionary and add each option to the string
            foreach(KeyValuePair<int,string> pair in crustTypes)
            {
                OrderRequestStrings[1] += ($"{pair.Key+1}. {pair.Value}\n");
            }
            //build string to request topping selection from user
            OrderRequestStrings.Add("What toppings would you like(enter your selections one at a time, then enter 'Done' when done)?\n");
            //iterate through toppings and add each option not flagged as removed(-1 inventory value)
            foreach (KeyValuePair<int,string> pair in toppings)
            {
                if (inventory[pair.Key] != -1)
                {
                    OrderRequestStrings[2] += ($"{pair.Key + 1}. {pair.Value}\n");
                }
            }
        }
        /// <summary>
        /// Get users size selection
        /// </summary>
        private void GetSizeOrder()
        {
            bool isValidInput = false;
            do
            {
                //Request user input
                Console.Write(OrderRequestStrings[0]);
                input = Console.ReadLine();
                //parse user input
                isValidInput = (int.TryParse(input, out int number) && (sizes.ContainsKey(number-1)));
                if (isValidInput)
                    //set size to users selection
                    size = number - 1;
                else
                    //request user provide valid input
                    Console.WriteLine("Invalid entry, please enter the number of your selection");
            } while (!isValidInput);
        }
        /// <summary>
        /// Get users Crust type selection
        /// </summary>
        private void GetCrustOrder()
        {
            bool isValidInput = false;
            do
            {
                //Request user input
                Console.Write(OrderRequestStrings[1]);
                input = Console.ReadLine();
                //parse user input
                isValidInput = (int.TryParse(input, out int number) && (crustTypes.ContainsKey(number-1)));
                if (isValidInput)
                    //set crust to users selection
                    crust = number - 1;
                else
                    //request user provide valid input
                    Console.WriteLine("Invalid entry, please enter the number of your selection");
            } while (!isValidInput);
        }
        /// <summary>
        /// Get users topping selections
        /// </summary>
        private void GetToppigsOrder()
        {
            bool isValidInput = false;
            toppingChoices = new bool[toppings.Count];
            bool done = false;
            do
            {
                //request user input
                Console.Write(OrderRequestStrings[2]);
                input = Console.ReadLine();
                //parse user input
                isValidInput = (int.TryParse(input, out int number) && toppings.ContainsKey(number-1) && inventory[number - 1] != -1);
                if (isValidInput)
                    //set toppingChoices index representing selected topping to true
                    toppingChoices[number-1] = true;
                //check if user is done selecting toppings
                else if (input.ToLower() == "done")
                    done = true;
                else
                    //request user provide valid input
                    Console.WriteLine("Invalid entry, please enter the number of your selection");
            } while (!done);
        }
        /// <summary>
        /// Check inventory to ensure we have all the ingredients necessary to make the ordered pizza
        /// </summary>
        private bool CheckInventory()
        {
            //iterate thrugh toppingChoices and check inventory values for all used ingredients
            for (int i = 0; i < toppingChoices.Length; i++)
            {
                if (inventory[i] == 0)
                {
                    //inform user that the order cannot be fulfilled and return false
                    Console.WriteLine($"We cannot fulfill that order because we are out of {toppings[i]}");
                    return false;
                }
            }
            //if the loop completes, return true
            return true;
        }
        /// <summary>
        /// Decrements values in inventory corresponing to the selected ingredients
        /// </summary>
        private void DecrementInventory()
        {
            for (int i = 0; i < toppingChoices.Length; i++)
            {
                if (toppingChoices[i] && inventory[i] > 0)
                {
                    inventory[i]--;
                }
            }
        }
        /// <summary>
        /// Converts menu dictionaries into a string to be stored in the database
        /// </summary>
        /// <returns>string representing the locations menu</returns>
        public string GetMenu()
        {
            //declare and instantiate menuString as an empty string
            string menuString = "";
            //iterate through sizes dictionary and add each size option to the string delimited by commas
            bool firstLoop = true;
            foreach (KeyValuePair<int, string> pair in sizes)
            {
                if (!firstLoop)
                    menuString += ",";
                menuString += $"{pair.Value}";
                if (firstLoop)
                    firstLoop = false;
            }
            //add a slash to delimit menu sections
            menuString += "/";
            //iterate through crustTypes dictionary and add each option to the string delimited by commas
            firstLoop = true;
            foreach (KeyValuePair<int, string> pair in crustTypes)
            {
                if (!firstLoop)
                    menuString += ",";
                menuString += $"{pair.Value}";
                if (firstLoop)
                    firstLoop = false;
            }
            //add a slash to delimit menu sections
            menuString += "/";
            //iterate through toppings dictionary and add each option to the string delimited by commas
            firstLoop = true;
            foreach (KeyValuePair<int, string> pair in toppings)
            {
                if (!firstLoop)
                    menuString += ",";
                menuString += $"{pair.Value}";
                if (firstLoop)
                    firstLoop = false;
            }
            //return completed menu string
            return menuString;
        }
        /// <summary>
        /// converts inventory dictionary into a string to be stored in the database
        /// </summary>
        /// <returns>string representing current inventory values</returns>
        public string GetInventory()
        {
            //declare and instantiate inventoryString as an empty string
            string inventoryString = "";
            //iterate through inventory dictionary and add each inventory value to the string delimited by commas
            bool firstLoop = true;
            foreach ( KeyValuePair<int,int> pair in inventory)
            {
                if (!firstLoop)
                    inventoryString += ",";
                inventoryString += $"{pair.Value}";
                if (firstLoop)
                    firstLoop = false;
            }
            //return completed inventoryString
            return inventoryString;
        }
        /// <summary>
        /// Builds Menu sections from strings retrieved from database
        /// </summary>
        /// <param name="Substring">Substring representing the section of the menu to be built</param>
        /// <param name="menu">dictionary representing the section of the menu to be built</param>
        private void BuildMenu(string Substring, Dictionary<int,string> menu)
        {
            string[] strings = Substring.Split(',');
            for (int i = 0; i < strings.Length; i++)
            {
                menu.Add(i, strings[i]);
            }
        }
        /// <summary>
        /// Builds inventory from inventory string retrieved from database
        /// </summary>
        /// <param name="Substring">string containing the inventory values retrieved from database</param>
        private void BuildInventory(string Substring)
        {
            //re-instantiate inventory to remove default values
            inventory = new Dictionary<int, int>();
            //split inventory string from database to separate individual inventory values
            string[] strings = Substring.Split(',');
            //iterate through strings array and parse inventory values, check their validity, and add them to the inventory dictionary
            for (int i = 0; i < strings.Length; i++)
            {
                if (int.TryParse(strings[i], out int number))
                    inventory.Add(i, number);
                else
                    Console.WriteLine("Error Building Inventory, Inventory String contains Invalid Value");
            }
        }
        /// <summary>
        /// Check the time argument against DateTime.Now to see if 2 hours have elapsed
        /// </summary>
        /// <param name="time">time value from users most recent order</param>
        /// <returns>true if time is more than 2 hours ago, false if not</returns>
        public bool TimeCheck(DateTime time)
        {
            if (DateTime.Now.Subtract(time).TotalHours > 2)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Suggests an order based on users order history
        /// </summary>
        /// <param name="orders">users order history</param>
        /// <param name="newOrder">order currently being placed</param>
        /// <returns>true if user accepts suggestion, false if they choose to order something else</returns>
        private bool SuggestFromHistory(IList<OrderClass> orders, OrderClass newOrder)
        {
            //Group orders by what was ordered, then select the most frequent
            var order = orders.GroupBy(o => o.ToString()).First().First();
            //suggest order to user
            Console.WriteLine($"Would you like to reorder your previous order?(y/n)\n{order.ToString()}");
            string input = Console.ReadLine();
            if (input.ToLower() == "y" || input.ToLower() == "yes")
            {
                newOrder = order;
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// reset inventory values back to full
        /// </summary>
        public void RestockInventory()
        {
            for(int i = 0; i < inventory.Count; i++)
            {
                //check if inventory item has been removed from menu
                if (inventory[i] != -1)
                    inventory[i] = 50;
            }
        }
        /// <summary>
        /// Prints locations order history to console
        /// </summary>
        public void ShowOrderHistory()
        {
            foreach (var order in OrderHistory)
            {
                Console.Write($"{order.ToString()}\n\n");
            }
        }
        /// <summary>
        /// Adds a specified new topping to the menu of this location
        /// </summary>
        public void AddToppingToMenu()
        {
            //ask user for new topping
            Console.WriteLine("Enter the name of the new Topping:");
            string input = Console.ReadLine();
            //check if new topping has been on the menu previously and if so, restore it
            if (toppings.ContainsValue(input))
            {
                foreach (var item in toppings)
                {
                    if (item.Value == input)
                        inventory[item.Key] = 50;
                }
            }
            //if not add it to the toppings dictionary
            else
            {
                int index = toppings.Count;
                toppings.Add(index, input);
                inventory.Add(index, 50);
            }
        }
        /// <summary>
        /// Remove a topping from the menu of this location
        /// </summary>
        public void RemoveToppingFromMenu()
        {
            //ask the user what topping to remove
            Console.WriteLine("What Topping would you like to remove?");
            foreach (var topping in toppings)
            {
                Console.WriteLine($"{topping.Key + 1}. {topping.Value}");
            }
            string input = Console.ReadLine();
            //parse user input and set inventory value for selected topping to -1 to mark it as removed
            if (int.TryParse(input, out int number) && inventory.ContainsKey(number - 1))
            {
                inventory[number - 1] = -1;
            }
        }
        /// <summary>
        /// Method that displays locations inventory
        /// </summary>
        public void ShowInventory()
        {
            string inventoryString = "";
            for (int i = 0; i < toppings.Count; i++)
            {
                //check if topping has been removed from menu
                if (inventory[i] != -1)
                {
                    inventoryString += $"{toppings[i]}: {inventory[i]}\n";
                }
            }
            Console.Write(inventoryString);
        }
    }
}