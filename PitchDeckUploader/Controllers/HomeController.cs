using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using Ghostscript.NET.Rasterizer;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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

        private void PdfToPng(string inputFile, string outputFileName)
        {
            var xDpi = 300; //set the x DPI
            var yDpi = 300; //set the y DPI

            using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.CustomSwitches.Add("-dUseCropBox");
                rasterizer.CustomSwitches.Add("-c");
                rasterizer.CustomSwitches.Add("[/CropBox [24 72 559 794] /PAGES pdfmark");
                rasterizer.CustomSwitches.Add("-f");

                rasterizer.Open(inputFile);

                for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    string pageFilePath = Path.Combine(StorageRoot, "images", "Page-" + pageNumber.ToString() + ".png");

                    Image img = rasterizer.GetPage(xDpi, yDpi, pageNumber);
                    img.Save(pageFilePath, ImageFormat.Png);

                    Console.WriteLine(pageFilePath);
                }
            }
        }

        private string StorageRoot
        {
            get { return env.WebRootPath; }
        }

    }
}
