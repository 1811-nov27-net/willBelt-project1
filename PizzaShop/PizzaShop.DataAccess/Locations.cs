using System;
using System.Collections.Generic;

namespace PizzaShop.DataAccess
{
    public partial class Locations
    {
        public Locations()
        {
            Orders = new HashSet<Orders>();
            Users = new HashSet<Users>();
        }

        public int LocationId { get; set; }
        public string LocationDescription { get; set; }
        public string Inventory { get; set; }
        public string Menu { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Users> Users { get; set; }
    }
}
