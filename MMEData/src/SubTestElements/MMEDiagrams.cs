using System;
using System.Collections.Generic;
using System.IO;

namespace MMEData
{
    public class MMEDiagrams : List<MMEDiagram>, IMMESubTestElement
    {
        public const string DIAGRAM_DIRECTORY = "Diagram";
        public MMEDataSet DataSet { get; private set; }

        public string DiagramsPath
        {
            get
            {
                return Path.Combine(DataSet.RootDir, DIAGRAM_DIRECTORY);
            }
        }

        public MMEDiagrams(MMEDataSet dataSet)
        {
            // TODO: refactor this function
            DataSet = dataSet;

            if (Directory.Exists(DiagramsPath))
            {
                foreach (string file in Directory.EnumerateFiles(DiagramsPath))
                {
                    MMEDiagram diagram = new MMEDiagram(DiagramsPath, Path.GetFileName(file));
                    this.Add(diagram);
                }
            }
        }
    }
}