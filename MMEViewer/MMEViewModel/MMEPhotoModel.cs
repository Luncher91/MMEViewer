using MMEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEViewer.MMEViewModel
{
    class MMEPhotoModel : IMMESubTestModel
    {
        private MMEPhoto actualPhoto;

        public MMEPhoto ActualPhoto
        {
            get
            {
                return actualPhoto;
            }
        }

        public string ImagePath
        {
            get
            {
                return actualPhoto.FullFilePath;
            }
        }

        public MMEPhotoModel(MMEPhoto photo)
        {
            this.actualPhoto = photo;
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
