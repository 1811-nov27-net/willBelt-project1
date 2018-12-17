using PizzaShop.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaShop.UI.Models
{
    public class Topping
    {
        public string NewTopping { get; set; }
        public bool[] toppings { get; set; }
        Dictionary<int, string> toppingMenu { get; set; }
        public List<string> toppingList { get; set; }
        public List<int> toppingKeys { get; set; }

        public void BuildMenus(LocationClass location)
        {
            toppingMenu = new Dictionary<int, string>();
            foreach (var topping in location.toppings)
            {
                if (location.inventory[topping.Key] != -1)
                {
                    toppingMenu.Add(topping.Key, topping.Value);
                }
            }
            toppingList = new List<string>();
            toppingKeys = new List<int>();
            foreach (var topping in toppingMenu)
            {
                toppingList.Add(topping.Value);
                toppingKeys.Add(topping.Key);
            }
        }
    }
}
