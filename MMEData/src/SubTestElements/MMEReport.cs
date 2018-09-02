using System.IO;

namespace MMEData
{
    public class MMEReport
    {
        private string reportPath;
        public string Filename{ get; private set; }
        public string FullFilePath {
            get
            {
                return Path.Combine(reportPath, Filename);
            }
        }

        public MMEReport(string reportPath, string filename)
        {
            this.reportPath = reportPath;
            Filename = filename;
        }

    }
}
