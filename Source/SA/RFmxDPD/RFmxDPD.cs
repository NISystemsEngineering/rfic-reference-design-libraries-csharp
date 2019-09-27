using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxDPD
    {
        #region Type_Definitionss
        public struct CommonConfiguration
        {
            public double MeasurementInterval_s;                                                 
            public RFmxSpecAnMXDpdMeasurementSampleRateMode SampleRateMode;
            public double SampleRate_Hz;
            public RFmxSpecAnMXDpdSignalType SignalType;
            public RFmxSpecAnMXDpdPreDpdCfrEnabled PreDpdCFREnabled;
            public RFmxSpecAnMXDpdApplyDpdCfrEnabled ApplyDpdCFREnabled;
            public double DutAverageInputPower_dBm;
                                                            

            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    MeasurementInterval_s = 100e-6,
                    SampleRateMode = RFmxSpecAnMXDpdMeasurementSampleRateMode.ReferenceWaveform,
                    SampleRate_Hz = 320e6,
                    SignalType = RFmxSpecAnMXDpdSignalType.Modulated,
                    DutAverageInputPower_dBm = -20,
                    PreDpdCFREnabled = RFmxSpecAnMXDpdPreDpdCfrEnabled.False,
                    ApplyDpdCFREnabled = RFmxSpecAnMXDpdApplyDpdCfrEnabled.False
                };
            }

        }
        public struct LookupTableConfiguration
        {
            public RFmxSpecAnMXDpdLookupTableThresholdEnabled ThresholdEnabled;
            public RFmxSpecAnMXDpdLookupTableThresholdType ThresholdType;
            public double ThresholdLevel_dB;
            public double LookupTableStepSize_dB;
            public RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType LookupTableCorrectionType;
            public RFmxSpecAnMXDpdLookupTableType LookupTableType;

            public static LookupTableConfiguration GetDefault()
            {
                return new LookupTableConfiguration
                {
                    LookupTableType = RFmxSpecAnMXDpdLookupTableType.Linear,
                    ThresholdType = RFmxSpecAnMXDpdLookupTableThresholdType.Relative,
                    ThresholdEnabled = RFmxSpecAnMXDpdLookupTableThresholdEnabled.True,
                    ThresholdLevel_dB = -20,
                    LookupTableStepSize_dB = 0.1,
                    LookupTableCorrectionType = RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType.MagnitudeAndPhase

                };
            }
        }

        public struct MemoryPolynomialConfiguration
        {
            public RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType CorrectionType;
            public RFmxSpecAnMXDpdIterativeDpdEnabled IterativeDpdEnabled;
            public int NumberOfIterations;
            public int Order, Depth, LeadOrder, LagOrder,LeadMemoryDepth, LagMemoryDepth, MaxLead, MaxLag;
            public static MemoryPolynomialConfiguration GetDefault()
            {
                return new MemoryPolynomialConfiguration
                {
                    CorrectionType = RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType.MagnitudeAndPhase,
                    IterativeDpdEnabled = RFmxSpecAnMXDpdIterativeDpdEnabled.False,
                    NumberOfIterations = 3,
                    Depth = 2,
                    Order = 3,
                    LeadOrder = 2,
                    LagOrder = 2,
                    LeadMemoryDepth = 2,
                    LagMemoryDepth = 2,
                    MaxLead = 2,
                    MaxLag = 2
                };
            }
        }

        public struct LookupTableResults
        {
            public SG.Waveform PostDpdWaveform;
            public float[] LookupTableInputPowers_dBm;
            public ComplexSingle[] LookupTableComplexGains_dB;
            public double PostDpdPapr_dB;
            public double PowerOffset_dB;
        }

        public struct MemoryPolynomialResults
        {
            public SG.Waveform PostDpdWaveform;
            public ComplexSingle[] DpdPolynomial;
            public double PostDpdPapr_dB;
            public double PowerOffset_dB;
        }
        #endregion
        #region ConfigureDPD
        public static void ConfigureCommon(RFmxSpecAnMX specAnSignal, CommonConfiguration commonConfig, Waveform waveform, string selectorString = "")
        {
            RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent idlePresent;
     
            idlePresent = waveform.IdleDurationPresent ? RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.False;

            specAnSignal.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Dpd, true);

            specAnSignal.Dpd.Configuration.ConfigureReferenceWaveform(selectorString, waveform.WaveformData, idlePresent, commonConfig.SignalType);
            specAnSignal.Dpd.Configuration.ConfigureDutAverageInputPower(selectorString, commonConfig.DutAverageInputPower_dBm);
            specAnSignal.Dpd.Configuration.ConfigureMeasurementInterval(selectorString, commonConfig.MeasurementInterval_s);
            specAnSignal.Dpd.Configuration.ConfigureMeasurementSampleRate(selectorString, commonConfig.SampleRateMode, commonConfig.SampleRate_Hz);
            
        }

        public static void ConfigureLookupTable(RFmxSpecAnMX specAnSignal, LookupTableConfiguration lutConfig, string selectorString = "")
        {
            specAnSignal.Dpd.Configuration.ConfigureDpdModel(selectorString, RFmxSpecAnMXDpdModel.LookupTable);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableType(selectorString, lutConfig.LookupTableType);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableThreshold(selectorString, lutConfig.ThresholdEnabled, lutConfig.ThresholdLevel_dB,
                lutConfig.ThresholdType);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableStepSize(selectorString, lutConfig.LookupTableStepSize_dB);
        }

        public static void ConfigureMemoryPolynomial(RFmxSpecAnMX specAnSignal, MemoryPolynomialConfiguration mpConfig, string selectorString = "")
        {
            specAnSignal.Dpd.Configuration.ConfigureDpdModel(selectorString, RFmxSpecAnMXDpdModel.GeneralizedMemoryPolynomial);
            specAnSignal.Dpd.Configuration.ConfigureMemoryPolynomial(selectorString, mpConfig.Order, mpConfig.Depth);
            specAnSignal.Dpd.Configuration.ConfigureGeneralizedMemoryPolynomialCrossTerms(selectorString, mpConfig.LeadOrder,
                mpConfig.LagOrder, mpConfig.LeadMemoryDepth, mpConfig.LagMemoryDepth,mpConfig.MaxLead, mpConfig.MaxLag);
            specAnSignal.Dpd.Configuration.ConfigureIterativeDpdEnabled(selectorString, mpConfig.IterativeDpdEnabled);
            specAnSignal.Dpd.ApplyDpd.ConfigureMemoryModelCorrectionType(selectorString, mpConfig.CorrectionType);
        }
        #endregion

        #region ApplyDPD
        public static LookupTableResults ApplyLookupTable(RFmxSpecAnMX specAnSignal, NIRfsg rfsgSession, LookupTableConfiguration lutConfig, Waveform waveform, string selectorString = "", double timeout_s = 3)
        {
            LookupTableResults lutResults = new LookupTableResults();       
            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent idlePresent = waveform.IdleDurationPresent ? RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;
            specAnSignal.Initiate("", selectorString);
            specAnSignal.Dpd.ApplyDpd.ConfigureLookupTableCorrectionType(selectorString, lutConfig.LookupTableCorrectionType);
            specAnSignal.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, waveform.WaveformData, idlePresent, timeout_s, ref lutResults.PostDpdWaveform.WaveformData,
                out lutResults.PostDpdPapr_dB, out lutResults.PowerOffset_dB);

            waveform.WaveformData = lutResults.PostDpdWaveform.WaveformData;
            waveform.SampleRate = 1 / lutResults.PostDpdWaveform.WaveformData.PrecisionTiming.SampleInterval.TotalSeconds;
            waveform.PAPR_dB = lutResults.PostDpdPapr_dB;
            waveform.SignalBandwidth_Hz = 0.8 * waveform.SampleRate;

            SG.DownloadWaveform(rfsgSession, waveform);
            rfsgSession.RF.PowerLevel = rfsgSession.RF.PowerLevel - lutResults.PowerOffset_dB;
            rfsgSession.Initiate();
            specAnSignal.Dpd.Results.FetchLookupTable(selectorString, timeout_s, ref lutResults.LookupTableInputPowers_dBm, ref lutResults.LookupTableComplexGains_dB);
            return lutResults;

        }

        public static MemoryPolynomialResults ApplyMemoryPolynomial(RFmxSpecAnMX specAnSignal, NIRfsg rfsgSession, MemoryPolynomialConfiguration mpConfig, Waveform waveform, string selectorString = "", double timeout_s = 3)
        {
            MemoryPolynomialResults mPResults = new MemoryPolynomialResults();
            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent idlePresent = waveform.IdleDurationPresent ? RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;

            if (mpConfig.IterativeDpdEnabled == RFmxSpecAnMXDpdIterativeDpdEnabled.False)
            {
                mpConfig.NumberOfIterations = 1;
            }

            for (int i = 0; i < mpConfig.NumberOfIterations; i++)
            {
                specAnSignal.Dpd.Configuration.ConfigurePreviousDpdPolynomial(selectorString, mPResults.DpdPolynomial);
                specAnSignal.Initiate("", selectorString);
                specAnSignal.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, waveform.WaveformData, idlePresent, timeout_s, ref mPResults.PostDpdWaveform.WaveformData,
               out mPResults.PostDpdPapr_dB, out mPResults.PowerOffset_dB);

                waveform.WaveformData = mPResults.PostDpdWaveform.WaveformData;
                waveform.SampleRate = 1 / mPResults.PostDpdWaveform.WaveformData.PrecisionTiming.SampleInterval.TotalSeconds;
                waveform.PAPR_dB = mPResults.PostDpdPapr_dB;
                waveform.SignalBandwidth_Hz = 0.8 * waveform.SampleRate;

                SG.DownloadWaveform(rfsgSession, waveform);
                rfsgSession.RF.PowerLevel = rfsgSession.RF.PowerLevel - mPResults.PowerOffset_dB;
                rfsgSession.Initiate();
                specAnSignal.Dpd.Results.FetchDpdPolynomial(selectorString, timeout_s, ref mPResults.DpdPolynomial);
            }
            return mPResults;


        }
        #endregion


    }
}

