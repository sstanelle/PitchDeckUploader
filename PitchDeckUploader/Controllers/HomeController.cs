using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

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
            string documentExtension = Path.GetExtension(uploadedFile.FileName);
            if (!documentExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase) &&
                !documentExtension.Equals(".ppt", StringComparison.OrdinalIgnoreCase))
            {
                return Json("Pitch deck must be either a PDF or a PowerPoint.");
            }

            byte[] pdfData = new byte[uploadedFile.Length];
            using (Stream s = uploadedFile.OpenReadStream())
            {
                s.Read(pdfData, 0, (int) uploadedFile.Length);
            }
            string sourceFilePath = Path.Combine(StorageRoot, "images", uploadedFile.FileName);
            System.IO.File.WriteAllBytes(sourceFilePath, pdfData);

            PdfToPng(sourceFilePath, "image");

            List<string> imageList = new List<string>();
            imageList.Add("banner1.svg");
            imageList.Add("banner2.svg");
            imageList.Add("banner3.svg");

            return Json(imageList);
        }

        public void PdfToPng(string sourceFilePath, string destinationFilePath)
        {
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

                        string pageFilePath = Path.Combine(StorageRoot, "images", "Page-" + i.ToString() + ".png");
                        bmp.Save(pageFilePath, ImageFormat.Png);
                    }
                }
            }
        }

        private string StorageRoot
        {
            get { return env.WebRootPath; }
        }

    }
}
