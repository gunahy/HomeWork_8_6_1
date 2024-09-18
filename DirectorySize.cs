using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module8
{
    public static class DirectorySize
    {
        /// <summary>
        /// Класс DirectorySize предназначен для вычисления общего размера заданной директории, 
        /// включая все вложенные подкаталоги и файлы.
        /// 
        /// Особенности:
        /// - Если доступ к каким-либо файлам или директориям запрещен (например, отсутствуют права доступа),
        ///   программа обрабатывает исключение и выводит предупреждающее сообщение, не прерывая выполнение.
        /// - Класс является статическим и может использоваться для работы с любыми директориями на диске. 
        /// Методы:
        /// - GetDirectorySize(string folderPath): Рекурсивно вычисляет размер всех файлов в заданной директории.
        /// 
        /// Исключения:
        /// - DirectoryNotFoundException: выбрасывается, если директория по указанному пути не найдена.
        /// - UnauthorizedAccessException: выбрасывается, если доступ к файлу или папке запрещен.
        /// </summary>
        public static long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Папка {path} не найдена");
            }

            long size = 0;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    size += file.Length;
                }

                foreach (DirectoryInfo subPath in dirInfo.GetDirectories())
                {
                    size += GetDirectorySize(subPath.FullName);
                }
            }
            catch (UnauthorizedAccessException e) {

                Console.WriteLine($"Ошибка: Недостаточно прав для доступа к файлам. {e.Message}");

            }

            return size;

        }
    }
}
