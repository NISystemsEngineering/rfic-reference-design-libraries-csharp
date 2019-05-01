using System;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.DataInfrastructure;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public class RFmxNR
    {
        //Structs were chosen over a basic class due to the ease of viewing function inputs inside of TestStand(avoiding encapsulation)
        //This has the downside of requiring two steps to initialize the struct to the default values
        #region Type Definitions
        public struct CommonConfiguration
        {
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string frequencyReferenceSource;
            public string DigitalEdgeSource;
            public RFmxNRMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            public bool EnableTrigger;
        }
        public static CommonConfiguration GetDefaultCommonConfiguration()
        {
            return new CommonConfiguration
            {
                CenterFrequency_Hz = 1e9,
                ReferenceLevel_dBm = 0,
                ExternalAttenuation_dB = 0,
                frequencyReferenceSource = RFmxInstrMXConstants.PxiClock,
                DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0,
                DigitalEdgeType = RFmxNRMXDigitalEdgeTriggerEdge.Rising,
                TriggerDelay_s = 0,
                EnableTrigger = true
            };
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
        }
        public static SignalConfiguration GetDfaultSignalConfiguration()
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


        #region Measurement Definitions
        public struct ModAccConfiguration
        {
            public RFmxNRMXModAccSynchronizationMode synchronizationMode;
            public RFmxNRMXModAccMeasurementLengthUnit measurementLengthUnit;

            public double measurementOffset;
            public double measurementLength;

            public RFmxNRMXModAccAveragingEnabled averagingEnabled;
            public int averagingCount;
        }
        public static ModAccConfiguration GetDefaultModaccConfiguration()
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
        }

        public static AcpConfiguration GetDefaultAcpConfiguration()
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

                numberOfNROffsets = 1,
                numberOfEutraOffsets = 0,
                numberOfUtraOffsets = 0
            };
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

        #endregion
        #endregion

        #region Instrument Configuration

        public static void ConfigureCommon(ref RFmxInstrMX sessionHandle, ref RFmxNRMX nrSignal, CommonConfiguration commonConfig, string selectorString = "")
        {
            nrSignal.ConfigureRF(selectorString, commonConfig.CenterFrequency_Hz, commonConfig.ReferenceLevel_dBm, commonConfig.ExternalAttenuation_dB);
            sessionHandle.ConfigureFrequencyReference("", commonConfig.frequencyReferenceSource, 10e6);
            nrSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalEdgeSource, commonConfig.DigitalEdgeType, commonConfig.TriggerDelay_s, commonConfig.EnableTrigger);
        }

      
        #endregion

        #region Measurement Configuration
        public static void ConfigureSignal( ref RFmxNRMX nrSignal, SignalConfiguration signalConfig, string selectorString = "")
        {
            string subblockString;
            string carrierString;
            string bandwidthPartString;
            string userString;
            string puschString;
            string puschClusterString;

            nrSignal.SetFrequencyRange("", signalConfig.frequencyRange);
            nrSignal.ComponentCarrier.SetBandwidth("", signalConfig.carrierBandwidth);
            nrSignal.ComponentCarrier.SetCellID("", signalConfig.cellID);
            nrSignal.SetBand("", signalConfig.band);
            nrSignal.ComponentCarrier.SetBandwidthPartSubcarrierSpacing("", signalConfig.subcarrierSpacing);
            nrSignal.SetAutoResourceBlockDetectionEnabled("", signalConfig.autoResourceBlockDetectionEnabled);

            nrSignal.ComponentCarrier.SetPuschTransformPrecodingEnabled("", signalConfig.puschTransformPrecodingEnabled);
            nrSignal.ComponentCarrier.SetPuschSlotAllocation("", signalConfig.puschSlotAllocation);
            nrSignal.ComponentCarrier.SetPuschSymbolAllocation("", signalConfig.puschSymbolAllocation);
            nrSignal.ComponentCarrier.SetPuschModulationType("", signalConfig.puschModulationType);

            nrSignal.ComponentCarrier.SetPuschNumberOfResourceBlockClusters("", signalConfig.NumberOfResourceBlockClusters);

            subblockString = RFmxNRMX.BuildSubblockString("", 0);
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

            nrSignal.ComponentCarrier.SetPuschDmrsPowerMode("", signalConfig.puschDmrsPowerMode);
            nrSignal.ComponentCarrier.SetPuschDmrsPower("", signalConfig.puschDmrsPower);
            nrSignal.ComponentCarrier.SetPuschDmrsConfigurationType("", signalConfig.puschDmrsConfigurationType);
            nrSignal.ComponentCarrier.SetPuschMappingType("", signalConfig.puschMappingType);
            nrSignal.ComponentCarrier.SetPuschDmrsTypeAPosition("", signalConfig.puschDmrsTypeAPosition);
            nrSignal.ComponentCarrier.SetPuschDmrsDuration("", signalConfig.puschDmrsDuration);
            nrSignal.ComponentCarrier.SetPuschDmrsAdditionalPositions("", signalConfig.puschDmrsAdditionalPositions);
        }

        public static void ConfigureAcp(ref RFmxNRMX nrSignal, AcpConfiguration acpConfig, string selectorString = "")
        {
            nrSignal.Acp.Configuration.ConfigureMeasurementMethod("", acpConfig.measurementMethod);
            nrSignal.Acp.Configuration.ConfigureNoiseCompensationEnabled("", acpConfig.noiseCompensationEnabled);
            nrSignal.Acp.Configuration.ConfigureSweepTime("", acpConfig.sweepTimeAuto, acpConfig.sweepTimeInterval);
            nrSignal.Acp.Configuration.ConfigureAveraging("", acpConfig.averagingEnabled, acpConfig.averagingCount, acpConfig.averagingType);

            nrSignal.Acp.Configuration.ConfigureNumberOfNROffsets(selectorString, acpConfig.numberOfNROffsets);
            nrSignal.Acp.Configuration.ConfigureNumberOfEutraOffsets(selectorString, acpConfig.numberOfEutraOffsets);
            nrSignal.Acp.Configuration.ConfigureNumberOfUtraOffsets(selectorString, acpConfig.numberOfUtraOffsets);

        }

        public static void ConfigureModacc(ref RFmxNRMX nrSignal, ModAccConfiguration modaccConfig, string selectorString = "")
        {
            nrSignal.ModAcc.Configuration.SetSynchronizationMode("", modaccConfig.synchronizationMode);
            nrSignal.ModAcc.Configuration.SetAveragingEnabled("", modaccConfig.averagingEnabled);
            nrSignal.ModAcc.Configuration.SetAveragingCount("", modaccConfig.averagingCount);

            nrSignal.ModAcc.Configuration.SetMeasurementLengthUnit("", modaccConfig.measurementLengthUnit);
            nrSignal.ModAcc.Configuration.SetMeasurementOffset("", modaccConfig.measurementOffset);
            nrSignal.ModAcc.Configuration.SetMeasurementLength("", modaccConfig.measurementLength);

        }


        #endregion

        #region Measurement Results
        public static ModAccResults FetchModAcc(ref RFmxNRMX nrSignal, string selectorString = "")
        {
            ModAccResults modaccResults = new ModAccResults();


            nrSignal.ModAcc.Results.GetCompositeRmsEvmMean("", out modaccResults.compositeRmsEvmMean);
            nrSignal.ModAcc.Results.GetCompositePeakEvmMaximum("", out modaccResults.compositePeakEvmMaximum);
            nrSignal.ModAcc.Results.GetCompositePeakEvmSlotIndex("", out modaccResults.compositePeakEvmSlotIndex);
            nrSignal.ModAcc.Results.GetCompositePeakEvmSymbolIndex("", out modaccResults.compositePeakEvmSymbolIndex);
            nrSignal.ModAcc.Results.GetCompositePeakEvmSubcarrierIndex("", out modaccResults.compositePeakEvmSubcarrierIndex);

            nrSignal.ModAcc.Results.GetComponentCarrierFrequencyErrorMean("", out modaccResults.componentCarrierFrequencyErrorMean);
            nrSignal.ModAcc.Results.GetComponentCarrierIQOriginOffsetMean("", out modaccResults.componentCarrierIQOriginOffsetMean);
            nrSignal.ModAcc.Results.GetComponentCarrierIQGainImbalanceMean("", out modaccResults.componentCarrierIQGainImbalanceMean);
            nrSignal.ModAcc.Results.GetComponentCarrierQuadratureErrorMean("", out modaccResults.componentCarrierQuadratureErrorMean);
            nrSignal.ModAcc.Results.GetInBandEmissionMargin("", out modaccResults.inBandEmissionMargin);

            nrSignal.ModAcc.Results.FetchPuschDataConstellationTrace(selectorString, 10, ref modaccResults.puschDataConstellation);
            nrSignal.ModAcc.Results.FetchPuschDmrsConstellationTrace(selectorString, 10, ref modaccResults.puschDmrsConstellation);

            nrSignal.ModAcc.Results.FetchRmsEvmPerSubcarrierMeanTrace(selectorString, 10, ref modaccResults.rmsEvmPerSubcarrierMean);
            nrSignal.ModAcc.Results.FetchRmsEvmPerSymbolMeanTrace(selectorString, 10, ref modaccResults.rmsEvmPerSymbolMean);

            nrSignal.ModAcc.Results.FetchSpectralFlatnessTrace(selectorString, 10, ref modaccResults.spectralFlatness, ref modaccResults.spectralFlatnessLowerMask, ref modaccResults.spectralFlatnessUpperMask);

            return modaccResults;
        }

        public static AcpResults FetchAcp(ref RFmxNRMX nrSignal, string selectorString = "")
        {
            AcpResults acpResults = new AcpResults();

            nrSignal.Acp.Results.FetchOffsetMeasurementArray("", 10, ref acpResults.lowerRelativePower,
            ref acpResults.upperRelativePower, ref acpResults.lowerAbsolutePower, ref acpResults.upperAbsolutePower);

            nrSignal.Acp.Results.ComponentCarrier.FetchMeasurement("", 10, out acpResults.absolutePower, out acpResults.relativePower);

            for (int i = 0; i < acpResults.lowerRelativePower.Length; i++)
            {
                nrSignal.Acp.Results.FetchRelativePowersTrace("", 10, i, ref acpResults.relativePowersTrace);
            }

            nrSignal.Acp.Results.FetchSpectrum("", 10, ref acpResults.spectrum);

            return acpResults;
        }
        #endregion

    }

}


