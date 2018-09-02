using System;
using System.Collections.Generic;
using System.IO;

namespace MMEData
{
    public class MMEDocuments : List<MMEDocument>, IMMESubTestElement
    {
        public const string DOCUMENT_DIRECTORY = "Document";
        public MMEDataSet DataSet { get; private set; }

        public string DocumentsPath
        {
            get
            {
                return Path.Combine(DataSet.RootDir, DOCUMENT_DIRECTORY);
            }
        }

        public MMEDocuments(MMEDataSet dataSet)
        {
            // TODO: refactor this function
            DataSet = dataSet;

            if (Directory.Exists(DocumentsPath))
            {
                foreach (string file in Directory.EnumerateFiles(DocumentsPath))
                {
                    MMEDocument document = new MMEDocument(DocumentsPath, Path.GetFileName(file));
                    this.Add(document);
                }
            }
        }
    }
}