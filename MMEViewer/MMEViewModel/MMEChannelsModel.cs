using System;
using System.Collections.Generic;
using MMEData;

namespace MMEViewer.MMEViewModel
{
    internal class MMEChannelsModel : IMMESubTestModel
    {
        public MMEChannels ActualChannels { get; }

        List<IMMESubTestModel> subElements;

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return subElements;
            }
        }

        public MMEChannelsModel(MMEChannels channels)
        {
            this.ActualChannels = channels;

            GenerateChannelModels();
        }

        private void GenerateChannelModels()
        {
            subElements = new List<IMMESubTestModel>();

            foreach(MMEChannel chn in ActualChannels)
            {
                subElements.Add(new MMEChannelModel(chn));
            }
        }
    }
}