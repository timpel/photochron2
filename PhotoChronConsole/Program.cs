using PhotoChronLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace PhotoChronConsole
{
    class Program
    {
        private List<String> _filePaths;
        private IPhotoRenamingService _renamingService;
        private String[] _filter;

        public Program(string directory)
        {
            _filter = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
            _filePaths = GetFilesFrom(directory, _filter, false);
            _renamingService = new PhotoRenamingService(_filePaths);
        }

        public static List<String> GetFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound;
        }

        private void Run()
        {
            _renamingService.RenameImagesByDateTaken();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to PhotoChron");

            if (args.Length != 1)
            {
                Console.WriteLine("One command line argument required (path to directory containing photos to chronologize)");
                return;
            }

            var prog = new Program(args[0]);
            prog.Run();
        }
    }
}
