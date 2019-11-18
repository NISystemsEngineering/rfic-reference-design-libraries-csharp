using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Methods
{
    public static class RFmxDPD
    {
        #region Type Definitions
        public struct PreDpdCrestFactorReductionCarrierChannel
        {
            public double Offset_Hz;
            public double Bandwidth_Hz;

            public static PreDpdCrestFactorReductionCarrierChannel GetDefault()
            {
                return new PreDpdCrestFactorReductionCarrierChannel
                {
                    Offset_Hz = 0.000,
                    Bandwidth_Hz = 20e6
                };
            }
        }

        public struct PreDpdCrestFactorReductionConfiguration
        {
            public RFmxSpecAnMXDpdPreDpdCfrEnabled Enabled;
            public RFmxSpecAnMXDpdPreDpdCfrMethod Method;
            public int MaxIterations;
            public double TargetPapr_dB;
            public RFmxSpecAnMXDpdPreDpdCfrWindowType WindowType;
            public int WindowLength;
            public double ShapingFactor;
            public double ShapingThreshold_dB;
            public RFmxSpecAnMXDpdPreDpdCfrFilterEnabled FilterEnabled;
            public PreDpdCrestFactorReductionCarrierChannel[] CarrierChannels;

            public static PreDpdCrestFactorReductionConfiguration GetDefault()
            {
                return new PreDpdCrestFactorReductionConfiguration
                {
                    Enabled = RFmxSpecAnMXDpdPreDpdCfrEnabled.True,
                    Method = RFmxSpecAnMXDpdPreDpdCfrMethod.Clipping,
                    MaxIterations = 10,
                    TargetPapr_dB = 8,
                    WindowType = RFmxSpecAnMXDpdPreDpdCfrWindowType.KaiserBessel,
                    WindowLength = 10,
                    ShapingFactor = 5,
                    ShapingThreshold_dB = -5,
                    FilterEnabled = RFmxSpecAnMXDpdPreDpdCfrFilterEnabled.False,
                    CarrierChannels = new PreDpdCrestFactorReductionCarrierChannel[] { PreDpdCrestFactorReductionCarrierChannel.GetDefault() }
                };
            }
        }

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

        public struct ApplyDpdCrestFactorReductionConfiguration
        {
            public RFmxSpecAnMXDpdApplyDpdCfrEnabled Enabled;
            public RFmxSpecAnMXDpdApplyDpdCfrMethod Method;
            public int MaxIterations;
            public RFmxSpecAnMXDpdApplyDpdCfrTargetPaprType TargetPaprType;
            public double TargetPapr_dB;
            public RFmxSpecAnMXDpdApplyDpdCfrWindowType WindowType;
            public int WindowLength;
            public double ShapingFactor;
            public double ShapingThreshold_dB;

            public static ApplyDpdCrestFactorReductionConfiguration GetDefault()
            {
                return new ApplyDpdCrestFactorReductionConfiguration
                {
                    Enabled = RFmxSpecAnMXDpdApplyDpdCfrEnabled.True,
                    Method = RFmxSpecAnMXDpdApplyDpdCfrMethod.Clipping,
                    MaxIterations = 10,
                    TargetPaprType = RFmxSpecAnMXDpdApplyDpdCfrTargetPaprType.InputPapr,
                    TargetPapr_dB = 8,
                    WindowType = RFmxSpecAnMXDpdApplyDpdCfrWindowType.KaiserBessel,
                    WindowLength = 10,
                    ShapingFactor = 5,
                    ShapingThreshold_dB = -5,
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
        public static Waveform ConfigurePreDpdCrestFactorReduction(RFmxSpecAnMX specAn, Waveform referenceWaveform, PreDpdCrestFactorReductionConfiguration preDpdCfrConfig, string selectorString = "")
        {
            Waveform preDpdCfrWaveform = referenceWaveform;

            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent preDpdCfrIdlePresent = preDpdCfrWaveform.IdleDurationPresent ?
                RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;
            specAn.Dpd.PreDpd.SetCfrEnabled(selectorString, preDpdCfrConfig.Enabled);
            specAn.Dpd.PreDpd.SetCfrMethod(selectorString, preDpdCfrConfig.Method);
            specAn.Dpd.PreDpd.SetCfrMaximumIterations(selectorString, preDpdCfrConfig.MaxIterations);
            specAn.Dpd.PreDpd.SetCfrTargetPapr(selectorString, preDpdCfrConfig.TargetPapr_dB);
            specAn.Dpd.PreDpd.SetCfrWindowType(selectorString, preDpdCfrConfig.WindowType);
            specAn.Dpd.PreDpd.SetCfrWindowLength(selectorString, preDpdCfrConfig.WindowLength);
            specAn.Dpd.PreDpd.SetCfrShapingFactor(selectorString, preDpdCfrConfig.ShapingFactor);
            specAn.Dpd.PreDpd.SetCfrShapingThreshold(selectorString, preDpdCfrConfig.ShapingThreshold_dB);
            specAn.Dpd.PreDpd.SetCfrFilterEnabled(selectorString, preDpdCfrConfig.FilterEnabled);
            specAn.Dpd.PreDpd.SetCfrNumberOfCarriers(selectorString, preDpdCfrConfig.CarrierChannels.Length);

            // Configure the carrier channels
            for (int i = 0; i < preDpdCfrConfig.CarrierChannels.Length; i++)
            {
                string carrierString = RFmxSpecAnMX.BuildCarrierString2(selectorString, i);
                specAn.Dpd.PreDpd.SetCarrierOffset(carrierString, preDpdCfrConfig.CarrierChannels[i].Offset_Hz);
                specAn.Dpd.PreDpd.SetCarrierBandwidth(carrierString, preDpdCfrConfig.CarrierChannels[i].Bandwidth_Hz);
            }

            // Only call Apply Pre-DPD CFR and modify the waveform if Pre-DPD CFR is enabled
            if (preDpdCfrConfig.Enabled == RFmxSpecAnMXDpdPreDpdCfrEnabled.True)
            {
                // Configure the new waveform
                preDpdCfrWaveform.Data = referenceWaveform.Data.Clone(); // clone waveform so RFmx can't act on reference waveform
                preDpdCfrWaveform.UpdateWaveformNameAndScript(referenceWaveform.Name + "preDPDCFR");

                // Apply CFR and return
                specAn.Dpd.PreDpd.ApplyPreDpdSignalConditioning(selectorString, referenceWaveform.Data, preDpdCfrIdlePresent, ref preDpdCfrWaveform.Data, out preDpdCfrWaveform.PAPR_dB);
            }
            return preDpdCfrWaveform;
        }

        public static void ConfigureCommon(RFmxSpecAnMX specAn, CommonConfiguration commonConfig, Waveform referenceWaveform, string selectorString = "")
        {
            RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent idlePresent = referenceWaveform.IdleDurationPresent ?
                RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.False;
            specAn.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Dpd, true);
            specAn.Dpd.Configuration.ConfigureReferenceWaveform(selectorString, referenceWaveform.Data, idlePresent, commonConfig.SignalType);
            specAn.Dpd.Configuration.ConfigureDutAverageInputPower(selectorString, commonConfig.DutAverageInputPower_dBm);
            specAn.Dpd.Configuration.ConfigureMeasurementInterval(selectorString, commonConfig.MeasurementInterval_s);
            specAn.Dpd.Configuration.ConfigureMeasurementSampleRate(selectorString, RFmxSpecAnMXDpdMeasurementSampleRateMode.ReferenceWaveform, referenceWaveform.SampleRate);
            specAn.Dpd.Configuration.ConfigureSynchronizationMethod(selectorString, commonConfig.SynchronizationMethod);
        }

        public static void ConfigureApplyDpdCrestFactorReduction(RFmxSpecAnMX specAn, ApplyDpdCrestFactorReductionConfiguration applyDpdCfrConfig, string selectorString = "")
        {
            specAn.Dpd.ApplyDpd.SetCfrEnabled(selectorString, applyDpdCfrConfig.Enabled);
            specAn.Dpd.ApplyDpd.SetCfrMethod(selectorString, applyDpdCfrConfig.Method);
            specAn.Dpd.ApplyDpd.SetCfrMaximumIterations(selectorString, applyDpdCfrConfig.MaxIterations);
            specAn.Dpd.ApplyDpd.SetCfrTargetPaprType(selectorString, applyDpdCfrConfig.TargetPaprType);
            specAn.Dpd.ApplyDpd.SetCfrTargetPapr(selectorString, applyDpdCfrConfig.TargetPapr_dB);
            specAn.Dpd.ApplyDpd.SetCfrWindowType(selectorString, applyDpdCfrConfig.WindowType);
            specAn.Dpd.ApplyDpd.SetCfrWindowLength(selectorString, applyDpdCfrConfig.WindowLength);
            specAn.Dpd.ApplyDpd.SetCfrShapingFactor(selectorString, applyDpdCfrConfig.ShapingFactor);
            specAn.Dpd.ApplyDpd.SetCfrShapingThreshold(selectorString, applyDpdCfrConfig.ShapingThreshold_dB);
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
            lutResults.PredistortedWaveform.Data = referenceWaveform.Data.Clone(); // clone waveform so RFmx can't act on reference waveform
            lutResults.PredistortedWaveform.UpdateWaveformNameAndScript(referenceWaveform.Name + "postLutDpd");

            RfsgGenerationStatus preDpdGenerationStatus = rfsgSession.CheckGenerationStatus();
            if (preDpdGenerationStatus == RfsgGenerationStatus.Complete)
                rfsgSession.Initiate(); // initiate if not already generating

            specAn.Initiate(selectorString, "");
            RFmxSpecAnMXDpdApplyDpdIdleDurationPresent idlePresent = referenceWaveform.IdleDurationPresent ? RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.True : RFmxSpecAnMXDpdApplyDpdIdleDurationPresent.False;
            specAn.WaitForMeasurementComplete(selectorString, 10.0); // wait for LUT creation to finish
                                                                     //waveform data and PAPR are overwritten in post DPD waveform
            specAn.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, referenceWaveform.Data, idlePresent, 10.0, ref lutResults.PredistortedWaveform.Data,
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
            mpResults.PredistortedWaveform.Data = referenceWaveform.Data.Clone(); // clone waveform so RFmx can't act on reference waveform
            mpResults.PredistortedWaveform.UpdateWaveformNameAndScript(referenceWaveform.Name + "postMpDpd");

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
                specAn.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, referenceWaveform.Data, idlePresent, 10.0, ref mpResults.PredistortedWaveform.Data,
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