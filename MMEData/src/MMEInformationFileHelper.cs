using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MMEData
{
    internal class MMEInformationFileHelper
    {
        // specified as ISO-8859-1
        // public const string MME_ENCODING = "iso-8859-1";
        // to be more tolerant we are using windows-1252 or Unicode indicated by BOM (UTF8, LE16, BE16)
        public const string DEFAULT_MME_ENCODING = "windows-1252";
        public const NumberStyles DEFAULT_MME_NUMBERSTYLE = NumberStyles.Float;
        public static readonly NumberFormatInfo DEFAULT_MME_NUMBERFORMATINFO = NumberFormatInfo.InvariantInfo;

        internal class MMEReadConfiguration
        {
            public NumberStyles MMENumberStyle = DEFAULT_MME_NUMBERSTYLE;
            public NumberFormatInfo InvariantNumberFormatInfo = DEFAULT_MME_NUMBERFORMATINFO;
            public Encoding FileEncoding = Encoding.GetEncoding(DEFAULT_MME_ENCODING);
            public bool DetectByteOrderMask = true;
        }

        internal static RawMMEFile ReadInformationFile(string filePath, bool readValues = false, MMEReadConfiguration config = null)
        {
            // TODO: refactor this function!
            if (config == null)
                config = new MMEReadConfiguration();

            RawMMEFile rawMMEData = new RawMMEFile()
            {
                AdditionalComments = new List<string>(),
                Attributes = new List<MMEAttribute>(),
                Values = new List<double>(),
            };
            
            using (StreamReader reader = new StreamReader(filePath, config.FileEncoding, config.DetectByteOrderMask))
            {
                string uninterpretedLine = "";

                while (!reader.EndOfStream)
                {
                    uninterpretedLine = ReadAttribute(rawMMEData, reader);

                    if (uninterpretedLine.Length > 0)
                        break;
                }

                if (uninterpretedLine.Length > 0 && readValues)
                {
                    ReadMMEValue(rawMMEData, uninterpretedLine, config);
                    while (!reader.EndOfStream)
                    {
                        ReadMMEValue(rawMMEData, reader.ReadLine(), config);
                    }
                }
            }

            return rawMMEData;
        }

        private static void ReadMMEValue(RawMMEFile rawMMEData, string valueLine, MMEReadConfiguration config)
        {
            double parsedValue = ParseMMEValue(valueLine, config.InvariantNumberFormatInfo, config.MMENumberStyle);

            rawMMEData.Values.Add(parsedValue);
        }

        internal static double ParseMMEValue(string valueLine, NumberFormatInfo numberFormatInfo = null, NumberStyles numberStyle = DEFAULT_MME_NUMBERSTYLE)
        {
            if (numberFormatInfo == null)
                numberFormatInfo = DEFAULT_MME_NUMBERFORMATINFO;

            bool parseSucceeded;
            double parsedValue;

            parseSucceeded = double.TryParse(valueLine, numberStyle, numberFormatInfo, out parsedValue);

            if (!parseSucceeded)
                parsedValue = double.NaN;

            return parsedValue;
        }

        private static string ReadAttribute(RawMMEFile rawMMEData, StreamReader reader)
        {
            // TODO: refactor this function
            MMEAttribute newAttribute = null;

            string uninterpratedLine = MMEAttribute.ParseFromStream(reader, out newAttribute);

            if (newAttribute.Name == "" && newAttribute.Value == "")
            {
                if (newAttribute.Comments.Count > 0)
                    rawMMEData.AdditionalComments.AddRange(newAttribute.Comments);
            }
            else
                rawMMEData.Attributes.Add(newAttribute);

            return uninterpratedLine;
        }

        internal static void RemoveNumberFromAttributes(Dictionary<string, MMEAttribute> attributes)
        {
            // TODO: refactor this function
            List<string> oldKeys = new List<string>();

            foreach (var attr in attributes)
            {
                oldKeys.Add(attr.Key);

                attr.Value.Name = RemoveLastWordIfInt(attr.Value.Name);
            }

            foreach(string oldEntry in oldKeys)
            {
                MMEAttribute element = attributes[oldEntry];
                attributes.Add(element.Name, element);
                attributes.Remove(oldEntry);
            }
        }

        private static string RemoveLastWordIfInt(string name)
        {
            // TODO: refactor this function
            List<string> splitted = new List<string>(name.Split(new[] { ' ' }));
            int dump = 0;

            if (int.TryParse(splitted[splitted.Count - 1], out dump))
                splitted.RemoveAt(splitted.Count - 1);

            return string.Join(" ", splitted);
        }

        internal struct RawMMEFile
        {
            public List<MMEAttribute> Attributes;
            public List<string> AdditionalComments;
            public List<double> Values;
        }
    }
}
