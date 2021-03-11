using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleFileManager
{
    class Program : ClearWindow
    {
        enum Commands
        {
            copyFile,
            copyFolder,
            deleteFile,
            deleteFolder,
        }
        static void Main(string[] args)
        {
            var infoWindowHeight = 5;
            var commandLineHeight = 2;

            string startDirectory = "C:\\New" ;
            Settings();
            DrawWindows(infoWindowHeight,commandLineHeight);
            WriteFolderContents(startDirectory);
            WriteFolderInfo(startDirectory);
            SetCommandLine(commandLineHeight, startDirectory);
            var userCommand = ParseString(Console.ReadLine());
            startDirectory = userCommand[1];
            ClearFoldersWindow(1, Console.WindowHeight - 9);
            ClearFoldersWindow(Console.WindowHeight - 10, Console.WindowHeight - 15);
            WriteFolderContents(startDirectory);
            WriteFolderInfo(startDirectory);
            Console.ReadLine();
        }

        static void WriteFolderInfo(string path)
        {
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            Console.SetCursorPosition(1, Console.WindowHeight - 8);
            Console.Write(@"Info:");
            Console.SetCursorPosition(1, Console.WindowHeight - 7);
            Console.Write($"Creation: {Directory.GetCreationTime(path)}");
            Console.SetCursorPosition(1, Console.WindowHeight - 6);
            Console.Write($"Last Access: {Directory.GetLastAccessTime(path)}");
            Console.SetCursorPosition(1, Console.WindowHeight - 5);
            Console.Write($"Last Write: {Directory.GetLastWriteTime(path)}");
            Console.SetCursorPosition(1, Console.WindowHeight - 4);
            Console.Write($"Contents: {directories.Length} Folders and {files.Length} Files");
            SetCommandLine(2, path);
        }

        static void WriteFolderContents(string path)
        {
            Console.SetCursorPosition(1, 1);
            Console.Write($"{path} :");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string[] subDirectories = Directory.GetDirectories(path);
            string[] subFiles = Directory.GetFiles(path);
            for (int i = 0; i < subDirectories.Length; i++)
            {
                Console.SetCursorPosition(1, Console.CursorTop + 1);
                Console.Write(subDirectories[i].Substring(path.Length + 1));
                Console.SetCursorPosition(Console.WindowWidth - 10, Console.CursorTop);
                Console.Write("Folder");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < subFiles.Length; i++)
            {
                Console.SetCursorPosition(1, Console.CursorTop + 1);
                Console.Write(subFiles[i].Substring(path.Length + 1));
                Console.SetCursorPosition(Console.WindowWidth - 10, Console.CursorTop);
                Console.Write("File");
            }
            Console.ForegroundColor = ConsoleColor.White;
            SetCommandLine(2, path);
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

        static void DrawWindows(int infoWindowHeight, int commandLineHeight)
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

        static void SetCommandLine(int commandLineHeight, string path)
        {
            Console.SetCursorPosition(1, Console.WindowHeight - commandLineHeight);
            Console.Write($"{path}\\");
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
                }

            }
            else
            {
                commands = null;
                return commands;
            }
            return commands;
        } //разделение строки на подстроки-команды

        static void ClearFoldersWindow(int yPoint1, int yPoint2) //Метод для затирания текста в нужном окне
        {
            if (yPoint1 > 0 && yPoint2 > 0 && yPoint1 < yPoint2 && yPoint2 < Console.WindowHeight)
            {
                for (int i = yPoint1; i <= yPoint2; i++)
                {
                    Console.SetCursorPosition(1, i);
                    for (int j = 1; j < Console.WindowWidth; j++)
                    {
                        Console.Write(' ');
                    }
                }
            }
        }
    }
}
