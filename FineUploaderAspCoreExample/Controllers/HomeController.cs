using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FineUploaderAspCoreExample.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FineUploaderAspCoreExample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(string qquuid, string qqfilename, int qqtotalfilesize, IFormFile qqfile)
        {
            int intFileDBID = 0;
            string strRealFilePath = "";
            string strCurrentPath = @"c:\tmp\";

            // original file name - qqfile.FileName
            // user file name - qqfilename

            if (qqfile.Length > 0)
            {
                var filePath = Path.Combine(strCurrentPath, qqfilename); // never do it without validation in real project!!!
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        //throw new ApplicationException("Some Error"); // for testing errors
                        await qqfile.CopyToAsync(stream);

                        // DO SOMETHING ELSE, for example save file info to database
                        
                        intFileDBID = 1; // change on file DB ID, if needed
                        strRealFilePath = filePath;  // change on file real path, if needed
                    }
                }
                catch (Exception ex)
                {
                    // return to JS fail and error
                    return Ok(new { success = false, error = ex.Message });
                }
            }

            // return to JS success and extra information about uploded file
            return Ok(new { success = true, fileid = intFileDBID, fpath = strRealFilePath, error = "" });
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
