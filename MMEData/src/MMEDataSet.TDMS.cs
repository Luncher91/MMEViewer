using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Tdms;
using System.IO;

namespace MMEData
{
    public partial class MMEDataSet
    {
        public MMEDataSet()
        {
            InitProperties();
        }

        public static MMEDataSet FromTDMS(string tdmsFilePath)
        {
            // TODO: refactor this function
            string fullFilePath = Path.GetFullPath(tdmsFilePath);

            MMEDataSet tdmsData = new MMEDataSet()
            {
                Name = Path.GetFileNameWithoutExtension(fullFilePath),
                RootDir = Path.GetDirectoryName(fullFilePath)
            };

            tdmsData.Load();
            tdmsData.LoadTDMSChannels(fullFilePath);

            return tdmsData;
        }

        private void LoadTDMSChannels(string tdmsFilePath)
        {
            // TODO: refactor this function
            this.Channels = new MMEChannels(this);

            // NationalInstruments.Tdms.File
            using (NationalInstruments.Tdms.File tdmsFile = new NationalInstruments.Tdms.File(tdmsFilePath))
            {
                tdmsFile.Open();

                foreach(var attribute in tdmsFile.Properties)
                {
                    Attributes.Add(new MMEAttribute() {
                        IsCustomAttribute = true,
                        Name = attribute.Key,
                        Value = attribute.Value.ToString()
                    });
                }

                int channelNumber = 1;
                foreach (var channelGroup in tdmsFile)
                {
                    foreach (var tdmsChannel in channelGroup)
                    {
                        MMEChannel mmeChannel = new MMEChannel(
                            new MMEChannelMeta()
                            {
                                Name = channelGroup.Name + "/" + tdmsChannel.Name,
                                Code = tdmsChannel.Name,
                                Number = channelNumber++
                            }
                        );

                        ExtractAttributesFromTdms(tdmsChannel, mmeChannel);
                        ExtractValuesFromTdms(tdmsChannel, mmeChannel);

                        Channels.Add(mmeChannel);
                    }
                }
            }
        }

        private static string MapTDMSChannelAttributeToMME(string tdms)
        {
            // TODO: refactor this function
            switch (tdms.ToLowerInvariant())
            {
                case "description": return "Name of the channel"; // v00566_001aa0.uds; Sensor: type=AC, Attach=VECG, Prefilter Frequency=300; min@0.06s, max@0.173s
                case "unit_string": return "Unit"; // gn
                //case "datatype": return ""; // DT_DOUBLE
                case "minimum": return "First global minimum value"; // -6,37680006027222
                case "maximum": return "First global maximum value"; // 2,02900004386902
                //case "inst_axis": return ""; // XG
                //case "inst_chstat": return ""; // P
                //case "inst_curno": return ""; // 1
                //case "inst_dastat": return ""; // AM
                //case "inst_inivel": return ""; // 95,8
                //case "inst_nfp": return ""; // 0
                //case "inst_nlp": return ""; // 499
                //case "inst_prefil": return ""; // 300
                //case "inst_senatt": return ""; // VECG
                //case "inst_senloc": return ""; // NA
                //case "inst_sentyp": return ""; // AC
                case "inst_ytype": return "Dimension"; // ACCELERATION
                //case "inst_yunits": return ""; // gn
                //case "sourcename": return ""; // NHTSA R&D Database, US DOT
                //case "test_clsspd": return ""; // 95,8
                //case "test_impang": return ""; // 285
                //case "test_offset": return ""; // 0
                //case "test_tstcfn": return ""; // VTB
                //case "test_tstdat": return ""; // 3/31/31
                //case "test_tstno": return ""; // 566
                //case "test_tstprf": return ""; // TTI
                //case "test_tstref": return ""; // 4798-5
                //case "test_tsttyp": return ""; // RSB
                //case "veh_body": return ""; // 3H
                //case "veh_crbang": return ""; // 0
                //case "veh_maked": return ""; // HONDA
                //case "veh_modeld": return ""; // CIVIC
                //case "veh_nhtsano": return ""; // 
                //case "veh_pdof": return ""; // 15
                //case "veh_vehno": return ""; // 1
                //case "veh_vehspd": return ""; // 95,8
                //case "veh_vehtwt": return ""; // 953
                //case "veh_year": return ""; // 1978
                case "wf_increment": return MMEChannel.MME_CHANNEL_SAMPLE_INTERVAL_ATTRIBUTE; // 0,001
                //case "wf_samples": return ""; // 1
                case "wf_start_offset": return MMEChannel.MME_CHANNEL_TIME_OF_FIRST_SAMPLE_ATTRIBUTE; // 0
                //case "wf_start_time": return ""; // 31.03.1983 07:00:00
                //case "wf_xname": return ""; // Time
                //case "wf_xunit_string": return ""; // s
                default: return tdms;
            }
        }

        private void ExtractValuesFromTdms(Channel tdmsChannel, MMEChannel mmeChannel)
        {
            // TODO: refactor this function
            if (tdmsChannel.DataType != typeof(double))
                return;

            var rawData = tdmsChannel.GetData<double>();
            foreach (var rawValue in rawData)
            {
                mmeChannel.Values.Add(rawValue);
            }
        }

        private static void ExtractAttributesFromTdms(Channel tdmsChannel, MMEChannel mmeChannel)
        {
            // TODO: refactor this function
            foreach (var tdmsProperty in tdmsChannel.Properties)
            {
                string mappedName;
                mappedName = tdmsProperty.Key;
                mappedName = MapTDMSChannelAttributeToMME(tdmsProperty.Key);

                string value = tdmsProperty.Value.ToString();
                if (value.Contains(",") && !value.Contains("."))
                    value = value.Replace(",", ".");

                mmeChannel.Attributes.Add(
                    mappedName,
                    new MMEAttribute()
                    {
                        Name = mappedName,
                        Value = value,
                        IsCustomAttribute = tdmsProperty.Key == mappedName
                    }
                );
            }
        }
    }
}
