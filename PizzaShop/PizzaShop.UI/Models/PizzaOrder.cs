using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaShop.UI.Models
{
    public class PizzaOrder
    {
        public int size { get; set; }
        public int crust { get; set; }
        public bool[] toppings { get; set; }
        public List<SelectListItem> sizeList { get; set; }
        public List<SelectListItem> crustList { get; set; }
        Dictionary<int, string> toppingMenu { get; set; }
        public List<string> toppingList { get; set; }
        public List<int> toppingKeys { get; set; }

        public PizzaOrder()
        {

        }

        public void InitalizeMenus (Dictionary<int,string> sizes, Dictionary<int,string> crustTypes, Dictionary<int,string> toppings, Dictionary<int,int> inventory)
        {
            toppingMenu = new Dictionary<int, string>();
            foreach (var topping in toppings)
            {
                if(inventory[topping.Key] != -1)
                {
                    toppingMenu.Add(topping.Key, topping.Value);
                }
            }
            toppingList = new List<string>();
            toppingKeys = new List<int>();
            foreach(var topping in toppingMenu)
            {
                toppingList.Add(topping.Value);
                toppingKeys.Add(topping.Key);
            }
            sizeList = new List<SelectListItem>();
            foreach (var item in sizes)
            {
                sizeList.Add(new SelectListItem { Value = $"{item.Key}", Text = item.Value });
            }
            crustList = new List<SelectListItem>();
            foreach (var item in crustTypes)
            {
                crustList.Add(new SelectListItem { Value = $"{item.Key}", Text = item.Value });
            }
        }
    }
}
