using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Aspose.Slides;

using Docnet.Core;
using Docnet.Core.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

using PitchDeckUploader.Models;

namespace PitchDeckUploader.Controllers
{
    public class HomeController : Controller
    {
        public class UploadResult
        {
            public int returnCode { get; set; }
            public string message { get; set; }
            public List<string> imageList { get; set; }

            public UploadResult(int retCode, string msg, List<string> imgList)
            {
                returnCode = retCode;
                message = msg;
                imageList = imgList;
            }
        }

        public Deck deck { get; set; }

        private IWebHostEnvironment env;
 
        public HomeController(IWebHostEnvironment _environment)
        {
            deck = null;
            env = _environment;
        }

        public IActionResult Index()
        {
            return View(deck);
        }
        public IActionResult About()
        {
            ViewData["Message"] = "";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "For more information";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult UploadPitchDeck(IFormFile uploadedFile)
        {
            try
            {
                if (uploadedFile == null)
                {
                    return Json(new UploadResult(-1, "Please select a pitch deck to upload", null));
                }

                string documentExtension = Path.GetExtension(uploadedFile.FileName);
                if (!documentExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase) &&
                    !documentExtension.Equals(".ppt", StringComparison.OrdinalIgnoreCase) &&
                    !documentExtension.Equals(".pptx", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new UploadResult(-1, "Pitch deck must be a PDF or PowerPoint.", null));
                }

                byte[] pdfData = new byte[uploadedFile.Length];
                using (Stream s = uploadedFile.OpenReadStream())
                {
                    s.Read(pdfData, 0, (int)uploadedFile.Length);
                }
                string sourceFilePath = Path.Combine(StorageRoot, "images", uploadedFile.FileName);
                System.IO.File.WriteAllBytes(sourceFilePath, pdfData);

                DeletePitchDeck();

                if (documentExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    deck = new PdfDeck(sourceFilePath, StorageRoot);
                else
                    deck = new PowerPointDeck(sourceFilePath, StorageRoot);

                deck.ExtractImages();

                return Json(new UploadResult(0, "Success", deck.ImageList));
            }
            catch (Exception e)
            {
                return Json(new UploadResult(-1, "An error occurred. Pitch deck could not be uploaded.<br/><br/>" + e.Message, null));
            }
        }

        private void DeletePitchDeck()
        {
            string path = Path.Combine(StorageRoot, "images");
            string[] filePaths = Directory.GetFiles(path);
            foreach (string filePath in filePaths)
            {
                if (filePath.Contains("Page"))
                {
                    System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
                    System.IO.File.Delete(filePath);
                }
            }
        }

        private string StorageRoot
        {
            get { return env.WebRootPath; }
        }
    }
}
