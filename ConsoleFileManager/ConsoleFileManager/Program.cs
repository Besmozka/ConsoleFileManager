using System;

namespace ConsoleFileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            DrawWindows();
            Console.ReadKey();
        }

        #region DrawWindows()

        static void DrawWindows()
        {
            int infoWindowHeight = 5;
            int commandLineHeight = 1;
            Console.SetCursorPosition(0, 0);            
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                
                Console.SetCursorPosition(0, i);
                Console.Write("|");
                if (Console.CursorTop == 0)
                {
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.SetCursorPosition(j, Console.CursorTop);
                        Console.Write("_");
                    }
                }
                if ((Console.WindowHeight - Console.CursorTop - 1) == (infoWindowHeight + commandLineHeight))
                {
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.SetCursorPosition(j, Console.CursorTop);
                        Console.Write("_");
                    }
                }
                if ((Console.WindowHeight - Console.CursorTop - 1) == commandLineHeight)
                {
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.SetCursorPosition(j, Console.CursorTop);
                        Console.Write("_");
                    }
                }
                if (Console.CursorTop == Console.WindowHeight - 1)
                {
                    for (int j = 1; j < Console.WindowWidth - 1; j++)
                    {
                        Console.SetCursorPosition(j, Console.CursorTop);
                        Console.Write("_");
                    }
                }
                Console.SetCursorPosition(Console.WindowWidth - 1, i);
                Console.Write("|");
            }
            return;
        }
        #endregion

    }
}
