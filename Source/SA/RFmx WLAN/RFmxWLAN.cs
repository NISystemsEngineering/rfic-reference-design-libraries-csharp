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
        public struct StandardConfiguration
        {
            public RFmxWlanMXStandard Standard;
            public double ChannelBandwidth_Hz;

            public static StandardConfiguration GetDefault()
            {
                return new StandardConfiguration
                {
                    Standard = RFmxWlanMXStandard.Standard802_11ac,
                    ChannelBandwidth_Hz = 20e6
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

        public struct SEMResults
        {
            public RFmxWlanMXSemMeasurementStatus measurementStatus;
            public double AbsolutePower_dBm;
            public double RelativePower_dB;

            public RFmxWlanMXSemUpperOffsetMeasurementStatus[] upperOffsetMeasurementStatus;
            public double[] UpperOffsetMargin_dB;
            public double[] UpperOffsetMarginFrequency_Hz;
            public double[] UpperOffsetMarginAbsolutePower_dBm;
            public double[] UpperOffsetMarginRelativePower_dB;

            public RFmxWlanMXSemLowerOffsetMeasurementStatus[] lowerOffsetMeasurementStatus;
            public double[] LowerOffsetMargin_dB;
            public double[] LowerOffsetMarginFrequency_Hz;
            public double[] LowerOffsetMarginAbsolutePower_dBm;
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
        public static void ConfigureStandard(RFmxWlanMX wlanSignal, StandardConfiguration standardConfig, string selectorString = "")
        {
            wlanSignal.ConfigureStandard(selectorString, standardConfig.Standard);
            wlanSignal.ConfigureChannelBandwidth(selectorString, standardConfig.ChannelBandwidth_Hz);
        }

        public static void ConfigureTxP(RFmxWlanMX wlanSignal, TxPConfiguration txPConfig, string selectorString = "")
        {

            wlanSignal.SelectMeasurements(selectorString, RFmxWlanMXMeasurementTypes.Txp, false);
            //Disabled because we are triggering by default
            wlanSignal.Txp.Configuration.SetBurstDetectionEnabled(selectorString, RFmxWlanMXTxpBurstDetectionEnabled.False);

            wlanSignal.Txp.Configuration.ConfigureAveraging(selectorString, txPConfig.AveragingEnabled, txPConfig.AveragingCount);
            wlanSignal.Txp.Configuration.ConfigureMaximumMeasurementInterval(selectorString, txPConfig.MaximumMeasurementInterval_s);
        }
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
        public static void SelectAndInitiateMeasurements(RFmxWlanMX wlanSignal, RFmxWlanMXMeasurementTypes[] measurements, AutoLevelConfiguration autoLevelConfig,
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
        public static void ConfigureSEM(RFmxWlanMX wlanSignal, SEMConfiguration semConfig, string selectorString = "")
        {
            wlanSignal.SelectMeasurements(selectorString, RFmxWlanMXMeasurementTypes.Sem, false);
            wlanSignal.Sem.Configuration.ConfigureSweepTime(selectorString, semConfig.SweepTimeAuto, semConfig.SweepTime_s);
            wlanSignal.Sem.Configuration.ConfigureAveraging(selectorString, semConfig.AveragingEnabled, semConfig.AveragingCount, semConfig.AveragingType);
            wlanSignal.Sem.Configuration.ConfigureSpan(selectorString, semConfig.SpanAuto, semConfig.Span_Hz);

            // Support for custom masks has not been implemented in this module
            wlanSignal.Sem.Configuration.ConfigureMaskType(selectorString, RFmxWlanMXSemMaskType.Standard);
        }
        #endregion
        #region Measurement Results
        public static TxPResults FetchTxP(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            TxPResults txpResults = new TxPResults();

            wlanSignal.Txp.Results.FetchMeasurement(selectorString, 10, out txpResults.AveragePowerMean_dBm, out txpResults.PeakPowerMaximum_dBm);

            return txpResults;
        }

        public static OFDMModAccResults FetchOFDMModAcc(RFmxWlanMX wlanSignal, string selectorString = "")
        {
            OFDMModAccResults modAccResults = new OFDMModAccResults();
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
                ref semResults.LowerOffsetMarginRelativePower_dB);
            wlanSignal.Sem.Results.FetchUpperOffsetMarginArray(selectorString, 10, ref semResults.upperOffsetMeasurementStatus,
               ref semResults.UpperOffsetMargin_dB, ref semResults.UpperOffsetMarginFrequency_Hz, ref semResults.UpperOffsetMarginAbsolutePower_dBm,
               ref semResults.UpperOffsetMarginRelativePower_dB);

            return semResults;
        }
        #endregion
    }
}
