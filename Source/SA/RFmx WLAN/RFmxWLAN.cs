using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    /// <summary>Defines common types and methods for NI-RFmx WLAN measurements.</summary>
    public static class RFmxWLAN
    {
        #region Type Definitions
        
        /// <summary>Defines common settings related to the standard of the measured WLAN signal.</summary>
        public struct StandardConfiguration
        {
            /// <summary>Specifies the signal under analysis as defined under IEEE Standard 802.11. See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXStandard Standard;
            /// <summary>Specifies the channel bandwidth in Hz. See the RFmx help for more documention of this parameter.</summary>
            public double ChannelBandwidth_Hz;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static StandardConfiguration GetDefault()
            {
                return new StandardConfiguration
                {
                    Standard = RFmxWlanMXStandard.Standard802_11ag,
                    ChannelBandwidth_Hz = 20e6
                };
            }
        }
        
        /// <summary>Defines common settings for the TxP measurement.</summary>
        public struct TxPConfiguration
        {
            /// <summary>Specifies whether to enable averaging for the TXP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXTxpAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging, when you set the <see cref="AveragingEnabled"/> parameter to True.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies the maximum measurement interval in seconds. See the RFmx help for more documention of this parameter.</summary>
            public double MaximumMeasurementInterval_s;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static TxPConfiguration GetDefault()
            {
                return new TxPConfiguration
                {
                    AveragingEnabled = RFmxWlanMXTxpAveragingEnabled.False,
                    AveragingCount = 10,
                    MaximumMeasurementInterval_s = 1e-3
                };
            }
        }

        /// <summary>Defines common results of the TxP measurement.</summary>
        public struct TxPResults
        {
            /// <summary>Specifies the average power of the acquired signal. This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double AveragePowerMean_dBm;
            /// <summary>Specifies the peak power of the acquired signal. This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double PeakPowerMaximum_dBm;
        }

        /// <summary>Defines common settings for the OFDM ModAcc measurement.</summary>
        public struct OFDMModAccConfiguration
        {
            /// <summary>Specifies the length of the waveform to be acquired in seconds. If the default value of 0 is used,
            /// <see cref="ConfigureOFDMModAcc(RFmxWlanMX, OFDMModAccConfiguration, string)"/> will set the acquisition length mode property to 
            /// <see cref="RFmxWlanMXOfdmModAccAcquisitionLengthMode.Auto"/>. See the RFmx help for more documention of this parameter.</summary>
            public double AcquisitionLength_s;
            /// <summary>Specifies the number of data symbols to be ignored from the start of the data field for EVM computation. This value is expressed as a number of symbols.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public int MeasurementOffset_sym;
            /// <summary>Specifies the maximum number of OFDM symbols that the measurement uses to compute EVM. This value is expressed as a number of symbols.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public int MaximumMeasurementLength_sym;
            /// <summary>Specifies whether to optimize the analyzer's dynamic range for the EVM measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXOfdmModAccOptimizeDynamicRangeForEvmEnabled OptimizeDynamicRangeForEvmEnabled;
            /// <summary>Specifies the margin above the reference level you specify when you set <see cref="OptimizeDynamicRangeForEvmEnabled"/> to True. This value is expressed in dB.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public double OptimizeDynamicRangeForEVMMargin_dB;
            /// <summary>Specifies whether to enable averaging for OFDMModAcc measurements. See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXOfdmModAccAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging, when you set <see cref="AveragingEnabled"/> to True.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static OFDMModAccConfiguration GetDefault()
            {
                return new OFDMModAccConfiguration
                {
                    AcquisitionLength_s = 0, //This will trigger auto-length mode in the code
                    MeasurementOffset_sym = 0,
                    MaximumMeasurementLength_sym = 16,
                    OptimizeDynamicRangeForEvmEnabled = RFmxWlanMXOfdmModAccOptimizeDynamicRangeForEvmEnabled.True,
                    OptimizeDynamicRangeForEVMMargin_dB = 0,
                    AveragingEnabled = RFmxWlanMXOfdmModAccAveragingEnabled.False,
                    AveragingCount = 10,
                };
            }
        }

        /// <summary>Defines common results of the OFDM ModAcc measurement.</summary>
        public struct OFDMModAccResults
        {
            /// <summary>Specifies the RMS EVM of all subcarriers in all OFDM symbols. This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double CompositeRMSEVMMean_dB;
            /// <summary>Specifies the RMS EVM of data-subcarriers in all OFDM symbols. This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double CompositeDataRMSEVMMean_dB;
            /// <summary>Specifies the RMS EVM of pilot-subcarriers in all OFDM symbols. This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double CompositePilotRMSEVMMean_dB;
            /// <summary>Specifies the number of OFDM symbols used by the measurement. See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfSymbolsUsed;
        }

        /// <summary>Defines common settings for the SEM measurement.</summary>
        public struct SEMConfiguration
        {
            /// <summary>Specifies whether the sweep time for the SEM measurement is computed automatically by the measurement or is configured manually.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXSemSweepTimeAuto SweepTimeAuto;
            /// <summary>Specifies the sweep time for the SEM measurement. This value is expressed in seconds.  
            /// This property is ignored when <see cref="SweepTimeAuto"/> is True. See the RFmx help for more documention of this parameter.</summary>
            public double SweepTime_s;
            /// <summary>Specifies whether the frequency range of the spectrum used for the SEM measurement is computed automatically by the measurement or manually.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXSemSpanAuto SpanAuto;
            /// <summary>Specifies the frequency range of the spectrum that is used for the SEM measurement. This value is expressed in Hz. 
            /// This parameter is applicable only when you set <see cref="SpanAuto"/> to False. See the RFmx help for more documention of this parameter.</summary>
            public double Span_Hz;
            /// <summary>Specifies whether to enable averaging for the SEM measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXSemAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set <see cref="AveragingEnabled"/> to True. See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies the averaging type for averaging multiple spectrum acquisitions. The averaged spectrum is used for SEM measurement.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXSemAveragingType AveragingType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static SEMConfiguration GetDefault()
            {
                return new SEMConfiguration
                {
                    SweepTimeAuto = RFmxWlanMXSemSweepTimeAuto.True,
                    SweepTime_s = 1e-3,
                    SpanAuto = RFmxWlanMXSemSpanAuto.True,
                    Span_Hz = 100e6,
                    AveragingEnabled = RFmxWlanMXSemAveragingEnabled.False,
                    AveragingCount = 5,
                    AveragingType = RFmxWlanMXSemAveragingType.Rms,
                };
            }
        }

        /// <summary>Defines common results of the SEM measurement.</summary>
        public struct SEMResults
        {
            /// <summary>Specifies  the overall measurement status, indicating whether the spectrum exceeds the SEM measurement mask limits in any of the offset segments.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXSemMeasurementStatus measurementStatus;
            /// <summary>Specifies the average power of the carrier channel over the bandwidth indicated by the SEM Carrier IBW property. This value is expressed in dBm.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public double AbsolutePower_dBm;
            /// <summary>Specifies the average power of the carrier channel, relative to the peak power of the carrier channel, over the bandwidth indicated by the SEM Carrier IBW property.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public double RelativePower_dB;
            /// <summary>Specifies the upper (positive) offset segment measurement status, indicating whether the spectrum exceeds the SEM measurement mask limits in the upper offset segments.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXSemUpperOffsetMeasurementStatus[] upperOffsetMeasurementStatus;
            /// <summary>Specifies the array of margins from the SEM measurement mask for the upper offset. This value is expressed in dB. 
            /// Margin is defined as the maximum difference between the spectrum and the mask. See the RFmx help for more documention of this parameter.</summary>
            public double[] UpperOffsetMargin_dB;
            /// <summary>Specifies the array of frequencies corresponding to the margins for the upper (positive) offsets. See the RFmx help for more documention of this parameter.</summary>
            public double[] UpperOffsetMarginFrequency_Hz;
            /// <summary>Specifies the array of absolute powers corresponding to the margins for the upper offsets. This value is expressed in dBm.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public double[] UpperOffsetMarginAbsolutePower_dBm;
            /// <summary> Specifies the array of relative powers corresponding to the margins for the upper offsets. The relative powers are 
            /// relative to the peak power of the carrier channel.This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double[] UpperOffsetMarginRelativePower_dB;
            /// <summary>Specifies the array of lower (negative) offset segment measurement status, indicating whether the spectrum exceeds the SEM measurement mask limits in the lower offset segments.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public RFmxWlanMXSemLowerOffsetMeasurementStatus[] lowerOffsetMeasurementStatus;
            /// <summary>Margin returns the array of margins from the SEM measurement mask for the lower offset. This value is expressed in dB. 
            /// Margin is defined as the maximum difference between the spectrum and the mask. See the RFmx help for more documention of this parameter.</summary>
            public double[] LowerOffsetMargin_dB;
            /// <summary>Specifies the array of frequencies corresponding to the margins for the lower (negative) offsets. See the RFmx help for more documention of this parameter.</summary>
            public double[] LowerOffsetMarginFrequency_Hz;
            /// <summary>Specifies the array of absolute powers corresponding to the margins for the lower offsets. This value is expressed in dBm.
            ///  See the RFmx help for more documention of this parameter.</summary>
            public double[] LowerOffsetMarginAbsolutePower_dBm;
            /// <summary>Specifies the array of relative powers corresponding to the margins for the lower offsets. The relative powers are 
            /// relative to the peak power of the carrier channel. This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double[] LowerOffsetMarginRelativePower_dB;

        }
        public struct TxPServoConfiguration
        {
            public double TargetTxPPower_dBm;
            public double Tolerance_dBm;
            public int MaxNumberOfIterations;
            public static TxPServoConfiguration GetDefault()
            {
                return new TxPServoConfiguration
                {
                    TargetTxPPower_dBm = 0,
                    Tolerance_dBm = 0.05,
                    MaxNumberOfIterations = 10
                };
            }
        }

        public struct TxPServoResults
        {
            public double[] ServoTrace;
            public double FinalInputPower_dBm;
            public double FinalOutputPower_dBm;
        }
        #endregion
        #region Instrument Configuration

        /// <summary>Configures common measurement settings for the personality.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to configure.</param>
        /// <param name="commonConfig">Specifies the common settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used.
        /// See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureCommon(RFmxWlanMX wlanSignal, CommonConfiguration commonConfig, string selectorString = "")
        {
            wlanSignal.SetSelectedPorts(selectorString, commonConfig.SelectedPorts);
            wlanSignal.ConfigureFrequency(selectorString, commonConfig.CenterFrequency_Hz);
            wlanSignal.ConfigureReferenceLevel(selectorString, commonConfig.ReferenceLevel_dBm);
            wlanSignal.ConfigureExternalAttenuation(selectorString, commonConfig.ExternalAttenuation_dB);
            wlanSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalTriggerSource, RFmxWlanMXDigitalEdgeTriggerEdge.Rising, commonConfig.TriggerDelay_s, commonConfig.TriggerEnabled);
        }
        #endregion
        #region Measurement Configuration

        /// <summary>Configures common settings related to the WLAN standard of the measured signal.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to configure.</param>
        /// <param name="standardConfig">Specifies the WLAN standard settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used.See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureStandard(RFmxWlanMX wlanSignal, StandardConfiguration standardConfig, string selectorString = "")
        {
            wlanSignal.ConfigureStandard(selectorString, standardConfig.Standard);
            wlanSignal.ConfigureChannelBandwidth(selectorString, standardConfig.ChannelBandwidth_Hz);
        }

        /// <summary>Configures common settings for the TxP measurement and selects the measurement.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to configure.</param>
        /// <param name="txPConfig">Specifies the TxP settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used.See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureTxP(RFmxWlanMX wlanSignal, TxPConfiguration txPConfig, string selectorString = "")
        {

            wlanSignal.SelectMeasurements(selectorString, RFmxWlanMXMeasurementTypes.Txp, false);
            //Disabled because we are triggering by default
            wlanSignal.Txp.Configuration.SetBurstDetectionEnabled(selectorString, RFmxWlanMXTxpBurstDetectionEnabled.False);

            wlanSignal.Txp.Configuration.ConfigureAveraging(selectorString, txPConfig.AveragingEnabled, txPConfig.AveragingCount);
            wlanSignal.Txp.Configuration.ConfigureMaximumMeasurementInterval(selectorString, txPConfig.MaximumMeasurementInterval_s);
        }

        /// <summary>Configures common settings for the OFDM ModAcc measurement and selects the measurement.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to configure.</param>
        /// <param name="modAccConfig">Specifies the OFDM ModAcc settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used.See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureOFDMModAcc(RFmxWlanMX wlanSignal, OFDMModAccConfiguration modAccConfig, string selectorString = "")
        {
            wlanSignal.SelectMeasurements(selectorString, RFmxWlanMXMeasurementTypes.OfdmModAcc, false);

            RFmxWlanMXOfdmModAccAcquisitionLengthMode acMode;
            if (modAccConfig.AcquisitionLength_s == 0) acMode = RFmxWlanMXOfdmModAccAcquisitionLengthMode.Auto;
            else acMode = RFmxWlanMXOfdmModAccAcquisitionLengthMode.Manual;

            wlanSignal.OfdmModAcc.Configuration.ConfigureAcquisitionLength(selectorString, acMode, modAccConfig.AcquisitionLength_s);
            wlanSignal.OfdmModAcc.Configuration.ConfigureMeasurementLength(selectorString,
                modAccConfig.MeasurementOffset_sym, modAccConfig.MaximumMeasurementLength_sym);
            wlanSignal.OfdmModAcc.Configuration.ConfigureOptimizeDynamicRangeForEvm(selectorString, modAccConfig.OptimizeDynamicRangeForEvmEnabled,
                modAccConfig.OptimizeDynamicRangeForEVMMargin_dB);
            wlanSignal.OfdmModAcc.Configuration.ConfigureAveraging(selectorString, modAccConfig.AveragingEnabled, modAccConfig.AveragingCount);

            //Disable the following measurements because SG clocks and LOs are being shared; hence, any measured error would be invalid
            wlanSignal.OfdmModAcc.Configuration.SetFrequencyErrorEstimationMethod(selectorString, RFmxWlanMXOfdmModAccFrequencyErrorEstimationMethod.Disabled);
            wlanSignal.OfdmModAcc.Configuration.SetIQGainImbalanceCorrectionEnabled(selectorString, RFmxWlanMXOfdmModAccIQGainImbalanceCorrectionEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetIQQuadratureErrorCorrectionEnabled(selectorString, RFmxWlanMXOfdmModAccIQQuadratureErrorCorrectionEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetIQTimingSkewCorrectionEnabled(selectorString, RFmxWlanMXOfdmModAccIQTimingSkewCorrectionEnabled.False);


            //The following values are defaults, but called out explicitly for clarity
            wlanSignal.OfdmModAcc.Configuration.SetEvmUnit(selectorString, RFmxWlanMXOfdmModAccEvmUnit.dB);
            wlanSignal.OfdmModAcc.Configuration.ConfigureChannelEstimationType(selectorString, RFmxWlanMXOfdmModAccChannelEstimationType.Reference);
            wlanSignal.OfdmModAcc.Configuration.SetBurstStartDetectionEnabled(selectorString, RFmxWlanMXOfdmModAccBurstStartDetectionEnabled.False); //Triggering, so no burst detection
            wlanSignal.OfdmModAcc.Configuration.SetAmplitudeTrackingEnabled(selectorString, RFmxWlanMXOfdmModAccAmplitudeTrackingEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetPhaseTrackingEnabled(selectorString, RFmxWlanMXOfdmModAccPhaseTrackingEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetChannelEstimationSmoothingEnabled(selectorString, RFmxWlanMXOfdmModAccChannelEstimationSmoothingEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetCommonClockSourceEnabled(selectorString, RFmxWlanMXOfdmModAccCommonClockSourceEnabled.True);

        }
        public static TxPServoResults TxPServoPower(RFmxWlanMX wlanSignal, NIRfsg rfsgSession, TxPServoConfiguration servoConfig,
            AutoLevelConfiguration autoLevelConfig, string selectorString = "")
        {
            //Duplicate the existing configuration so that we can select only TxP for the power servo to save time, 
            //but not disrupt all of the other user enabled measurements. 
            wlanSignal.CloneSignalConfiguration("servo_txp", out RFmxWlanMX servoTxpSession);
            servoTxpSession.SelectMeasurements(selectorString, RFmxWlanMXMeasurementTypes.Txp, false);
            double[] servoTrace = new double[servoConfig.MaxNumberOfIterations];
            double powerLevel = 0, outputPower = 0, margin = 0;
            bool servoSucess = false;
            for (int i = 0; i < servoConfig.MaxNumberOfIterations; i++)
            {
                if (autoLevelConfig.Enabled) servoTxpSession.AutoLevel(selectorString, autoLevelConfig.MeasurementInterval_s);
                servoTxpSession.Initiate(selectorString, "");

                powerLevel = rfsgSession.RF.PowerLevel;
                servoTxpSession.Txp.Results.FetchMeasurement(selectorString, 10, out outputPower, out _);

                margin = servoConfig.TargetTxPPower_dBm - outputPower;
                servoTrace[i] = outputPower;

                if (Math.Abs(margin) <= servoConfig.Tolerance_dBm) //Servo complete; exit the loop
                {
                    servoSucess = true;
                    break;
                }
                else //Still more room to go
                {
                    rfsgSession.RF.PowerLevel = powerLevel + margin;
                    rfsgSession.Utility.WaitUntilSettled(1000);
                }
            }
            //If we auto-leveled we need to set the original configuration to the newly calculated ref level
            servoTxpSession.GetReferenceLevel(selectorString, out double newRefLevel);
            wlanSignal.ConfigureReferenceLevel(selectorString, newRefLevel);

            servoTxpSession.Dispose();

            TxPServoResults servoResults = new TxPServoResults();
            servoResults.FinalInputPower_dBm = powerLevel;
            servoResults.FinalOutputPower_dBm = outputPower;
            servoResults.ServoTrace = servoTrace;

            if (!servoSucess)
            {
                throw new System.TimeoutException("WLAN TxP Power Servo exceeded max iterations without success.");
            }
            return servoResults;
        }

        /// <summary>Configures common settings for the SEM measurement and selects the measurement.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to configure.</param>
        /// <param name="semConfig">Specifies the SEM settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used.See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureSEM(RFmxWlanMX wlanSignal, SEMConfiguration semConfig, string selectorString = "")
        {
            wlanSignal.SelectMeasurements(selectorString, RFmxWlanMXMeasurementTypes.Sem, false);
            wlanSignal.Sem.Configuration.ConfigureSweepTime(selectorString, semConfig.SweepTimeAuto, semConfig.SweepTime_s);
            wlanSignal.Sem.Configuration.ConfigureAveraging(selectorString, semConfig.AveragingEnabled, semConfig.AveragingCount, semConfig.AveragingType);
            wlanSignal.Sem.Configuration.ConfigureSpan(selectorString, semConfig.SpanAuto, semConfig.Span_Hz);

            // Support for custom masks has not been implemented in this module
            wlanSignal.Sem.Configuration.ConfigureMaskType(selectorString, RFmxWlanMXSemMaskType.Standard);
        }

        /// <summary>Performs actions to initiate acquisition and measurement.<para></para> Enables the specified measurement(s) before optionally 
        /// automatically adjusting the reference level before beginning measurements. Finally, initiates the acquisition and measurement(s).</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to configure.</param>
        /// <param name="measurements">Specifies one or more previously configured measurements to enable for this acquisition.</param>
        /// <param name="autoLevelConfig">Specifies the configuration for the optional AutoLevel process which will automatically set the analyzer's reference level.</param>
        /// <param name="enableTraces">(Optional) Specifies whether traces should be enabled for the measurement(s). See the RFmx help for more documention of this parameter.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used.See the RFmx help for more documention of this parameter.</param>
        /// <param name="resultName">(Optional) Specifies the name to be associated with measurement results. Provide a unique name, such as "r1" to enable 
        /// fetching of multiple measurement results and traces. See the RFmx help for more documentation of this parameter.</param>
        public static void SelectAndInitiateMeasurements(RFmxWlanMX wlanSignal, RFmxWlanMXMeasurementTypes[] measurements, AutoLevelConfiguration autoLevelConfig = default,
            bool enableTraces = false, string selectorString = "", string resultName = "")
        {
            // Aggregate the selected measurements into a single value
            // OR of 0 and x equals x
            RFmxWlanMXMeasurementTypes selectedMeasurements = 0;
            foreach (RFmxWlanMXMeasurementTypes measurement in measurements)
                selectedMeasurements |= measurement;
            wlanSignal.SelectMeasurements(selectorString, selectedMeasurements, enableTraces);

            if (autoLevelConfig.Enabled)
                wlanSignal.AutoLevel(selectorString, autoLevelConfig.MeasurementInterval_s);

            // Initiate acquisition and measurement for the selected measurements
            wlanSignal.Initiate(selectorString, resultName);
        }
        #endregion
        #region Measurement Results

        /// <summary>Fetches common results from the TxP measurement.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common TxP measurement results.</returns>
        public static TxPResults FetchTxP(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            TxPResults txpResults = new TxPResults();
            wlanSignal.Txp.Results.FetchMeasurement(selectorString, 10, out txpResults.AveragePowerMean_dBm, out txpResults.PeakPowerMaximum_dBm);

            return txpResults;
        }

        /// <summary>Fetches common results from the OFDM ModAcc measurement.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common OFDM ModAcc measurement results.</returns>
        public static OFDMModAccResults FetchOFDMModAcc(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            OFDMModAccResults modAccResults = new OFDMModAccResults();
            wlanSignal.OfdmModAcc.Results.FetchCompositeRmsEvm(selectorString, 10, out modAccResults.CompositeRMSEVMMean_dB,
                out modAccResults.CompositeDataRMSEVMMean_dB, out modAccResults.CompositePilotRMSEVMMean_dB);
            wlanSignal.OfdmModAcc.Results.FetchNumberOfSymbolsUsed(selectorString, 10, out modAccResults.NumberOfSymbolsUsed);

            return modAccResults;
        }

        /// <summary>Fetches common results from the SEM measurement.</summary>
        /// <param name="wlanSignal">Specifies the WLAN signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common SEM measurement results.</returns>
        public static SEMResults FetchSEM(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            SEMResults semResults = new SEMResults();
            wlanSignal.Sem.Results.FetchMeasurementStatus(selectorString, 10, out semResults.measurementStatus);
            wlanSignal.Sem.Results.FetchCarrierMeasurement(selectorString, 10, out semResults.AbsolutePower_dBm, out semResults.RelativePower_dB);
            wlanSignal.Sem.Results.FetchLowerOffsetMarginArray(selectorString, 10, ref semResults.lowerOffsetMeasurementStatus,
                ref semResults.LowerOffsetMargin_dB, ref semResults.LowerOffsetMarginFrequency_Hz, ref semResults.LowerOffsetMarginAbsolutePower_dBm,
                ref semResults.LowerOffsetMarginRelativePower_dB);
            wlanSignal.Sem.Results.FetchUpperOffsetMarginArray(selectorString, 10, ref semResults.upperOffsetMeasurementStatus,
               ref semResults.UpperOffsetMargin_dB, ref semResults.UpperOffsetMarginFrequency_Hz, ref semResults.UpperOffsetMarginAbsolutePower_dBm,
               ref semResults.UpperOffsetMarginRelativePower_dB);

            return semResults;
        }
        #endregion
    }
}
