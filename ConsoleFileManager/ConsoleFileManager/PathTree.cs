using System;
using System.IO;

namespace ConsoleFileManager
{
    class PathTree
    {
        string directoryTreeTXT = @"C:\New\DirectoryTree.txt";
        public string GetUserPath
        {
            set
            {
                File.WriteAllText(directoryTreeTXT, "Дерево каталогов c рекурсией:\n");
                SaveDirectoryTreeRecusion(directoryTreeTXT, value);
                File.AppendAllText(directoryTreeTXT, "\nДерево файлов c рекурсией:\n");
                SaveFilesTreeRecusion(directoryTreeTXT, value);
                File.AppendAllText(directoryTreeTXT, "\nДерево каталогов без рекурсии:\n");
                SaveDirectoryTree(directoryTreeTXT, value);
                File.AppendAllText(directoryTreeTXT, "\nДерево файлов без рекурсии:\n");
                SaveFilesTree(directoryTreeTXT, value);
                Console.WriteLine($"Дерево каталогов и файлов созданы {directoryTreeTXT}");
            }
        }

        private void SaveTextToFile(string directoryTreeTXT, string text)
        {
            File.AppendAllText(directoryTreeTXT, text + "\n");
            return;
        }

        private void SaveDirectoryTreeRecusion(string directoryTreeTXT, string userPath)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(userPath);
            if (subdirectoryEntries.Length != 0)
                foreach (var subDirectory in subdirectoryEntries)
                {
                    SaveTextToFile(directoryTreeTXT, subDirectory);
                    SaveDirectoryTreeRecusion(directoryTreeTXT, subDirectory);
                }
            else
                return;
        }

        private void SaveFilesTreeRecusion(string directoryTreeTXT, string userPath)
        {
            string[] filesName = Directory.GetFiles(userPath);
            if (filesName.Length != 0)
                foreach (var fileName in filesName)
                    SaveTextToFile(directoryTreeTXT, fileName);
            string[] subdirectoryEntries = Directory.GetDirectories(userPath);
            if (subdirectoryEntries.Length != 0)
                foreach (string subdirectory in subdirectoryEntries)
                    SaveFilesTreeRecusion(directoryTreeTXT, subdirectory);
            else
                return;
        }

        private void SaveDirectoryTree(string directoryTreeTXT, string userPath)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(userPath, "*", SearchOption.AllDirectories);
            if (subdirectoryEntries.Length != 0)
                foreach (var subDirectory in subdirectoryEntries)
                    SaveTextToFile(directoryTreeTXT, subDirectory);
            else
                return;
        }

        private void SaveFilesTree(string directoryTreeTXT, string userPath)
        {
            string[] subdirectoryEntries = Directory.GetDirectories(userPath, "*", SearchOption.AllDirectories);
            if (subdirectoryEntries.Length != 0)
            {
                foreach (var subDirectory in subdirectoryEntries)
                {
                    string[] filesName = Directory.GetFiles(userPath);
                    foreach (var fileName in filesName)
                        SaveTextToFile(directoryTreeTXT, fileName);
                    userPath = subDirectory;
                }
            }
            else
                return;
        }

    }
}