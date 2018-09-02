using System;
using System.Collections.Generic;
using MMEData;
using OxyPlot;
using System.Linq;
using MMEDataMath;

namespace MMEViewer.MMEViewModel
{
    internal class MMEChannelModel : IMMESubTestModel
    {
        private MMEChannel chn;

        public MMEChannel ActualChannel
        {
            get
            {
                return chn;
            }
        }

        public MMEChannelModel(MMEChannel chn)
        {
            this.chn = chn;
        }

        public List<IMMESubTestModel> SubElements
        {
            get
            {
                return new List<IMMESubTestModel>();
            }
        }

        public List<DataPoint> OxyDataPoints { get; private set; }

        public void RefreshDataPoints()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            ActualChannel.Load();
            ActualChannel.CFCFilter(MainWindow.CutOffFrequency);

            foreach(Tuple<double, double> v in ActualChannel.GetDataPoints())
            {
                dataPoints.Add(new DataPoint(v.Item1, v.Item2));
            }

            // OxyDataPoints = ReduceElements(dataPoints, 100, Avg);
            OxyDataPoints = dataPoints;
        }

        private List<DataPoint> Avg(List<DataPoint> arg)
        {
            double minDataPoint = arg.Min(a => a.Y);
            double maxDataPoint = arg.Max(a => a.Y);

            List<DataPoint> reducedSegment = new List<DataPoint>();

            if (arg.Count >= 2)
            {
                if (arg.Count >= 4)
                {
                    reducedSegment.Add(arg.First());
                    reducedSegment.Add(arg.Last());
                }

                reducedSegment.Add(arg.First(a => a.Y == minDataPoint));
            }

            reducedSegment.Add(arg.Last(a => a.Y == maxDataPoint));                

            return reducedSegment;
        }

        private List<DataPoint> ReduceElements(List<DataPoint> data, int segmentLength, Func<List<DataPoint>, List<DataPoint>> reduceFunction)
        {
            List<DataPoint> reducedSet = new List<DataPoint>();
            List<DataPoint> segment = new List<DataPoint>();

            for (int i = 0; i < data.Count; i++)
            {
                segment.Add(data[i]);

                if ((i+1) % segmentLength == 0)
                {
                    reducedSet.AddRange(reduceFunction(segment));
                    segment.Clear();
                }
            }

            if(segment.Any())
            {
                reducedSet.AddRange(reduceFunction(segment));
            }

            return reducedSet.OrderBy(a => a.X).ToList();
        }

        public string YUnit
        {
            get
            {
                if(ActualChannel.Attributes.ContainsKey("Unit"))
                    return ActualChannel.Attributes["Unit"].Value;

                return "";
            }
        }
    }
}