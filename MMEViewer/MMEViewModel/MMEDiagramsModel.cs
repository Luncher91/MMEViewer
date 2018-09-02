using System;
using System.Collections.Generic;
using MMEData;

namespace MMEViewer.MMEViewModel
{
    internal class MMEDiagramsModel : IMMESubTestModel
    {
        public MMEDiagrams ActualDiagrams { get; }

        List<IMMESubTestModel> subElements;

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return subElements;
            }
        }

        public MMEDiagramsModel(MMEDiagrams diagrams)
        {
            this.ActualDiagrams = diagrams;

            GenerateDocumentModels();
        }

        private void GenerateDocumentModels()
        {
            subElements = new List<IMMESubTestModel>();

            foreach (MMEDiagram dia in ActualDiagrams)
            {
                subElements.Add(new MMEDiagramModel(dia));
            }
        }
    }
}