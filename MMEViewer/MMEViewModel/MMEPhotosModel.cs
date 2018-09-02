using MMEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEViewer.MMEViewModel
{
    class MMEPhotosModel : IMMESubTestModel
    {
        public MMEPhotos ActualPhotos { get; }

        List<IMMESubTestModel> subElements;

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return subElements;
            }
        }

        public MMEPhotosModel(MMEPhotos photos)
        {
            this.ActualPhotos = photos;

            GeneratePhotoModels();
        }

        private void GeneratePhotoModels()
        {
            subElements = new List<IMMESubTestModel>();

            foreach (MMEPhoto pho in ActualPhotos)
            {
                subElements.Add(new MMEPhotoModel(pho));
            }
        }
    }
}

