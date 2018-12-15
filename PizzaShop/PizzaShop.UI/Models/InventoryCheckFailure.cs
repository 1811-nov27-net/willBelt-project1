using PizzaShop.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaShop.UI.Models
{
    public class InventoryCheckFailure
    {
        public IList<string> UnavailableItems { get; set; }

        public void BuildList(Dictionary<int,string> toppings, Dictionary<int,int> inventory, PizzaClass pizza)
        {
            UnavailableItems = new List<string>();
            foreach (var topping in toppings)
            {
                if(pizza.toppingSelection[topping.Key] && inventory[topping.Key] == 0)
                {
                    UnavailableItems.Add(topping.Value);
                }
            }
        }
    }
}
