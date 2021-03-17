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
        public int pageLines { get; }
        public string settingsFile { get; }
        public int pageNumber { get; set; }
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
            settingsFile = "Settings.json";
            pageNumber = 1;
            pageLines = windowHeight - infoWindowHeight - commandLineHeight - 1; //вычисляем кол-во элементов на странице с учетом символа рамки
        }
    }
    class Program
    {
        // FileInfo мозг ебет
        // рекурсивное удаление каталогов
        // вывод каталогов в виде дерева
        // размер файлов и папок в итнформации
        static void Main(string[] args)
        {
            var settings = new Settings();

            CheckSettingsFile(settings.settingsFile, ref settings);

            List<string> userCommands;
            var currentDirectory = settings.lastPath;
            int pageNumber = settings.pageNumber;
            string pathFrom;
            string pathTo;

            DrawWindows(settings);
            Directories(currentDirectory, pageNumber, settings);
            Files(currentDirectory, pageNumber, settings);
            DirectoryInfo(currentDirectory);

            while (true)
            {
                SetCommandLine();                
                userCommands = ParseString(Console.ReadLine());
                if (userCommands.Count <= 1)
                {
                    SetCommandLine();
                    Help(settings);
                    Console.Write("Неправильная команда. Набор команд в окне информации. 'exit' -  для выхода (Нажмите любую клавишу)");
                    Console.ReadKey();
                    continue;
                }
                var command = userCommands[0];
                switch (command)
                {
                    case "test":
                        Directories("C:\\New\\1 — копия", 1, settings);
                        break;







                    case "copy":
                        pathFrom = userCommands[1];
                        if (Directory.Exists(pathFrom) && userCommands.Count == 3)
                        {
                            pathTo = userCommands[2];
                            if (!Directory.Exists(pathTo))
                            {
                                CopyDirectory(pathFrom, pathTo);
                            }
                            else
                            {
                                SetCommandLine();
                                Console.Write($"{pathTo} уже существует (Нажмите любую клавишу)");
                                Console.ReadKey();
                                break;
                            }
                        }
                        else if (File.Exists(pathFrom) && userCommands.Count == 3)
                        {
                            pathTo = userCommands[2];
                            if (!File.Exists(pathTo))
                            {
                                CopyFile(pathFrom, pathTo);
                            }
                            else
                            {
                                SetCommandLine();
                                Console.Write($"{pathTo} уже существует (Нажмите любую клавишу)");
                                Console.ReadKey();
                                break;
                            }
                        }
                        else
                        {
                            SetCommandLine();
                            Console.Write($"{pathFrom} не существует (Нажмите любую клавишу)");
                            Console.ReadKey();
                        }
                        Directories(currentDirectory, pageNumber, settings);
                        Files(currentDirectory, pageNumber, settings);
                        break;

                    case "del":
                        pathFrom = userCommands[1];
                        if (Directory.Exists(pathFrom))
                        {
                            DeleteDirectory(pathFrom);
                        }
                        else if (File.Exists(pathFrom))
                        {
                            DeleteFile(pathFrom);
                        }
                        else
                        {
                            SetCommandLine();
                            Console.Write($"{pathFrom} не существует (Нажмите любую клавишу)");
                            Console.ReadKey();
                        }
                        Directories(currentDirectory, pageNumber, settings);
                        Files(currentDirectory, pageNumber, settings);
                        break;

                    case "cd":
                        var newDirectory = userCommands[1];
                        if (Directory.Exists(newDirectory))
                        {
                            if (userCommands.Count == 4)
                            {
                                if (userCommands[2] == "-p")
                                {
                                    try
                                    {
                                        pageNumber = Convert.ToInt32(userCommands[3]);
                                    }
                                    catch (Exception)
                                    {
                                        SetCommandLine();
                                        Console.Write("Неверный формат номера страницы");
                                        Console.ReadKey();
                                        break;
                                    }
                                    currentDirectory = newDirectory;
                                    Directories(currentDirectory, pageNumber, settings);
                                    Files(currentDirectory, pageNumber, settings);
                                    DirectoryInfo(currentDirectory);
                                    break;
                                }
                                else
                                {
                                    SetCommandLine();
                                    Console.Write("Для команды 'cd' можно использовать только -р аттрибут");
                                    Console.ReadKey();
                                    break;
                                }
                            }
                            currentDirectory = newDirectory;
                            Directories(currentDirectory, pageNumber, settings);
                            Files(currentDirectory, pageNumber, settings);
                            DirectoryInfo(currentDirectory);
                        }
                        else if (Directory.Exists(Path.Combine(currentDirectory,newDirectory)))
                        {
                            if (userCommands.Count == 4)
                            {
                                if (userCommands[2] == "-p")
                                {
                                    try
                                    {
                                        pageNumber = Convert.ToInt32(userCommands[3]);
                                    }
                                    catch (Exception)
                                    {
                                        SetCommandLine();
                                        Console.Write("Неверный формат номера страницы");
                                        Console.ReadKey();
                                        break;
                                    }
                                    currentDirectory = Path.Combine(currentDirectory, newDirectory);
                                    Directories(currentDirectory, pageNumber, settings);
                                    Files(currentDirectory, pageNumber, settings);
                                    DirectoryInfo(currentDirectory);
                                    break;
                                }
                                else
                                {
                                    SetCommandLine();
                                    Console.Write("Для команды 'cd' можно использовать только -р аттрибут");
                                    Console.ReadKey();
                                    break;
                                }
                            }
                            currentDirectory = Path.Combine(currentDirectory, newDirectory);
                            Directories(currentDirectory, pageNumber, settings);
                            Files(currentDirectory, pageNumber, settings);
                            DirectoryInfo(currentDirectory);
                        }
                        else 
                        { 
                            SetCommandLine();
                            Console.Write($"{newDirectory} не существует, попробуйте снова (Нажмите любую клавишу)");
                            Console.ReadKey();
                        }
                        break;

                    case "page":
                        if (userCommands.Count == 3)
                        {
                            try
                            {
                                pageNumber = Convert.ToInt32(userCommands[2]);
                            }
                            catch (Exception)
                            {
                                SetCommandLine();
                                Console.Write("Неверный формат номера страницы (Нажмите любую клавишу)");
                                Console.ReadKey();
                                break;
                            }
                            if (userCommands[1] == "-f")
                            {                                
                                Files(currentDirectory, pageNumber, settings);
                            }
                            else if (userCommands[1] == "-d")
                            {
                                Directories(currentDirectory, pageNumber, settings);
                            }
                        }
                        else
                        {
                            try
                            {
                                pageNumber = Convert.ToInt32(userCommands[1]);
                            }
                            catch (Exception)
                            {
                                SetCommandLine();
                                Console.Write("Неверный формат номера страницы (Нажмите любую клавишу)");
                                Console.ReadKey();
                                break;
                            }
                            Directories(currentDirectory, pageNumber, settings);
                            Files(currentDirectory, pageNumber, settings);
                        }
                        break;

                    case "info":
                        pathFrom = userCommands[1];
                        if (File.Exists(pathFrom))
                        {
                            FileInfo(pathFrom);
                        }
                        else
                        {
                            SetCommandLine();
                            Console.Write($"{pathFrom} не существует, попробуйте снова (Нажмите любую клавишу)");
                            Console.ReadKey();
                        }
                        break;

                    default:
                        break;
                }
                if (userCommands[0] == "exit")
                {
                    settings.lastPath = currentDirectory;
                    SaveSettingsFile(settings.settingsFile, settings);
                    break;
                }
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
            if (File.Exists(Path.Combine(path, settingsFile)))
            {
                try
                {
                    string jsonSettings = File.ReadAllText(Path.Combine(path, settingsFile));
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
                File.WriteAllText(Path.Combine(path, settingsFile), jsonSettings);
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
                    Console.Write("Directories");
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
                    Console.Write("Directory Info");
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
            var settings = new Settings();

            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - (settings.infoWindowHeight + settings.commandLineHeight));
            Console.Write($"File: {path}".PadRight(Console.WindowWidth / 2 - 3));
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - (settings.infoWindowHeight + settings.commandLineHeight) + 1);
            Console.Write($"Last Access: {File.GetLastAccessTime(path)} / ");
            Console.Write($"Last Write: {File.GetLastWriteTime(path)}".PadRight(Console.WindowWidth / 2 - 3));
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - (settings.infoWindowHeight + settings.commandLineHeight) + 2);
            Console.Write($"Creation: {File.GetCreationTime(path)} / ");
            Console.Write($"Size: {File.GetCreationTime(path)} / ".PadRight(Console.WindowWidth / 2 - 3));
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.WindowHeight - (settings.infoWindowHeight + settings.commandLineHeight) + 3);
            Console.Write($"Attributes: {File.GetAttributes(path)}".PadRight(Console.WindowWidth / 2 - 3));
            SetCommandLine();
        } //вывод информации о файле

        static void DirectoryInfo(string path)
        {
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            var settings = new Settings();

            Console.SetCursorPosition(1, Console.WindowHeight - 7);
            Console.Write($"Creation: {Directory.GetCreationTime(path)}".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, Console.WindowHeight - 6);
            Console.Write($"Last Access: {Directory.GetLastAccessTime(path)}".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, Console.WindowHeight - 5);
            Console.Write($"Last Write: {Directory.GetLastWriteTime(path)}".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, Console.WindowHeight - 4);
            Console.Write($"Contents: {directories.Length} Folders and {files.Length} Files".PadRight(Console.WindowWidth / 2 - 2));
            SetCommandLine();
        } //вывод информации о каталоге

        static void Files(string path, int pageNumber, Settings settings)  //вывод файлов
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 + 1, 1); 
            Console.Write(path.ToUpper().PadRight(Console.WindowWidth / 2 - 3));

            string[] files = Directory.GetFiles(path);
            var pages = GetPagesNumber(settings.pageLines, files.Length);
            if (pageNumber <= 0 || pageNumber > pages)
            {
                SetCommandLine();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            if (files.Length < settings.pageLines * pageNumber - 2)
            {
                for (int i = (settings.pageLines * pageNumber) - settings.pageLines; i < files.Length; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.CursorTop + 1);
                    Console.Write(files[i].Substring(path.Length).PadRight(Console.WindowWidth / 2 - 3));
                }
                for (int i = files.Length; i < Console.WindowHeight - 10; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.CursorTop + 1);
                    Console.Write(" ".PadRight(Console.WindowWidth / 2 - 3));
                }
            }
            else
            {
                for (int i = (settings.pageLines * pageNumber) - settings.pageLines; i < settings.pageLines * pageNumber - 2; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2 + 1, Console.CursorTop + 1);
                    Console.Write(files[i].Substring(path.Length).PadRight(Console.WindowWidth / 2 - 3));
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth / 4 * 3, Console.WindowHeight - 8);
            Console.Write($"Page {pageNumber} of {pages}");
            SetCommandLine();
        }

        static void Directories(string path, int pageNumber, Settings settings) //вывод каталогов
        {
            Console.SetCursorPosition(1, 1);
            Console.Write(path.ToUpper().PadRight(Console.WindowWidth / 2 - 2));

            string[] directories = Directory.GetDirectories(path);
            var pages = GetPagesNumber(settings.pageLines, directories.Length);
            if (pageNumber <= 0 || pageNumber > pages)
            {
                SetCommandLine();
                return;
            }


            Console.ForegroundColor = ConsoleColor.Yellow;
            if (directories.Length < settings.pageLines * pageNumber - 2)
            {
                for (int i = (settings.pageLines * pageNumber) - settings.pageLines; i < directories.Length; i++)
                {
                    Console.SetCursorPosition(1, Console.CursorTop + 1);
                    Console.Write(directories[i].Substring(path.Length).PadRight(Console.WindowWidth / 2 - 2));

                }
                for (int i = directories.Length; i < Console.WindowHeight - 10; i++)
                {
                    Console.SetCursorPosition(1, Console.CursorTop + 1);
                    Console.Write(" ".PadRight(Console.WindowWidth / 2 - 3));
                }
            }
            else
            {
                for (int i = (settings.pageLines * pageNumber) - settings.pageLines; i < settings.pageLines * pageNumber - 2; i++)
                {
                    Console.SetCursorPosition(1, Console.CursorTop + 1);
                    Console.Write(directories[i].Substring(path.Length).PadRight(Console.WindowWidth / 2 - 2));
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth / 4, Console.WindowHeight - 8);
            Console.Write($"Page {pageNumber} of {pages}");
            SetCommandLine();
        }

        static void DeleteDirectory(string path) //удаление каталога
        {
            try
            {
                Directory.Delete(path, true);
                SetCommandLine();
                Console.Write("Удаление успешно");
            }
            catch (Exception e)
            {
                SetCommandLine();
                Console.Write($"Ошибка при удалении каталога: {path} {e.Message}");
                Console.ReadKey();
            }
        }

        static void DeleteFile(string path) //удаление файла
        {
            try
            {
                File.Delete(path);
                Console.Write("Удаление успешно");
            }
            catch (Exception e)
            {
                SetCommandLine();
                Console.Write($"Ошибка при удалении файла: {path} {e.Message}");
                Console.ReadKey();
            }
        }        

        static void CopyDirectory(string pathFrom, string pathTo)
        {
            DirectoryInfo dir = new DirectoryInfo(pathFrom);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            Directory.CreateDirectory(pathTo);
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(pathTo, file.Name);
                try
                {
                    file.CopyTo(tempPath, false);
                }
                catch (Exception)
                {
                    SetCommandLine();
                    Console.Write($"Ошибка при копировании файла {file.Name} (Нажмите любую клавишу)");
                    Console.ReadKey();
                }
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(pathTo, subdir.Name);
                try
                {
                    CopyDirectory(subdir.FullName, tempPath);
                }
                catch (Exception)
                {
                    SetCommandLine();
                    Console.Write($"Ошибка при копировании директории {subdir.FullName} (Нажмите любую клавишу)");
                    Console.ReadKey();
                }
            }
        } //копирование директории

        static void CopyFile(string pathFrom, string pathTo)
        {
            try
            {
                File.Copy(pathFrom, pathTo);
                SetCommandLine();
                Console.Write("Копирование успешно");
            }
            catch (Exception e)
            {
                SetCommandLine();
                Console.Write($"При копировании произошла ошибка");
                Console.ReadKey();
            }
        } //копирование файла

        static void Help(Settings settings)
        {
            var height = Console.WindowHeight - settings.infoWindowHeight + 1;
            Console.SetCursorPosition(1, height);
            Console.Write($"'cd ДИРЕКТОРИЯ' - переход в директорию");
            Console.CursorLeft = Console.WindowWidth / 2;
            Console.Write($"'page НОМЕР_СТРАНИЦЫ' - переход по страницам".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, --height);
            Console.Write($"'-p НОМЕР_СТРАНИЦЫ' - переход в директорию на страницу");
            Console.CursorLeft = Console.WindowWidth / 2;
            Console.Write($"'-d' - переход на нужную страницу директорий".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, --height);
            Console.Write($"'del ДИРЕКТОРИЯ' - удаление директории или файла");
            Console.CursorLeft = Console.WindowWidth / 2;
            Console.Write($"'-f' - переход на нужную страницу файлов".PadRight(Console.WindowWidth / 2 - 2));
            Console.SetCursorPosition(1, --height);
            Console.Write($"'info ПУТЬ_К_ФАЙЛУ' - информация о файле");
            Console.CursorLeft = Console.WindowWidth / 2;
            Console.Write($"'copy КОПИРУЕМАЯ_ДИРЕКТОРИЯ КОНЕЧНАЯ_ДИРЕКТОРИЯ' - копирование".PadRight(Console.WindowWidth / 2 - 2));
            SetCommandLine();
        } //вывод помощи

        static List<string> ParseString(string userCommand)
        {
            var commands = new List<string>();
            string tempString = null;
            if (userCommand != null)
            {
                for (int i = 0; i < userCommand.Length; i++) //проходим по строке от [0] индекса до первого пробела..
                {
                    if (userCommand[i] == ' ')
                    {
                        for (int t = 0; t < i; t++)
                        {
                            tempString = tempString + userCommand[t];
                        }
                        commands.Add(tempString); //.. и добавляем в Лист полученную строку.
                        tempString = null;
                        i++; //пропускаем пробел.
                        int tempIndex = i;
                        if (commands[0] == "copy")
                        {
                            for (int k = i + 2; k < userCommand.Length; k++) //пропускаем первое двоеточие
                            {
                                if (userCommand[k] == ':')
                                {
                                    for (int h = i; h < k - 2; h++)
                                    {
                                        tempString = tempString + userCommand[h];
                                    }
                                    commands.Add(tempString); //.. добавляем в Лист полученный путь.
                                    tempString = null;
                                    for (int l = k - 1; l < userCommand.Length; l++)
                                    {
                                        tempString = tempString + userCommand[l];
                                    }
                                    commands.Add(tempString); //.. добавляем в Лист полученный путь.
                                }
                            }
                            return commands;
                        }

                        if (commands[0] == "info" || commands[0] == "del")
                        {
                            for (int k = i; k < userCommand.Length; k++)
                            {
                                tempString = tempString + userCommand[k];
                            }
                            commands.Add(tempString); //.. добавляем в Лист оставшийся путь.
                            return commands;
                        }

                        if (commands[0] == "cd")
                        {
                            for (int k = i; k < userCommand.Length; k++)
                            {
                                if (userCommand[k] == '-')
                                {
                                    for (int j = i; j < k - 1; j++)
                                    {
                                        tempString = tempString + userCommand[j];
                                    }
                                    commands.Add(tempString); //добавляем в лист путь
                                    tempString = null;
                                    tempString = tempString + userCommand[k];
                                    tempString = tempString + userCommand[k + 1]; //добавляем аргумент -p
                                    commands.Add(tempString);
                                    tempString = null;
                                    tempString = tempString + userCommand[k + 3]; //добавляем номер страницы
                                    commands.Add(tempString);
                                    return commands;
                                }
                                if (k + 1 == userCommand.Length)
                                {
                                    for (int j = i; j < userCommand.Length; j++)
                                    {
                                        tempString = tempString + userCommand[j];
                                    }
                                    commands.Add(tempString);
                                    return commands;
                                }
                            }
                        }

                        for (int j = i; j < userCommand.Length; j++)  //продолжаем идти по строке начиная с i-го индекса,
                        {
                            if (userCommand[j] == ' ') //если после пути есть пробел, то добавляем путь в Лист и продолжаем идти по строке
                            {
                                commands.Add(userCommand.Substring(tempIndex, j - tempIndex));
                                tempIndex = ++j; //пропускаем пробел
                            }
                            if (j + 1 == userCommand.Length) //если строка закончится на следующей итерации цикла, то добавляем полученный путь в Лист
                            {
                                commands.Add(userCommand.Substring(tempIndex, (j + 1) - tempIndex));
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
