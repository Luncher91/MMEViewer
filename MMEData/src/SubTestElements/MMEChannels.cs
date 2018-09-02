using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MMEData
{
    public class MMEChannels : List<MMEChannel>, IMMESubTestElement
    {
        public const string MME_CHANNEL_DIRECTORY_NAME = "Channel";
        public const string MME_CHANNEL_LIST_FILE_EXTENSION = ".chn";
        public const string MME_NUMBER_OF_CHANNELS_KEY = "Number of channels";
        public const string MME_NAME_OF_CHANNEL_KEY_PREFIX = "Name of channel";
        public const char MME_CODE_NAME_SPLITTER = '/';

        public MMEDataSet DataSet
        {
            get; private set;
        }

        public string ChannelDirectoryPath
        {
            get
            {
                return Path.Combine(DataSet.RootDir, MME_CHANNEL_DIRECTORY_NAME);
            }
        }

        public string ChannelInformationFilePath
        {
            get
            {
                return Path.Combine(ChannelDirectoryPath, DataSet.Name + MME_CHANNEL_LIST_FILE_EXTENSION);
            }
        }

        public List<string> Comments { get; private set; }
        public Dictionary<string, MMEAttribute> Attributes { get; private set; }

        public MMEChannels(MMEDataSet mME2Test)
        {
            DataSet = mME2Test;

            InitComments();
            InitAttributes();

            Clear();

            if (File.Exists(ChannelInformationFilePath))
                LoadChannelsMetaData();
        }

        private void InitComments()
        {
            Comments = new List<string>();
        }

        private void InitAttributes()
        {
            StringComparer caseInsensitiveComparer = StringComparer.Create(CultureInfo.InvariantCulture, true);
            Attributes = new Dictionary<string, MMEAttribute>(caseInsensitiveComparer);
        }

        private void LoadChannelsMetaData()
        {
            using (StreamReader reader = new StreamReader(ChannelInformationFilePath, 
                Encoding.GetEncoding(MMEInformationFileHelper.DEFAULT_MME_ENCODING), true))
            {
                while (!reader.EndOfStream)
                {
                    MMEAttribute newAttribute = null;

                    MMEAttribute.ParseFromStream(reader, out newAttribute);

                    if (newAttribute == null)
                        continue;

                    if (newAttribute.Name == "" && newAttribute.Value == "")
                    {
                        Comments.AddRange(newAttribute.Comments);
                    }
                    else
                    {
                        if(newAttribute.Name.Equals(MME_NUMBER_OF_CHANNELS_KEY, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // do not add Number of channels to the attributes list
                        }
                        else if(newAttribute.Name.StartsWith(MME_NAME_OF_CHANNEL_KEY_PREFIX, StringComparison.InvariantCultureIgnoreCase))
                        {
                            MMEChannelMeta metaInf = ParseToMMEMeta(newAttribute);
                            string channelFilePath = Path.Combine(ChannelDirectoryPath, DataSet.Name + "." + metaInf.Number.ToString("D3"));

                            this.Add(new MMEChannel(channelFilePath, metaInf));
                        }
                        else
                            Attributes.Add(newAttribute.Name, newAttribute);
                    }
                }
            }
        }

        private MMEChannelMeta ParseToMMEMeta(MMEAttribute newAttribute)
        {
            string[] splittedValue = newAttribute.Value.Split(new[] { MME_CODE_NAME_SPLITTER }, 2);

            MMEChannelMeta mmeMeta = new MMEChannelMeta();

            if (splittedValue.Length > 0)
                mmeMeta.Code = splittedValue[0].Trim();

            if (splittedValue.Length > 1)
                mmeMeta.Name = splittedValue[1].Trim();

            mmeMeta.Number = int.Parse(newAttribute.Name.Substring(MME_NAME_OF_CHANNEL_KEY_PREFIX.Length).Trim());

            return mmeMeta;
        }

        public new void Clear()
        {
            Comments.Clear();
            Attributes.Clear();
            base.Clear();
        }
    }
}