using PizzaShop.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaShop.UI.Models
{
    public class Inventory
    {
        public List<string> ToppingNames { get; set; }
        public List<int> ToppingStock { get; set; }

        public void BuildInventory(LocationClass location)
        {
            ToppingNames = new List<string>();
            ToppingStock = new List<int>();
            for (int i = 0; i < location.toppings.Count; i++)
            {
                if(location.inventory[i] != -1)
                {
                    ToppingNames.Add(location.toppings[i]);
                    ToppingStock.Add(location.inventory[i]);
                }
            }
        }
    }
}
