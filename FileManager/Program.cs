using System;
using System.Diagnostics;
using System.IO;
using System.Net.Security;

/*
- Поддержка копирование файлов, каталогов
- Поддержка удаление файлов, каталогов
- Получение информации о размерах, системных атрибутов файла, каталога
- Вывод файловой структуры должен быть постраничным
- В конфигурационном файле должна быть настройка вывода количества элементов на страницу
- При выходе должно сохраняться, последнее состояние
- Должна быть документация к проекту в формате md
- При успешном выполнение предыдущих пунктов – реализовать сохранение ошибки в текстовом файле
в каталоге errors/random_name_exception.txt
- При успешном выполнение предыдущих пунктов – реализовать движение по истории команд
(стрелочки вверх, вниз)
*/

/*
Настройка отображение страницы - должно быть задано в конфигурационном файле
При выходе нужно запоминать последнее состояние (активный каталог)
Задание на звездочку: при нажатии вверх либо вниз движение по истории команд
*/

namespace FileManager
{
    public static class Global
    {
        public static string[] fullPath = {"/", "/dev", "/dev/cpu"}; // используемый путь
        public static int filesPage = 1; // страница доступа к файлам
    }
    class FileManager
    {
        public static void Main()
        {
            while (true)
            {
                string[] fullPath = Global.fullPath;
                int filesPage = Global.filesPage;
                
                ShowHeader(fullPath);
                ShowDirectoriesTree(fullPath, 0);
                ShowFiles(fullPath, filesPage);
                
                // ввод команды
                Console.Write(fullPath.Last() + " > ");
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
                // изменение директории
                case "cd":
                {
                    switch (commandArgs[1].ToLower())
                    {
                        // перейти на директорию выше
                        // cd -
                        // cd ..
                        case "-":
                        case "..":
                        {
                            // общий путь должен иметь минимум 1 аргумент
                            if (Global.fullPath.Length != 1)
                            {
                                RemoveLastVariable();
                            }
                            break;
                        }
                        
                        // cd [директория]
                        default:
                        {
                            PathBuilder(commandArgs[1]);
                            break;
                        }
                    }

                    break;
                }
                
                // копирование папки
                // copy [директория] [путь]
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
        static void ShowHeader(string[] fullPath)
        {
            // отображение полного пути
            Console.WriteLine(fullPath.Last());
            
            // линия во всю ширину
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write("─");
            }
            
            Console.WriteLine();
        }
        static void ShowDirectoriesTree(string[] fullPath, int recursionDepth)
        {
            // fullPath вида { "/", "/dir", "/dir/dir_2", "/dir/dir_2/dir_3" }
            
            /*
            Вывод файлового дерева
            ├── dir
            │   ├── dir2
            │   └── dir2
            │       └── dir3
                0   1   3 - глубина рекурсии (recursionDepth)
            */
            
            string[] directories = Directory.GetDirectories(fullPath[recursionDepth]);
            
            for (int i = 0; i < directories.Length; i++)
            {
                // построение отступов вида "│   ├──"
                string offset = OffsetBuilder(i, directories.Length, recursionDepth);
                Console.Write(offset);

                DirectoryInfo dir = new DirectoryInfo(directories[i]);
                Console.WriteLine($"{dir.Name}");
                
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
            ──── страница: 1/2 ───────────────────────────────────
            */
            
            const float pageSize = 5;
            
            string[] files = Directory.GetFiles(fullPath.Last());
            double maxPage = Math.Ceiling(files.Length / pageSize);
            
            /*
            // страница не может быть больше макс. значения или меньше 1
            if (page > maxPage || page < 1)
            {
                Console.WriteLine("Индекс страницы имеет недопустимое значение.");
                return;
            }
            */

            PrintPageSeparator(page, maxPage);
            
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
        static void PrintPageSeparator(int page, double maxPage)
        {
            // линия с номером страницы во всю ширину консоли
            
            string pageSeparatorMessage = $"\n─── Файлы на странице: {page}/{maxPage} ";

            // кол-во символов, которые нужно напечатать для того, чтобы сделать
            // линию во всю ширину
            int symbolsToPrint = Console.WindowWidth - pageSeparatorMessage.Length;
            for (int i = 0; i < symbolsToPrint; i++)
            {
                pageSeparatorMessage += "─";
            }
            
            Console.WriteLine(pageSeparatorMessage);
        }
        static string OffsetBuilder(int currentIndex, int directoriesLength, int recursionDepth)
        {
            // отступ вида "├──" при открытии папки

            string result = String.Empty;

            if (recursionDepth == 0)
            {
                result += "├── ";
                return result;
            }
            else
            {
                result += "│";
            }

            for (int i = 0; i < recursionDepth; i++)
            {
                result += "   ";

                if (i != recursionDepth - 1)
                {
                    result += " ";
                }
            }

            if (currentIndex != directoriesLength - 1)
            {
                result += "├── ";
            }
            else
            {
                result += "└── ";
            }
            return result;
        }
        static void RemoveLastVariable()
        {
            // удаление последнего элемента массива
            
            string[] newArray = new string[Global.fullPath.Length - 1];

            for (int i = 0; i < newArray.Length; i++)
            {
                newArray[i] = Global.fullPath[i];
            }

            Global.fullPath = newArray;
        }
        static void PathBuilder(string folder)
        {
            /*
            folder - string вида "/папка"
            построение полного пути вида:
            {"/", "/dev", "/folder"};
            */

            string[] newArray = new string[Global.fullPath.Length + 1];
            string finalPath = String.Empty;
            
            // добаляем в новый массив всё кроме последнего значения
            for (int i = 0; i < newArray.Length - 1; i++)
            {
                newArray[i] = Global.fullPath[i];
            }

            // построение последнего элемента нового массива
            for (int i = 1; i < Global.fullPath.Length; i++)
            {
                finalPath += Global.fullPath[i];
            }
            finalPath += folder;
            
            newArray[newArray.Length - 1] = finalPath;
            
            Global.fullPath = newArray;
        }
    }
}