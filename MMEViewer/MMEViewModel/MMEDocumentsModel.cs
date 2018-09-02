using MMEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEViewer.MMEViewModel
{
    class MMEDocumentsModel : IMMESubTestModel
    {
        public MMEDocuments ActualDocuments { get; }

        List<IMMESubTestModel> subElements;

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return subElements;
            }
        }

        public MMEDocumentsModel(MMEDocuments documents)
        {
            this.ActualDocuments = documents;

            GenerateDocumentModels();
        }

        private void GenerateDocumentModels()
        {
            subElements = new List<IMMESubTestModel>();

            foreach (MMEDocument doc in ActualDocuments)
            {
                subElements.Add(new MMEDocumentModel(doc));
            }
        }
    }
}
