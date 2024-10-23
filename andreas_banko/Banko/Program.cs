using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.IO;
using System.Collections;

class Program
{
    // Class representing the alternative bingo game
    public class AlternativeBingo : BankoPlade
    {
        Hashtable bingoPlates = new(); // Hashtable to store bingo plates by name
        readonly List<int> usedBingoNumbers = new(); // List to keep track of used bingo numbers

        // Method to start the bingo game
        public void StartGame()
        {
            Console.WriteLine("Welcome to Bingo!");

            string input = "";
            while (true)
            {
                Console.Write("Please enter action\n" +
                    "   make\n" +
                    "   view one\n" +
                    "   view all\n" +
                    "   insert\n" +
                    ">");
                input = Console.ReadLine();

                if (input == "make")
                {
                    Console.Write("Enter name>");
                    CreatePlate(name: Console.ReadLine());
                }
                else if (input == "view one")
                {
                    Console.Write("Enter plate name>");
                    try
                    {
                        PrintPlate(name: Console.ReadLine());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error caught: " + ex.Message);
                    }
                }
                else if (input == "view all")
                {
                    Console.WriteLine("All your sexy plates:");
                    PrintAllPlates();
                }
                else if (input == "insert")
                {
                    Console.Write("Enter number>");
                    try
                    {
                        int number = ConvertBingoNumberToInt(Console.ReadLine());
                        InsertBingoNumber(number);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error caught: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }
        }

        // Method to print all bingo plates
        private void PrintAllPlates()
        {
            BankoPlade plade = new();
            foreach (DictionaryEntry entry in bingoPlates)
            {
                plade = (BankoPlade)entry.Value;
                plade.PrintPlade();
            }
        }

        // Method to create a new bingo plate
        private void CreatePlate(string name)
        {
            BankoPlade tempPlate = new();
            tempPlate.CreateRows();
            tempPlate.SetName(name);
            bingoPlates[name] = tempPlate;
        }

        // Method to print a specific bingo plate by name
        private void PrintPlate(string name)
        {
            BankoPlade tempPlate = new();
            tempPlate = (BankoPlade)bingoPlates[name];
            tempPlate.PrintPlade();
        }

        // Method to convert bingo number from string to integer
        private int ConvertBingoNumberToInt(string strNum)
        {
            return Convert.ToInt32(strNum);
        }

        // Method to insert a bingo number and check for winners
        private void InsertBingoNumber(int bingoNumber)
        {
            if (bingoNumber < 1 || bingoNumber > 90)
            {
                Console.WriteLine("Number is either less than 1, or more than 90.");
                return;
            }
            else if (usedBingoNumbers.Contains(bingoNumber))
            {
                Console.WriteLine($"Number {bingoNumber} has already been said.");
                return;
            }
            else
            {
                bool plateVictory = false;
                string winnerName = "";
                BankoPlade winnerPlate = new();
                usedBingoNumbers.Add(bingoNumber);
                foreach (DictionaryEntry entry in bingoPlates)
                {
                    BankoPlade plade = (BankoPlade)entry.Value;
                    plade.CheckAndInsertPlateNumber(bingoNumber, plade);

                    if (plade.GottenEntirePlate())
                    {
                        plateVictory = true;
                        winnerName = plade.GetName();
                        winnerPlate = plade;
                        break;
                    }
                }

                if (plateVictory)
                {
                    Console.WriteLine(winnerName + " Has won!");
                    winnerPlate.PrintPlade();
                }
            }
        }
    }

    // Class representing a basic bingo game
    public class BingoBankoSpil
    {
        BankoPlade[] bankoPlader; // Array of bingo plates
        readonly List<int> usedBingoNumbers = new(); // List to keep track of used bingo numbers

        /// <summary>
        /// Constructor for BingoBankoSpil.
        /// Takes the amount of bingo plates as parameter.
        /// </summary>
        /// <param name="initSize"></param>
        public BingoBankoSpil(int initSize)
        {
            bankoPlader = new BankoPlade[initSize];

            for (int i = 0; i < bankoPlader.Length; i++)
            {
                bankoPlader[i] = new BankoPlade();
                bankoPlader[i].AssignID(i);
                bankoPlader[i].SetName($"{i} of {i}");
                bankoPlader[i].CreateRows();
            }
        }

        // Method to print a bingo plate by name
        public void PrintByName(string name)
        {
            foreach (BankoPlade plade in bankoPlader)
            {
                if (plade.GetName() == name)
                {
                    plade.PrintPlade();
                    return;
                }
            }
            Console.WriteLine("Could not find plate based on name.");
        }

        // Method to export bingo plates to a JSON file
        public void ExportPlatesToJSON(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonString = JsonSerializer.Serialize(bankoPlader, options);
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine($"Successfully exported to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Method to check for errors in bingo plates
        public void ErrorCheckingPlates()
        {
            foreach (var plade in bankoPlader)
                plade.ErrorCheckingRows();
        }

        // Method to simulate the game and determine the winner
        public void ResolvePlates()
        {
            bool plateVictory = false;
            string winnerName = "";
            BankoPlade winnerPlate = new();
            for (int i = 0; i < 90; i++)
            {
                int bingoNumber = GetBingoNumber();
                foreach (BankoPlade plade in bankoPlader)
                {
                    plade.CheckAndInsertPlateNumber(bingoNumber, plade);

                    if (plade.GottenEntirePlate())
                    {
                        plateVictory = true;
                        winnerName = plade.GetName();
                        winnerPlate = plade;
                        break;
                    }
                }

                if (plateVictory)
                {
                    Console.WriteLine(i + " in, and " + winnerName + " Has won!");
                    winnerPlate.PrintPlade();
                    break;
                }
            }
        }

        // Method to get a random bingo number
        private int GetBingoNumber()
        {
            Random randCaller = new Random();
            while (true)
            {
                int bingoNumber = randCaller.Next(1, 91);
                if (usedBingoNumbers.Contains(bingoNumber))
                {
                    continue;
                }
                else
                {
                    usedBingoNumbers.Add(bingoNumber);
                    return bingoNumber;
                }
            }
        }

        // Method to print all bingo plates in the game
        public void PrintGame()
        {
            foreach (BankoPlade plade in bankoPlader)
                plade.PrintPlade();
        }

        // Method to print a bingo plate by ID
        public void PrintById(int idNum)
        {
            foreach (BankoPlade plade in bankoPlader)
            {
                if (plade.id == idNum)
                    plade.PrintPlade();
            }
        }
    }

    // Class representing an individual bingo plate
    public class BankoPlade
    {
        // Properties of a bingo plate
        public string? name { get; set; } // Name of the plate
        public int? id { get; set; } // ID of the plate
        public int earnedRows { get; set; } = 0; // Number of rows earned (marked)
        public Row row1 { get; set; } = new(); // First row of the plate
        public Row row2 { get; set; } = new(); // Second row of the plate
        public Row row3 { get; set; } = new(); // Third row of the plate

        // Method to assign an ID to the plate
        public void AssignID(int idNum)
        {
            id = idNum;
        }

        // Method to get the name of the plate
        public string GetName()
        {
            return name;
        }

        // Method to check if the entire plate is completed
        public bool GottenEntirePlate()
        {
            if (row1.lineStatus == true && row2.lineStatus == true && row3.lineStatus == true)
                return true;
            else
                return false;
        }

        // Method to check for errors in the rows of the plate
        public void ErrorCheckingRows()
        {
            int rowOneCounter = row1.ExamineRow();
            int rowTwoCounter = row2.ExamineRow();
            int rowThreeCounter = row3.ExamineRow();

            if (rowOneCounter != 4)
                Console.WriteLine("Error at row 1");
            else if (rowTwoCounter != 4)
                Console.WriteLine("Error at row 2");
            else if (rowThreeCounter != 4)
                Console.WriteLine("Error at row 3");
            else
                Console.WriteLine("We good");
        }

        // Method to check a bingo number on the plate and update status
        public void CheckAndInsertPlateNumber(int bingoNumber, BankoPlade plade)
        {
            int bingoNumberIndex = bingoNumber / 10;
            if (bingoNumberIndex == 9)
                bingoNumberIndex--;

            if (!row1.lineStatus && row1.numbers[bingoNumberIndex] == bingoNumber)
                row1.InsertBingoNumberIntoRow(row1, bingoNumberIndex, 1, plade);
            else if (!row2.lineStatus && row2.numbers[bingoNumberIndex] == bingoNumber)
                row2.InsertBingoNumberIntoRow(row2, bingoNumberIndex, 2, plade);
            else if (!row3.lineStatus && row3.numbers[bingoNumberIndex] == bingoNumber)
                row3.InsertBingoNumberIntoRow(row3, bingoNumberIndex, 3, plade);
        }

        // Method to get a random bingo number
        public int GetBingoNumber()
        {
            Random r = new();
            return r.Next(1, 91);
        }

        // Method to create rows for the bingo plate
        public void CreateRows()
        {
            List<int> usedNumbers = new();
            row1.CreateRow(usedNumbers);
            row2.CreateRow(usedNumbers);
            row3.CreateRow(usedNumbers);
        }

        // Method to print the bingo plate
        public void PrintPlade()
        {
            Console.WriteLine($"{name}:");
            PrintRow(row1);
            PrintRow(row2);
            PrintRow(row3);
        }

        // Helper method to print a row
        private void PrintRow(Row row)
        {
            foreach (int elm in row.numbers)
            {
                string elementStr = elm.ToString();
                Console.Write(elementStr.PadRight(3));
            }
            Console.WriteLine();
        }

        // Method to set the name of the plate
        public void SetName(string passedName)
        {
            name = passedName;
        }
    }

    // Class representing a row in a bingo plate
    public class Row
    {
        // Properties of a row
        public int[] numbers { get; set; } = new int[9]; // Numbers in the row
        public int points { get; set; } = 0; // Points earned in the row
        public bool lineStatus { get; set; } = false; // Status of the line (completed or not)

        // Method to create a row with random numbers
        public void CreateRow(List<int> usedNumbers)
        {
            int[] indexes = getIndexies(); // Where the numbers will be placed.
            numbers = GetNumbers(indexes, usedNumbers); // What numbers will be placed.
        }

        // Method to examine the row for errors (number of empty spots)
        public int ExamineRow()
        {
            int zeroCounter = 0;
            foreach (int number in numbers)
            {
                if (number == 0)
                {
                    zeroCounter++;
                }
            }
            return zeroCounter;
        }

        // Method to insert a bingo number into the row
        public void InsertBingoNumberIntoRow(Row row, int bingoNumberIndex, int rowNum, BankoPlade plade)
        {
            row.numbers[bingoNumberIndex] = 99;
            row.points++;
            if (row.points == 5)
            {
                row.lineStatus = true;
                row.points++;
                plade.earnedRows++;
                if (plade.earnedRows < 3)
                    Console.WriteLine($"You got row {rowNum}!");
            }
            //Console.WriteLine("Row1" + Row1.points);
            //Console.WriteLine(bingoNumber);
            //PrintPlade();

        }

        // Helper method to get numbers for the row
        private int[] GetNumbers(int[] indexes, List<int> usedNumbers)
        {
            int number;
            Random r = new();
            int[] row = new int[9];

            foreach (int index in indexes)
            {
                do
                {
                    // For the last column of a Bingo Board, we use different code, because we also need to include 90.
                    if (index == 8)
                    {
                        int adder = 10 * index;
                        number = r.Next(1, 11) + adder;
                    }
                    else
                    {
                        int adder = 10 * index;
                        number = r.Next(1, 10) + adder;
                    }

                }
                while (CheckDuplicateNumber(usedNumbers, number));

                usedNumbers.Add(number);
                row[index] = number;
            }

            return row;
        }

        // Helper method to check for duplicate numbers
        private bool CheckDuplicateNumber(List<int> usedNumbers, int number)
        {
            if (usedNumbers.Contains(number))
                return true;
            else
                return false;
        }

        // Helper method to get indexes for number placement in the row
        private int[] getIndexies()
        {
            int[] indexes = new int[5]; // Indexes where our values will be placed.
            List<int> availabileIndexes = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // List, so that we can change the size.

            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                int j = r.Next(0, availabileIndexes.Count);
                indexes[i] = availabileIndexes[j];
                availabileIndexes.RemoveAt(j);
            }

            return indexes;
        }
    }

    static void Main()
    {
        AlternativeBingo test = new();
        test.StartGame();
    }
}