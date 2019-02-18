using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoChronLib;
using System;
using System.Collections.Generic;
using System.IO;
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
            var files = new List<IFormFile>();
            var filePaths = new List<String>();
            foreach (var file in HttpContext.Request.Form.Files)
            {
                // save file to disk somewhere
                // add its full path to a list to paths
                files.Add(file);                
            }

            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(destinationPath);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var fullFileDestination = Path.Combine(destinationPath, formFile.FileName);
                    using (var stream = new FileStream(fullFileDestination, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        filePaths.Add(fullFileDestination);
                    }
                }
            }

            IPhotoRenamingService renamer = new PhotoRenamingService(filePaths);
            renamer.RenameImagesByDateTaken();

            foreach (var file in Directory.EnumerateFiles(destinationPath))
            {
                
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, destinationPath });
            // make a renamer service
            // add all the files to it
            // rename
            // zip up output folder
            // serve up the zip somehow
        }
    }
}
