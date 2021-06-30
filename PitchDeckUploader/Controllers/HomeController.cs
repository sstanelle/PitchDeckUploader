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

        private IWebHostEnvironment env;
 
        public HomeController(IWebHostEnvironment _environment)
        {
            env = _environment;
        }

        public IActionResult Index()
        {
            return View();
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

                List<string> imageList = new List<string>();
                if (documentExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                    imageList = PdfToPng(sourceFilePath);
                else
                    imageList = PptToPng(sourceFilePath);

                return Json(new UploadResult(0, "Success", imageList));
            }
            catch (Exception)
            {
                return Json(new UploadResult(-1, "An error occurred. Pitch deck could not be uploaded.", null));
            }
        }

        public List<string> PdfToPng(string sourceFilePath)
        {
            List<string> imageList = new List<string>();

            using (var docReader = DocLib.Instance.GetDocReader(sourceFilePath, new PageDimensions(1080, 1920)))
            {
                for (var i = 0; i < docReader.GetPageCount(); i++)
                {
                    using (var pageReader = docReader.GetPageReader(i))
                    {
                        var rawBytes = pageReader.GetImage();
                        var width = pageReader.GetPageWidth();
                        var height = pageReader.GetPageHeight();

                        using var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                        // create a BitmapData and Lock all pixels to be written 
                        BitmapData bmpData = bmp.LockBits(
                                             new Rectangle(0, 0, bmp.Width, bmp.Height),
                                             ImageLockMode.WriteOnly, bmp.PixelFormat);

                        // copy the data from the byte array into BitmapData.Scan0
                        Marshal.Copy(rawBytes, 0, bmpData.Scan0, rawBytes.Length);

                        // unlock the pixels
                        bmp.UnlockBits(bmpData);

                        string filename = "Page-" + Guid.NewGuid().ToString("N") + ".png";
                        imageList.Add(filename);
                        string pageFilePath = Path.Combine(StorageRoot, "images", filename);
                        bmp.Save(pageFilePath, ImageFormat.Png);
                    }
                }
            }

            return imageList;
        }

        public List<string> PptToPng(string sourceFilePath)
        {
            List<string> imageList = new List<string>();

            using (Presentation pres = new Presentation(sourceFilePath))
            {
                foreach (ISlide sld in pres.Slides)
                {
                    // Create a full scale image
                    Bitmap bmp = sld.GetThumbnail(1f, 1f);

                    // Save the image to disk in JPEG format
                    string filename = "Page-" + Guid.NewGuid().ToString("N") + ".png";
                    imageList.Add(filename);
                    string pageFilePath = Path.Combine(StorageRoot, "images", filename);
                    bmp.Save(pageFilePath, ImageFormat.Png);
                }
            }

            return imageList;
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
