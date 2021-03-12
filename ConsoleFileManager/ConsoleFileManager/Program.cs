using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ConsoleFileManager
{
    class Settings
    {
        public int windowHeight { get; }
        public int windowWidth { get; }
        public int bufferHeight { get; }
        public int bufferWidth { get; }
        public int infoWindowHeight { get; }
        public int commandLineHeight { get; }
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

            DrawWindows(settings);
            Folders(directory, settings);
            Files(directory, settings);
            FolderInfo(directory);
            FileInfo("C:\\EasyAntiCheat_Setup.exe");

            while (true)
            {
                SetCommandLine();
                userCommand = ParseString(Console.ReadLine());
                switch (userCommand[0])
                {
                    case "copy":
                        break;
                    case "del":
                        if (Directory.Exists(userCommand[1]))
                        {
                            DeleteFolder(userCommand[1]);
                        }
                        else if (File.Exists(userCommand[1]))
                        {
                            DeleteFile(userCommand[1]);
                        }
                        else
                        {
                            SetCommandLine();
                            Console.Write("Данной директории не существует (Нажмите любую клавишу)");
                            Console.ReadKey();
                        }
                        break;
                    case "cd":
                        if (Directory.Exists(userCommand[1]))
                        {
                            Folders(userCommand[1], settings);
                            Files(userCommand[1], settings);
                            FolderInfo(userCommand[1]);
                        }
                        else
                        {
                            SetCommandLine();
                            Console.WriteLine("Такого пути не существует, попробуйте снова (Нажмите любую клавишу)");
                            Console.ReadKey();
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


        static void SetCommandLine()
        {
            var settings = new Settings();
            Console.SetCursorPosition(1, Console.WindowHeight - settings.commandLineHeight);
            Console.WriteLine(" ".PadRight(Console.WindowWidth - 2));
            Console.SetCursorPosition(1, Console.WindowHeight - settings.commandLineHeight);
            Console.Write(">>");
        } //выставляем курсор в командную строку

        static void CheckSettingsFile(string settingsFile, ref Settings settings)
        {
            Console.Title = "Console File Manager";
            string path = Directory.GetCurrentDirectory();
            if (File.Exists(path + "\\" + settingsFile))
            {
                try
                {
                    string jsonSettings = File.ReadAllText(path + "\\" + settingsFile);
                    settings = JsonSerializer.Deserialize<Settings>(jsonSettings);
                    Console.SetWindowSize(settings.windowWidth, settings.windowHeight);
                    Console.SetBufferSize(settings.bufferWidth, settings.bufferHeight);
                }
                catch
                {
                    Console.Write("Ошибка при чтении настроек! Настройки сброшены");
                    settings = new Settings();
                }
            }
            else
            {
                settings = new Settings();
            }
        } //проверяем наличие файла настроек. При его наличии считываем настройки, иначе используем стандартные

        static void SaveSettingsFile(string settingsFile, Settings settings)
        {
            string path = Directory.GetCurrentDirectory();
            string jsonSettings = JsonSerializer.Serialize(settings);
            try
            {
                File.WriteAllText(path + "\\" + settingsFile, jsonSettings);
            }
            catch (Exception)
            {
                SetCommandLine();
                Console.Write("Ошибка при записи файла настроек!");
                Console.ReadKey();
            }
        } //сохраняем настройки в JSON файле 

        static int GetPagesNumber(int pageLines, int amountElements)
        {
            var count = 0;
            if (amountElements >= 0)
            {
                count++;
                amountElements = amountElements - pageLines;
                count = GetPagesNumber(pageLines, amountElements) + count;
            }            
            return count;
        } //получаем количество страниц

        static void DrawWindows(Settings settings)
        {
            char topRight = '\u2555'; // ╕
            char topLeft = '\u2552'; // ╒
            char topMiddle = '\u2564'; // ╤
            char downRight = '\u2518'; // ┘
            char downLeft = '\u2514'; // └
            char downMiddle = '\u2534'; // ┴
            char verticalLine = '\u2502'; // │
            char horizontalSingleLine = '\u2500'; // ─
            char horizontalDoubleLine = '\u2550'; // ═
            char middleDoubleRight = '\u2561'; // ╡
            char middleDoubleLeft = '\u255E'; // ╞
            char middleDoubleBoth = '\u256A'; // ╪
            char middleSingleRight = '\u2524'; // ┤
            char middleSingleLeft = '\u251C'; // ├


            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < Console.WindowHeight; i++) //цикл прохода по все высоте окна
            {
                Console.SetCursorPosition(Console.WindowWidth - 1, i); //рисуем боковые рамки в конце окна
                Console.Write(verticalLine);
                Console.SetCursorPosition(0, i);//рисуем боковые рамки в начале окна
                Console.Write(verticalLine);
                if (Console.CursorTop < (Console.WindowHeight - settings.commandLineHeight) && Console.CursorTop != 0)
                {
                    Console.SetCursorPosition((Console.WindowWidth / 2) - 1, Console.CursorTop);
                    Console.Write(verticalLine);
                }
                if (Console.CursorTop == 0) //рисуем верхнюю рамку
                {
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.Write(horizontalDoubleLine);
                    }
                    Console.SetCursorPosition((Console.WindowWidth / 2) - 1, Console.CursorTop);
                    Console.Write(topMiddle);
                    Console.SetCursorPosition(2, Console.CursorTop);
                    Console.Write("Folders");
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.CursorTop);
                    Console.Write("Files");
                }

                if ((Console.WindowHeight - Console.CursorTop - 1) == (settings.infoWindowHeight + settings.commandLineHeight))  //рисуем верхнюю границу окна инфрмации
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(middleDoubleLeft);
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.Write(horizontalDoubleLine);
                    }
                    Console.Write(middleDoubleRight);
                    Console.SetCursorPosition((Console.WindowWidth / 2) - 1, Console.CursorTop);
                    Console.Write(middleDoubleBoth);
                    Console.SetCursorPosition(2, Console.CursorTop);
                    Console.Write("Folder Info");
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.CursorTop);
                    Console.Write("Files Info");
                }
                if ((Console.WindowHeight - Console.CursorTop - 1) == settings.commandLineHeight) //рисуем верхнюю границу окна командной строки
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(middleSingleLeft);
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.Write(horizontalSingleLine);
                    }
                    Console.Write(middleSingleRight);
                    Console.SetCursorPosition((Console.WindowWidth / 2) - 1, Console.CursorTop);
                    Console.Write(downMiddle);
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
        } //рисуем рамку с окнами

        static void FileInfo(string path)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - 7);
            Console.Write($"Creation: {File.GetCreationTime(path)}".PadRight(Console.WindowWidth / 2 - 3));
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - 6);
            Console.Write($"Last Access: {File.GetLastAccessTime(path)}".PadRight(Console.WindowWidth / 2 - 3));
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - 5);
            Console.Write($"Last Write: {File.GetLastWriteTime(path)}".PadRight(Console.WindowWidth / 2 - 3));
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - 4);
            Console.Write($"Attributes: {File.GetAttributes(path)}".PadRight(Console.WindowWidth / 2 - 3));
            SetCommandLine();
        } //выводим информацию о каталоге в окно информации

        static void FolderInfo(string path)
        {
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            Console.SetCursorPosition(1, Console.WindowHeight - 7);
            Console.Write($"Creation: {Directory.GetCreationTime(path)}".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, Console.WindowHeight - 6);
            Console.Write($"Last Access: {Directory.GetLastAccessTime(path)}".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, Console.WindowHeight - 5);
            Console.Write($"Last Write: {Directory.GetLastWriteTime(path)}".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, Console.WindowHeight - 4);
            Console.Write($"Contents: {directories.Length} Folders and {files.Length} Files".PadRight(Console.WindowWidth / 2 - 2));
            SetCommandLine();
        } //выводим информацию о каталоге в окно информации

        static void Files(string path, Settings settings)  //выводим файлы
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, 1);
            Console.Write(path.ToUpper().PadRight(Console.WindowWidth / 2 - 3));
            string[] subFiles = Directory.GetFiles(path);
            var pageLines = settings.windowHeight - settings.infoWindowHeight - settings.commandLineHeight - 1;
            var pages = GetPagesNumber(pageLines,subFiles.Length);

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < subFiles.Length; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.CursorTop + 1);
                Console.Write(subFiles[i].Substring(path.Length).PadRight(Console.WindowWidth / 2 - 3));
            }
            Console.ForegroundColor = ConsoleColor.White;
            SetCommandLine();

        }
        static void Folders(string path, Settings settings) //выводим каталоги
        {
            Console.SetCursorPosition(1, 1);
            Console.Write(path.ToUpper().PadRight(Console.WindowWidth / 2 - 2));
            string[] subDirectories = Directory.GetDirectories(path);
            var pageLines = settings.windowHeight - settings.infoWindowHeight - settings.commandLineHeight - 1;
            var pages = GetPagesNumber(pageLines, subDirectories.Length);
            


            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < subDirectories.Length; i++)
            {
                Console.SetCursorPosition(1, Console.CursorTop + 1);
                Console.Write(subDirectories[i].Substring(path.Length).PadRight(Console.WindowWidth / 2 - 2));
            }
            Console.ForegroundColor = ConsoleColor.White;
            SetCommandLine();
        }

        static void DeleteFolder(string path) //удаление каталог
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch
            {
                SetCommandLine();
                Console.Write($"Ошибка при удалении каталога: {path}");
                Console.ReadKey();
            }
        }

        static void DeleteFile(string path) //удаление файла
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                SetCommandLine();
                Console.Write($"Ошибка при удалении файла: {path}");
                Console.ReadKey();
            }
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
