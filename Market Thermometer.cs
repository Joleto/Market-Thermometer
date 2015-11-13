using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, AutoRescale = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MarketThermometer : Indicator
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Periods", DefaultValue = 14)]
        public int Periods { get; set; }

        [Output("Market Thermometer", Color = Colors.Yellow, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries thermometer { get; set; }

        [Output("EMA Market Thermometer", Color = Colors.Red, PlotType = PlotType.Line)]
        public IndicatorDataSeries emaThermometer { get; set; }

        // Default setting for EMAPeriod
        private bool veryFirstTime = true;
        private double todayHigh = 100;
        private double yesterdayHigh = 100;
        private double todayLow = 100;
        private double yesterdayLow = 100;

        private IndicatorDataSeries atr;
        private ExponentialMovingAverage ema;

        protected override void Initialize()
        {
            atr = CreateDataSeries();
            ema = Indicators.ExponentialMovingAverage(atr, 13);
        }

        public override void Calculate(int index)
        {

            if (veryFirstTime)
            {

                thermometer[index] = 0;
                atr[index] = thermometer[index];
                yesterdayHigh = MarketSeries.High[index];
                yesterdayLow = MarketSeries.Low[index];
                veryFirstTime = false;

            }
            else
            {

                todayHigh = MarketSeries.High[index];
                todayLow = MarketSeries.Low[index];

                if ((todayHigh - yesterdayHigh) >= (todayLow - yesterdayLow))
                {

                    thermometer[index] = (Math.Abs(todayHigh - yesterdayHigh));
                    //if (atr.Count >= Periods)
                    atr[index] = thermometer[index];
                    //emaThermometer[index] = ema.Result[index];


                }
                else
                {
                    thermometer[index] = (Math.Abs(todayLow - yesterdayLow));
                    //if (atr.Count >= Periods)
                    atr[index] = thermometer[index];

                }

                emaThermometer[index] = ema.Result[index];
                yesterdayHigh = todayHigh;
                yesterdayLow = todayLow;

            }
        }
    }
}
