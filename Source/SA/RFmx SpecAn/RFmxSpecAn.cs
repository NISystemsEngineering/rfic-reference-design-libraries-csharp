using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public class RFmxSpecAn
    {
        #region Type Definitions
        public struct CommonConfiguration
        {
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double Span_Hz;
            public double ExternalAttenuation_dB;
            public string FrequencyReferenceSource;
            public string DigitalEdgeSource;
            public RFmxSpecAnMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            public bool EnableTrigger;
            public string LOSource;
            public double LOOffset;
        }
        public static CommonConfiguration GetDefaultCommonConfiguration()
        {
            return new CommonConfiguration
            {
                CenterFrequency_Hz = 1e9,
                ReferenceLevel_dBm = 0,
                Span_Hz = 1e6,
                ExternalAttenuation_dB = 0,
                FrequencyReferenceSource = RFmxInstrMXConstants.PxiClock,
                DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0,
                DigitalEdgeType = RFmxSpecAnMXDigitalEdgeTriggerEdge.Rising,
                TriggerDelay_s = 0,
                EnableTrigger = true,
                LOSource = RFmxInstrMXConstants.LOSourceLOIn,
                LOOffset = 0
            };
        }
        public struct AutoLevelConfiguration
        {
            public bool AutoLevelReferenceLevel;
            public double AutoLevelMeasureTime_s;
        }
        public static AutoLevelConfiguration GetDefaultAutoLevelConfiguration()
        {
            return new AutoLevelConfiguration
            {
                AutoLevelReferenceLevel = false,
                AutoLevelMeasureTime_s = 10e-3
        };
        }
        #endregion
        #region Instrument Configurations
        public static void ConfigureCommon( RFmxInstrMX sessionHandle, RFmxSpecAnMX specAnSignal, CommonConfiguration commonConfig, 
            AutoLevelConfiguration autoLevelConfig, string selectorString = "")
        {
            sessionHandle.ConfigureFrequencyReference("", commonConfig.FrequencyReferenceSource, 10e6);
            sessionHandle.SetLOSource("", commonConfig.LOSource);
            sessionHandle.SetDownconverterFrequencyOffset("", commonConfig.LOOffset);
            specAnSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalEdgeSource, commonConfig.DigitalEdgeType, commonConfig.TriggerDelay_s, commonConfig.EnableTrigger);
            specAnSignal.ConfigureFrequency(selectorString, commonConfig.CenterFrequency_Hz);
            specAnSignal.Spectrum.Configuration.ConfigureSpan(selectorString, commonConfig.Span_Hz);
            specAnSignal.ConfigureExternalAttenuation(selectorString, commonConfig.ExternalAttenuation_dB);

            if (autoLevelConfig.AutoLevelReferenceLevel) specAnSignal.AutoLevel(selectorString, commonConfig.Span_Hz, autoLevelConfig.AutoLevelMeasureTime_s, out _);
            else specAnSignal.ConfigureReferenceLevel(selectorString, commonConfig.ReferenceLevel_dBm);

        }
        #endregion
    }
}

