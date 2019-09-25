using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class DPD
    {
        #region Type_Definitionss
        public struct CommonConfiguration
        {
            public double DpdMeasurementInterval_s;                                                 
            public RFmxSpecAnMXDpdMeasurementSampleRateMode DpdSampleRateMode;
            public double SampleRate_Hz;
            public RFmxSpecAnMXDpdSignalType SignalType;
            public RFmxSpecAnMXDpdPreDpdCfrEnabled PreDpdCFREnabled;
            public RFmxSpecAnMXDpdApplyDpdCfrEnabled ApplyDpdCFREnabled;
            public double DutAverageInputPower;
            public double Timeout_s;                                                              


            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    Timeout_s = 3,
                    DpdMeasurementInterval_s = 100e-6,
                    DpdSampleRateMode = RFmxSpecAnMXDpdMeasurementSampleRateMode.ReferenceWaveform,
                    SampleRate_Hz = 320e6,
                    SignalType = RFmxSpecAnMXDpdSignalType.Modulated,
                    DutAverageInputPower = -20,
                    PreDpdCFREnabled = RFmxSpecAnMXDpdPreDpdCfrEnabled.False,
                    ApplyDpdCFREnabled = RFmxSpecAnMXDpdApplyDpdCfrEnabled.False
                };
            }

        }
        public struct LookupTableConfiguration
        {
            public RFmxSpecAnMXDpdLookupTableThresholdType ThresholdType;
            public RFmxSpecAnMXDpdLookupTableThresholdEnabled ThresholdEnabled;
            public double ThresholdLevel;
            public double LookupTableStepSize;
            public RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType LookupTableCorrectionType;
            public RFmxSpecAnMXDpdLookupTableType LookupTableType;

            public static LookupTableConfiguration GetDefault()
            {
                return new LookupTableConfiguration
                {
                    LookupTableType = RFmxSpecAnMXDpdLookupTableType.Linear,
                    ThresholdType = RFmxSpecAnMXDpdLookupTableThresholdType.Relative,
                    ThresholdEnabled = RFmxSpecAnMXDpdLookupTableThresholdEnabled.True,
                    ThresholdLevel = -20,
                    LookupTableStepSize = 0.1,
                    LookupTableCorrectionType = RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType.MagnitudeAndPhase

                };
            }
        }

        public struct MemoryPolynomialConfiguration
        {
            public RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType MemoryModelCorrectionType;
            public RFmxSpecAnMXDpdIterativeDpdEnabled IterativeDpdEnabled;
            public int NumberOfIteration;
            public int MemoryPolynomialOrder, MemoryPolynomialDepth, MemoryPolynomialLeadOrder, MemoryPolynomialLagOrder,
            MemoryPolynomialLeadMemoryDepth, MemoryPolynomialLagMemoryDepth, MemoryPolynomialMaxLead, MemoryPolynomialMaxLag;
            public static MemoryPolynomialConfiguration GetDefault()
            {
                return new MemoryPolynomialConfiguration
                {
                    MemoryModelCorrectionType = RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType.MagnitudeAndPhase,
                    IterativeDpdEnabled = RFmxSpecAnMXDpdIterativeDpdEnabled.False,
                    NumberOfIteration = 3,
                    MemoryPolynomialDepth = 2,
                    MemoryPolynomialOrder = 3,
                    MemoryPolynomialLeadOrder = 2,
                    MemoryPolynomialLagOrder = 2,
                    MemoryPolynomialLeadMemoryDepth = 2,
                    MemoryPolynomialLagMemoryDepth = 2,
                    MemoryPolynomialMaxLead = 2,
                    MemoryPolynomialMaxLag = 2
                };
            }
        }

        public struct LookupTableResults
        {
            public ComplexWaveform<ComplexSingle> WaveformWithDpdComplexSingle;
            public float[] LookupTableInputPowers;
            public ComplexSingle[] LookupTableComplexGains;
            public double PostDPDPAPR;
            public double PowerOffset;
        }

        public struct MemoryPolynomialResults
        {
            public ComplexWaveform<ComplexSingle> WaveformWithDpdComplexSingle;
            public ComplexSingle[] DpdPolynomial;
            public double PostDPDPAPR;
            public double PowerOffset;
        }
        #endregion
        #region ConfigureDPD
        public static void ConfigureCommon(RFmxSpecAnMX specAnSignal, CommonConfiguration commonConfig, Waveform waveform, string selectorString = "")
        {
            RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent idlePresent;
     
            idlePresent = waveform.IdleDurationPresent ? RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.False;

            specAnSignal.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Dpd, true);

            specAnSignal.Dpd.Configuration.ConfigureReferenceWaveform(selectorString, waveform.WaveformData, idlePresent, commonConfig.SignalType);
            specAnSignal.Dpd.Configuration.ConfigureDutAverageInputPower(selectorString, commonConfig.DutAverageInputPower);
            specAnSignal.Dpd.Configuration.ConfigureMeasurementInterval(selectorString, commonConfig.DpdMeasurementInterval_s);
            specAnSignal.Dpd.Configuration.ConfigureMeasurementSampleRate(selectorString, commonConfig.DpdSampleRateMode, commonConfig.SampleRate_Hz);

        }

        public static void ConfigureRFmxLUT(RFmxSpecAnMX specAnSignal, LookupTableConfiguration lutConfig, string selectorString = "")
        {
            specAnSignal.Dpd.Configuration.ConfigureDpdModel(selectorString, RFmxSpecAnMXDpdModel.LookupTable);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableType(selectorString, lutConfig.LookupTableType);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableThreshold(selectorString, lutConfig.ThresholdEnabled, lutConfig.ThresholdLevel,
                lutConfig.ThresholdType);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableStepSize(selectorString, lutConfig.LookupTableStepSize);
        }

        public static void ConfigureRFmxMP(RFmxSpecAnMX specAnSignal, MemoryPolynomialConfiguration mpConfig, string selectorString = "")
        {
            specAnSignal.Dpd.Configuration.ConfigureDpdModel(selectorString, RFmxSpecAnMXDpdModel.GeneralizedMemoryPolynomial);
            specAnSignal.Dpd.Configuration.ConfigureMemoryPolynomial(selectorString, mpConfig.MemoryPolynomialOrder, mpConfig.MemoryPolynomialDepth);
            specAnSignal.Dpd.Configuration.ConfigureGeneralizedMemoryPolynomialCrossTerms(selectorString, mpConfig.MemoryPolynomialLeadOrder,
                mpConfig.MemoryPolynomialLagOrder, mpConfig.MemoryPolynomialLeadMemoryDepth, mpConfig.MemoryPolynomialLagMemoryDepth,
                mpConfig.MemoryPolynomialMaxLead, mpConfig.MemoryPolynomialMaxLag);
            specAnSignal.Dpd.Configuration.ConfigureIterativeDpdEnabled(selectorString, mpConfig.IterativeDpdEnabled);
            specAnSignal.Dpd.ApplyDpd.ConfigureMemoryModelCorrectionType(selectorString, mpConfig.MemoryModelCorrectionType);
        }
        #endregion

        #region ApplyDPD
        public static LookupTableResults ApplyLookupTableDPD(RFmxSpecAnMX specAnSignal, NIRfsg rfsgSession, CommonConfiguration commonConfig, LookupTableConfiguration lutConfig, Waveform waveform, string selectorString = "")
        {
            LookupTableResults lutResults = new LookupTableResults();
            IntPtr rfsgPtr;
            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent idlePresent;
            idlePresent = waveform.IdleDurationPresent ? RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;
            rfsgPtr = rfsgSession.GetInstrumentHandle().DangerousGetHandle();
            specAnSignal.Initiate("", selectorString);
            specAnSignal.Dpd.ApplyDpd.ConfigureLookupTableCorrectionType(selectorString, lutConfig.LookupTableCorrectionType);
            specAnSignal.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, waveform.WaveformData, idlePresent, commonConfig.Timeout_s, ref lutResults.WaveformWithDpdComplexSingle,
                out lutResults.PostDPDPAPR, out lutResults.PowerOffset);

            waveform.WaveformData = lutResults.WaveformWithDpdComplexSingle;
            waveform.SampleRate = 1 / lutResults.WaveformWithDpdComplexSingle.PrecisionTiming.SampleInterval.TotalSeconds;
            waveform.PAPR_dB = lutResults.PostDPDPAPR;
            waveform.SignalBandwidth_Hz = 0.8 * waveform.SampleRate;

            SG.DownloadWaveform(rfsgSession, waveform);
            rfsgSession.RF.PowerLevel = commonConfig.DutAverageInputPower - lutResults.PowerOffset;
            rfsgSession.Initiate();
            specAnSignal.Dpd.Results.FetchLookupTable(selectorString, commonConfig.Timeout_s, ref lutResults.LookupTableInputPowers, ref lutResults.LookupTableComplexGains);
            return lutResults;

        }

        public static MemoryPolynomialResults ApplyPolynomialDPD(RFmxSpecAnMX specAnSignal, NIRfsg rfsgSession, CommonConfiguration commonConfig, MemoryPolynomialConfiguration mpConfig, Waveform waveform, string selectorString = "")
        {
            MemoryPolynomialResults mPResults = new MemoryPolynomialResults();
            IntPtr rfsgPtr;
            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent idlePresent;
            idlePresent = waveform.IdleDurationPresent ? RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;
            rfsgPtr = rfsgSession.GetInstrumentHandle().DangerousGetHandle();
            if (mpConfig.IterativeDpdEnabled == RFmxSpecAnMXDpdIterativeDpdEnabled.False)
            {
                mpConfig.NumberOfIteration = 1;
            }

            for (int i = 0; i < mpConfig.NumberOfIteration; i++)
            {
                specAnSignal.Dpd.Configuration.ConfigurePreviousDpdPolynomial(selectorString, mPResults.DpdPolynomial);
                specAnSignal.Initiate("", selectorString);
                specAnSignal.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, waveform.WaveformData, idlePresent, commonConfig.Timeout_s, ref mPResults.WaveformWithDpdComplexSingle,
               out mPResults.PostDPDPAPR, out mPResults.PowerOffset);

                waveform.WaveformData = mPResults.WaveformWithDpdComplexSingle;
                waveform.SampleRate = 1 / mPResults.WaveformWithDpdComplexSingle.PrecisionTiming.SampleInterval.TotalSeconds;
                waveform.PAPR_dB = mPResults.PostDPDPAPR;
                waveform.SignalBandwidth_Hz = 0.8 * waveform.SampleRate;

                SG.DownloadWaveform(rfsgSession, waveform);
                rfsgSession.RF.PowerLevel = commonConfig.DutAverageInputPower - mPResults.PowerOffset;
                rfsgSession.Initiate();
                specAnSignal.Dpd.Results.FetchDpdPolynomial(selectorString, commonConfig.Timeout_s, ref mPResults.DpdPolynomial);
            }
            return mPResults;


        }
        #endregion


    }
}

