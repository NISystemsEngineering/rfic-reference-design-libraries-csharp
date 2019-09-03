using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxNR
    {
        //Structs were chosen over a basic class due to the ease of viewing function inputs inside of TestStand(avoiding encapsulation)
        //This has the downside of requiring two steps to initialize the struct to the default values
        #region Type Definitions
        public struct CommonConfiguration
        {
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string FrequencyReferenceSource;
            public string DigitalEdgeSource;
            public RFmxNRMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            public bool EnableTrigger;

            public bool AutoLevelEnabled;
            public double AutoLevelMeasurementInterval;

            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    CenterFrequency_Hz = 3.5e9,
                    ReferenceLevel_dBm = 0,
                    ExternalAttenuation_dB = 0,
                    FrequencyReferenceSource = RFmxInstrMXConstants.PxiClock,
                    DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0,
                    DigitalEdgeType = RFmxNRMXDigitalEdgeTriggerEdge.Rising,
                    TriggerDelay_s = 0,
                    EnableTrigger = true,

                    AutoLevelEnabled = false,
                    AutoLevelMeasurementInterval = 10e-3
                };
            }
        }

        public struct SignalConfiguration
        {
            public RFmxNRMXFrequencyRange frequencyRange;
            public int band;
            public int cellID;
            public double carrierBandwidth;
            public double subcarrierSpacing;
            public RFmxNRMXAutoResourceBlockDetectionEnabled autoResourceBlockDetectionEnabled;

            public RFmxNRMXPuschTransformPrecodingEnabled puschTransformPrecodingEnabled;
            public RFmxNRMXPuschModulationType puschModulationType;
            public int NumberOfResourceBlockClusters;
            public int[] puschResourceBlockOffset;
            public int[] puschNumberOfResourceBlocks;
            public string puschSlotAllocation;
            public string puschSymbolAllocation;

            public RFmxNRMXPuschDmrsPowerMode puschDmrsPowerMode;
            public double puschDmrsPower;
            public RFmxNRMXPuschDmrsConfigurationType puschDmrsConfigurationType;
            public RFmxNRMXPuschMappingType puschMappingType;
            public int puschDmrsTypeAPosition;
            public RFmxNRMXPuschDmrsDuration puschDmrsDuration;
            public int puschDmrsAdditionalPositions;

            public static SignalConfiguration GetDefault()
            {
                return new SignalConfiguration
                {
                    frequencyRange = RFmxNRMXFrequencyRange.Range1,
                    band = 78,
                    cellID = 0,
                    carrierBandwidth = 100e6,
                    subcarrierSpacing = 30e3,
                    autoResourceBlockDetectionEnabled = RFmxNRMXAutoResourceBlockDetectionEnabled.False,

                    puschTransformPrecodingEnabled = RFmxNRMXPuschTransformPrecodingEnabled.False,
                    puschModulationType = RFmxNRMXPuschModulationType.Qpsk,
                    NumberOfResourceBlockClusters = 1,
                    puschResourceBlockOffset = new int[] { 0 },
                    puschNumberOfResourceBlocks = new int[] { -1 },
                    puschSlotAllocation = "0-Last",
                    puschSymbolAllocation = "0-Last",

                    puschDmrsPowerMode = RFmxNRMXPuschDmrsPowerMode.CdmGroups,
                    puschDmrsPower = 0,
                    puschDmrsConfigurationType = RFmxNRMXPuschDmrsConfigurationType.Type1,
                    puschMappingType = RFmxNRMXPuschMappingType.TypeA,
                    puschDmrsTypeAPosition = 2,
                    puschDmrsDuration = RFmxNRMXPuschDmrsDuration.SingleSymbol,
                    puschDmrsAdditionalPositions = 0
                };
            }
        }

        #region Measurement Definitions
        public struct ModAccConfiguration
        {
            public RFmxNRMXModAccSynchronizationMode synchronizationMode;
            public RFmxNRMXModAccMeasurementLengthUnit measurementLengthUnit;

            public double measurementOffset;
            public double measurementLength;

            public RFmxNRMXModAccAveragingEnabled averagingEnabled;
            public int averagingCount;

            public static ModAccConfiguration GetDefault()
            {
                return new ModAccConfiguration
                {
                    synchronizationMode = RFmxNRMXModAccSynchronizationMode.Slot,
                    measurementLengthUnit = RFmxNRMXModAccMeasurementLengthUnit.Slot,

                    measurementOffset = 0,
                    measurementLength = 1,

                    averagingEnabled = RFmxNRMXModAccAveragingEnabled.False,
                    averagingCount = 10
                };
            }
        }

        public struct AcpConfiguration
        {
            public RFmxNRMXAcpMeasurementMethod measurementMethod;
            public RFmxNRMXAcpNoiseCompensationEnabled noiseCompensationEnabled;

            public RFmxNRMXAcpSweepTimeAuto sweepTimeAuto;
            public double sweepTimeInterval;

            public RFmxNRMXAcpAveragingEnabled averagingEnabled;
            public int averagingCount;
            public RFmxNRMXAcpAveragingType averagingType;

            public int numberOfNROffsets;
            public int numberOfEutraOffsets;
            public int numberOfUtraOffsets;

            public static AcpConfiguration GetDefault()
            {
                return new AcpConfiguration
                {
                    measurementMethod = RFmxNRMXAcpMeasurementMethod.Normal,
                    noiseCompensationEnabled = RFmxNRMXAcpNoiseCompensationEnabled.False,

                    sweepTimeAuto = RFmxNRMXAcpSweepTimeAuto.True,
                    sweepTimeInterval = 1.0e-3,

                    averagingEnabled = RFmxNRMXAcpAveragingEnabled.False,
                    averagingCount = 10,
                    averagingType = RFmxNRMXAcpAveragingType.Rms,

                    numberOfNROffsets = 2,
                    numberOfEutraOffsets = 0,
                    numberOfUtraOffsets = 0
                };
            }
        }

        public struct ChpConfiguration
        {
            public RFmxNRMXChpSweepTimeAuto sweepTimeAuto;
            public double sweepTimeInterval;

            public RFmxNRMXChpAveragingEnabled averagingEnabled;
            public int averagingCount;
            public RFmxNRMXChpAveragingType averagingType;

            public static ChpConfiguration GetDefault()
            {
                return new ChpConfiguration
                {
                    sweepTimeAuto = RFmxNRMXChpSweepTimeAuto.True,
                    sweepTimeInterval = 1.0e-3,
                    averagingEnabled = RFmxNRMXChpAveragingEnabled.False,
                    averagingCount = 10,
                    averagingType = RFmxNRMXChpAveragingType.Rms
                };
            }
        }

        public struct ChpServoConfiguration
        {
            public double TargetChpPower_dBm;
            public double Tolerance_dBm;
            public int MaxNumberOfIterations;
            public static ChpServoConfiguration GetDefault()
            {
                return new ChpServoConfiguration
                {
                    TargetChpPower_dBm = 0,
                    Tolerance_dBm = 0.05,
                    MaxNumberOfIterations = 10
                };
            }
        }

        #endregion
        #endregion
        #region Instrument Configuration
        public static void ConfigureCommon(RFmxInstrMX sessionHandle, RFmxNRMX nrSignal, CommonConfiguration commonConfig, string selectorString = "")
        {
            nrSignal.ConfigureFrequency(selectorString, commonConfig.CenterFrequency_Hz);
            nrSignal.ConfigureExternalAttenuation(selectorString, commonConfig.ExternalAttenuation_dB);
            sessionHandle.ConfigureFrequencyReference(selectorString, commonConfig.FrequencyReferenceSource, 10e6);
            nrSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalEdgeSource, commonConfig.DigitalEdgeType, commonConfig.TriggerDelay_s, commonConfig.EnableTrigger);

            if (commonConfig.AutoLevelEnabled)
            {
                nrSignal.AutoLevel(selectorString, commonConfig.AutoLevelMeasurementInterval, out commonConfig.ReferenceLevel_dBm);
            }
            else
            {
                nrSignal.ConfigureReferenceLevel(selectorString, commonConfig.ReferenceLevel_dBm);
            }
        }
        #endregion

        #region Measurement Configuration
        public static void ConfigureSignal(RFmxNRMX nrSignal, SignalConfiguration signalConfig, string selectorString = "")
        {
            string subblockString;
            string carrierString;
            string bandwidthPartString;
            string userString;
            string puschString;
            string puschClusterString;

            nrSignal.SetFrequencyRange(selectorString, signalConfig.frequencyRange);
            nrSignal.ComponentCarrier.SetBandwidth(selectorString, signalConfig.carrierBandwidth);
            nrSignal.ComponentCarrier.SetCellID(selectorString, signalConfig.cellID);
            nrSignal.SetBand(selectorString, signalConfig.band);
            nrSignal.ComponentCarrier.SetBandwidthPartSubcarrierSpacing(selectorString, signalConfig.subcarrierSpacing);
            nrSignal.SetAutoResourceBlockDetectionEnabled(selectorString, signalConfig.autoResourceBlockDetectionEnabled);

            nrSignal.ComponentCarrier.SetPuschTransformPrecodingEnabled(selectorString, signalConfig.puschTransformPrecodingEnabled);
            nrSignal.ComponentCarrier.SetPuschSlotAllocation(selectorString, signalConfig.puschSlotAllocation);
            nrSignal.ComponentCarrier.SetPuschSymbolAllocation(selectorString, signalConfig.puschSymbolAllocation);
            nrSignal.ComponentCarrier.SetPuschModulationType(selectorString, signalConfig.puschModulationType);

            nrSignal.ComponentCarrier.SetPuschNumberOfResourceBlockClusters(selectorString, signalConfig.NumberOfResourceBlockClusters);

            subblockString = RFmxNRMX.BuildSubblockString(selectorString, 0);
            carrierString = RFmxNRMX.BuildCarrierString(subblockString, 0);
            bandwidthPartString = RFmxNRMX.BuildBandwidthPartString(carrierString, 0);
            userString = RFmxNRMX.BuildUserString(bandwidthPartString, 0);
            puschString = RFmxNRMX.BuildPuschString(userString, 0);

            for (int i = 0; i < signalConfig.NumberOfResourceBlockClusters; i++)
            {
                puschClusterString = RFmxNRMX.BuildPuschClusterString(puschString, i);
                nrSignal.ComponentCarrier.SetPuschResourceBlockOffset(puschClusterString, signalConfig.puschResourceBlockOffset[i]);
                nrSignal.ComponentCarrier.SetPuschNumberOfResourceBlocks(puschClusterString, signalConfig.puschNumberOfResourceBlocks[i]);
            }

            nrSignal.ComponentCarrier.SetPuschDmrsPowerMode(selectorString, signalConfig.puschDmrsPowerMode);
            nrSignal.ComponentCarrier.SetPuschDmrsPower(selectorString, signalConfig.puschDmrsPower);
            nrSignal.ComponentCarrier.SetPuschDmrsConfigurationType(selectorString, signalConfig.puschDmrsConfigurationType);
            nrSignal.ComponentCarrier.SetPuschMappingType(selectorString, signalConfig.puschMappingType);
            nrSignal.ComponentCarrier.SetPuschDmrsTypeAPosition(selectorString, signalConfig.puschDmrsTypeAPosition);
            nrSignal.ComponentCarrier.SetPuschDmrsDuration(selectorString, signalConfig.puschDmrsDuration);
            nrSignal.ComponentCarrier.SetPuschDmrsAdditionalPositions(selectorString, signalConfig.puschDmrsAdditionalPositions);
        }
        public static void ConfigureModacc(RFmxNRMX nrSignal, ModAccConfiguration modaccConfig, string selectorString = "")
        {
            nrSignal.ModAcc.Configuration.SetMeasurementEnabled(selectorString, true);
            nrSignal.ModAcc.Configuration.SetAllTracesEnabled(selectorString, true);

            nrSignal.ModAcc.Configuration.SetSynchronizationMode(selectorString, modaccConfig.synchronizationMode);
            nrSignal.ModAcc.Configuration.SetAveragingEnabled(selectorString, modaccConfig.averagingEnabled);
            nrSignal.ModAcc.Configuration.SetAveragingCount(selectorString, modaccConfig.averagingCount);

            nrSignal.ModAcc.Configuration.SetMeasurementLengthUnit(selectorString, modaccConfig.measurementLengthUnit);
            nrSignal.ModAcc.Configuration.SetMeasurementOffset(selectorString, modaccConfig.measurementOffset);
            nrSignal.ModAcc.Configuration.SetMeasurementLength(selectorString, modaccConfig.measurementLength);
        }
        public static void ConfigureAcp(RFmxNRMX nrSignal, AcpConfiguration acpConfig, string selectorString = "")
        {
            nrSignal.Acp.Configuration.SetMeasurementEnabled(selectorString, true);
            nrSignal.Acp.Configuration.SetAllTracesEnabled(selectorString, true);

            nrSignal.Acp.Configuration.ConfigureMeasurementMethod(selectorString, acpConfig.measurementMethod);
            nrSignal.Acp.Configuration.ConfigureNoiseCompensationEnabled(selectorString, acpConfig.noiseCompensationEnabled);
            nrSignal.Acp.Configuration.ConfigureSweepTime(selectorString, acpConfig.sweepTimeAuto, acpConfig.sweepTimeInterval);
            nrSignal.Acp.Configuration.ConfigureAveraging(selectorString, acpConfig.averagingEnabled, acpConfig.averagingCount, acpConfig.averagingType);

            nrSignal.Acp.Configuration.ConfigureNumberOfNROffsets(selectorString, acpConfig.numberOfNROffsets);
            nrSignal.Acp.Configuration.ConfigureNumberOfEutraOffsets(selectorString, acpConfig.numberOfEutraOffsets);
            nrSignal.Acp.Configuration.ConfigureNumberOfUtraOffsets(selectorString, acpConfig.numberOfUtraOffsets);

        }
        public static void ConfigureChp(RFmxNRMX nrSignal, ChpConfiguration chpConfig, string selectorString = "")
        {
            nrSignal.Chp.Configuration.SetMeasurementEnabled(selectorString, true);
            nrSignal.Chp.Configuration.SetAllTracesEnabled(selectorString, true);

            nrSignal.Chp.Configuration.ConfigureSweepTime(selectorString, chpConfig.sweepTimeAuto, chpConfig.sweepTimeInterval);
            nrSignal.Chp.Configuration.ConfigureAveraging(selectorString, chpConfig.averagingEnabled, chpConfig.averagingCount, chpConfig.averagingType);
        }
        public static ChpServoResults ChpServoPowerFDD(RFmxNRMX nrSignal, NIRfsg rfsgSession, ChpServoConfiguration servoConfig,
            CommonConfiguration commonConfig, string selectorString = "")
        {
            //Duplicate the existing configuration so that we can select only TxP for the power servo to save time, 
            //but not disrupt all of the other user enabled measurements. 
            nrSignal.CloneSignalConfiguration("servo_chp", out RFmxNRMX servoChpSession);
            servoChpSession.SelectMeasurements(selectorString, RFmxNRMXMeasurementTypes.Chp, false);
            double[] servoTrace = new double[servoConfig.MaxNumberOfIterations];
            double powerLevel = 0, outputPower = 0, margin = 0;
            bool servoSucess = false;
            for (int i = 0; i < servoConfig.MaxNumberOfIterations; i++)
            {
                if (commonConfig.AutoLevelEnabled) servoChpSession.AutoLevel(selectorString, commonConfig.AutoLevelMeasurementInterval, out commonConfig.ReferenceLevel_dBm);
                servoChpSession.Initiate(selectorString, "");

                powerLevel = rfsgSession.RF.PowerLevel;
                servoChpSession.Chp.Results.FetchTotalAggregatedPower(selectorString, 10, out outputPower);

                margin = servoConfig.TargetChpPower_dBm - outputPower;
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
            servoChpSession.GetReferenceLevel(selectorString, out double newRefLevel);
            nrSignal.ConfigureReferenceLevel(selectorString, newRefLevel);

            servoChpSession.Dispose();

            ChpServoResults servoResults = new ChpServoResults();
            servoResults.FinalInputPower_dBm = powerLevel;
            servoResults.FinalOutputPower_dBm = outputPower;
            servoResults.ServoTrace = servoTrace;

            if (!servoSucess)
            {
                throw new System.TimeoutException("NR CHP FDD Power Servo exceeded max iterations without success.");
            }
            return servoResults;
        }
        #endregion

        #region Measurement Results
        public struct ModAccResults
        {
            public double compositeRmsEvmMean;
            public double compositePeakEvmMaximum;
            public int compositePeakEvmSlotIndex;
            public int compositePeakEvmSymbolIndex;
            public int compositePeakEvmSubcarrierIndex;

            public double componentCarrierFrequencyErrorMean;
            public double componentCarrierIQOriginOffsetMean;
            public double componentCarrierIQGainImbalanceMean;
            public double componentCarrierQuadratureErrorMean;
            public double inBandEmissionMargin;

            public ComplexSingle[] puschDataConstellation;
            public ComplexSingle[] puschDmrsConstellation;

            public AnalogWaveform<float> rmsEvmPerSubcarrierMean;
            public AnalogWaveform<float> rmsEvmPerSymbolMean;

            public Spectrum<float> spectralFlatness;
            public Spectrum<float> spectralFlatnessLowerMask;
            public Spectrum<float> spectralFlatnessUpperMask;
        }
        public struct AcpResults
        {
            public double absolutePower;
            public double relativePower;

            public double[] lowerRelativePower;
            public double[] upperRelativePower;
            public double[] lowerAbsolutePower;
            public double[] upperAbsolutePower;

            public Spectrum<float> spectrum;
            public Spectrum<float> relativePowersTrace;

        }
        public struct ChpResults
        {
            public double absolutePower;
            public double relativePower;

            public Spectrum<float> spectrum;
        }
        public struct ChpServoResults
        {
            public double[] ServoTrace;
            public double FinalInputPower_dBm;
            public double FinalOutputPower_dBm;
        }

        public static ModAccResults FetchModAcc(RFmxNRMX nrSignal, string selectorString = "")
        {
            ModAccResults modaccResults = new ModAccResults();


            nrSignal.ModAcc.Results.GetCompositeRmsEvmMean(selectorString, out modaccResults.compositeRmsEvmMean);
            nrSignal.ModAcc.Results.GetCompositePeakEvmMaximum(selectorString, out modaccResults.compositePeakEvmMaximum);
            nrSignal.ModAcc.Results.GetCompositePeakEvmSlotIndex(selectorString, out modaccResults.compositePeakEvmSlotIndex);
            nrSignal.ModAcc.Results.GetCompositePeakEvmSymbolIndex(selectorString, out modaccResults.compositePeakEvmSymbolIndex);
            nrSignal.ModAcc.Results.GetCompositePeakEvmSubcarrierIndex(selectorString, out modaccResults.compositePeakEvmSubcarrierIndex);

            nrSignal.ModAcc.Results.GetComponentCarrierFrequencyErrorMean(selectorString, out modaccResults.componentCarrierFrequencyErrorMean);
            nrSignal.ModAcc.Results.GetComponentCarrierIQOriginOffsetMean(selectorString, out modaccResults.componentCarrierIQOriginOffsetMean);
            nrSignal.ModAcc.Results.GetComponentCarrierIQGainImbalanceMean(selectorString, out modaccResults.componentCarrierIQGainImbalanceMean);
            nrSignal.ModAcc.Results.GetComponentCarrierQuadratureErrorMean(selectorString, out modaccResults.componentCarrierQuadratureErrorMean);
            nrSignal.ModAcc.Results.GetInBandEmissionMargin(selectorString, out modaccResults.inBandEmissionMargin);

            nrSignal.ModAcc.Results.FetchPuschDataConstellationTrace(selectorString, 10, ref modaccResults.puschDataConstellation);
            nrSignal.ModAcc.Results.FetchPuschDmrsConstellationTrace(selectorString, 10, ref modaccResults.puschDmrsConstellation);

            nrSignal.ModAcc.Results.FetchRmsEvmPerSubcarrierMeanTrace(selectorString, 10, ref modaccResults.rmsEvmPerSubcarrierMean);
            nrSignal.ModAcc.Results.FetchRmsEvmPerSymbolMeanTrace(selectorString, 10, ref modaccResults.rmsEvmPerSymbolMean);

            nrSignal.ModAcc.Results.FetchSpectralFlatnessTrace(selectorString, 10, ref modaccResults.spectralFlatness, ref modaccResults.spectralFlatnessLowerMask, ref modaccResults.spectralFlatnessUpperMask);

            return modaccResults;
        }
        public static AcpResults FetchAcp(RFmxNRMX nrSignal, string selectorString = "")
        {
            AcpResults acpResults = new AcpResults();

            nrSignal.Acp.Results.FetchOffsetMeasurementArray(selectorString, 10, ref acpResults.lowerRelativePower,
            ref acpResults.upperRelativePower, ref acpResults.lowerAbsolutePower, ref acpResults.upperAbsolutePower);

            nrSignal.Acp.Results.ComponentCarrier.FetchMeasurement(selectorString, 10, out acpResults.absolutePower, out acpResults.relativePower);

            for (int i = 0; i < acpResults.lowerRelativePower.Length; i++)
            {
                nrSignal.Acp.Results.FetchRelativePowersTrace(selectorString, 10, i, ref acpResults.relativePowersTrace);
            }

            nrSignal.Acp.Results.FetchSpectrum(selectorString, 10, ref acpResults.spectrum);

            return acpResults;
        }
        public static ChpResults FetchChp(RFmxNRMX nrSignal, string selectorString = "")
        {
            ChpResults chpResults = new ChpResults();

            nrSignal.Chp.Results.ComponentCarrier.FetchMeasurement(selectorString, 10, out chpResults.absolutePower, out chpResults.relativePower);
            nrSignal.Chp.Results.FetchSpectrum(selectorString, 10, ref chpResults.spectrum);

            return chpResults;
        }


        #endregion

    }

}
