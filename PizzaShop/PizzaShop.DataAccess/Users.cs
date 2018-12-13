using System;
using System.Collections.Generic;

namespace PizzaShop.DataAccess
{
    public partial class Users
    {
        public Users()
        {
            Orders = new HashSet<Orders>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DefaultLocation { get; set; }
        public string Password { get; set; }

        public virtual Locations DefaultLocationNavigation { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
