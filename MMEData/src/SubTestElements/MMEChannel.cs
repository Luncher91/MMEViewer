using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MMEData
{
    public class MMEChannelMeta
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
    }

    public class MMEChannel
    {   
        public const string MME_CHANNEL_SAMPLE_INTERVAL_ATTRIBUTE = "Sampling interval";
        public const string MME_CHANNEL_TIME_OF_FIRST_SAMPLE_ATTRIBUTE = "Time of first sample";

        public string ChannelFilePath { get; private set; }

        public bool IsLoaded { get; private set; }

        public List<double> Values { get; set; }
        public List<string> Comments { get; private set; }
        public Dictionary<string, MMEAttribute> Attributes { get; private set; }
        public MMEChannelMeta Meta { get; private set; }

        public double SampleInterval
        {
            get
            {
                if (!Attributes.ContainsKey(MME_CHANNEL_SAMPLE_INTERVAL_ATTRIBUTE))
                    return 1;

                return MMEInformationFileHelper.ParseMMEValue(Attributes[MME_CHANNEL_SAMPLE_INTERVAL_ATTRIBUTE].Value);
            }
            set
            {
                AssureAttribute(MME_CHANNEL_SAMPLE_INTERVAL_ATTRIBUTE);

                Attributes[MME_CHANNEL_SAMPLE_INTERVAL_ATTRIBUTE].Value = value.ToString(MMEInformationFileHelper.DEFAULT_MME_NUMBERFORMATINFO);
            }
        }

        public double TimeOfFirstSample
        {
            get
            {
                if (!Attributes.ContainsKey(MME_CHANNEL_TIME_OF_FIRST_SAMPLE_ATTRIBUTE))
                    return 0;

                return MMEInformationFileHelper.ParseMMEValue(Attributes[MME_CHANNEL_TIME_OF_FIRST_SAMPLE_ATTRIBUTE].Value);
            }
            set
            {
                AssureAttribute(MME_CHANNEL_TIME_OF_FIRST_SAMPLE_ATTRIBUTE);

                Attributes[MME_CHANNEL_TIME_OF_FIRST_SAMPLE_ATTRIBUTE].Value = value.ToString(MMEInformationFileHelper.DEFAULT_MME_NUMBERFORMATINFO);
            }
        }

        public MMEChannel(string channelFilePath, MMEChannelMeta mMEChannelMeta)
        {
            ChannelFilePath = Path.GetFullPath(channelFilePath);

            Meta = mMEChannelMeta;

            InitializeAttributes();
            InitializeComments();
            InitializeValues();

            IsLoaded = File.Exists(channelFilePath);
        }

        public MMEChannel(MMEChannelMeta mMEChannelMeta)
        {
            ChannelFilePath = "";
            Meta = mMEChannelMeta;

            InitializeAttributes();
            InitializeComments();
            InitializeValues();

            IsLoaded = true;
        }

        public MMEChannel()
        {
        }

        private void InitializeValues()
        {
            Values = new List<double>();
        }

        private void InitializeComments()
        {
            Comments = new List<string>();
        }

        private void InitializeAttributes()
        {
            StringComparer caseInsensitiveComparer = StringComparer.Create(CultureInfo.InvariantCulture, true);
            Attributes = new Dictionary<string, MMEAttribute>(caseInsensitiveComparer);
        }

        public enum MMEFilter
        {
            None,
            CFC1000,
            CFC600,
            CFC180,
            CFC60
        }

        public List<Tuple<double, double>> GetDataPoints()
        {
            List<Tuple<double, double>> dataPoints = new List<Tuple<double, double>>();
            double sampleRate = SampleInterval;
            double timeOffset = TimeOfFirstSample;

            for (int i = 0; i < Values.Count; i++)
            {
                double timeActualSample = i * sampleRate + timeOffset;
                dataPoints.Add(new Tuple<double, double>(timeActualSample, Values[i]));
            }

            return dataPoints;
        }

        public static int GetCFCNumber(MMEFilter filter)
        {
            switch (filter)
            {
                case MMEFilter.CFC1000:
                    return 1000;
                case MMEFilter.CFC600:
                    return 600;
                case MMEFilter.CFC180:
                    return 180;
                case MMEFilter.CFC60:
                    return 60;
                default:
                    return 0;
            }
        }

        public static MMEFilter GetFilterEnum(int filterClass)
        {
            switch(filterClass)
            {
                case 1000:
                    return MMEFilter.CFC1000;
                case 600:
                    return MMEFilter.CFC600;
                case 180:
                    return MMEFilter.CFC180;
                case 60:
                    return MMEFilter.CFC60;
                default:
                    return MMEFilter.None;
            }
        }

        public void Load()
        {
            LoadChannelFile();
            IsLoaded = true;
        }

        public void Free()
        {
            IsLoaded = false;

            ResetAttributesAndComments();
            ResetValues();
        }

        private void LoadChannelFile()
        {
            Free();

            var rawData = MMEInformationFileHelper.ReadInformationFile(ChannelFilePath, true);

            Attributes = rawData.Attributes.ToDictionary(a => a.Name);
            Comments = rawData.AdditionalComments;
            Values = rawData.Values;
        }

        private void AssureAttribute(string key)
        {
            if (!Attributes.ContainsKey(key))
            {
                MMEAttribute newAttribute = new MMEAttribute();

                newAttribute.Name = key;
                newAttribute.Value = "";
                newAttribute.IsCustomAttribute = false;

                Attributes[key] = newAttribute;
            }
        }

        private void ResetValues()
        {
            Values.Clear();
        }

        private void ResetAttributesAndComments()
        {
            Attributes.Clear();
            Comments.Clear();
        }
    }
}
