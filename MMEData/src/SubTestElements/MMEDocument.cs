using System.IO;

namespace MMEData
{
    public class MMEDocument
    {
        private string documentsPath;
        public string Filename { get; private set; }
        public string FullFilePath
        {
            get
            {
                return Path.Combine(documentsPath, Filename);
            }
        }

        public MMEDocument(string documentsPath, string filename)
        {
            this.documentsPath = documentsPath;
            Filename = filename;
        }
    }
}
