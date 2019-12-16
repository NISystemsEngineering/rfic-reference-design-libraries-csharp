using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Methods
{
    /// <summary>Defines common types and methods for implementing DPD with NI-RFmx.</summary>
    public static class RFmxDPD
    {
        #region Type Definitions

        /// <summary>Defines commmon settings for the application of crest factor reduction on the reference waveform prior to applying DPD for a single carrier channel.</summary>
        public struct PreDpdCrestFactorReductionCarrierChannel
        {
            /// <summary>Specifies the carrier offset relative to the center of the complex baseband equivalent of the RF signal.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double Offset_Hz;
            /// <summary>Specifies the carrier bandwidth. See the RFmx help for more documention of this parameter.</summary>
            public double Bandwidth_Hz;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static PreDpdCrestFactorReductionCarrierChannel GetDefault()
            {
                return new PreDpdCrestFactorReductionCarrierChannel
                {
                    Offset_Hz = 0.000,
                    Bandwidth_Hz = 20e6
                };
            }
        }

        /// <summary>Defines commmon settings for the application of crest factor reduction on the reference waveform prior to applying DPD.</summary>
        public struct PreDpdCrestFactorReductionConfiguration
        {
            /// <summary>Specifies whether to enable the crest factor reduction (CFR) when applying pre-DPD signal conditioning.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdPreDpdCfrEnabled Enabled;
            /// <summary>Specifies the method used to perform crest factor reduction (CFR). See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdPreDpdCfrMethod Method;
            /// <summary>Specifies the maximum number of iterations allowed to converge waveform PAPR to target PAPR.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int MaxIterations;
            /// <summary>Specifies the target peak-to-average power ratio. See the RFmx help for more documention of this parameter.</summary>
            public double TargetPapr_dB;
            /// <summary>Specifies the window type to be used when you set <see cref="Method"/> to Peak Windowing.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdPreDpdCfrWindowType WindowType;
            /// <summary>Specifies the maximum window length to be used when you set <see cref="Method"/> to Peak Windowing.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int WindowLength;
            /// <summary>Specifies the shaping factor to be used when you set <see cref="Method"/> to Sigmoid.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double ShapingFactor;
            /// <summary>Specifies the shaping threshold to be used when you set <see cref="Method"/> to Sigmoid.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double ShapingThreshold_dB;
            /// <summary>Specifies whether to enable the filtering operation. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdPreDpdCfrFilterEnabled FilterEnabled;
            /// <summary>Specifies an array of carrier channel configurations for the signal.</summary>
            public PreDpdCrestFactorReductionCarrierChannel[] CarrierChannels;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
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

        /// <summary>Defines common settings for the application of DPD, regardless of which model is used.</summary>
        public struct CommonConfiguration
        {
            /// <summary>Specifies the duration of the reference waveform considered for the DPD measurement. See the RFmx help for more documention of this parameter.</summary>
            public double MeasurementInterval_s;
            /// <summary>Specifies whether the reference waveform is a modulated signal or a combination of one or more sinusoidal signals.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdSignalType SignalType;
            /// <summary>Specifies the method used for synchronization of the acquired waveform with the reference waveform.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdSynchronizationMethod SynchronizationMethod;
            /// <summary>Specifies the average power of the signal at the device under test input port. This value is expressed in dBm.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double DutAverageInputPower_dBm;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
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

        /// <summary>Defines commmon settings for the application of crest factor reduction on the predistorted waveform after DPD is applied.</summary>
        public struct ApplyDpdCrestFactorReductionConfiguration
        {
            /// <summary>Specifies whether to enable the crest factor reduction (CFR) on the pre-distorted waveform.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdApplyDpdCfrEnabled Enabled;
            /// <summary>Specifies the method used to perform the crest factor reduction (CFR). See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdApplyDpdCfrMethod Method;
            /// <summary>Specifies the maximum number of iterations allowed to converge waveform PAPR to target PAPR.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int MaxIterations;
            /// <summary>Specifies the target PAPR type. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdApplyDpdCfrTargetPaprType TargetPaprType;
            /// <summary>Specifies the target PAPR when you set <see cref="TargetPaprType"/> to Custom.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double TargetPapr_dB;
            /// <summary>Specifies the window type to be used when you set <see cref="Method"/> to Peak Windowing.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdApplyDpdCfrWindowType WindowType;
            /// <summary>Specifies the maximum window length to be used when you set <see cref="Method"/> to Peak Windowing.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int WindowLength;
            /// <summary>Specifies the shaping factor to be used when you set <see cref="Method"/> to Sigmoid.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double ShapingFactor;
            /// <summary>Specifies the shaping threshold to be used when you set <see cref="Method"/> to Sigmoid.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double ShapingThreshold_dB;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
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

        /// <summary>Defines commmon settings for lookup table based DPD.</summary>
        public struct LookupTableConfiguration
        {
            /// <summary>Specifies the type of the DPD lookup table (LUT). See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdLookupTableType Type;
            /// <summary>Specifies the predistortion type. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType CorrectionType;
            /// <summary>Specifies whether to enable thresholding of the acquired samples to be used for the DPD measurement.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdLookupTableThresholdEnabled ThresholdEnabled;
            /// <summary>Specifies the reference for the power level used for thresholding. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdLookupTableThresholdType ThresholdType;
            /// <summary>Specifies either the relative or absolute threshold power level based on the value of <see cref="ThresholdType"/>.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double ThresholdLevel_dB_or_dBm;
            /// <summary>Specifies the step size of the input power levels in the predistortion lookup table.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double StepSize_dB;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static LookupTableConfiguration GetDefault()
            {
                return new LookupTableConfiguration
                {
                    Type = RFmxSpecAnMXDpdLookupTableType.Linear,
                    CorrectionType = RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType.MagnitudeAndPhase,
                    ThresholdEnabled = RFmxSpecAnMXDpdLookupTableThresholdEnabled.True,
                    ThresholdType = RFmxSpecAnMXDpdLookupTableThresholdType.Relative,
                    ThresholdLevel_dB_or_dBm = -20,
                    StepSize_dB = 0.1
                };
            }
        }

        /// <summary>Defines commmon settings for memory polynomial based DPD.</summary>
        public struct MemoryPolynomialConfiguration
        {
            /// <summary>Specifies the predistortion type. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType CorrectionType;
            /// <summary>Specifies the number of iterations over which DPD polynomial is computed using indirect-learning architecture.</summary>
            public int NumberOfIterations;
            /// <summary>Specifies the order of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int Order;
            /// <summary>Specifies the memory depth of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int Depth;
            /// <summary>Specifies the lead order cross term of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int LeadOrder;
            /// <summary>Specifies the lag order cross term of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int LagOrder;
            /// <summary>Specifies the lead memory depth cross term of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int LeadMemoryDepth;
            /// <summary>Specifies the lag memory depth cross term of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int LagMemoryDepth;
            /// <summary>Specifies the maximum lead stagger cross term of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int MaximumLead;
            /// <summary>Specifies the maximum lag stagger cross term of the DPD polynomial. See the RFmx help for more documention of this parameter.</summary>
            public int MaximumLag;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
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

        /// <summary>Defines common power results after the application of DPD.</summary>
        public struct PowerResults
        {
            /// <summary>Returns the change in the average power in the waveform due to applying digital predistion. This value is expressed in dB.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double WaveformPowerOffset_dB;
            /// <summary>Returns the actual PAPR for the pre-distorted waveform. The PAPR of the pre-distorted waveform is set to this value plus
            ///  <see cref="WaveformPowerOffset_dB"/> so that the user-specified power level of the generator is not modified. This behavior
            ///  ensures that the user does not have to change the power level to switch between the original and pre-distorted waveforms,
            ///  while ensuring the pre-distorted waveform is generated correctly.</summary>
            public double WaveformTruePapr_dB;
            /// <summary>Returns the power level of the signal generator during DPD model training.</summary>
            public double TrainingPower_dBm;
        }

        /// <summary>Defines common results after the application of lookup table based DPD.</summary>
        public struct LookupTableResults
        {
            /// <summary>Returns the <see cref="Waveform"/> whose data represents the complex baseband equivalent of the RF signal
            /// after applying digital pre-distortion. See the RFmx help for more documention of this parameter.</summary>
            public Waveform PredistortedWaveform;
            /// <summary>Returns the lookup table power levels, in dBm. See the RFmx help for more documention of this parameter.</summary>
            public float[] InputPowers_dBm;
            /// <summary>Returns the lookup table complex gain values, in dB, for magnitude and phase predistortion.
            /// See the RFmx help for more documention of this parameter.</summary>
            public ComplexSingle[] ComplexGains_dB;
            /// <summary>Returns common power results after digital predistortion.</summary>
            public PowerResults PowerResults;
        }

        /// <summary>Defines common results after the application of memory polynomial based DPD.</summary>
        public struct MemoryPolynomialResults
        {
            /// <summary>Returns the <see cref="Waveform"/> whose data represents the complex baseband equivalent of the RF signal
            /// after applying digital pre-distortion. See the RFmx help for more documention of this parameter.</summary>
            public Waveform PredistortedWaveform;
            /// <summary>Returns the memory polynomial or generalized memory polynomial coefficients. See the RFmx help for more documention of this parameter.</summary>
            public ComplexSingle[] Polynomial;
            /// <summary>Returns common power results after digital predistortion.</summary>
            public PowerResults PowerResults;
        }
        #endregion

        #region ConfigureDPD

        /// <summary>Configures common pre-DPD CFR settings and applies CFR to the reference waveform, which is returned by the function.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="referenceWaveform">Specifies the <see cref="Waveform"/> whose data defines the complex baseband equivalent of the RF signal on which the 
        /// pre-DPD signal conditioning is applied. See the RFmx help for more documention of this parameter.</param>
        /// <param name="preDpdCfrConfig">Specifies common pre-DPD CFR settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        /// <returns>The reference waveform with CFR applied.</returns>
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
                preDpdCfrWaveform.UpdateNameAndScript(referenceWaveform.Name + "preDPDCFR");

                // Apply CFR and return
                specAn.Dpd.PreDpd.ApplyPreDpdSignalConditioning(selectorString, referenceWaveform.Data, preDpdCfrIdlePresent, ref preDpdCfrWaveform.Data, out preDpdCfrWaveform.PAPR_dB);
            }
            return preDpdCfrWaveform;
        }

        /// <summary>Configures common settings for the application of DPD, regardless of which model is used.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="commonConfig">Specifies the common settings to apply.</param>
        /// <param name="referenceWaveform">Specifies the <see cref="Waveform"/> whose data defines the complex baseband equivalent of the RF signal applied at the input 
        /// port of the device under test when performing the measurement. See the RFmx help for more documention of this parameter.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureCommon(RFmxSpecAnMX specAn, CommonConfiguration commonConfig, Waveform referenceWaveform, string selectorString = "")
        {
            RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent idlePresent = referenceWaveform.IdleDurationPresent ?
                RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent.False;
            specAn.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Dpd, false);
            specAn.Dpd.Configuration.ConfigureReferenceWaveform(selectorString, referenceWaveform.Data, idlePresent, commonConfig.SignalType);
            specAn.Dpd.Configuration.ConfigureDutAverageInputPower(selectorString, commonConfig.DutAverageInputPower_dBm);
            specAn.Dpd.Configuration.ConfigureMeasurementInterval(selectorString, commonConfig.MeasurementInterval_s);
            specAn.Dpd.Configuration.ConfigureMeasurementSampleRate(selectorString, RFmxSpecAnMXDpdMeasurementSampleRateMode.ReferenceWaveform, referenceWaveform.SampleRate);
            specAn.Dpd.Configuration.ConfigureSynchronizationMethod(selectorString, commonConfig.SynchronizationMethod);
        }

        /// <summary>Configures commmon settings for the application of crest factor reduction on the predistorted waveform after DPD is applied.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="applyDpdCfrConfig">Specifies the apply DPD CFR settings to configure.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
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

        /// <summary>Configures common settings for lookup table based DPD.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="lutConfig">Specifies the common lookup table settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureLookupTable(RFmxSpecAnMX specAn, LookupTableConfiguration lutConfig, string selectorString = "")
        {
            specAn.Dpd.Configuration.ConfigureDpdModel(selectorString, RFmxSpecAnMXDpdModel.LookupTable);
            specAn.Dpd.Configuration.ConfigureLookupTableType(selectorString, lutConfig.Type);
            specAn.Dpd.Configuration.ConfigureLookupTableThreshold(selectorString, lutConfig.ThresholdEnabled, lutConfig.ThresholdLevel_dB_or_dBm, lutConfig.ThresholdType);
            specAn.Dpd.Configuration.ConfigureLookupTableStepSize(selectorString, lutConfig.StepSize_dB);
            specAn.Dpd.ApplyDpd.ConfigureLookupTableCorrectionType(selectorString, lutConfig.CorrectionType);
        }

        /// <summary>Configures common settings for memory polynomial based DPD.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="mpConfig">Specifies the memory polynomial settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
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

        /// <summary>Acquires the incoming signal, trains the DPD model, applies the lookup table to the reference waveform, and downloads the predistorted waveform to the generator.
        ///  If generation is in progress when this function is called, generation will resume with the predistorted waveform.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="rfsgSession">Specifies the open RFSG session to configure.</param>
        /// <param name="referenceWaveform">Specifies the <see cref="Waveform"/> whose data defines the complex baseband equivalent of the RF signal applied at the input 
        /// port of the device under test when performing the measurement. See the RFmx help for more documention of this parameter.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        /// <returns>Common results after the application of lookup table based DPD.</returns>
        public static LookupTableResults PerformLookupTable(RFmxSpecAnMX specAn, NIRfsg rfsgSession, Waveform referenceWaveform, string selectorString = "")
        {
            //Instantiate new waveform with reference waveform properties
            LookupTableResults lutResults = new LookupTableResults()
            {
                PredistortedWaveform = referenceWaveform,
            };
            lutResults.PredistortedWaveform.Data = referenceWaveform.Data.Clone(); // clone waveform so RFmx can't act on reference waveform
            lutResults.PredistortedWaveform.UpdateNameAndScript(referenceWaveform.Name + "postLutDpd");

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

        /// <summary>Acquires the incoming signal, trains the DPD model, applies the memory polynomial to the reference waveform, and downloads the predistorted waveform to the generator.
        ///  If generation is in progress when this function is called, generation will resume with the predistorted waveform.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="rfsgSession">Specifies the open RFSG session to configure.</param>
        /// <param name="referenceWaveform">Specifies the <see cref="Waveform"/> whose data defines the complex baseband equivalent of the RF signal applied at the input 
        /// port of the device under test when performing the measurement. See the RFmx help for more documention of this parameter.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        /// <returns>Common results after the application of memory polynomial based DPD.</returns>
        public static MemoryPolynomialResults PerformMemoryPolynomial(RFmxSpecAnMX specAn, NIRfsg rfsgSession, MemoryPolynomialConfiguration mpConfig,
            Waveform referenceWaveform, string selectorString = "")
        {
            //Instantiate new waveform with reference waveform properties
            MemoryPolynomialResults mpResults = new MemoryPolynomialResults()
            {
                PredistortedWaveform = referenceWaveform
            };
            mpResults.PredistortedWaveform.Data = referenceWaveform.Data.Clone(); // clone waveform so RFmx can't act on reference waveform
            mpResults.PredistortedWaveform.UpdateNameAndScript(referenceWaveform.Name + "postMpDpd");

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