using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.IO;

class Program
{
    public class BingoBankoSpil
    {
        BankoPlade[] bankoPlader;
        readonly List<int> usedBingoNumbers = new();

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

        //public void ExportPlatesToJSON()
        //{
        //    JsonObject[] jsons = new JsonObject[bankoPlader.Length];
        //    for (int i = 0; i < bankoPlader.Length; i++)
        //    {
        //        JsonObject tempJson = new JsonObject();
        //        tempJson.plade = bankoPlader[i];
        //        jsons[i] = tempJson;
        //    }

        //    string jsonString = JsonSerializer.Serialize(jsons, new JsonSerializerOptions { WriteIndented = true });
        //    File.WriteAllText("jsonfile.json", jsonString);
        //}

        public void ErrorCheckingPlates()
        {
            foreach (var plade in bankoPlader)
                plade.ErrorCheckingRows();
        }

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
                    plade.CheckPlateNumber(bingoNumber);

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

        public void PrintGame()
        {
            foreach (BankoPlade plade in bankoPlader)
                plade.PrintPlade();
        }

        public void PrintById(int idNum)
        {
            foreach (BankoPlade plade in bankoPlader)
            {
                if (plade.id == idNum)
                    plade.PrintPlade();
            }
        }
    }
    public class BankoPlade
    {
        public string? name { get; set; }
        public int? id { get; set; }
        public int earnedRows { get; set; } = 0;
        public Row Row1 { get; set; } = new();
        public Row Row2 { get; set; } = new();
        public Row Row3 { get; set; } = new();

        public void AssignID(int idNum)
        {
            id = idNum;
        }

        public string GetName()
        {
            return name;
        }

        public bool GottenEntirePlate()
        {
            if (Row1.lineStatus == true && Row2.lineStatus == true && Row3.lineStatus == true)
                return true;
            else
                return false;
        }

        public void ErrorCheckingRows()
        {
            int rowOneCounter = Row1.ExamineRow();
            int rowTwoCounter = Row2.ExamineRow();
            int rowThreeCounter = Row3.ExamineRow();

            if (rowOneCounter != 4)
                Console.WriteLine("Error at row 1");
            else if (rowTwoCounter != 4)
                Console.WriteLine("Error at row 2");
            else if (rowThreeCounter != 4)
                Console.WriteLine("Error at row 3");
            else
                Console.WriteLine("We good");
        }


        public void CheckPlateNumber(int bingoNumber)
        {
            int bingoNumberIndex = bingoNumber / 10;
            if (bingoNumberIndex == 9)
                bingoNumberIndex--;

            if (!Row1.lineStatus && Row1.numbers[bingoNumberIndex] == bingoNumber)
                Row1.InsertBingoNumberIntoRow(Row1, bingoNumberIndex, 1, earnedRows: ref earnedRows);
            else if (!Row2.lineStatus && Row2.numbers[bingoNumberIndex] == bingoNumber)
                Row2.InsertBingoNumberIntoRow(Row2, bingoNumberIndex, 2, earnedRows: earnedRows);
            else if (!Row3.lineStatus && Row3.numbers[bingoNumberIndex] == bingoNumber)
                Row3.InsertBingoNumberIntoRow(Row3, bingoNumberIndex, 3, ref earnedRows);
        }

        public int GetBingoNumber()
        {
            Random r = new();
            return r.Next(1, 91);
        }

        public void CreateRows()
        {
            List<int> usedNumbers = new();
            Row1.CreateRow(usedNumbers);
            Row2.CreateRow(usedNumbers);
            Row3.CreateRow(usedNumbers);
        }

        public void PrintPlade()
        {
            Console.WriteLine($"{name}:");
            PrintRow(Row1);
            PrintRow(Row2);
            PrintRow(Row3);
        }

        private void PrintRow(Row row)
        {
            foreach (int elm in row.numbers)
            {
                string elementStr = elm.ToString();
                Console.Write(elementStr.PadRight(3));
            }
            Console.WriteLine();
        }

        public void SetName(string passedName)
        {
            name = passedName;
        }
    }

    public class Row
    {
        public int[] numbers { get; set; } = new int[9];
        public int points { get; set; } = 0;
        public bool lineStatus { get; set; } = false;

        // The full creation of a row.
        public void CreateRow(List<int> usedNumbers)
        {
            int[] indexes = getIndexies(); // Where the numbers will be placed.
            numbers = GetNumbers(indexes, usedNumbers); // What numbers will be placed.
        }
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

        public void InsertBingoNumberIntoRow(Row row, int bingoNumberIndex, int rowNum, ref int earnedRows)
        {
            row.numbers[bingoNumberIndex] = 99;
            row.points++;
            if (row.points == 5)
            {
                row.lineStatus = true;
                row.points++;
                earnedRows++;
                if (earnedRows < 3)
                    Console.WriteLine($"You got row {rowNum}!");
            }
            //Console.WriteLine("Row1" + Row1.points);
            //Console.WriteLine(bingoNumber);
            //PrintPlade();

        }

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

        private bool CheckDuplicateNumber(List<int> usedNumbers, int number)
        {
            if (usedNumbers.Contains(number))
                return true;
            else
                return false;
        }

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
        BingoBankoSpil spil = new(10);
        spil.ResolvePlates();
        spil.ExportPlatesToJSON("jsonData.json");

    }
}