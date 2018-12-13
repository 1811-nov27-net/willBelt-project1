using System;
using System.Collections.Generic;

namespace PizzaShop.DataAccess
{
    public partial class Orders
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int LocationId { get; set; }
        public decimal TotalCost { get; set; }
        public string OrderDescription { get; set; }
        public DateTime Time { get; set; }

        public virtual Locations Location { get; set; }
        public virtual Users User { get; set; }
    }
}
