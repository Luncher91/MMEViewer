using MMEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEViewer.MMEViewModel
{
    public class MMETestModel
    {
        private List<IMMESubTestModel> subElements;

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return subElements;
            }
        }

        public MMEDataSet DataSet { get; private set; }

        public MMETestModel(MMEDataSet mmeDataSet)
        {
            DataSet = mmeDataSet;

            GenerateSubStructure();
        }

        private void GenerateSubStructure()
        {
            subElements = new List<IMMESubTestModel>();

            subElements.Add(GenerateChannelsModel());
            subElements.Add(GeneratePhotosModel());
            subElements.Add(GenerateMoviesModel());
            subElements.Add(GenerateDocumentsModel());
            subElements.Add(GenerateDiagramsModel());
        }

        private MMEDiagramsModel GenerateDiagramsModel()
        {
            MMEDiagramsModel diagramSubTest = new MMEDiagramsModel(DataSet.Diagrams);
            return diagramSubTest;
        }

        private MMEDocumentsModel GenerateDocumentsModel()
        {
            MMEDocumentsModel docsSubTest = new MMEDocumentsModel(DataSet.Documents);
            return docsSubTest;
        }

        private MMEMoviesModel GenerateMoviesModel()
        {
            MMEMoviesModel movieSubTest = new MMEMoviesModel(DataSet.Movies);
            return movieSubTest;
        }

        private MMEPhotosModel GeneratePhotosModel()
        {
            MMEPhotosModel photoSubTest = new MMEPhotosModel(DataSet.Images);
            return photoSubTest;
        }

        private MMEChannelsModel GenerateChannelsModel()
        {
            MMEChannelsModel channelSubTest = new MMEChannelsModel(DataSet.Channels);
            return channelSubTest;
        }

        private List<MMEChannelModel> GenerateMetaChannelModel()
        {
            List<MMEChannelModel> metaChannelView = new List<MMEChannelModel>();

            foreach(MMEChannel chn in DataSet.Channels)
            {
                MMEChannelModel model = new MMEChannelModel(chn);
                metaChannelView.Add(model);
            }

            return metaChannelView;
        }
    }
}
