using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaShop.Library
{
    public class UserClass
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public LocationClass DefaultLocation;
        public LocationClass location;
        public int UserID { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userID">UserID associated with user in the database</param>
        /// <param name="first">users first name</param>
        /// <param name="last">users last name</param>
        /// <param name="defaultLocation">default location to which user places order</param>
        public UserClass(int userID, string first, string last, LocationClass defaultLocation)
        {
            UserID = userID;
            FirstName = first;
            LastName = last;
            DefaultLocation = defaultLocation;
        }
        /// <summary>
        /// dtermines location to which user is placing order
        /// </summary>
        /// <param name="locations">List of locations user can choose</param>
        public void GetLocation(IList<LocationClass> locations)
        {
            bool isValidInput = false;
            string locationsString = "";
            string input;
            //build string to request users chosen location
            for (int i = 0; i < locations.Count; i++)
            {
                locationsString += $"{i+1}. {locations[i].LocationDescription}\n";
            }
            do
            {
                //suggest users default location and request user confirmation
                Console.Write($"Your Default Location is: {DefaultLocation.LocationDescription}. Is this Correct?(y/n)\n");
                input = Console.ReadLine();
                if (input.ToLower() == "y" || input.ToLower() == "yes")
                {
                    //if user chooses default location, set current location to default location and exit the loop
                    location = DefaultLocation;
                    isValidInput = true;
                }
                else if (input.ToLower() == "n" || input.ToLower() == "no")
                {
                    //if user rejects suggestion, ask user to specify the location to which they would like to place their order
                    Console.Write($"Which location do you wish to order from?\n{locationsString}");
                    input = Console.ReadLine();
                    //parse user input
                    isValidInput = (int.TryParse(input, out int number) && number - 1 > -1 && number - 1 < locations.Count);
                    if (isValidInput)
                    {
                        //set current location to that chosen by user
                        location = locations[number - 1];
                        //ask user if they would like to make this locatione their new default location
                        Console.WriteLine("Would you like to set this as your default location? (y/n)");
                        input = Console.ReadLine();
                        if (input.ToLower() == "y" || input.ToLower() == "yes")
                            DefaultLocation = location;
                    }
                    else
                        //ask user for valid input
                        Console.WriteLine("Invalid entry, please enter the number of your selection");
                }
                else
                    //ask user for valid input
                    Console.WriteLine("Invalid entry, please enter yes or no");
            } while (!isValidInput);
        }
    }
}
