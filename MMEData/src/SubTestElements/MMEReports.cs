using System.Collections.Generic;
using System.IO;

namespace MMEData
{
    public class MMEReports : List<MMEReport>, IMMESubTestElement
    {
        public const string REPORT_DIRECTORY = "Report";
        public MMEDataSet DataSet { get; private set; }

        public string ReportsPath
        {
            get
            {
                return Path.Combine(DataSet.RootDir, REPORT_DIRECTORY);
            }
        }

        public MMEReports(MMEDataSet dataSet)
        {
            DataSet = dataSet;

            if (Directory.Exists(ReportsPath))
            {
                foreach (string file in Directory.EnumerateFiles(ReportsPath))
                {
                    MMEReport report = new MMEReport(ReportsPath, Path.GetFileName(file));
                    this.Add(report);
                }
            }
        }
    }
}