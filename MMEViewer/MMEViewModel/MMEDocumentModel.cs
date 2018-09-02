using System;
using System.Collections.Generic;
using MMEData;

namespace MMEViewer.MMEViewModel
{
    internal class MMEDocumentModel : IMMESubTestModel
    {
        public MMEDocument ActualDocument { get; private set; }

        public string DocumentPath
        {
            get
            {
                return ActualDocument.FullFilePath;
            }
        }

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return new List<IMMESubTestModel>();
            }
        }

        public MMEDocumentModel(MMEDocument doc)
        {
            ActualDocument = doc;
        }
    }
}