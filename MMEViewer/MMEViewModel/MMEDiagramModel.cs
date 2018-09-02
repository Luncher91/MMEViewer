using System;
using System.Collections.Generic;
using MMEData;

namespace MMEViewer.MMEViewModel
{
    internal class MMEDiagramModel : IMMESubTestModel
    {
        public MMEDiagram ActualDiagram { get; private set; }

        public MMEDiagramModel(MMEDiagram dia)
        {
            ActualDiagram = dia;
        }

        public string Filename
        {
            get
            {
                return ActualDiagram.FullFilePath;
            }
        }

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return new List<IMMESubTestModel>();
            }
        }
    }
}