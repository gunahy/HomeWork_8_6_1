using System;
using System.IO;


namespace Module8
{

    class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = @"C:\testProgram"; // Путь к папке
            TimeSpan timeLimit = TimeSpan.FromMinutes(30);

            // Путь к лог-файлу во временной папке
            string logFilePath = Path.GetTempFileName();

            try
            {
                CleanDirectory(directoryPath, timeLimit, logFilePath);
            }
            catch (Exception e)
            {
                // Логируем исключение в файл
                LogException(logFilePath, e);
            }

            Console.WriteLine("Операция завершена.");

            // Спрашиваем пользователя, нужно ли открыть файл лога
            Console.Write("Открыть лог-файл? Д/н: ");
            string response = Console.ReadLine();

            if (response?.Trim().ToLower() == "д")
            {
                try
                {
                    System.Diagnostics.Process.Start("notepad.exe", logFilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Не удалось открыть файл: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Рекурсивный метод для очистки директорий. Удаляет файлы и папки, которые не использовались дольше указанного времени.
        /// </summary>
        /// <param name="directoryPath">Путь к директории для очистки.</param>
        /// <param name="timeLimit">Время, по истечении которого файлы/папки считаются устаревшими и подлежат удалению.</param>
        /// <param name="logFilePath">Запись результата удаления файла/папки в лог-файл</param>
        static void CleanDirectory(string directoryPath, TimeSpan timeLimit, string logFilePath)
        {
            // Проверяем, существует ли директория перед обработкой
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Директория не существует: {directoryPath}");
                return;
            }

            // Удаление файлов в текущей директории
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                DateTime lastAccessTime = File.GetLastAccessTime(file);
                if (DateTime.Now - lastAccessTime > timeLimit)
                {
                    Logger(logFilePath, file);
                    File.Delete(file);
                }
            }

            // Рекурсивная очистка вложенных папок
            foreach (var dir in Directory.GetDirectories(directoryPath))
            {
                // Рекурсивно проверяем файлы во вложенных папках
                CleanDirectory(dir, timeLimit, logFilePath);

                // Проверяем и удаляем пустые папки, если они не использовались дольше указанного времени
                DateTime lastAccessTime = Directory.GetLastWriteTime(dir);
                Console.WriteLine(lastAccessTime.ToString());
                if (DateTime.Now - lastAccessTime > timeLimit && IsDirectoryEmpty(dir))
                {
                    Logger(logFilePath, dir);
                    Directory.Delete(dir, true);
                }
            }
        }

        /// <summary>
        /// Проверяет, пуста ли директория.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <returns>true, если директория пуста, иначе false.</returns>
        static bool IsDirectoryEmpty(string path)
        {
            return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }

        /// <summary>
        /// Логирует информацию об исключении в указанный файл.
        /// </summary>
        /// <param name="logFilePath">Путь к файлу для логов.</param>
        /// <param name="e">Исключение, которое нужно записать в лог.</param>
        static void LogException(string logFilePath, Exception e)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("Дата и время: " + DateTime.Now);
                    writer.WriteLine("Сообщение: " + e.Message);
                    writer.WriteLine("Error: " + e.StackTrace);
                    writer.WriteLine("---------------------------------------------------");
                }
            }
            catch (Exception logE)
            {
                Console.WriteLine($"Не удалось записать лог: {logE.Message}");
            }
        }

        /// <summary>
        /// Логирует имя удаленного объекта во временный файл.
        /// </summary>
        /// <param name="logFilePath">Путь к файлу для логов.</param>
        /// <param name="FilePath">Удаленный объект</param>
        /// 
        static void Logger(string logFilePath, string FilePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(DateTime.Now + " - " + FilePath + " удален");
                }
            }
            catch (Exception logE)
            {
                Console.WriteLine($"Не удалось записать лог: {logE.Message}");
            }
        }
    }

}