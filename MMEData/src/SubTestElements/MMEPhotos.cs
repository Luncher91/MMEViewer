using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MMEData
{
    public class MMEPhotos : List<MMEPhoto>, IMMESubTestElement
    {
        public const string MME_PHOTOS_DIRECTORY_NAME = "Photo";
        public const string MME_PHOTO_INFORMATION_FILE_EXTENSION = ".pho";
        public const string MME_NUMBER_OF_PHOTOS_KEY = "Number of photos";

        public MMEDataSet DataSet
        {
            get;
            private set;
        }

        public List<string> Comments { get; private set; }

        public string PhotoDirectoryPath
        {
            get
            {
                return Path.Combine(DataSet.RootDir, MME_PHOTOS_DIRECTORY_NAME);
            }
        }

        public string PhotoInformationFilePath
        {
            get
            {
                return Path.Combine(PhotoDirectoryPath, DataSet.Name + MME_PHOTO_INFORMATION_FILE_EXTENSION);
            }
        }

        public MMEPhotos(MMEDataSet parent)
        {
            DataSet = parent;

            if(!File.Exists(PhotoInformationFilePath))
            {
                Comments = new List<string>();
            }
            else
                LoadInformationFile();
        }

        private void LoadInformationFile()
        {
            List<string> additionalComments = new List<string>();
            var rawData = MMEInformationFileHelper.ReadInformationFile(PhotoInformationFilePath);

            Comments = rawData.AdditionalComments;

            MMEAttribute numberOfPhotosAttribute = rawData.Attributes.First(
                    a => a.Name.Equals(MME_NUMBER_OF_PHOTOS_KEY, StringComparison.InvariantCultureIgnoreCase)
                );

            int numberOfPhotos = int.Parse(numberOfPhotosAttribute.Value);

            for(int i = 0; i < numberOfPhotos; i++)
            {
                Dictionary<string, MMEAttribute> photoAttributes;

                photoAttributes = rawData.Attributes
                    .Where(a => a.Name.EndsWith(" " + (i + 1)))
                    .ToDictionary(a => a.Name);

                MMEPhoto mmePhoto = new MMEPhoto(this, photoAttributes);
                Add(mmePhoto);
            }
        }
    }
}