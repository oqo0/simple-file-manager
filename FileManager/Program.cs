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

            while (true)
            {
                // ввод команды
                string? command = Console.ReadLine();
                CommandHandler(command.Split(' '));
            }
        }

        static void CommandHandler(string[] commandArgs)
        {
            switch (commandArgs[0].ToLower())
            {
                // копирование папки
                // copy [папка] [путь]
                case "copydir":
                {
                    string sourcePath = @commandArgs[1];
                    string targetPath = @commandArgs[2];

                    try
                    {
                        if (Directory.Exists(sourcePath))
                        {
                            // если папка не существует она будет создана
                            Directory.CreateDirectory(targetPath);

                            string[] files = Directory.GetFiles(sourcePath);

                            // Копирование файлов и перезаписывание если они существуют
                            for (int i = 0; i < files.Length; i++)
                            {
                                string fileName = Path.GetFileName(files[i]);
                                string destFile = Path.Combine(targetPath, fileName);

                                File.Copy(files[i], destFile, true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                    break;
                }

                // копирование файла
                // copy [файл] [путь]
                case "copyfile":
                {
                    string sourceFile = @commandArgs[1];
                    string targetPath = @commandArgs[2];

                    try
                    {
                        // Копирование файла и перезаписывание если он существует
                        File.Copy(sourceFile, targetPath, true);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                    break;
                }

                // удаление
                // rm [файл]
                case "rm":
                {
                    string filePath = @commandArgs[1];

                    try
                    {
                        // удаление каталога и его содержимого
                        File.Delete(filePath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                    break;
                }
                
                // очистка консоли для удобства
                // clear
                case "clear":
                {
                    Console.Clear();
                    break;
                }
                
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
            ───────────────────────────────────────
            File.txt    File.txt    File.txt
            File.txt    File.txt    File.txt
            */
            
            Console.WriteLine("───────────────────────────────────────");
            
            string[] files = Directory.GetFiles(String.Concat(fullPath));

            const int pageSize = 5;

            // постраничный вывод элементов
            for (int e = 1; e <= pageSize; e++)
            {
                
            }
            
            const int colonLength = 35;
            const int colonAmount = 3;
            
            // вывод файлов в виде колонн
            for (int i = 0; i < files.Length; i += colonAmount)
            {
                // остановка перечисления после выхода за предел массива
                try
                {
                    for (int k = 0; k < colonAmount; k++)
                    {
                        Console.WriteLine($"{files[i + k], colonLength}");
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
        }
    }
}