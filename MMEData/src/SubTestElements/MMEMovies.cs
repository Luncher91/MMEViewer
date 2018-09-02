using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MMEData
{
    public class MMEMovies : List<MMEMovie>, IMMESubTestElement
    {
        public const string MME_MOVIES_DIRECTORY_NAME = "Movie";
        public const string MME_MOVIE_INFORMATION_FILE_EXTENSION = ".mii";
        public const string MME_NUMBER_OF_MOVIES_KEY = "Number of movies";

        public MMEDataSet DataSet
        {
            get;
            private set;
        }

        public List<string> Comments { get; private set; }

        public string MovieDirectoryPath
        {
            get
            {
                return Path.Combine(DataSet.RootDir, MME_MOVIES_DIRECTORY_NAME);
            }
        }

        public string MovieInformationFilePath
        {
            get
            {
                return Path.Combine(MovieDirectoryPath, DataSet.Name + MME_MOVIE_INFORMATION_FILE_EXTENSION);
            }
        }

        public MMEMovies(MMEDataSet parent)
        {
            DataSet = parent;

            if (!File.Exists(MovieInformationFilePath))
                Comments = new List<string>();
            else
                LoadInformationFile();
        }

        private void LoadInformationFile()
        {
            List<string> additionalComments = new List<string>();
            var rawData = MMEInformationFileHelper.ReadInformationFile(MovieInformationFilePath);

            Comments = rawData.AdditionalComments;

            MMEAttribute numberOfMoviesAttribute = rawData.Attributes.First(
                    a => a.Name.Equals(MME_NUMBER_OF_MOVIES_KEY, StringComparison.InvariantCultureIgnoreCase)
                );

            int numberOfMovies = int.Parse(numberOfMoviesAttribute.Value);

            for (int i = 0; i < numberOfMovies; i++)
            {
                Dictionary<string, MMEAttribute> photoAttributes;

                photoAttributes = rawData.Attributes
                    .Where(a => a.Name.EndsWith(" " + (i + 1)))
                    .ToDictionary(a => a.Name);

                MMEMovie mmeMovie = new MMEMovie(this, photoAttributes);
                Add(mmeMovie);
            }
        }
    }
}