using cbgb.Resources;
using cbgb.Sheet;
using cbgb.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace cbgb.UI
{
    class ConsoleUI
    {
        private bool keepRunning = true;
        private readonly string notValidOption = "That's not a valid option";

        public void RunUI()
        {
            while (keepRunning)
            {
                Menu();
            }
        }

        private void Menu()
        {
            Console.WriteLine("Select option.");
            Console.WriteLine("Load data [L]");
            Console.WriteLine("Export to Sheet [E]");
            Console.WriteLine("Quit [Q]");
            Option(Console.ReadLine(), "menu");
        }

        private void Option(string option, string source)
        {
            var validOption = true;
            switch (option.ToLower())
            {
                case "l":
                    if (source == "menu")
                    {
                        Console.Clear();
                        LoadData();
                        validOption = true;
                    }
                    else
                        validOption = false;
                    break;
                case "e":
                    if (source == "menu")
                    {
                        Console.Clear();
                        UploadToSheet();
                        validOption = true;
                    }
                    else
                        validOption = false;
                    break;
                case "q":
                    if (source != "menu")
                    {
                        Console.Clear();
                        validOption = false;
                    } else
                        Quit(); 
                    break;
                case "b":
                    Console.Clear();
                    if (source != "menu")
                        Menu();
                    break;
                default:
                    if (option.Length > 1 && source != "menu")
                    {
                        if (source == "loaddata")
                            if (PathUtil.IsValidPath(option))
                                DataManager.UpdateGuildBank(DataManager.ReadImportFile(option));
                            else
                                Error("Not a valid path.", source);

                    }
                        Error(notValidOption, source);
                    break;
            }
            if (!validOption)
                Error(notValidOption, source);
        }

        private void Quit()
        {
            keepRunning = false;
            Environment.Exit(0);
        }

        private void UploadToSheet()
        {
            Console.WriteLine("Back to Menu [B]");
            Console.WriteLine("This is the uploading page.");
            new SheetManager().UpdateSheet();
            Option(Console.ReadLine(), "uploaddata");
        }

        private void LoadData()
        {
            Console.WriteLine("Back to Menu [B]");
            Console.WriteLine("Please enter data path.");
            Option(Console.ReadLine(), "loaddata");
            Console.Clear();
        }

        private void Error(string message, string source)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
            switch (source.ToLower())
            {
                case "menu":
                    Menu();
                    break;
                case "loaddata":
                    LoadData();
                    break;
                case "uploaddata":
                    UploadToSheet();
                    break;
                default:
                    break;
            }
        }
    }
}
