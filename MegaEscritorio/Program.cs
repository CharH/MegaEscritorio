using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MegaEscritorio
{   public interface deskOrder
    {
        void saveOrderInfo(Desk desk);
        int CalcRushOrder(int[,] rushOrderPricing, int rushOrder, int deskArea);
        int CalcMaterialPrice(SurfaceMaterial material);
        int CalcAreaPrice(int deskArea);
        void GetRushOrder(int[,] array);
        int GetIntegerOption(string prompt, int opt1, int opt2, int opt3, int opt4);
        SurfaceMaterial GetSurfaceType(string prompt);
        string GetStringOption(string prompt, string opt1, string opt2);
        int GetIntegerInRange(string prompt, int minValue, int maxValue);
        string GetInput(string prompt);
    }
    public enum SurfaceMaterial
    {
        None,
        Oak,
        Laminate,
        Pine,
        Marble,
        Walnut,
        Metal
    };
    class Program
    {
        static void Main(string[] args)
        {   //set up do...while loop to allow users to restart the process
            string endOrder = null;
            do
            {


                //display initial welcome message
                Console.WriteLine(@"
                ************************************************
                Welcome to the Mega Escritorio custom desk quote 
                application! Please follow the prompts below in 
                order to recieve your personalized price quote.
                ************************************************");
                //variables to be used
                Desk newDeskQuote = new Desk();
                DeskOrderMethods d = new DeskOrderMethods();
                
                //get user input
                newDeskQuote.deskWidth = d.GetIntegerInRange("Please enter your desired desk width in inches:", 1, 500);
                newDeskQuote.deskLength = d.GetIntegerInRange("Please enter your desired desk length in inches:", 1, 500);
                newDeskQuote.noOfDrawers = d.GetIntegerInRange("Please enter your desired number of drawers (max 7):", 0, 7);
                newDeskQuote.deskTopType = d.GetSurfaceType("Please select a surface material from the following: "
                    +"Oak, Laminate, Pine, Marble, Walnut, or Metal");
                newDeskQuote.rushOrder = d.GetIntegerOption("Normal production time is 14 days."
                    + " If you would like to rush your order, "
                    + "please enter 3, 5, or 7 to speed up production time for an extra fee."
                    + " Otherwise, enter 0 if you do not wish to rush this order.", 3, 5, 7, 0);
                
                //calculate surface area of desk
                int deskArea = newDeskQuote.deskWidth * newDeskQuote.deskLength;
                int areaPrice = d.CalcAreaPrice(deskArea);

                //calculate drawer pricing
                int drawerPrice = 50 * newDeskQuote.noOfDrawers;

                //calculate material cost
                int materialPrice = d.CalcMaterialPrice(newDeskQuote.deskTopType);

                //read in rush order pricing file
                int[,] rushOrderPricing = new int[3, 3];
                d.GetRushOrder(rushOrderPricing);

                //calculate rush order pricing if applicable.
                int rushOrderPrice = d.CalcRushOrder(rushOrderPricing, newDeskQuote.rushOrder, deskArea);

                //total up the various costs and display total to user.
                newDeskQuote.priceEstimate = areaPrice + drawerPrice + materialPrice + rushOrderPrice;

                Console.WriteLine("The total cost of a " + newDeskQuote.deskWidth + "x" +
                    newDeskQuote.deskLength + " " + newDeskQuote.deskTopType + " desk with " + newDeskQuote.noOfDrawers 
                    + " drawers and a " + newDeskQuote.rushOrder + " day production time is $"
                    + newDeskQuote.priceEstimate);
                
                //ask user if they would like to save file
                string saveOrder = d.GetStringOption("Would you like to save this order? Y/N?", "Y", "N");
                if (saveOrder == "Y")
                {
                    d.saveOrderInfo(newDeskQuote);
                }
                endOrder = d.GetStringOption("Thank you for using our application. Please select 'E' to exit, or 'N' to start over.", "E", "N");
            } while (endOrder == "N");
            
        }



        
    }
    public class Desk
    {
        public int deskWidth { get; set; }
        public int deskLength { get; set; }
        public int noOfDrawers { get; set; }
        public int rushOrder { get; set; }
        public SurfaceMaterial deskTopType { get; set; }
        public int priceEstimate { get; set; }

        
    }
    public class DeskOrderMethods : deskOrder
    {
        //save users order info to external file
        public void saveOrderInfo(Desk orderInfo)
        {
            bool printCheck = true;
            string orderName = GetInput("Please enter the name you would like to save your quote under.");
            try
            {
                //organize order info into JSON format
                
                string deskOrderInfo = JsonConvert.SerializeObject(orderInfo, Formatting.Indented);
                string[] stringArray = new string[1] { deskOrderInfo };
                
                //write order info to external file
                File.WriteAllLines("DeskQuote_" + orderName + ".txt", stringArray);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                printCheck = false;

            }
            if (printCheck != false)
                Console.WriteLine("Your order has been saved.");
        }

        //determine rush order pricing using size of desk and rush order timing.
        public int CalcRushOrder(int[,] rushOrderPricing, int rushOrder, int deskArea)
        {

            int rushCost = 0;
            int i = 0, j = 0;

            if (rushOrder == 0)
                return 0;
            else
            {
                //use desk area to set i
                if (deskArea <= 1000)
                    j = 0;
                else if ((deskArea > 1000) && (deskArea < 2000))
                    j = 1;
                else
                    j = 2;
                //use rush order timing to set j
                switch (rushOrder)
                {
                    case 3:
                        i = 0;
                        break;
                    case 5:
                        i = 1;
                        break;
                    case 7:
                        i = 2;
                        break;
                }
                //use i and j values to pull up appropriate pricing from 2D array
                rushCost = rushOrderPricing[i, j];

                return rushCost;
            }

        }

        //determine material price based on user selection and return price
        public int CalcMaterialPrice(SurfaceMaterial material)
        {
            int price = 0;
            switch (material)
            {
                case SurfaceMaterial.Oak:
                    price = 200;
                    break;
                case SurfaceMaterial.Laminate:
                    price = 100;
                    break;
                case SurfaceMaterial.Pine:
                    price = 50;
                    break;
                case SurfaceMaterial.Marble:
                    price = 500;
                    break;
                case SurfaceMaterial.Walnut:
                    price = 250;
                    break;
                case SurfaceMaterial.Metal:
                    price = 300;
                    break;
                default:
                    Console.WriteLine("Invalid material choice detected during calculations.");
                    return 0;
            }
            return price;
        }

        //calculate a price based on desk surface area and return price
        public int CalcAreaPrice(int deskArea)
        {
            int price = 0;
            if (deskArea > 1000)
            {
                price = 5 * (deskArea - 1000) + 200;
            }
            else
                price = 200;
            return price;
        }

        //read in pricing info file and place it into a 2D array
        public void GetRushOrder(int[,] array)
        {
            try
            {
                string[] lines = File.ReadAllLines("rushOrderPrices.txt");
                int count = 0;
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        array[i, j] = int.Parse(lines[count]);
                        count++;
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //Get an int from the user that matches a given option
        public int GetIntegerOption(string prompt, int opt1, int opt2, int opt3, int opt4)
        {
            int chosenNumber;
            do
            {
                chosenNumber = int.Parse(GetInput(prompt));

                if ((chosenNumber != opt1) && (chosenNumber != opt2) && (chosenNumber != opt3) && (chosenNumber != opt4))
                {
                    Console.WriteLine("Sorry, that isn't an option at this time. "
                        + "Please select " + opt1 + " or " + opt2 + " or " + opt3 + " or " + opt4 + ".");
                }
            } while ((chosenNumber != opt1) && (chosenNumber != opt2) && (chosenNumber != opt3) && (chosenNumber != opt4));
            return chosenNumber;
        }

        //Get a string from the user and use it to set the surfaceMaterial
        public SurfaceMaterial GetSurfaceType(string prompt)
        {
            string materialChoice;
            SurfaceMaterial deskTop = SurfaceMaterial.None;
            do
            {
                materialChoice = GetInput(prompt);
                materialChoice.ToLower();

                switch (materialChoice)
                {
                    case "oak":
                        deskTop = SurfaceMaterial.Oak;
                        break;
                    case "laminate":
                        deskTop = SurfaceMaterial.Laminate;
                        break;
                    case "pine":
                        deskTop = SurfaceMaterial.Pine;
                        break;
                    case "marble":
                        deskTop = SurfaceMaterial.Marble;
                        break;
                    case "walnut":
                        deskTop = SurfaceMaterial.Walnut;
                        break;
                    case "metal":
                        deskTop = SurfaceMaterial.Metal;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. " + prompt);
                        break;
                }

            } while (deskTop == SurfaceMaterial.None);
            return deskTop;
        }

        //Get a string from the user that matches a given option
        public string GetStringOption(string prompt, string opt1, string opt2)
        {
            string finalValue = null;
            string stringInput = null;
            do
            {
                stringInput = GetInput(prompt);

                if (stringInput.Equals(opt1, StringComparison.OrdinalIgnoreCase))
                    finalValue = opt1;
                else if (stringInput.Equals(opt2, StringComparison.OrdinalIgnoreCase))
                    finalValue = opt2;
                else
                {
                    Console.WriteLine("Please select either " + opt1 + " or " + opt2 + ".");
                }
            } while (finalValue == null);
            return finalValue;
        }

        //Get an int from the user between the min and max
        public int GetIntegerInRange(string prompt, int minValue, int maxValue)
        {
            int number = 0;
            do
            {
                number = int.Parse(GetInput(prompt));

                if ((number < minValue) || (number > maxValue))
                {
                    Console.WriteLine("Sorry, we don't make desks to those dimensions. "
                        + "Please enter a number between " + minValue + " and " + maxValue + ".");
                }
            } while ((number < minValue) || (number > maxValue));
            return number;

        }

        //Get user input using prompt, trim any spaces and return a value.
        public string GetInput(string prompt)
        {
            string finalValue = null;
            do
            {
                try
                {
                    Console.WriteLine(prompt);
                    finalValue = Console.ReadLine();
                    finalValue = finalValue.Trim();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while ((finalValue == null));
            return finalValue;
        }
    }
}
