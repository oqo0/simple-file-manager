using System;
using System.IO;

namespace FileManager
{
    class FileManager
    {
        public static void Main()
        {
            // /home/oqpin
            string[] fullPath = {"/", "/home", "/oqpin", "/Pictures/"};
            
            ShowDirectoriesTree(fullPath, 0);
            ShowFiles(fullPath, 1);

            while (true)
            {
                // ввод команды
                Console.Write(String.Join(" ", fullPath) + " > ");
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
                string outputString = String.Empty;
                
                // отступ при открытии папки
                for (int j = 0; j <= recursionDepth; j++)
                {
                    outputString += "├──";
                }
                
                // информация о папке
                DirectoryInfo dir = new DirectoryInfo(directories[i]);
                
                Console.WriteLine($"{outputString} {directories[i]}");


                if (recursionDepth + 1 < fullPath.Length && Convert.ToString(directories[i]) == fullPath[recursionDepth + 1])
                {
                    ShowDirectoriesTree(fullPath, recursionDepth + 1);
                }
            }
        }
        static void ShowFiles(string[] fullPath, int page)
        {
            // Постраничный вывод файлов папки
            /*
            ───────────────────────────────────────
            File.txt
            File.txt
            File.txt
            File.txt
            File.txt
            ──── 12 ───────────────────────────────────
            */

            // линия во всю ширину консоли
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("─");
            }
            Console.WriteLine();
            
            string[] files = Directory.GetFiles(String.Concat(fullPath));

            const int pageSize = 5;

            // постраничный вывод элементов
            for (int e = 1; e <= pageSize; e++)
            {
                
            }

            // вывод файлов и их свойств
            for (int i = 0; i < files.Length; i += 1)
            {
                FileInfo file = new FileInfo(files[i]);
                Console.WriteLine($"{files[i], 45} {file.Length, 10} bytes {file.CreationTime.Date, 20}");
            }
            
            // линия во всю ширину консоли с номером страницы
            string pageMessage = $"─── page: {page} ";
            Console.Write(pageMessage);
            for (int i = 0; i < Console.WindowWidth - pageMessage.Length; i++)
            {
                Console.Write("─");
            }
            Console.WriteLine();
        }
    }
}