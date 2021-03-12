using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConsoleFileManager
{
    class Settings
    {
        public int windowHeight { get; set; }
        public int windowWidth { get; set; }
        public int bufferHeight { get; set; }
        public int bufferWidth { get; set; }
        int infoWindowHeight { get; }
        int commandLineHeight { get; }
        public string lastPath { get; set; }
        public Settings()
        {
            windowHeight = 50;
            windowWidth = 160;

            bufferHeight = 250;
            bufferWidth = 160;

            infoWindowHeight = 5;
            commandLineHeight = 2;

            lastPath = "C:\\";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string settingsFile = "Settings.json";
            var settings = new Settings();

            CheckSettingsFile(settingsFile, ref settings);

            List<string> userCommand;
            var directory = settings.lastPath;

            DrawWindows();
            while (true)
            {   
                FolderContents(directory);
                FolderInfo(directory);
                SetCommandLine(2, directory);
                userCommand = ParseString(Console.ReadLine());
                switch (userCommand[0])
                {
                    case "copy":
                        break;
                    case "del":
                        break;
                    case "cd":
                        if (Directory.Exists(userCommand[1]))
                        {
                            FolderContents(userCommand[1]);
                            FolderInfo(userCommand[1]);
                            SetCommandLine(2, userCommand[1]);
                        }
                        else
                        {
                            Console.WriteLine("Такого пути не существует, попробуйте снова");
                        }
                        break;
                }
                if (userCommand[0] == "exit")
                {
                    settings.lastPath = directory;
                    SaveSettingsFile(settingsFile, settings);
                    break;
                }
                directory = userCommand[1];
            }
        }

        static void CheckSettingsFile(string settingsFile, ref Settings settings)
        {
            Console.Title = "Console File Manager";
            string path = Directory.GetCurrentDirectory();
            if (File.Exists(path + "\\" + settingsFile))
            {
                string jsonSettings = File.ReadAllText(path + "\\" + settingsFile);
                settings = JsonSerializer.Deserialize<Settings>(jsonSettings);
            }
            else
            {
                settings = new Settings();
            }
        }

        static void SaveSettingsFile(string settingsFile, Settings settings)
        {
            string path = Directory.GetCurrentDirectory();
            string jsonSettings = JsonSerializer.Serialize(settings);
            File.WriteAllText(path + "\\" + settingsFile, jsonSettings);
        }

        static void DrawWindows()
        {
            char topRight = '\u2555'; // ╕
            char topLeft = '\u2552'; // ╒           
            char downRight = '\u2518'; // ┘
            char downLeft = '\u2514'; // └
            char verticalLine = '\u2502'; // │
            char horizontalSingleLine = '\u2500'; // ─
            char horizontalDoubleLine = '\u2550'; // ═
            char middleDoubleRight = '\u2561'; // ╡
            char middleDoubleLeft = '\u255E'; // ╞
            char middleSingleRight = '\u2524'; // ┤
            char middleSingleLeft = '\u251C'; // ├

            var infoWindowHeight = 5;
            var commandLineHeight = 2;


            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 1, i);
                Console.Write(verticalLine);
                Console.SetCursorPosition(0, i);
                Console.Write(verticalLine);
                if (Console.CursorTop == 0)
                {
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.Write(horizontalDoubleLine);
                    }
                    Console.SetCursorPosition(2, Console.CursorTop);
                    Console.Write("Contents");
                }

                if ((Console.WindowHeight - Console.CursorTop - 1) == (infoWindowHeight + commandLineHeight))
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(middleDoubleLeft);
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.Write(horizontalDoubleLine);
                    }
                    Console.Write(middleDoubleRight);
                    Console.SetCursorPosition(2, Console.CursorTop);
                    Console.Write("Info");
                }
                if ((Console.WindowHeight - Console.CursorTop - 1) == commandLineHeight)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(middleSingleLeft);
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.Write(horizontalSingleLine);
                    }
                    Console.Write(middleSingleRight);
                    Console.SetCursorPosition(2, Console.CursorTop);
                    Console.Write("Command line");
                }
                if (Console.CursorTop == Console.WindowHeight - 1)
                {
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.Write(horizontalSingleLine);
                    }
                }
            }
            Console.SetCursorPosition(0, 0);
            Console.Write(topLeft);
            Console.SetCursorPosition(Console.WindowWidth - 1, 0);
            Console.Write(topRight);
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write(downLeft);
            Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
            Console.Write(downRight);
            return;
        }

        static void FolderInfo(string path)
        {
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            Console.SetCursorPosition(1, Console.WindowHeight - 7);
            Console.Write($"Creation: {Directory.GetCreationTime(path)}".PadRight(Console.WindowWidth - 1));
            Console.SetCursorPosition(1, Console.WindowHeight - 6);
            Console.Write($"Last Access: {Directory.GetLastAccessTime(path)}".PadRight(Console.WindowWidth - 1));
            Console.SetCursorPosition(1, Console.WindowHeight - 5);
            Console.Write($"Last Write: {Directory.GetLastWriteTime(path)}".PadRight(Console.WindowWidth - 1));
            Console.SetCursorPosition(1, Console.WindowHeight - 4);
            Console.Write($"Contents: {directories.Length} Folders and {files.Length} Files".PadRight(Console.WindowWidth - 1));
            SetCommandLine(2, path);
        }

        static void FolderContents(string path)
        {            
            Console.SetCursorPosition(1, 1);
            Console.Write(path.ToUpper().PadRight(Console.WindowWidth - 1));
            string[] subDirectories = Directory.GetDirectories(path);
            string[] subFiles = Directory.GetFiles(path);

            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < subDirectories.Length; i++)
            {
                Console.SetCursorPosition(1, Console.CursorTop + 1);
                Console.Write(subDirectories[i].Substring(path.Length).PadRight(Console.WindowWidth - 1));
                Console.SetCursorPosition(Console.WindowWidth - 10, Console.CursorTop);
                Console.Write("Folder");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < subFiles.Length; i++)
            {
                Console.SetCursorPosition(1, Console.CursorTop + 1);
                Console.Write(subFiles[i].Substring(path.Length).PadRight(Console.WindowWidth - 1));
                Console.SetCursorPosition(Console.WindowWidth - 10, Console.CursorTop);
                Console.Write("File");
            }
            Console.ForegroundColor = ConsoleColor.White;
            SetCommandLine(2, path);
        }
        
        static void Delete()
        {

        }
        

        static void SetCommandLine(int commandLineHeight, string path)
        {
            Console.SetCursorPosition(1, Console.WindowHeight - commandLineHeight);
            Console.WriteLine(" ".PadRight(Console.WindowWidth - 1));
            Console.SetCursorPosition(1, Console.WindowHeight - commandLineHeight);
            Console.Write(">>");
        }

        static List<string> ParseString(string userCommand)
        {
            var commands = new List<string>();
            string temp = null;
            if (userCommand != null)
            {
                for (int i = 0; i < userCommand.Length; i++) //проходим по строке от [0] индекса до первого пробела..
                {
                    if (userCommand[i] == ' ')
                    {
                        commands.Add(userCommand.Substring(0, i)); //.. и добавляем в Лист полученную строку.
                        i++; //пропускаем пробел.
                        for (int j = i; j < userCommand.Length; j++)  //продолжаем идти по строке начиная с i-го индекса,
                        {
                            if (userCommand[j] == ' ') //если после пути есть пробел, то добавляем путь в Лист и продолжаем идти по строке
                            {
                                commands.Add(userCommand.Substring(i, j - i));
                                j++;
                                break;
                            }
                            if (j + 1 == userCommand.Length) //если строка закончится на следующей итерации цикла, то добавляем полученный путь в Лист
                            {
                                commands.Add(userCommand.Substring(i, (j + 1) - i));
                                break;
                            }
                        }
                        break;
                    }
                    if (i + 1 == userCommand.Length) //если строка закончится на следующей итерации цикла, то добавляем полученную команду в Лист
                    {
                        commands.Add(userCommand);
                    }
                }

            }
            else
            {
                commands = null;
                return commands;
            }
            return commands;
        } //разделение строки на подстроки-команды

    }
}
