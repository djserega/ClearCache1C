using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ClearCache1C
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() > 0)
            {
                ProgrammProcessing(args);
            }
            else
            {
                FromUser();
            }
        }

        #region ProgrammProcessing

        private static void ProgrammProcessing(string[] args)
        {
            if (!string.IsNullOrEmpty(args.FirstOrDefault(f => f == "/?")))
            {
                Console.WriteLine("Доступные команды:");
                Console.WriteLine("     /all - полная очиска каталогов");
                Console.WriteLine("     /local - очистка каталога AppData\\Local");
                Console.WriteLine("     /roaming - очистка каталога AppData\\Roaming");
                Console.WriteLine("");
                Console.WriteLine("Для очистки всех кешей нужно воспользоваться командами: /all /local или /all /roaming");
                Console.WriteLine("");
                Console.WriteLine("Для очистки конкретной базы или нескольких баз необходимо передать /local или /roaming с именами каталогов баз через пробел:");
                Console.WriteLine("     /local ИмяКаталога");
                Console.WriteLine("     или");
                Console.WriteLine("     /roaming ИмяКаталога1 ИмяКаталога2 ИмяКаталога3 ...");

                Console.ReadKey();
            }
            else
            {
                bool clearAll = !string.IsNullOrEmpty(args.FirstOrDefault(f => f == "/all"));
                bool clearLocal = !string.IsNullOrEmpty(args.FirstOrDefault(f => f == "/local"));
                bool clearRoaming = !string.IsNullOrEmpty(args.FirstOrDefault(f => f == "/roaming"));

                if (clearAll)
                {
                    if (clearLocal)
                        foreach (string directoryName in GetListDirectoryCache(DefaultValues.GetCacheAppDataLocal))
                            DeleteDirectory(DefaultValues.GetCacheAppDataLocal, directoryName);
                    if (clearRoaming)
                        foreach (string directoryName in GetListDirectoryCache(DefaultValues.GetCacheAppDataRoaming))
                            DeleteDirectory(DefaultValues.GetCacheAppDataRoaming, directoryName);
                }
                else
                {
                    foreach (string arg in args)
                    {
                        if (IsDirectoryCache(arg))
                        {
                            if (clearLocal)
                                DeleteDirectory(DefaultValues.GetCacheAppDataLocal, arg);
                            if (clearRoaming)
                                DeleteDirectory(DefaultValues.GetCacheAppDataRoaming, arg);
                        }
                    }
                }
            }
        }

        #endregion

        #region FromUser

        private static void FromUser()
        {
            string text = string.Empty;

            do
            {
                WriteCommand();

                text = Console.ReadLine();

                if (int.TryParse(text, out int comand))
                {
                    switch (comand)
                    {
                        case 1: // Вывести список баз из каталога: Local
                            WriteAllCacheDirectory(DefaultValues.GetCacheAppDataLocal);
                            break;
                        case 2: // Вывести список баз из каталога: Roaming
                            WriteAllCacheDirectory(DefaultValues.GetCacheAppDataRoaming);
                            break;
                        case 5: // Удалить каталоги кеша из: Local
                            break;
                        case 6: // Удалить каталоги кеша из: Roaming
                            break;
                        case 8: //  Удалить каталог кеша Local по имени
                            Console.WriteLine("Введите имя каталога:");
                            text = Console.ReadLine();
                            DeleteDirectory(DefaultValues.GetCacheAppDataLocal, text);
                            break;
                        case 9: //  Удалить каталог кеша Roaming по имени
                            Console.WriteLine("Введите имя каталога:");
                            text = Console.ReadLine();
                            DeleteDirectory(DefaultValues.GetCacheAppDataRoaming, text);
                            break;
                        default:
                            Console.WriteLine("Введена неопознанная команда");
                            break;
                    }
                }
                else if (text == "clear")
                {
                    Console.Clear();
                }
                else if (text != "exit")
                {
                    Console.WriteLine("Ошибка определения команды.");
                    Console.WriteLine("");
                }
            } while (text != "exit");


            Console.WriteLine("Для выхода из приложения нажмите любую клавишу.");
            Console.ReadKey();
        }

        private static void WriteCommand()
        {
            Console.WriteLine("Доступны следующие команды:");
            Console.WriteLine("1. Вывести список баз из каталога: Local");
            Console.WriteLine("2. Вывести список баз из каталога: Roaming");
            Console.WriteLine("5. Удалить каталоги кеша из: Local");
            Console.WriteLine("6. Удалить каталоги кеша из: Roaming");
            Console.WriteLine("8. Удалить каталог кеша Local по имени");
            Console.WriteLine("9. Удалить каталог кеша Roaming по имени");
            Console.WriteLine("");

            Console.WriteLine("clear - очистка консоли");
            Console.WriteLine("exit - завершение работы");
            Console.WriteLine("");
        }

        #endregion

        private static List<string> GetListDirectoryCache(string path)
        {
            List<string> list = new List<string>();

            foreach (DirectoryInfo dir in new DirectoryInfo(path).GetDirectories())
            {
                string nameDir = dir.Name;

                if (IsDirectoryCache(nameDir))
                    list.Add(nameDir);
            }

            return list;
        }

        private static void WriteAllCacheDirectory(string path)
        {
            Console.WriteLine("Список баз каталога: " + path);

            foreach (string nameDir in GetListDirectoryCache(path))
                Console.WriteLine(nameDir);

            Console.WriteLine("");
            Console.WriteLine("");
        }

        private static void DeleteDirectory(string dir, string name)
        {
            if (IsDirectoryCache(name))
            {
                string fullNameDirectory = Path.Combine(dir, name);

                Console.WriteLine("Попытка удаления каталога: " + fullNameDirectory);

                DeleteDir(fullNameDirectory);

                Console.WriteLine(new DirectoryInfo(fullNameDirectory).Exists ? "Ошибка удаления каталога." : "Каталог удален.");
            }
            else
            {
                Console.WriteLine($"Имя каталога '{name}' не соответствует шаблону каталога кеша.");
            }
        }

        private static bool IsDirectoryCache(string nameDir)
        {
            return nameDir.Length == 36
                && nameDir.Count(f => f == '-') == 4;
        }

        private static void DeleteDir(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                DeleteSubDir(path);
                try
                {
                    Directory.Delete(path);
                }
                catch (Exception)
                {
                }
            }
        }

        private static void DeleteSubDir(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (dirInfo.Exists)
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception)
                    {
                    }
                }
                foreach (DirectoryInfo currentSubDir in dirInfo.GetDirectories())
                {
                    try
                    {
                        DeleteSubDir(currentSubDir.FullName);
                        currentSubDir.Delete(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

    }
}
