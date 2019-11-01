using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Methods
{
    public static class RFmxDPD
    {
        #region Type Definitionss
        public struct CommonConfiguration
        {
            public double MeasurementInterval_s;
            public RFmxSpecAnMXDpdSignalType SignalType;
            public RFmxSpecAnMXDpdSynchronizationMethod SynchronizationMethod;
            public double DutAverageInputPower_dBm;

            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    MeasurementInterval_s = 100e-6,
                    SignalType = RFmxSpecAnMXDpdSignalType.Modulated,
                    DutAverageInputPower_dBm = -20.0,
                    SynchronizationMethod = RFmxSpecAnMXDpdSynchronizationMethod.Direct
                };
            }
        }

        public struct LookupTableConfiguration
        {
            public RFmxSpecAnMXDpdLookupTableType Type;
            public RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType CorrectionType;
            public RFmxSpecAnMXDpdLookupTableThresholdEnabled ThresholdEnabled;
            public RFmxSpecAnMXDpdLookupTableThresholdType ThresholdType;
            public double ThresholdLevel_dB;
            public double StepSize_dB;

            public static LookupTableConfiguration GetDefault()
            {
                return new LookupTableConfiguration
                {
                    Type = RFmxSpecAnMXDpdLookupTableType.Linear,
                    CorrectionType = RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType.MagnitudeAndPhase,
                    ThresholdEnabled = RFmxSpecAnMXDpdLookupTableThresholdEnabled.True,
                    ThresholdType = RFmxSpecAnMXDpdLookupTableThresholdType.Relative,
                    ThresholdLevel_dB = -20,
                    StepSize_dB = 0.1
                };
            }
        }

        public struct MemoryPolynomialConfiguration
        {
            public RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType CorrectionType;
            public int NumberOfIterations;
            public int Order, Depth, LeadOrder, LagOrder, LeadMemoryDepth, LagMemoryDepth, MaximumLead, MaximumLag;

            public static MemoryPolynomialConfiguration GetDefault()
            {
                return new MemoryPolynomialConfiguration
                {
                    CorrectionType = RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType.MagnitudeAndPhase,
                    NumberOfIterations = 3,
                    Order = 3,
                    Depth = 2,
                    LeadOrder = 2,
                    LagOrder = 2,
                    LeadMemoryDepth = 2,
                    LagMemoryDepth = 2,
                    MaximumLead = 2,
                    MaximumLag = 2
                };
            }
        }

        public struct PowerResults
        {
            public double WaveformPowerOffset_dB;
            public double WaveformTruePapr_dB;
            public double TrainingPower_dBm;
        }

        public struct LookupTableResults
        {
            public Waveform PredistortedWaveform;
            public float[] InputPowers_dBm;
            public ComplexSingle[] ComplexGains_dB;
            public PowerResults PowerResults;
        }

        public struct MemoryPolynomialResults
        {
            public Waveform PredistortedWaveform;
            public ComplexSingle[] Polynomial;
            public PowerResults PowerResults;
        }
        #endregion

        #region ConfigureDPD
        public static void ConfigureCommon(RFmxSpecAnMX specAn, CommonConfiguration commonConfig, Waveform referenceWaveform, string selectorString = "")
        {
            RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent idlePresent = referenceWaveform.IdleDurationPresent ? RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.False;
            specAn.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Dpd, true);
            specAn.Dpd.Configuration.ConfigureReferenceWaveform(selectorString, referenceWaveform.WaveformData, idlePresent, commonConfig.SignalType);
            specAn.Dpd.Configuration.ConfigureDutAverageInputPower(selectorString, commonConfig.DutAverageInputPower_dBm);
            specAn.Dpd.Configuration.ConfigureMeasurementInterval(selectorString, commonConfig.MeasurementInterval_s);
            specAn.Dpd.Configuration.ConfigureMeasurementSampleRate(selectorString, RFmxSpecAnMXDpdMeasurementSampleRateMode.ReferenceWaveform, referenceWaveform.SampleRate);
            specAn.Dpd.Configuration.ConfigureSynchronizationMethod(selectorString, commonConfig.SynchronizationMethod);           
        }

        public static void ConfigureLookupTable(RFmxSpecAnMX specAn, LookupTableConfiguration lutConfig, string selectorString = "")
        {
            specAn.Dpd.Configuration.ConfigureDpdModel(selectorString, RFmxSpecAnMXDpdModel.LookupTable);
            specAn.Dpd.Configuration.ConfigureLookupTableType(selectorString, lutConfig.Type);
            specAn.Dpd.Configuration.ConfigureLookupTableThreshold(selectorString, lutConfig.ThresholdEnabled, lutConfig.ThresholdLevel_dB, lutConfig.ThresholdType);
            specAn.Dpd.Configuration.ConfigureLookupTableStepSize(selectorString, lutConfig.StepSize_dB);
            specAn.Dpd.ApplyDpd.ConfigureLookupTableCorrectionType(selectorString, lutConfig.CorrectionType);
        }

        public static void ConfigureMemoryPolynomial(RFmxSpecAnMX specAn, MemoryPolynomialConfiguration mpConfig, string selectorString = "")
        {
            specAn.Dpd.Configuration.ConfigureDpdModel(selectorString, RFmxSpecAnMXDpdModel.GeneralizedMemoryPolynomial);
            specAn.Dpd.Configuration.ConfigureMemoryPolynomial(selectorString, mpConfig.Order, mpConfig.Depth);
            specAn.Dpd.Configuration.ConfigureGeneralizedMemoryPolynomialCrossTerms(selectorString, mpConfig.LeadOrder,
                mpConfig.LagOrder, mpConfig.LeadMemoryDepth, mpConfig.LagMemoryDepth, mpConfig.MaximumLead, mpConfig.MaximumLag);
            RFmxSpecAnMXDpdIterativeDpdEnabled iterativeDpdEnabled = mpConfig.NumberOfIterations < 1 ? RFmxSpecAnMXDpdIterativeDpdEnabled.True : RFmxSpecAnMXDpdIterativeDpdEnabled.False;
            specAn.Dpd.Configuration.ConfigureIterativeDpdEnabled(selectorString, iterativeDpdEnabled);
            specAn.Dpd.ApplyDpd.ConfigureMemoryModelCorrectionType(selectorString, mpConfig.CorrectionType);
        }
        #endregion

        #region PerformDPD
        public static LookupTableResults PerformLookupTable(RFmxSpecAnMX specAn, NIRfsg rfsgSession, Waveform referenceWaveform, string selectorString = "")
        {
            //Instantiate new waveform with reference waveform properties
            LookupTableResults lutResults = new LookupTableResults()
            {
                PredistortedWaveform = referenceWaveform,
            };
            lutResults.PredistortedWaveform.WaveformName = referenceWaveform.WaveformName + "postLutDpd";
            lutResults.PredistortedWaveform.WaveformData = referenceWaveform.WaveformData.Clone(); // clone waveform so RFmx can't act on reference waveform
            lutResults.PredistortedWaveform.Script = lutResults.PredistortedWaveform.Script?.Replace(referenceWaveform.WaveformName, lutResults.PredistortedWaveform.WaveformName);
            
            RfsgGenerationStatus preDpdGenerationStatus = rfsgSession.CheckGenerationStatus();
            if (preDpdGenerationStatus == RfsgGenerationStatus.Complete)
                rfsgSession.Initiate(); // initiate if not already generating
            
            specAn.Initiate(selectorString, "");
            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent idlePresent = referenceWaveform.IdleDurationPresent ? RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;
            specAn.WaitForMeasurementComplete(selectorString, 10.0); // wait for LUT creation to finish
            //waveform data and PAPR are overwritten in post DPD waveform
            specAn.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, referenceWaveform.WaveformData, idlePresent, 10.0, ref lutResults.PredistortedWaveform.WaveformData,
                out lutResults.PowerResults.WaveformTruePapr_dB, out lutResults.PowerResults.WaveformPowerOffset_dB);

            //Waveform's PAPR is modified to adjust the output power of the waveform on a per waveform basis rather than changing the
            //user's configured output power on the instrument
            lutResults.PredistortedWaveform.PAPR_dB = lutResults.PowerResults.WaveformPowerOffset_dB + lutResults.PowerResults.WaveformTruePapr_dB;
            DownloadWaveform(rfsgSession, lutResults.PredistortedWaveform); // implicit call to rfsg abort
            ApplyWaveformAttributes(rfsgSession, lutResults.PredistortedWaveform);
            lutResults.PowerResults.TrainingPower_dBm = rfsgSession.RF.PowerLevel;
            specAn.Dpd.Results.FetchLookupTable(selectorString, 10.0, ref lutResults.InputPowers_dBm, ref lutResults.ComplexGains_dB);
            
            if (preDpdGenerationStatus == RfsgGenerationStatus.InProgress)
                rfsgSession.Initiate(); // restart generation if it was running on function call

            return lutResults;
        }

        public static MemoryPolynomialResults PerformMemoryPolynomial(RFmxSpecAnMX specAn, NIRfsg rfsgSession, MemoryPolynomialConfiguration mpConfig, 
            Waveform referenceWaveform, string selectorString = "")
        {
            //Instantiate new waveform with reference waveform properties
            MemoryPolynomialResults mpResults = new MemoryPolynomialResults()
            {
                PredistortedWaveform = referenceWaveform
            };
            mpResults.PredistortedWaveform.WaveformName = referenceWaveform.WaveformName + "postMpDpd";
            mpResults.PredistortedWaveform.WaveformData = referenceWaveform.WaveformData.Clone(); // clone waveform so RFmx can't act on reference waveform
            mpResults.PredistortedWaveform.Script = mpResults.PredistortedWaveform.Script?.Replace(referenceWaveform.WaveformName, mpResults.PredistortedWaveform.WaveformName);
            
            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent idlePresent = referenceWaveform.IdleDurationPresent ? RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;

            RfsgGenerationStatus preDpdGenerationStatus = rfsgSession.CheckGenerationStatus();
            rfsgSession.Abort(); // abort so we don't mess with the loop logic

            for (int i = 0; i < mpConfig.NumberOfIterations; i++)
            {
                specAn.Dpd.Configuration.ConfigurePreviousDpdPolynomial(selectorString, mpResults.Polynomial);
                rfsgSession.Initiate();
                specAn.Initiate(selectorString, "");
                specAn.WaitForMeasurementComplete(selectorString, 10.0); // wait for polynomial coefficients to be calculated
                //waveform data and PAPR are overwritten in post DPD waveform
                specAn.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, referenceWaveform.WaveformData, idlePresent, 10.0, ref mpResults.PredistortedWaveform.WaveformData,
                    out mpResults.PowerResults.WaveformTruePapr_dB, out mpResults.PowerResults.WaveformPowerOffset_dB);
                //Waveform's PAPR is modified to adjust the output power of the waveform on a per waveform basis rather than changing the
                //user's configured output power on the instrument
                mpResults.PredistortedWaveform.PAPR_dB = mpResults.PowerResults.WaveformPowerOffset_dB + mpResults.PowerResults.WaveformTruePapr_dB;
                DownloadWaveform(rfsgSession, mpResults.PredistortedWaveform); // implicit abort
                ApplyWaveformAttributes(rfsgSession, mpResults.PredistortedWaveform);
                specAn.Dpd.Results.FetchDpdPolynomial(selectorString, 10.0, ref mpResults.Polynomial);
            }

            mpResults.PowerResults.TrainingPower_dBm = rfsgSession.RF.PowerLevel;
            if (preDpdGenerationStatus == RfsgGenerationStatus.InProgress)
                rfsgSession.Initiate(); // restart generation if it was running on function call

            return mpResults;
        }
        #endregion
    }
}

