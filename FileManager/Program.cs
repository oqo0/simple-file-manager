using System;
using System.Diagnostics;
using System.IO;

/*
- Просмотр файловой структуры
- Поддержка копирование файлов, каталогов
- Поддержка удаление файлов, каталогов
- Получение информации о размерах, системных атрибутов файла, каталога
- Вывод файловой структуры должен быть постраничным
- В конфигурационном файле должна быть настройка вывода количества элементов на страницу
- При выходе должно сохраняться, последнее состояние
- Должны быть комментарии в коде
- Должна быть документация к проекту в формате md
- Приложение должно обрабатывать непредвиденные ситуации (не падать)
- При успешном выполнение предыдущих пунктов – реализовать сохранение ошибки в текстовом файле
в каталоге errors/random_name_exception.txt
- При успешном выполнение предыдущих пунктов – реализовать движение по истории команд
(стрелочки вверх, вниз)
- Команды должны быть консольного типа, как, например, консоль в Unix или Windows.
Соответственно требуется создать парсер команд, который по минимуму использует стандартные
методы по строкам.
*/

namespace FileManager
{
    class FileManager
    {
        public static void Main()
        {
            // используемый путь
            string[] fullPath = {"/", "/home", "/oqpin", "/Downloads"};

            while (true)
            {
                ShowDirectoriesTree(fullPath, 0);
                ShowFiles(fullPath, 1);
                
                // ввод команды
                Console.Write(String.Join(" ", fullPath) + " > ");
                string? command = Console.ReadLine();
                CommandHandler(command.Split(' '));
                
                if (command.ToLower() == "end")
                {
                    return;
                }
            }
        }
        static void CommandHandler(string[] commandArgs)
        {
            Console.Clear();
            
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
            File.txt
            File.txt
            File.txt
            ──── page: 1/2 ───────────────────────────────────
            */
            
            const float pageSize = 5;
            
            string[] files = Directory.GetFiles(String.Concat(fullPath));
            double maxPage = Math.Ceiling(files.Length / pageSize);
            
            // страница не может быть больше макс. значения или меньше 1
            if (page > maxPage || page < 1)
            {
                Console.WriteLine("Индекс страницы имеет недопустимое значение.");
                return;
            }
            
            // линия с номером страницы
            Console.WriteLine($"\n─── page: {page}/{maxPage} ───────────────────────────");
            
            int startFileIndex = Convert.ToInt32((page - 1) * pageSize);
            int endFileIndex = Convert.ToInt32((page - 1) * pageSize + pageSize);

            for (int i = startFileIndex; i < endFileIndex; i += 1)
            {
                try
                {
                    FileInfo file = new FileInfo(files[i]);
                    Console.WriteLine($"{file.Name,45} {file.CreationTime.Date,25} {file.Length,15} bytes");
                }
                catch (IndexOutOfRangeException) {}
            }
            
            Console.WriteLine();
        }
        static void PathBuilder()
        {
            
        }
    }
}