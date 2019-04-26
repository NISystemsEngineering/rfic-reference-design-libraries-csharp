using System;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.DataInfrastructure;
using NationalInstruments.RFmx.NRMX;
using NationalInstruments.ModularInstruments.NIRfsg;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public class RFmxNR
    {
        //Structs were chosen over a basic class due to the ease of viewing function inputs inside of TestStand(avoiding encapsulation)
        //This has the downside of requiring two steps to initialize the struct to the default values
        #region Type_Definitionss
        public struct CommonConfiguration
        {
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string DigitalEdgeSource;
            public RFmxNRMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            public bool EnableTrigger;

            public void SetDefaults()
            {
                CenterFrequency_Hz = 1e9;
                ReferenceLevel_dBm = 0;
                ExternalAttenuation_dB = 0;
                DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0;
                DigitalEdgeType = RFmxNRMXDigitalEdgeTriggerEdge.Rising;
                TriggerDelay_s = 0;
                EnableTrigger = true;
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

                public void SetDefaults()
                {
                    frequencyRange = RFmxNRMXFrequencyRange.Range1;
                    band = 78;
                    cellID = 0;
                    carrierBandwidth = 100e6;
                    subcarrierSpacing = 30e3;
                    autoResourceBlockDetectionEnabled = RFmxNRMXAutoResourceBlockDetectionEnabled.False;

                    puschTransformPrecodingEnabled = RFmxNRMXPuschTransformPrecodingEnabled.False;
                    puschModulationType = RFmxNRMXPuschModulationType.Qpsk;
                    NumberOfResourceBlockClusters = 1;
                    puschResourceBlockOffset = new int[] { 0 };
                    puschNumberOfResourceBlocks = new int[] { -1 };
                    puschSlotAllocation = "0-Last";
                    puschSymbolAllocation = "0-Last";

                    puschDmrsPowerMode = RFmxNRMXPuschDmrsPowerMode.CdmGroups;
                    puschDmrsPower = 0;
                    puschDmrsConfigurationType = RFmxNRMXPuschDmrsConfigurationType.Type1;
                    puschDmrsTypeAPosition = 2;
                    puschDmrsDuration = RFmxNRMXPuschDmrsDuration.SingleSymbol;
                    puschDmrsAdditionalPositions = 0;
                }

                #region Measurement Definitions

                public struct ModAccConfiguration
                {
                    RFmxNRMXModAccSynchronizationMode synchronizationMode;

                    RFmxNRMXModAccMeasurementLengthUnit measurementLengthUnit;
                    double measurementOffset;
                    double measurementLength;

                    RFmxNRMXModAccAveragingEnabled averagingEnabled;
                    int averagingCount;

                    public void SetDefaults()
                    {
                        synchronizationMode = RFmxNRMXModAccSynchronizationMode.Slot;
                        measurementLengthUnit = RFmxNRMXModAccMeasurementLengthUnit.Slot;
                        measurementOffset = 0;
                        measurementLength = 1;

                        averagingEnabled = RFmxNRMXModAccAveragingEnabled.False;
                        averagingCount = 10;
                    }
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
                    public void SetDefaults()
                    {

                    }
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
                #endregion

            }
            #endregion
        }
    }
}
