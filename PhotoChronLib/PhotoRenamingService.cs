using ExifLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoChronLib
{
    public class PhotoRenamingService : IPhotoRenamingService
    {
        private List<string> _filePaths;

        public PhotoRenamingService()
        {
            _filePaths = new List<string>();
        }

        public PhotoRenamingService(List<String> paths)
        {
            _filePaths = paths;
        }

        public void AddFilePath(string path)
        {
            _filePaths.Add(path);
        }

        public void RenameImagesByDateTaken()
        {
            int fileNum = 0;
            if (_filePaths.Count == 0)
            {
                Console.WriteLine("No files found.");
                return;
            }
            int nameLength = Convert.ToInt32(Math.Log10(_filePaths.Count));
            sortFilePaths();
            List<FileInfo> files = _filePaths.Select(path => new FileInfo(path)).ToList();

            foreach (var file in files)
            {
                var newName = fileNum.ToString($"D{nameLength}") + "_" + file.Name;
                File.Move(file.FullName, Path.Combine(file.Directory.FullName, newName));
                fileNum++;
            }
        }

        private void sortFilePaths()
        {
            _filePaths.Sort((a, b) => DateTaken(a).CompareTo(DateTaken(b)));
        }

        private DateTime DateTaken(string path)
        { 
            using (ExifReader reader = new ExifReader(path))
            {
                DateTime datePictureTaken;
                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out datePictureTaken))
                {
                    return datePictureTaken;
                }
                else
                {
                    return DateTime.MaxValue;
                }
            }
        }
    }
}
