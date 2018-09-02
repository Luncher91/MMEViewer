using System;
using System.Collections.Generic;
using System.IO;

namespace MMEData
{
    public class MMEAttribute
    {
        public const string MME_ATTRIBUTE_COMMENT_KEY = "Comments";
        public const string MME_NOVALUE = "NOVALUE";
        public const int MAX_ATTRIBUTE_KEY_LENGTH = 28;
        public const int MAX_ATTRIBUTE_LINE_LENGTH = 80;

        public bool IsCustomAttribute { get; set; }

        private string name;
        public string Name {
            get
            {
                return name;
            }

            set
            {
                if (value.StartsWith("."))
                {
                    name = value.Substring(1);
                    IsCustomAttribute = true;
                }
                else
                    name = value;
            }
        }

        public string Value { get; set; }
        public List<string> Comments { get; private set; }

        public MMEAttribute()
        {
            IsCustomAttribute = false;
            Name = "";
            Value = "";
            Comments = new List<string>();
        }

        public static string ParseFromStream(StreamReader reader, out MMEAttribute newAttribute)
        {
            newAttribute = new MMEAttribute();

            while(!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (line.Trim() == "")
                    continue;

                string[] splittedLine = null;

                try
                {
                    splittedLine = SplitMMEAttributeLine(line);
                }
                catch(ArgumentOutOfRangeException)
                {
                    // we can no longer split the line 
                    // and so we return the line we were not able to split
                    return line;
                }

                if (!line.StartsWith("Comments"))
                {
                    newAttribute.Name = splittedLine[0];
                    newAttribute.Value = splittedLine[1];
                    break;
                }
                else
                {
                    newAttribute.Comments.Add(splittedLine[1]);
                }
            }

            return "";
        }

        private static string[] SplitMMEAttributeLine(string line)
        {
            string name = line.Substring(0, MAX_ATTRIBUTE_KEY_LENGTH).Trim();
            string value = line.Substring(MAX_ATTRIBUTE_KEY_LENGTH + 1).Trim();

            if (value == MME_NOVALUE)
                value = "";

            return new[] { name, value };
        }
    }
}
