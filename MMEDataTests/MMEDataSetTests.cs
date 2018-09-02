using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMEData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMEData.Tests
{
    [TestClass()]
    public class MMEDataSetTests
    {
        [TestMethod()]
        public void MMETestTest3239()
        {
            DataValidationTest(@".\testdata\3239\", 62, 133, new[] { 31, 35 }, 440495);
        }

        [TestMethod()]
        public void MMETestTest3239_lc()
        {
            DataValidationTest(@".\testdata\3239_lc\", 62, 133, new[] { 31, 35 }, 635903);
        }

        [TestMethod()]
        public void MMETestTestAK3T02SI()
        {
            DataValidationTest(@".\testdata\AK3T02SI\", 44, 79, new[] { 33 }, 474000);
        }

        [TestMethod()]
        public void MMETestTestAK3T02FO()
        {
            DataValidationTest(@".\testdata\AK3T02FO\", 35, 97, new[] { 33 }, 582000);
        }

        [TestMethod()]
        public void MMETestTestVW1FGS15()
        {
            DataValidationTest(@".\testdata\VW1FGS15\", 44, 10, new[] { 27 }, 1364);
        }

        [TestMethod()]
        public void MMETestTest987707()
        {
            DataValidationTest(@".\testdata\98_7707\", 62, 0, new int[0], 0);
        }

        private void DataValidationTest(string path, int numberAttributesData, int numberOfChannels, int[] numberAttributesChannels, int numberOfValues)
        {
            long valueCounter = 0;
            MMEDataSet mmeTest = new MMEDataSet(path);

            Assert.AreEqual(numberAttributesData, mmeTest.Attributes.Count);
            Assert.AreEqual(numberOfChannels, mmeTest.Channels.Count);

            foreach (var channel in mmeTest.Channels)
            {
                channel.Load();
                valueCounter += channel.Values.Count;

                if (!numberAttributesChannels.Contains(channel.Attributes.Count))
                    Assert.Fail("Attributes count is not in the given list of [" +
                        string.Join(",", numberAttributesChannels) + "]: " +
                        channel.Attributes.Count);


                Assert.AreEqual(
                    channel.Attributes["Number of samples"].Value,
                    channel.Values.Count.ToString());
            }

            Assert.AreEqual(numberOfValues, valueCounter);
            Debug.WriteLine(valueCounter);
        }
    }
}