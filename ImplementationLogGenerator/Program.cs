using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ImplementationLogGenerator
{
    // all commands begin with an !
    class Program
    {
        enum mode { End, Main, Log }; // end quits, main to swap between logs, Log to do stuff on a log
        static int currentMode; // mode of the program
        static string currentLog; // name of log being worked
        static DateTime startTime; // time that log mode was entered to determine time spent
        static void Main(string[] args)
        {
            // load into main mode to begin with
            currentMode = (int)mode.Main;
            PrintInColor("!help to get command data", ConsoleColor.Yellow);
            while (currentMode != (int)mode.End)
            {
                // get the user input
                PrintInColor("Input data: ", ConsoleColor.Cyan);

                string userInput = Console.ReadLine();
                string[] input = userInput.Split(' ');

                switch (currentMode)
                {
                    case (int)mode.Main:
                        PrintInColor("Main processing: ", ConsoleColor.Gray);
                        PrintInColor(input, ConsoleColor.Gray);
                        MainLoop(input);
                        break;

                    case (int)mode.Log:
                        PrintInColor("Log processing: ", ConsoleColor.Gray);
                        PrintInColor(input, ConsoleColor.Gray);
                        LogLoop(input);
                        break;

                    default:
                        PrintInColor("Not in a mode, something went wrong", ConsoleColor.Red);
                        break;
                }
            }
            PrintInColor("Closing program...", ConsoleColor.Red);
        }

        static void MakeFile(string fileName)
        {
            string path = fileName + ".txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine("Implementation Log:- " + fileName);
                    sw.WriteLine("");
                }
            }
        }

        // method to write an array to a file or a string
        static void WriteToFile(string fileName, string[] dataToWrite)
        {
            string path = fileName + ".txt";
            string content = "";
            // populate with the contents of array
            for (int i = 0; i < dataToWrite.Length; i++)
            {
                content += dataToWrite[i] + ' ';
            }

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                
                sw.WriteLine("> " + content);
                PrintInColor(content, ConsoleColor.DarkGreen);
                PrintInColor("written to: " + path, ConsoleColor.Green);
            }


        }
        static void WriteToFile(string fileName, string dataToWrite)
        {
            string path = fileName + ".txt";

            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(dataToWrite);
                PrintInColor(dataToWrite, ConsoleColor.DarkGreen);
                PrintInColor("written to: " + path, ConsoleColor.Green);
            }
        }

        // create log that can be worked on
        static void MainLoop(string[] input)
        {
            switch(input[0])
            {

                case "!log":
                    if(input.Length != 2) // check that log has been specified
                    {
                        PrintInColor("Must specify the log name", ConsoleColor.Red);
                    }
                    else
                    {
                        string path = input[1] + ".txt";
                        if(File.Exists(path)) // indicate whether creating or editing a log
                        {
                            PrintInColor("Now editing log: " + input[1], ConsoleColor.Green);
                        }
                        else
                        {
                            PrintInColor("Now creating log: " + input[1], ConsoleColor.Green);
                        }
                        currentMode = (int)mode.Log;
                        currentLog = input[1];
                        MakeFile(input[1]);
                    }
                    break;

                case "!quit":
                    currentMode = (int)mode.End;
                    break;

                case "!help":
                    string[] help = { "!log xxx open or create log xxx and enter log mode", "!quit to close the application"};
                    PrintInColor(help, ConsoleColor.Yellow);
                    break;

                default:
                    Console.WriteLine("No specified command");
                    break;
            }
        }

        static bool firstEntry = true; // use to set a timer the first time the log loop is entered
        static void LogLoop(string[] input)         // log loop is for doing work on a specific log
        {
            // on first entry to the loop set the time to then
            if (firstEntry)
            {
                // get the time loop is entered
                startTime = DateTime.Now;
                firstEntry = false;
                PrintInColor("Entering a non command will save it as a comment in the log", ConsoleColor.Yellow);
            }

            switch (input[0])
            {
                case "!start": // use to reset start time of a log
                    startTime = DateTime.Now;
                    break;

                case "!end": // save current log
                    SaveLog();
                    break;

                case "!read": // for reading the contents of the log back out
                    PrintLogToConsole(currentLog);
                    break;

                case "!main": // swap back to main mode and save current log
                    currentMode = (int)mode.Main;
                    firstEntry = true; // reset so that it will start a new time for the next log
                    SaveLog();
                    break;

                case "!quit": // close the program and save current log
                    currentMode = (int)mode.End;
                    SaveLog();
                    break;

                case "!help":
                    string[] help = { "!start to reset the start time of a log", "!end to save the duration of the log", "!read to print out the contents of the log",
                    "!main to be able to start a new log or change log", "!quit to close the application and save current log"};
                    PrintInColor(help, ConsoleColor.Yellow);
                    break;

                default: // if not a command then write it to the log
                    WriteToFile(currentLog, input);
                    break;
            }
        }
        // get time difference between log start time and end time and print out time elapsed, dash off next line to signify end of entry
        static void SaveLog()
        {
            DateTime endTime = DateTime.Now;
            WriteToFile(currentLog, "Start of session: " + startTime.ToString("F"));
            WriteToFile(currentLog, "End of session: " + endTime.ToString("F"));

            TimeSpan timeTaken = endTime.Subtract(startTime);
            float hours = timeTaken.Hours;
            float minutes = timeTaken.Minutes;
            float seconds = timeTaken.Seconds;
            WriteToFile(currentLog, "Elapsed time: " + hours + " hours " + minutes + " minutes and, " + seconds + " seconds.");
            WriteToFile(currentLog, "-----------------------------");
        }

        // prints a line in color, means i dont have to remember to change color back
        static void PrintInColor(string content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.White;
        }
        // mostly for printing the help commands but array version of the printincolour
        static void PrintInColor(string[] content, ConsoleColor color)
        {
            // turn array into string
            string arrayAsString = "";
            for(int i = 0; i < content.Length; i ++)
            {
                arrayAsString += content[i] + ' ';
            }

            // pass to string version
            PrintInColor(arrayAsString, color);
        }

        // print entirity of an implementation log to the console
        static void PrintLogToConsole(string fileName)
        {
            string path = fileName + ".txt";

            if (File.Exists(path))
            {
                string fileText = File.ReadAllText(path);
                PrintInColor(fileText, ConsoleColor.Green);
            }
            else
            {
                PrintInColor("File not found", ConsoleColor.Red);
            }
            
        }
    }
}
