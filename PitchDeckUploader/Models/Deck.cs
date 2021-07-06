using System.Collections.Generic;

namespace PitchDeckUploader.Models
{
    public abstract class Deck
    {
        public List<string> ImageList { get; set; }
        public string SourceFilePath { get; set; }
        public string StorageRoot { get; set; }

        protected Deck(string sourceFilePath, string storageRoot)
        {
            ImageList = new List<string>();
            SourceFilePath = sourceFilePath;
            StorageRoot = storageRoot;
        }

        public abstract void ExtractImages();
    }
}
