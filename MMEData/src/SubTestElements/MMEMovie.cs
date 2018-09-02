using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEData
{
    public class MMEMovie
    {
        public const string NAME_OF_MOVIE_FILE_ATTRIBUTE = "Name of movie file";

        private MMEMovies parent;
        private Dictionary<string, MMEAttribute> movieAttributes;

        public string Filename
        {
            get
            {
                return movieAttributes[NAME_OF_MOVIE_FILE_ATTRIBUTE].Value;
            }
            set
            {
                string oldPath = FullFilePath;

                movieAttributes[NAME_OF_MOVIE_FILE_ATTRIBUTE].Value = value;

                string newPath = FullFilePath;

                if (File.Exists(oldPath))
                    File.Move(oldPath, newPath);
            }
        }

        public string FullFilePath
        {
            get
            {
                return Path.Combine(parent.MovieDirectoryPath, Filename);
            }

        }
        public Dictionary<string, MMEAttribute> Attributes
        {
            get
            {
                return movieAttributes;
            }
        }

        public MMEMovie(MMEMovies mMEMovies, Dictionary<string, MMEAttribute> movieAttributes)
        {
            this.parent = mMEMovies;
            this.movieAttributes = movieAttributes;

            MMEInformationFileHelper.RemoveNumberFromAttributes(movieAttributes);
        }
    }
}
