using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingWithFiles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to the desired folder");
            string folderPath = Console.ReadLine();

            try
            {
                // Вызываем метод Cleanup, чтобы удалить все неиспользуемые файлы и папки
                Cleanup(folderPath, TimeSpan.FromMinutes(30));
                Console.WriteLine($"Cleanup completed for folder: {folderPath}");
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, выводим сообщение об ошибке
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void Cleanup(string folderPath, TimeSpan timeout)
        {
            // Проверяем, что папка существует
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
            }
            // Вычисляем момент времени, который был timeout минут назад
            DateTime offset = DateTime.UtcNow.Subtract(timeout);

            // Перебираем все файлы в папке и удаляем те, которые не использовались более timeout минут
            foreach (string filePath in Directory.GetFiles(folderPath))
            {
                if (IsFileInactive(filePath, offset))
                {
                    File.Delete(filePath);
                }
            }

            // Перебираем все подпапки и рекурсивно вызываем метод Cleanup для них
            foreach (string subfolderPath in Directory.GetDirectories(folderPath))
            {
                Cleanup(subfolderPath, timeout);

                // Если подпапка пустая, удаляем ее
                if (IsDirectoryEmpty(subfolderPath))
                {
                    Directory.Delete(subfolderPath);
                }
            }
        }

        static bool IsFileInactive(string filePath, DateTime offset)
        {
            // Возвращает true, если файл не использовался более offset минут
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.LastAccessTimeUtc < offset;
        }

        static bool IsDirectoryEmpty(string directoryPath)
        {
            // Возвращает true, если папка пустая
            return Directory.GetFiles(directoryPath).Length == 0 && Directory.GetDirectories(directoryPath).Length == 0;
        }
    }
}
