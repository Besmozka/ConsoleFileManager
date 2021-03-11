using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleFileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> userCommand;
            var startDirectory = "C:\\";

            Settings();
            DrawWindows();
            while (true)
            {   
                FolderContents(startDirectory);
                FolderInfo(startDirectory);
                SetCommandLine(2, startDirectory);
                userCommand = ParseString(Console.ReadLine());
                switch (userCommand[0])
                {
                    case "copy":
                        break;
                    case "del":
                        break;
                    case "cd":
                        FolderContents(userCommand[1]);
                        FolderInfo(userCommand[1]);
                        SetCommandLine(2, userCommand[1]);
                        break;
                    case "exit":
                        break;
                }
                if (userCommand[0] == "exit")
                {
                    break;
                }
                startDirectory = userCommand[1];
            }
        }

        static void Settings()
        {
            string lastPath = "";
            Console.Title = "Console File Manager";
            Console.SetWindowSize(160, 50);
            Console.SetBufferSize(160, 160);
            int infoWindowHeight = 5;
            int commandLineHeight = 2;
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
