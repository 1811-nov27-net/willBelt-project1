using System.Collections.Generic;

namespace PizzaShop.Library
{
    public class PizzaClass
    {
        Dictionary<int, string> sizes;
        Dictionary<int, string> crustTypes;
        Dictionary<int, string> toppings;
        public int size { get; set; }
        public int crustSelection { get; set; }
        public bool[] toppingSelection;
        public decimal price { get; set; }
        public string PizzaString{ get; set; }
        /// <summary>
        /// Pizza constructor
        /// </summary>
        /// <param name="sizes">Menu of Size options</param>
        /// <param name="crustTypes">Menu of crust types</param>
        /// <param name="toppings">menu of toppings</param>
        /// <param name="chosenSize">selected size</param>
        /// <param name="crust">selected crust type</param>
        /// <param name="toppingChoices">desired toppings</param>
        public PizzaClass(Dictionary<int, string> sizes, Dictionary<int, string> crustTypes, Dictionary<int, string> toppings, int chosenSize, int crust, bool[] toppingChoices)
        {
            //import menu from location
            this.sizes = sizes;
            this.crustTypes = crustTypes;
            this.toppings = toppings;
            this.toppingSelection = new bool[toppings.Count];
            //ensure topping selections are valid
            bool toppingsAreValid = true;
            for(int i = 0; i < toppingChoices.Length; i++)
            {
                if (toppingChoices[i] && !toppings.ContainsKey(i))
                    toppingsAreValid = false;
            }
            if (sizes.ContainsKey(chosenSize) && crustTypes.ContainsKey(crust) && toppingsAreValid)
            {
                size = chosenSize;
                crustSelection = crust;
                toppingSelection = toppingChoices;
                price = this.CalculatePrice();
                PizzaString = ToString();
            }
        }
        /// <summary>
        /// method that calculates the price of the individual pizza
        /// </summary>
        /// <returns>price of the pizza as a decimal value</returns>
        private decimal CalculatePrice()
        {
            int numberOfToppings = 0;
            decimal sizePrice = 0.0m;
            //determine base price from size
            switch (this.size)
            {
                case 0:
                    sizePrice = 5.99m;
                    break;
                case 1:
                    sizePrice = 7.99m;
                    break;
                case 2:
                    sizePrice = 9.99m;
                    break;
            }
            foreach(bool choice in toppingSelection)
            {
                if(choice)
                {
                    numberOfToppings++;
                }
            }
            //determine how much to add to base price for selected toppings
            if (numberOfToppings > 2)
                return sizePrice + ((numberOfToppings - 2) * .50m);
            else
                return sizePrice;
        }
        /// <summary>
        /// Method that returns a string describing the pizza
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            List<string> toppingsList = new List<string>();
            string toppingsString = "";
            bool noToppings = true;
            //iterate through toppingSelection to determine which toppings to add to toppingsList and do so
            for (int i = 0; i < toppingSelection.Length; i++)
            {
                if (toppingSelection[i])
                {
                    toppingsList.Add(toppings[i]);
                    noToppings = false;
                }   
            }
            //iterate through toppings list to build string describing topping selections
            for(int i = 0; i < toppingsList.Count; i++)
            {
                toppingsString += toppingsList[i];
                if (i + 1 < toppingsList.Count - 1)
                    toppingsString += ", ";
                else if (i + 1 == toppingsList.Count - 1)
                    toppingsString += ", and ";
            }
            //if no toppings selected, indicate as much
            if (noToppings)
                toppingsString = "no toppings";
            //return string describing the pizza and specifiying price
            return ($"{sizes[size]} {crustTypes[crustSelection]} pizza with {toppingsString}: ${price}");
        }
    }
}