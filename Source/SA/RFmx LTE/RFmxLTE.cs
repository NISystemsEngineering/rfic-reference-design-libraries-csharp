using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.LteMX;
using System.Text.RegularExpressions;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxLTE
    {
        #region Type Definitions
        public struct CommonConfiguration
        {
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string FrequencyReferenceSource;
            public bool EnableTrigger;
            public string DigitalEdgeSource;
            public RFmxLteMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            
            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    CenterFrequency_Hz = 3.5e9,
                    ReferenceLevel_dBm = 0.0,
                    ExternalAttenuation_dB = 0.0,
                    FrequencyReferenceSource = RFmxInstrMXConstants.PxiClock,
                    EnableTrigger = true,
                    DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0,
                    DigitalEdgeType = RFmxLteMXDigitalEdgeTriggerEdge.Rising,
                    TriggerDelay_s = 0.0
                };
            }
        }

        public struct ComponentCarrierConfiguration
        {
            public double Bandwidth;
            public int CellId;
            public RFmxLteMXPuschModulationType PuschModulationType;
            public int PuschResourceBlockOffset;
            public int PuschNumberOfResourceBlocks;
            public RFmxLteMXDownlinkTestModel DownlinkTestModel;

            public static ComponentCarrierConfiguration GetDefault()
            {
                return new ComponentCarrierConfiguration()
                {
                    Bandwidth = 10e6,
                    CellId = 0,
                    PuschModulationType = RFmxLteMXPuschModulationType.ModulationTypeQpsk,
                    PuschResourceBlockOffset = 0,
                    PuschNumberOfResourceBlocks = -1,
                    DownlinkTestModel = RFmxLteMXDownlinkTestModel.TM1_1
                };
            }
        }

        public struct SignalConfiguration
        {
            public RFmxLteMXLinkDirection LinkDirection;
            public RFmxLteMXDuplexScheme DuplexScheme;
            public RFmxLteMXUplinkDownlinkConfiguration UplinkDownlinkConfiguration;
            public int Band;
            public RFmxLteMXAutoResourceBlockDetectionEnabled PuschAutoResourceBlockDetectionEnabled;
            public RFmxLteMXAutoDmrsDetectionEnabled AutoDmrsDetectionEnabled;
            public RFmxLteMXDownlinkAutoCellIDDetectionEnabled DownlinkAutoCellIDDetectionEnabled;
            public ComponentCarrierConfiguration[] ComponentCarrierConfigurations;

            public static SignalConfiguration GetDefault()
            {
                return new SignalConfiguration()
                {
                    LinkDirection = RFmxLteMXLinkDirection.Uplink,
                    DuplexScheme = RFmxLteMXDuplexScheme.Fdd,
                    UplinkDownlinkConfiguration = RFmxLteMXUplinkDownlinkConfiguration.Configuration0,
                    Band = 1,
                    PuschAutoResourceBlockDetectionEnabled = RFmxLteMXAutoResourceBlockDetectionEnabled.True,
                    AutoDmrsDetectionEnabled = RFmxLteMXAutoDmrsDetectionEnabled.True,
                    DownlinkAutoCellIDDetectionEnabled = RFmxLteMXDownlinkAutoCellIDDetectionEnabled.False,
                    ComponentCarrierConfigurations = new ComponentCarrierConfiguration[] { ComponentCarrierConfiguration.GetDefault() }
                };
            }
        }

        public struct ModAccConfiguration
        {
            public RFmxLteMXModAccSynchronizationMode SynchronizationMode;
            public int MeasurementOffset;
            public int MeasurementLength;
            public RFmxLteMXModAccEvmUnit EvmUnit;
            public RFmxLteMXModAccAveragingEnabled AveragingEnabled;
            public int AveragingCount;

            public static ModAccConfiguration GetDefault()
            {
                return new ModAccConfiguration()
                {
                    SynchronizationMode = RFmxLteMXModAccSynchronizationMode.Slot,
                    MeasurementOffset = 0,
                    MeasurementLength = 1,
                    EvmUnit = RFmxLteMXModAccEvmUnit.Percentage,
                    AveragingEnabled = RFmxLteMXModAccAveragingEnabled.False,
                    AveragingCount = 10
                };
            }
        }

        public struct ModAccComponentCarrierResults
        {
            public int PeakCompositeEvmSubcarrierIndex;
            public int PeakCompositeEvmSymbolIndex;
            public double MeanRmsCompositeEvm;
            public double MaxPeakCompositeEvm;
            public double MeanFrequencyError;
            public int PeakCompositeEvmSlotIndex;
        }

        public struct ModAccResults
        {
            public ModAccComponentCarrierResults[] ComponentCarrierResults;
        }

        public struct AcpConfiguration
        {
            public int NumberOfGsmOffsets;
            public int NumberOfUtraOffsets;
            public int NumberOfEutraOffsets;
            public RFmxLteMXAcpSweepTimeAuto SweepTimeAuto;
            public double SweepTimeInterval_s;
            public RFmxLteMXAcpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxLteMXAcpAveragingType AveragingType;
            public RFmxLteMXAcpRbwAutoBandwidth RbwAuto;
            public double Rbw_Hz;
            public RFmxLteMXAcpRbwFilterType RbwFilterType;

            public static AcpConfiguration GetDefault()
            {
                return new AcpConfiguration()
                {
                    NumberOfGsmOffsets = 0,
                    NumberOfUtraOffsets = 2,
                    NumberOfEutraOffsets = 1,
                    SweepTimeAuto = RFmxLteMXAcpSweepTimeAuto.True,
                    SweepTimeInterval_s = 0.001,
                    AveragingEnabled = RFmxLteMXAcpAveragingEnabled.False,
                    AveragingCount = 10,
                    AveragingType = RFmxLteMXAcpAveragingType.Rms,
                    RbwAuto = RFmxLteMXAcpRbwAutoBandwidth.True,
                    Rbw_Hz = 30e3,
                    RbwFilterType = RFmxLteMXAcpRbwFilterType.FftBased
                };
            }
        }

        public struct AcpOffsetResults
        {
            public double LowerAbsolutePower_dBm;
            public double LowerRelativePower_dBm;
            public double UpperAbsolutePower_dB;
            public double UpperRelativePower_dB;
            public double OffsetFrequency_Hz;
            public double OffsetIntegrationBandwidth_Hz;
        }

        public struct AcpComponentCarrierResults
        {
            public double AbsolutePower_dBm;
            public double RelativePower_dB;
        }

        public struct AcpResults
        {
            public AcpOffsetResults[] OffsetResults;
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }
        #endregion

        #region Instrument Configuration
        public static void ConfigureCommon(RFmxInstrMX instr, RFmxLteMX lte, CommonConfiguration commonConfig, string selectorString = "")
        {
            instr.ConfigureFrequencyReference(selectorString, commonConfig.FrequencyReferenceSource, 10e6);
            lte.ConfigureRF(selectorString, commonConfig.CenterFrequency_Hz, commonConfig.ReferenceLevel_dBm, commonConfig.ExternalAttenuation_dB);
            lte.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalEdgeSource, commonConfig.DigitalEdgeType, commonConfig.TriggerDelay_s, commonConfig.EnableTrigger);
        }
        #endregion

        #region Measurement Configuration
        public static void ConfigureSignal(RFmxLteMX lte, SignalConfiguration signalConfig, string selectorString = "")
        {
            lte.ComponentCarrier.SetSpacingType(selectorString, RFmxLteMXComponentCarrierSpacingType.Nominal); // nominal spacing is assumed
            lte.ConfigureLinkDirection(selectorString, signalConfig.LinkDirection);
            lte.ConfigureDuplexScheme(selectorString, signalConfig.DuplexScheme, signalConfig.UplinkDownlinkConfiguration);
            lte.ConfigureBand(selectorString, signalConfig.Band);
            lte.ConfigureNumberOfComponentCarriers(selectorString, signalConfig.ComponentCarrierConfigurations.Length);
            for (int i = 0; i < signalConfig.ComponentCarrierConfigurations.Length; i++)
            {
                string carrierString = RFmxLteMX.BuildCarrierString(selectorString, i);
                ComponentCarrierConfiguration componentCarrierConfig = signalConfig.ComponentCarrierConfigurations[i];
                lte.ComponentCarrier.SetBandwidth(carrierString, componentCarrierConfig.Bandwidth);
                lte.ComponentCarrier.SetCellId(carrierString, componentCarrierConfig.CellId);
                lte.ComponentCarrier.ConfigurePuschModulationType(carrierString, componentCarrierConfig.PuschModulationType);
                lte.ComponentCarrier.ConfigurePuschResourceBlocks(carrierString, componentCarrierConfig.PuschResourceBlockOffset, componentCarrierConfig.PuschNumberOfResourceBlocks);
                lte.ComponentCarrier.ConfigureDownlinkTestModel(carrierString, componentCarrierConfig.DownlinkTestModel);
            }
            lte.ComponentCarrier.ConfigureAutoResourceBlockDetectionEnabled(selectorString, signalConfig.PuschAutoResourceBlockDetectionEnabled);
            lte.ConfigureAutoDmrsDetectionEnabled(selectorString, signalConfig.AutoDmrsDetectionEnabled);
            lte.ComponentCarrier.ConfigureDownlinkAutoCellIDDetectionEnabled(selectorString, signalConfig.DownlinkAutoCellIDDetectionEnabled);
        }

        public static void ConfigureAcp(RFmxLteMX lte, AcpConfiguration acpConfig, string selectorString = "")
        {
            lte.Acp.Configuration.SetMeasurementEnabled(selectorString, true);
            lte.Acp.Configuration.SetAllTracesEnabled(selectorString, true);
            lte.Acp.Configuration.ConfigureNumberOfEutraOffsets(selectorString, acpConfig.NumberOfEutraOffsets);
            lte.Acp.Configuration.ConfigureNumberOfUtraOffsets(selectorString, acpConfig.NumberOfUtraOffsets);
            lte.Acp.Configuration.ConfigureNumberOfGsmOffsets(selectorString, acpConfig.NumberOfGsmOffsets);
            lte.Acp.Configuration.ConfigureRbwFilter(selectorString, acpConfig.RbwAuto, acpConfig.Rbw_Hz, acpConfig.RbwFilterType);
            lte.Acp.Configuration.ConfigureAveraging(selectorString, acpConfig.AveragingEnabled, acpConfig.AveragingCount, acpConfig.AveragingType);
            lte.Acp.Configuration.ConfigureSweepTime(selectorString, acpConfig.SweepTimeAuto, acpConfig.SweepTimeInterval_s);
        }

        public static void ConfigureModAcc(RFmxLteMX lte, ModAccConfiguration modAccConfig, string selectorString = "")
        {
            lte.ModAcc.Configuration.SetMeasurementEnabled(selectorString, true);
            lte.ModAcc.Configuration.SetAllTracesEnabled(selectorString, true);
            lte.ModAcc.Configuration.ConfigureAveraging(selectorString, modAccConfig.AveragingEnabled, modAccConfig.AveragingCount);
            lte.ModAcc.Configuration.ConfigureSynchronizationModeAndInterval(selectorString, modAccConfig.SynchronizationMode, modAccConfig.MeasurementOffset, modAccConfig.MeasurementLength);
            lte.ModAcc.Configuration.ConfigureEvmUnit(selectorString, modAccConfig.EvmUnit);
        }
        #endregion

        #region Measurement Results
        public static AcpResults FetchAcp(RFmxLteMX lte, string selectorString = "")
        {
            double[] lowerRelativePower = null;
            double[] upperRelativePower = null;
            double[] lowerAbsolutePower = null;
            double[] upperAbsolutePower = null;
            lte.Acp.Results.FetchOffsetMeasurementArray(selectorString, 10.0, ref lowerRelativePower, ref upperRelativePower, ref lowerAbsolutePower, ref upperAbsolutePower);
            AcpResults results;
            results.OffsetResults = new AcpOffsetResults[lowerAbsolutePower.Length];
            for (int i = 0; i < lowerAbsolutePower.Length; i++)
            {
                string signalString = Regex.Match(selectorString, "signal::[^/]*").Value; // get the signal string
                lte.Acp.Configuration.GetOffsetFrequency(signalString, out double offsetFrequency);
                lte.Acp.Configuration.GetOffsetIntegrationBandwidth(signalString, out double offsetIbw);
                results.OffsetResults[i] = new AcpOffsetResults()
                {
                    LowerRelativePower_dBm = lowerRelativePower[i],
                    UpperRelativePower_dB = upperRelativePower[i],
                    LowerAbsolutePower_dBm = lowerAbsolutePower[i],
                    UpperAbsolutePower_dB = upperAbsolutePower[i],
                    OffsetFrequency_Hz = offsetFrequency,
                    OffsetIntegrationBandwidth_Hz = offsetIbw
                };
            }
            double[] absolutePower = null;
            double[] relativePower = null;
            lte.Acp.Results.ComponentCarrier.FetchMeasurementArray(selectorString, 10.0, ref absolutePower, ref relativePower);
            results.ComponentCarrierResults = new AcpComponentCarrierResults[absolutePower.Length];
            for (int i = 0; i < absolutePower.Length; i++)
            {
                results.ComponentCarrierResults[i] = new AcpComponentCarrierResults()
                {
                    AbsolutePower_dBm = absolutePower[i],
                    RelativePower_dB = relativePower[i]
                };
            }
            return results;
        }

        public static ModAccResults FetchModAcc(RFmxLteMX lte, string selectorString = "")
        {
            double[] meanRmsCompositeEvm = null;
            double[] maxPeakCompositeEvm = null;
            double[] meanFrequencyError = null;
            int[] peakCompositeEvmSymbolIndex = null;
            int[] peakCompositeEvmSubcarrierIndex = null;
            int[] peakCompositeEvmSlotIndex = null;
            lte.ModAcc.Results.FetchCompositeEvmArray(selectorString, 10.0, ref meanRmsCompositeEvm,
                ref maxPeakCompositeEvm, ref meanFrequencyError, ref peakCompositeEvmSymbolIndex,
                ref peakCompositeEvmSubcarrierIndex, ref peakCompositeEvmSlotIndex);
            ModAccResults results;
            results.ComponentCarrierResults = new ModAccComponentCarrierResults[meanRmsCompositeEvm.Length];
            for (int i = 0; i < meanRmsCompositeEvm.Length; i++)
            {
                results.ComponentCarrierResults[i] = new ModAccComponentCarrierResults()
                {
                    MeanRmsCompositeEvm = meanRmsCompositeEvm[i],
                    MaxPeakCompositeEvm = maxPeakCompositeEvm[i],
                    MeanFrequencyError = meanFrequencyError[i],
                    PeakCompositeEvmSymbolIndex = peakCompositeEvmSymbolIndex[i],
                    PeakCompositeEvmSubcarrierIndex = peakCompositeEvmSubcarrierIndex[i],
                    PeakCompositeEvmSlotIndex = peakCompositeEvmSlotIndex[i]
                };
            }
            return results;
        }
        #endregion
    }
}
