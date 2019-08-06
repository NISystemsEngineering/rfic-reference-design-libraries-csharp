using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxWLAN
    {
        //Structs were chosen over a basic class due to the ease of viewing function inputs inside of TestStand (avoiding encapsulation)
        //This has the downside of requiring two steps to initialize the struct to the default values
        #region Type_Definitionss
        public struct CommonConfiguration
        {
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string FrequencyReferenceSource;
            public string DigitalEdgeSource;
            public RFmxWlanMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            public bool EnableTrigger;
            public string LOSource;
            public double LOOffset;
            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    CenterFrequency_Hz = 1e9,
                    ReferenceLevel_dBm = 0,
                    ExternalAttenuation_dB = 0,
                    FrequencyReferenceSource = RFmxInstrMXConstants.PxiClock,
                    DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0,
                    DigitalEdgeType = RFmxWlanMXDigitalEdgeTriggerEdge.Rising,
                    TriggerDelay_s = 0,
                    EnableTrigger = true,
                    LOSource = RFmxInstrMXConstants.LOSourceLOIn,
                    LOOffset = 0
                };
            }
        }

        public struct SignalConfiguration
        {
            public bool AutoDetectSignal;
            public RFmxWlanMXStandard Standard;
            public double ChannelBandwidth_Hz;

            public static SignalConfiguration GetDefault()
            {
                return new SignalConfiguration
                {
                    AutoDetectSignal = true,
                    Standard = RFmxWlanMXStandard.Standard802_11ac,
                    ChannelBandwidth_Hz = 20e6
                };
            }
        }

        public struct AutoLevelConfiguration
        {
            public bool AutoLevelReferenceLevel;
            public double AutoLevelMeasureTime_s;

            public static AutoLevelConfiguration GetDefault()
            {
                return new AutoLevelConfiguration
                {
                    AutoLevelReferenceLevel = false,
                    AutoLevelMeasureTime_s = 10e-3
                };
            }
        }

        #region Measurement Definitions
        public struct TxPConfiguration
        {
            public RFmxWlanMXTxpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public double MaximumMeasurementInterval_s;

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

        #endregion
        public struct TxPResults
        {
            public double AveragePowerMean_dBm;
            public double PeakPowerMaximum_dBm;
            public AnalogWaveform<float> PowerVsTime;
        }
        public struct OFDMModAccConfiguration
        {
            public double AcquisitionLength_s;
            public int MeasurementOffset_sym;
            public int MaximumMeasurementLength_sym;
            public RFmxWlanMXOfdmModAccOptimizeDynamicRangeForEvmEnabled OptimizeDynamicRangeForEvmEnabled;
            public double OptimizeDynamicRangeForEVMMargin_dB;
            public RFmxWlanMXOfdmModAccAveragingEnabled AveragingEnabled;
            public int AveragingCount;
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

        public struct OFDMModAccResults
        {
            public AnalogWaveform<float> EVMperSymbolTrace;
            public ComplexSingle[] DataConstellation;
            public double CompositeRMSEVMMean_dB;
            public double CompositeDataRMSEVMMean_dB;
            public double CompositePilotRMSEVMMean_dB;
            public int NumberOfSymbolsUsed;
        }

        public struct SEMConfiguration
        {
            public RFmxWlanMXSemSweepTimeAuto SweepTimeAuto;
            public double SweepTime_s;
            public RFmxWlanMXSemSpanAuto SpanAuto;
            public double Span_Hz;
            public RFmxWlanMXSemAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxWlanMXSemAveragingType AveragingType;
            public RFmxWlanMXSemMaskType MaskType;
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
                    MaskType = RFmxWlanMXSemMaskType.Standard
                };
            }
        }

        public struct SEMResults
        {
            public RFmxWlanMXSemMeasurementStatus measurementStatus;
            public double AbsolutePower_dBm;
            public double RelativePower_dB;

            public RFmxWlanMXSemUpperOffsetMeasurementStatus[] upperOffsetMeasurementStatus;
            public double[] UpperOffsetMargin_dB;
            public double[] UpperOffsetMarginFrequency_Hz;
            public double[] UpperOffsetMarginAbsolutePower_dBm;
            public double[] UpperOffsetMarginRelativePower_dBm;

            public RFmxWlanMXSemLowerOffsetMeasurementStatus[] lowerOffsetMeasurementStatus;
            public double[] LowerOffsetMargin_dB;
            public double[] LowerOffsetMarginFrequency_Hz;
            public double[] LowerOffsetMarginAbsolutePower_dBm;
            public double[] LowerOffsetMarginRelativePower_dBm;

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
        public static void ConfigureCommon(RFmxInstrMX sessionHandle, RFmxWlanMX wlanSignal, CommonConfiguration commonConfig,
            AutoLevelConfiguration autoLevelConfig, string selectorString = "")
        {
            string instrModel;

            sessionHandle.ConfigureFrequencyReference("", commonConfig.FrequencyReferenceSource, 10e6);
            sessionHandle.GetInstrumentModel("", out instrModel);

            sessionHandle.SetLOSource("", commonConfig.LOSource);
            sessionHandle.SetDownconverterFrequencyOffset("", commonConfig.LOOffset);

            wlanSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalEdgeSource, commonConfig.DigitalEdgeType, commonConfig.TriggerDelay_s, commonConfig.EnableTrigger);
            wlanSignal.ConfigureFrequency(selectorString, commonConfig.CenterFrequency_Hz);
            wlanSignal.ConfigureExternalAttenuation(selectorString, commonConfig.ExternalAttenuation_dB);

            if (autoLevelConfig.AutoLevelReferenceLevel) wlanSignal.AutoLevel(selectorString, autoLevelConfig.AutoLevelMeasureTime_s);
            else wlanSignal.ConfigureReferenceLevel(selectorString, commonConfig.ReferenceLevel_dBm);

        }
        #endregion
        #region Measurement Configuration
        public static void ConfigureSignal(RFmxWlanMX wlanSignal, SignalConfiguration signalConfig, string selectorString = "")
        {

            if (signalConfig.AutoDetectSignal)
            {
                wlanSignal.AutoDetectSignal(selectorString, 10);
            }
            else
            {
                wlanSignal.ConfigureStandard(selectorString, signalConfig.Standard);
                wlanSignal.ConfigureChannelBandwidth(selectorString, signalConfig.ChannelBandwidth_Hz);
            }

        }

        public static void ConfigureTxP(RFmxWlanMX wlanSignal, TxPConfiguration txPConfig, string selectorString = "")
        {

            wlanSignal.Txp.Configuration.SetMeasurementEnabled(selectorString, true);
            wlanSignal.Txp.Configuration.SetAllTracesEnabled(selectorString, true);
            //Disabled because we are triggering by default
            wlanSignal.Txp.Configuration.SetBurstDetectionEnabled(selectorString, RFmxWlanMXTxpBurstDetectionEnabled.False);

            wlanSignal.Txp.Configuration.ConfigureAveraging(selectorString, txPConfig.AveragingEnabled, txPConfig.AveragingCount);
            wlanSignal.Txp.Configuration.ConfigureMaximumMeasurementInterval(selectorString, txPConfig.MaximumMeasurementInterval_s);
        }
        public static void ConfigureOFDMModAcc(RFmxWlanMX wlanSignal, OFDMModAccConfiguration modAccConfig, string selectorString = "")
        {
            RFmxWlanMXOfdmModAccAcquisitionLengthMode acMode;

            if (modAccConfig.AcquisitionLength_s == 0) acMode = RFmxWlanMXOfdmModAccAcquisitionLengthMode.Auto;
            else acMode = RFmxWlanMXOfdmModAccAcquisitionLengthMode.Manual;

            wlanSignal.OfdmModAcc.Configuration.SetMeasurementEnabled(selectorString, true);
            wlanSignal.OfdmModAcc.Configuration.SetAllTracesEnabled(selectorString, true);

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
            wlanSignal.OfdmModAcc.Configuration.ConfigureChannelEstimationType(selectorString, RFmxWlanMXOfdmModAccChannelEstimationType.ChannelEstimationReference);
            wlanSignal.OfdmModAcc.Configuration.SetBurstStartDetectionEnabled(selectorString, RFmxWlanMXOfdmModAccBurstStartDetectionEnabled.False); //Triggering, so no burst detection
            wlanSignal.OfdmModAcc.Configuration.SetAmplitudeTrackingEnabled(selectorString, RFmxWlanMXOfdmModAccAmplitudeTrackingEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetPhaseTrackingEnabled(selectorString, RFmxWlanMXOfdmModAccPhaseTrackingEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetChannelEstimationSmoothingEnabled(selectorString, RFmxWlanMXOfdmModAccChannelEstimationSmoothingEnabled.False);
            wlanSignal.OfdmModAcc.Configuration.SetCommonClockSourceEnabled(selectorString, RFmxWlanMXOfdmModAccCommonClockSourceEnabled.True);

        }
        [Obsolete("TxPServoPower is deprecated. Use instead the PowerServo module and related functions.")]
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
                if (autoLevelConfig.AutoLevelReferenceLevel) servoTxpSession.AutoLevel(selectorString, autoLevelConfig.AutoLevelMeasureTime_s);
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

        public static void ConfigureSEM(RFmxWlanMX wlanSignal, SEMConfiguration semConfig, string selectorString = "")
        {
            //Ensure that the measurement and traces are enabled
            wlanSignal.Sem.Configuration.SetMeasurementEnabled(selectorString, true);
            wlanSignal.Sem.Configuration.SetAllTracesEnabled(selectorString, true);

            wlanSignal.Sem.Configuration.ConfigureSweepTime(selectorString, semConfig.SweepTimeAuto, semConfig.SweepTime_s);
            wlanSignal.Sem.Configuration.ConfigureAveraging(selectorString, semConfig.AveragingEnabled, semConfig.AveragingCount, semConfig.AveragingType);
            wlanSignal.Sem.Configuration.ConfigureSpan(selectorString, semConfig.SpanAuto, semConfig.Span_Hz);

            switch (semConfig.MaskType)
            {
                case RFmxWlanMXSemMaskType.Standard:
                    wlanSignal.Sem.Configuration.ConfigureMaskType(selectorString, semConfig.MaskType);
                    break;
                default:
                    throw new System.NotImplementedException("Custom SEM Mask configurations have not been implemented in this Reference Design module.");
            }
        }
        #endregion
        #region Measurement Results
        public static TxPResults FetchTxP(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            TxPResults txpResults = new TxPResults();

            wlanSignal.Txp.Results.FetchPowerTrace(selectorString, 10, ref txpResults.PowerVsTime);
            wlanSignal.Txp.Results.FetchMeasurement(selectorString, 10, out txpResults.AveragePowerMean_dBm, out txpResults.PeakPowerMaximum_dBm);

            return txpResults;
        }

        public static OFDMModAccResults FetchOFDMModAcc(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            OFDMModAccResults modAccResults = new OFDMModAccResults();
            wlanSignal.OfdmModAcc.Results.FetchChainRmsEvmPerSymbolMeanTrace(selectorString, 10, ref modAccResults.EVMperSymbolTrace);
            wlanSignal.OfdmModAcc.Results.FetchDataConstellationTrace(selectorString, 10, ref modAccResults.DataConstellation);
            wlanSignal.OfdmModAcc.Results.FetchCompositeRmsEvm(selectorString, 10, out modAccResults.CompositeRMSEVMMean_dB,
                out modAccResults.CompositeDataRMSEVMMean_dB, out modAccResults.CompositePilotRMSEVMMean_dB);
            wlanSignal.OfdmModAcc.Results.FetchNumberOfSymbolsUsed(selectorString, 10, out modAccResults.NumberOfSymbolsUsed);

            return modAccResults;
        }

        public static SEMResults FetchSEM(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            SEMResults semResults = new SEMResults();
            wlanSignal.Sem.Results.FetchMeasurementStatus(selectorString, 10, out semResults.measurementStatus);
            wlanSignal.Sem.Results.FetchCarrierMeasurement(selectorString, 10, out semResults.AbsolutePower_dBm, out semResults.RelativePower_dB);
            wlanSignal.Sem.Results.FetchLowerOffsetMarginArray(selectorString, 10, ref semResults.lowerOffsetMeasurementStatus,
                ref semResults.LowerOffsetMargin_dB, ref semResults.LowerOffsetMarginFrequency_Hz, ref semResults.LowerOffsetMarginAbsolutePower_dBm,
                ref semResults.LowerOffsetMarginRelativePower_dBm);
            wlanSignal.Sem.Results.FetchUpperOffsetMarginArray(selectorString, 10, ref semResults.upperOffsetMeasurementStatus,
               ref semResults.UpperOffsetMargin_dB, ref semResults.UpperOffsetMarginFrequency_Hz, ref semResults.UpperOffsetMarginAbsolutePower_dBm,
               ref semResults.UpperOffsetMarginRelativePower_dBm);

            return semResults;
        }
        #endregion
    }
}
