using System.IO;

namespace MMEData
{
    public class MMEDiagram
    {
        private string diagramsPath;
        public string Filename { get; private set; }
        public string FullFilePath
        {
            get
            {
                return Path.Combine(diagramsPath, Filename);
            }
        }

        public MMEDiagram(string diagramsPath, string filename)
        {
            this.diagramsPath = diagramsPath;
            Filename = filename;
        }
    }
}