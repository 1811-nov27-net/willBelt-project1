using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.DataAccess
{
    public partial class Users
    {
        public Users()
        {
            Orders = new HashSet<Orders>();
        }

        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int DefaultLocation { get; set; }
        [Required]
        public string Password { get; set; }

        public virtual Locations DefaultLocationNavigation { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
