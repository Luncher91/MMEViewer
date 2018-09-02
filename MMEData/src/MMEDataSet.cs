using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MMEData
{
    public partial class MMEDataSet
    {
        public const string MME_EXTENSION = ".mme";

        public string RootDir { get; private set; }
        public string Name { get; private set; }
        public List<MMEAttribute> Attributes { get; private set; }
        public List<string> Comments { get; private set; }

        public MMEChannels Channels { get; private set; }
        public MMEPhotos Images { get; private set; }
        public MMEMovies Movies { get; private set; }
        public MMEDocuments Documents { get; private set; }
        public MMEReports Reports { get; private set; }
        public MMEStaticData StaticData { get; private set; }
        public MMEDiagrams Diagrams { get; private set; }

        public string MMEFilePath
        {
            get
            {
                return Path.Combine(RootDir, Name + MME_EXTENSION);
            }
        }

        public MMEDataSet(string testDirectoryOrFile)
        {
            // TODO: refactor this function
            InitProperties();

            string absolutePath = Path.GetFullPath(testDirectoryOrFile);
            RootDir = Path.GetDirectoryName(absolutePath);
            Name = Path.GetFileName(RootDir);

            Load();
        }

        private void InitProperties()
        {
            RootDir = "";
            Name = "";

            Attributes = new List<MMEAttribute>();
            Comments = new List<string>();
        }

        private void Load()
        {
            // TODO: refactor this function
            ResetAttributesAndComments();

            LoadAttributesAndComments();

            LoadChannels();
            LoadImages();
            LoadMovies();
            LoadDocuments();
            LoadReports();
            LoadStaticData();
            LoadDiagrams();
        }

        private void LoadDiagrams()
        {
            Diagrams = new MMEDiagrams(this);
        }

        private void LoadStaticData()
        {
            // TODO: implement static data
        }

        private void LoadReports()
        {
            Reports = new MMEReports(this);
        }

        private void LoadDocuments()
        {
            Documents = new MMEDocuments(this);
        }

        private void LoadMovies()
        {
            Movies = new MMEMovies(this);
        }

        private void LoadImages()
        {
            Images = new MMEPhotos(this);
        }

        private void LoadChannels()
        {
            Channels = new MMEChannels(this);
        }

        private void ResetAttributesAndComments()
        {
            ResetAttributes();
            ResetComments();
        }

        private void ResetComments()
        {
            Comments.Clear();
        }

        private void ResetAttributes()
        {
            Attributes.Clear();
        }

        private void LoadAttributesAndComments()
        {
            MMEInformationFileHelper.RawMMEFile rawData = MMEInformationFileHelper.ReadInformationFile(MMEFilePath);

            Attributes = rawData.Attributes;
            Comments = rawData.AdditionalComments;
        }
    }
}
