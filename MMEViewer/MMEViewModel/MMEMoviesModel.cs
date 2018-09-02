using MMEData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEViewer.MMEViewModel
{
    class MMEMoviesModel : IMMESubTestModel
    {
        public MMEMovies ActualMovies { get; }

        List<IMMESubTestModel> subElements;

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return subElements;
            }
        }

        public MMEMoviesModel(MMEMovies movies)
        {
            this.ActualMovies = movies;

            GenerateMovieModels();
        }

        private void GenerateMovieModels()
        {
            subElements = new List<IMMESubTestModel>();

            foreach (MMEMovie mov in ActualMovies)
            {
                subElements.Add(new MMEMovieModel(mov));
            }
        }
    }
}
