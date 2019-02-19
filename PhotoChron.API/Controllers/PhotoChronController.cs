using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoChronLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhotoChron.API.Controllers
{
    [Route("api/[controller]")]
    public class PhotoChronController : Controller
    {
        [HttpGet("[action]")]
        public IEnumerable<String> UploadFiles()
        {
            return new List<String>
            {
                "Hi",
                "You made it!"
            };
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Rename()
        {
            var files = HttpContext.Request.Form.Files;
            var filePaths = new List<String>();

            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(destinationPath);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // copy file to temp destination
                    var fullFileDestination = Path.Combine(destinationPath, formFile.FileName);
                    using (var stream = new FileStream(fullFileDestination, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        filePaths.Add(fullFileDestination);
                    }
                }
            }

            //sort/rename photos
            IPhotoRenamingService renamer = new PhotoRenamingService(filePaths);
            renamer.RenameImagesByDateTaken();

            var zipPath = destinationPath + "-zip.zip";

            // zip up output folder
            ZipFile.CreateFromDirectory(
                destinationPath,
                zipPath,
                CompressionLevel.Optimal,
                true);

            // delete original
            Directory.Delete(destinationPath, true);

            // return the zip
            return DownloadFile(zipPath);
        }

        private IActionResult DownloadFile(string path)
        {
            var net = new System.Net.WebClient();
            var data = net.DownloadData(path);
            var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            var fileName = "ordered.zip";

            // Delete zip from server
            System.IO.File.Delete(path);

            // return zip stream
            return File(content, contentType, fileName);
        }
    }
}
