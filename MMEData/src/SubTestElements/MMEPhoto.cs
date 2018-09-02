using System;
using System.Collections.Generic;
using System.IO;

namespace MMEData
{
    public class MMEPhoto
    {
        public const string NAME_OF_PHOTO_FILE_ATTRIBUTE = "Name of photo file";

        private MMEPhotos mMEPhotos;
        private Dictionary<string, MMEAttribute> photoAttributes;

        public string Filename
        {
            get
            {
                return photoAttributes[NAME_OF_PHOTO_FILE_ATTRIBUTE].Value;
            }
            set
            {
                string oldPath = FullFilePath;

                photoAttributes[NAME_OF_PHOTO_FILE_ATTRIBUTE].Value = value;

                string newPath = FullFilePath;

                if(File.Exists(oldPath))
                    File.Move(oldPath, newPath);
            }
        }

        public string FullFilePath
        {
            get
            {
                return Path.Combine(mMEPhotos.PhotoDirectoryPath, Filename);
            }

        }
        public Dictionary<string, MMEAttribute> Attributes
        {
            get
            {
                return photoAttributes;
            }
        }

        public MMEPhoto(MMEPhotos mMEPhotos, Dictionary<string, MMEAttribute> photoAttributes)
        {
            this.mMEPhotos = mMEPhotos;
            this.photoAttributes = photoAttributes;

            MMEInformationFileHelper.RemoveNumberFromAttributes(photoAttributes);
        }
    }
}