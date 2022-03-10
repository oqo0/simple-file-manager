using System;
using System.IO;

namespace FileManager
{
    class FileManager
    {
        public static void Main()
        {
            string[] fullPath = {"/", "/bin"};
            
            ShowDirectoriesTree(fullPath, 0);
            ShowFiles(fullPath);
                
            Console.ReadLine();
        }

        static void CommandHandler(string[] commandArgs)
        {
            switch (commandArgs[0].ToLower())
            {
                //  копирование
                case "cp":
                    break;
                
                // удаление
                case "rm":
                    break;
                
                default:
                    Console.WriteLine("Неизвестная команда.");
                    break;
            }
        }
        static void ShowDirectoriesTree(string[] fullPath, int recursionDepth)
        {
            /*
            Вывод файлового дерева
            ├──dir
            ├──├──dir
            ├──├──├──dir
            
            0  1  3 - глубина рекурсии (recursionDepth)
            */
            
            string[] directories = Directory.GetDirectories(fullPath[recursionDepth]);
            
            for (int i = 0; i < directories.Length; i++)
            {
                // отступ при открытии папки
                for (int j = 0; j <= recursionDepth; j++)
                {
                    Console.Write("├──");
                }
                
                Console.WriteLine(directories[i]);
                
                if (recursionDepth + 1 < fullPath.Length && Convert.ToString(directories[i]) == fullPath[recursionDepth + 1])
                {
                    ShowDirectoriesTree(fullPath, recursionDepth + 1);
                }
            }
        }
        static void ShowFiles(string[] fullPath)
        {
            // Вывод файлов папки
            /*
            File.txt    File.txt    File.txt
            File.txt    File.txt    File.txt
            */
            
            string[] files = Directory.GetFiles(String.Concat(fullPath));
            
            for (int i = 0; i < files.Length; i += 3)
            {
                try
                {
                    Console.WriteLine($"{files[i],40} {files[i + 1],40} {files[i + 5],40}");
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
        }
    }
}