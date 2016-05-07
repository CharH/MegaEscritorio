using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MegaEscritorio
{
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
                int width, length, drawers, rushOrder;
                string material;

                //get user input
                width = GetInput("Please enter your desired desk width in inches:", 1, 500);
                length = GetInput("Please enter your desired desk length in inches:", 1, 500);
                drawers = GetInput("Please enter your desired number of drawers (max 7):", 0, 7);
                material = GetInput("Please select a surface material. Options available are: Oak, Laminate, and Pine.", "OAK", "LAMINATE", "PINE");
                rushOrder = GetInput("Normal production time is 14 days."
                    + " If you would like to rush your order, "
                    + "please enter 3, 5, or 7 to speed up production time for an extra fee."
                    + " Otherwise, enter 0 if you do not wish to rush this order.", 3, 5, 7, 0);
               
                //calculate surface area of desk
                int deskArea = width * length;
                int areaPrice = CalcAreaPrice(deskArea);

                //calculate drawer pricing
                int drawerPrice = 50 * drawers;

                //calculate material cost
                int materialPrice = CalcMaterialPrice(material);

                //read in rush order pricing file
                int[,] rushOrderPricing = new int[3, 3];
                GetRushOrder(rushOrderPricing);

                //calculate rush order pricing if applicable.
                int rushOrderPrice = CalcRushOrder(rushOrderPricing, rushOrder, deskArea);

                //total up the various costs and display total to user.
                int totalDeskCost = areaPrice + drawerPrice + materialPrice + rushOrderPrice;

                Console.WriteLine("The total cost of a " + width + "x" +
                    length + " " + material + " desk with " + drawers + " drawers and a " + rushOrder + " day production time is $"
                    + totalDeskCost);
                //ask user if they would like to save file
                string saveOrder = GetInput("Would you like to save this order? Y/N?", "Y", "N");
                if (saveOrder == "Y")
                {
                    saveOrderInfo(width, length, drawers, material, rushOrder, totalDeskCost);
                }
                endOrder = GetInput("Thank you for using our application. Please select 'E' to exit, or 'N' to start over.", "E", "N");
            } while (endOrder == "N");
            
        }
        //save users order info to external file
        private static void saveOrderInfo(int width, int length, int drawers, string material, int rushOrder, int totalDeskCost)
        {
            bool printCheck = true;
            string orderName = GetInput("Please enter the name you would like to save your quote under.");
            try
            {
                //organize order info into JSON format
                string[] deskOrder ={"Desk Order for: " + orderName,
                    "width: " + width,
                    "length: "+length,
                    "surface material: "+material,
                    "number of drawers: "+drawers,
                    "rush order timing: "+rushOrder +" day rush production time",
                    "total cost: " + totalDeskCost };
            
                //write order info to external file
                File.WriteAllLines("DeskQuote_"+orderName+".txt", deskOrder);
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
        private static int CalcRushOrder(int[,] rushOrderPricing, int rushOrder, int deskArea)
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
        private static int CalcMaterialPrice(string material)
        {   
            int price = 0;
            switch (material)
            {
                case "OAK":
                    price = 200;
                    return price;
                case "LAMINATE":
                    price = 100;
                    return price;
                case "PINE":
                    price = 50;
                    return price;
                default:
                    Console.WriteLine("Invalid material choice detected during calculations.");
                    return 0;
            }
        }

        //calculate a price based on desk surface area and return price
        private static int CalcAreaPrice(int deskArea)
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
        private static void GetRushOrder(int[,] array)
        {           
            try
            {
                string[] lines = File.ReadAllLines("rushOrderPrices.txt");
                int count = 0;
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        if (count == lines.Length)
                            break;
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

        //Get user input and return a value
        private static int GetInput(string prompt, int opt1, int opt2, int opt3, int opt4)
        {
            int finalValue = 2;
            do
            {
                try
                {
                    Console.WriteLine(prompt);
                    string userInput = Console.ReadLine();
                    finalValue = int.Parse(userInput);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
                if ((finalValue != opt1) && (finalValue != opt2) && (finalValue != opt3) && (finalValue != opt4))
                {
                    Console.WriteLine("Sorry, that isn't an option at this time. "
                        + "Please select " + opt1 + " or " + opt2 + " or " + opt3 + " or " + 0 + ".");
                }
            } while ((finalValue != opt1) && (finalValue != opt2) && (finalValue != opt3) && (finalValue != opt4));
            return finalValue;
        }

        private static string GetInput(string prompt, string opt1, string opt2, string opt3)
        {
            string finalValue = null;
            string userInput = null;
            do
            {
                try
                {
                    Console.WriteLine(prompt);
                    userInput = Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
                if (userInput.Equals(opt1, StringComparison.OrdinalIgnoreCase))
                    finalValue = opt1;
                else if (userInput.Equals(opt2, StringComparison.OrdinalIgnoreCase))
                    finalValue = opt2;
                else if (userInput.Equals(opt3, StringComparison.OrdinalIgnoreCase))
                    finalValue = opt3;
                else
                {
                    Console.WriteLine("Sorry, that isn't an option at this time. "
                        + "Please select from " + opt1 + " or " + opt2 + " or " + opt3 + ".");
                }
            } while (finalValue==null);
            return finalValue;
        }

        private static int GetInput(string prompt, int minValue, int maxValue)
        { int finalValue = 0;
            do
            {
                try
                {
                    Console.WriteLine(prompt);
                    string userInput = Console.ReadLine();
                    finalValue = int.Parse(userInput);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if ((finalValue < minValue) || (finalValue > maxValue))
                {
                    Console.WriteLine("Sorry, we don't make desks to those dimensions. "
                        + "Please enter a number between " + minValue + " and " + maxValue + ".");
                }
            } while ((finalValue < minValue) || (finalValue > maxValue));
            return finalValue;
            
        }

        private static string GetInput(string prompt, string opt1, string opt2)
        {
            string finalValue = null;
            string userInput = null;
            do
            {
                try
                {
                    Console.WriteLine(prompt);
                    userInput = Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (userInput.Equals(opt1, StringComparison.OrdinalIgnoreCase))
                    finalValue = opt1;
                else if (userInput.Equals(opt2, StringComparison.OrdinalIgnoreCase))
                    finalValue = opt2;
                else
                {
                    Console.WriteLine("Please select either 'Y' for Yes, or 'N' for No.");
                }
            } while (finalValue == null);
            return finalValue;
        }
        private static string GetInput(string prompt)
        {
            string finalValue = null;
            do
            {
                try
                {
                    Console.WriteLine(prompt);
                    finalValue = Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            } while ((finalValue==null));
            return finalValue;
        }
    }
}
