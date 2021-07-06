using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

using Docnet.Core;
using Docnet.Core.Models;

namespace PitchDeckUploader.Models
{
    public class PdfDeck : Deck
    {
        public PdfDeck(string sourceFilePath, string storageRoot) : base(sourceFilePath, storageRoot)
        {
        }

        public override void ExtractImages()
        {
            using (var docReader = DocLib.Instance.GetDocReader(SourceFilePath, new PageDimensions(1080, 1920)))
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
                        ImageList.Add(filename);
                        string pageFilePath = Path.Combine(StorageRoot, "images", filename);
                        bmp.Save(pageFilePath, ImageFormat.Png);
                    }
                }
            }
        }
    }
}
