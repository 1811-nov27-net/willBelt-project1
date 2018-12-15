using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaShop.DataAccess;
using PizzaShop.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaShop.UI.Models
{
    public class LocationsList
    {
        public string locationDescription { get; set; }
        public List<SelectListItem> list { get; set; }

        public void InitializeList (IList<LocationClass> List)
        {
            list = new List<SelectListItem>();
            foreach(var item in List)
            {
                list.Add(new SelectListItem { Value = item.LocationDescription, Text = item.LocationDescription });
            }
        }
    }
}
