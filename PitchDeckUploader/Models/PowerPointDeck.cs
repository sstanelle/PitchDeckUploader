using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Aspose.Slides;

namespace PitchDeckUploader.Models
{
    public class PowerPointDeck : Deck
    {
        public PowerPointDeck(string sourceFilePath, string storageRoot) : base(sourceFilePath, storageRoot)
        {
        }

        public override void ExtractImages()
        {
            using (Presentation pres = new Presentation(SourceFilePath))
            {
                foreach (ISlide sld in pres.Slides)
                {
                    // Create a full scale image
                    Bitmap bmp = sld.GetThumbnail(1f, 1f);

                    // Save the image to disk in JPEG format
                    string filename = "Page-" + Guid.NewGuid().ToString("N") + ".png";
                    ImageList.Add(filename);
                    string pageFilePath = Path.Combine(StorageRoot, "images", filename);
                    bmp.Save(pageFilePath, ImageFormat.Png);
                }
            }
        }
    }
}
