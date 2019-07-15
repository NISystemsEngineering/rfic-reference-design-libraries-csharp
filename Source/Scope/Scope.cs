using System;
using NationalInstruments.ModularInstruments.NIScope;

namespace NationalInstruments.ReferenceDesignLibraries
{
    public static class Scope
    {
        public struct ScopeConfiguration
        {
            public enum ScopeInputImpedance { FiftyOhm = 50, OneMegaOhm = 1000000 }

            //Vertical Settigns
            public double VerticalRange_V;
            public double VerticalOffset_V;
            public ScopeVerticalCoupling ScopeCouplingMode;
            public double ProbeAttenuation;

            //General Settings
            public ScopeInputImpedance InputImpedance;
            public string ScopeClockSource;
            
        }
        public static ScopeConfiguration GetDefaultScopeConfiguration()
        {
            return new ScopeConfiguration
            {
                VerticalRange_V = 1,
                VerticalOffset_V = 0,
                ScopeCouplingMode = ScopeVerticalCoupling.DC,
                ProbeAttenuation = 1,
                InputImpedance = ScopeConfiguration.ScopeInputImpedance.FiftyOhm,
                ScopeClockSource = ScopeInputClockSource.PxiClock.ToString(),
            };
        }
        public struct MeasurementConfiguration
        {
            public ScopeTriggerType ScopeTriggerType;
            public string ScopeTriggerSource;
            public ScopeTriggerSlope TriggerEdge;
            public double SampleRate_Hz;
            public double MeasurementTime_s;
        }
        public static MeasurementConfiguration GetDefaultMeasurementConfiguration()
        {
            return new MeasurementConfiguration
            {
                ScopeTriggerType = ScopeTriggerType.DigitalEdge,
                ScopeTriggerSource = ScopeTriggerSource.Rtsi0, //Equivalent to PXI_Trig_0
                TriggerEdge = ScopeTriggerSlope.Positive, //Rising edge
                SampleRate_Hz = 20.00E+6,
                MeasurementTime_s = 1e-3,
            };
        }
        public struct MeasurementResults
        {
            public double[] ResultsTrace;
            public double AverageValue_V;
        }
        public static void ConfigureScope(NIScope scope, ScopeConfiguration scopeConfig, string channelNames = "0")
        {
            scope.Channels[channelNames].Coupling = scopeConfig.ScopeCouplingMode;
            scope.Channels[channelNames].InputImpedance = (double)scopeConfig.InputImpedance;
            scope.Channels[channelNames].Range = scopeConfig.VerticalRange_V;
            scope.Channels[channelNames].Offset = scopeConfig.VerticalOffset_V;
            scope.Channels[channelNames].ProbeAttenuation = scopeConfig.ProbeAttenuation;

            scope.Timing.ReferenceClockSource = ScopeInputClockSource.FromString(scopeConfig.ScopeClockSource);
            
            scope.Channels[channelNames].Enabled = true;
        }
        public static void ConfigureMeasurement(NIScope scope, MeasurementConfiguration measurementConfig, string channelNames = "0")
        {
            switch (measurementConfig.ScopeTriggerType)
            {
                case ScopeTriggerType.DigitalEdge:
                    scope.Trigger.ConfigureTriggerDigital(ScopeTriggerSource.FromString(measurementConfig.ScopeTriggerSource),
                        measurementConfig.TriggerEdge, PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero);
                    break;
                case ScopeTriggerType.Immediate:
                    scope.Trigger.ConfigureTriggerImmediate();
                    break;
                case ScopeTriggerType.Software:
                    scope.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero);
                    break;
                default:
                    throw new System.NotImplementedException("The functionality for the requested NI-SCOPE trigger type has not been implemented.");
            }

            scope.Acquisition.SampleRateMin = measurementConfig.SampleRate_Hz;
            scope.Acquisition.NumberOfPointsMin = (long)Math.Round(scope.Acquisition.SampleRate * measurementConfig.MeasurementTime_s);

            scope.Timing.NumberOfRecordsToAcquire = 1;
        }
        public static MeasurementResults MeasureChannel(NIScope scope, string channelName = "0")
        {
            MeasurementResults results = new MeasurementResults();

            scope.Measurement.Initiate();
            AnalogWaveformCollection<double> resultWaveform = new AnalogWaveformCollection<double>();
            resultWaveform = scope.Channels[channelName].Measurement.FetchDouble(PrecisionTimeSpan.FromSeconds(5), -1, null);
            results.ResultsTrace = resultWaveform[0].GetRawData();
            results.AverageValue_V = scope.Channels[channelName].Measurement.FetchScalarMeasurement(PrecisionTimeSpan.FromSeconds(5),
                ScopeScalarMeasurementType.VoltageAverage)[0];
            scope.Measurement.Abort();

            return results;
        }
    }
}
