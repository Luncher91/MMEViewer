using MMEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEViewer.MMEViewModel
{
    class MMEMovieModel : IMMESubTestModel
    {
        private MMEMovie actualMovie;

        public MMEMovie ActualMovie
        {
            get
            {
                return actualMovie;
            }
        }

        public string ImagePath
        {
            get
            {
                return actualMovie.FullFilePath;
            }
        }

        public MMEMovieModel(MMEMovie movie)
        {
            this.actualMovie = movie;
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
