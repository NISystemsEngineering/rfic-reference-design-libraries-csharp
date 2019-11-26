using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.NRMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxNR
    {
        #region Type Definitions
        /// <summary>Defines common settings for a single component carrier.</summary>
        public struct ComponentCarrierConfiguration
        {
            public double Bandwidth_Hz;
            public int CellId;
            public RFmxNRMXPuschModulationType PuschModulationType;
            public double SubcarrierSpacing_Hz;
            public int PuschResourceBlockOffset;
            public int PuschNumberOfResourceBlocks;
            public RFmxNRMXPuschTransformPrecodingEnabled PuschTransformPrecodingEnabled;
            public string PuschSlotAllocation;
            public string PuschSymbolAllocation;
            public RFmxNRMXPuschDmrsConfigurationType PuschDmrsConfigurationType;
            public RFmxNRMXPuschMappingType PuschMappingType;
            public int PuschDmrsTypeAPosition;
            public RFmxNRMXPuschDmrsDuration PuschDmrsDuration;
            public int PuschDmrsAdditionalPositions;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static ComponentCarrierConfiguration GetDefault()
            {
                return new ComponentCarrierConfiguration()
                {
                    Bandwidth_Hz = 100e6,
                    CellId = 0,
                    PuschModulationType = RFmxNRMXPuschModulationType.Qpsk,
                    SubcarrierSpacing_Hz = 30e3,
                    PuschResourceBlockOffset = 0,
                    PuschNumberOfResourceBlocks = -1,
                    PuschTransformPrecodingEnabled = RFmxNRMXPuschTransformPrecodingEnabled.False,
                    PuschSlotAllocation = "0-Last",
                    PuschSymbolAllocation = "0-Last",
                    PuschDmrsConfigurationType = RFmxNRMXPuschDmrsConfigurationType.Type1,
                    PuschMappingType = RFmxNRMXPuschMappingType.TypeA,
                    PuschDmrsTypeAPosition = 2,
                    PuschDmrsDuration = RFmxNRMXPuschDmrsDuration.SingleSymbol,
                    PuschDmrsAdditionalPositions = 0
                };
            }
        }

        /// <summary>Defines common settings related to the standard of the measured NR signal.</summary>
        public struct StandardConfiguration
        {
            public RFmxNRMXLinkDirection LinkDirection;
            public RFmxNRMXFrequencyRange FrequencyRange;
            public int Band;
            public RFmxNRMXAutoResourceBlockDetectionEnabled AutoResourceBlockDetectionEnabled;
            public RFmxNRMXDownlinkTestModel DownlinkTestModel;
            public RFmxNRMXDownlinkTestModelDuplexScheme DownlinkTestModelDuplexScheme;
            public ComponentCarrierConfiguration[] ComponentCarrierConfigurations;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static StandardConfiguration GetDefault()
            {
                return new StandardConfiguration
                {
                    LinkDirection = RFmxNRMXLinkDirection.Uplink,
                    FrequencyRange = RFmxNRMXFrequencyRange.Range1,
                    Band = 78,
                    AutoResourceBlockDetectionEnabled = RFmxNRMXAutoResourceBlockDetectionEnabled.True,
                    DownlinkTestModel = RFmxNRMXDownlinkTestModel.TM1_1,
                    DownlinkTestModelDuplexScheme = RFmxNRMXDownlinkTestModelDuplexScheme.Fdd,
                    ComponentCarrierConfigurations = new ComponentCarrierConfiguration[] { ComponentCarrierConfiguration.GetDefault() }
                };
            }
        }

        /// <summary>Defines common settings for the ModAcc measurement.</summary>
        public struct ModAccConfiguration
        {
            public RFmxNRMXModAccMeasurementLengthUnit MeasurementLengthUnit;
            public double MeasurementOffset;
            public double MeasurementLength;
            public RFmxNRMXModAccEvmUnit EvmUnit;
            public RFmxNRMXModAccAveragingEnabled AveragingEnabled;
            public int AveragingCount;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static ModAccConfiguration GetDefault()
            {
                return new ModAccConfiguration
                {
                    MeasurementLengthUnit = RFmxNRMXModAccMeasurementLengthUnit.Slot,
                    MeasurementOffset = 0,
                    MeasurementLength = 1,
                    EvmUnit = RFmxNRMXModAccEvmUnit.Percentage,
                    AveragingEnabled = RFmxNRMXModAccAveragingEnabled.False,
                    AveragingCount = 10
                };
            }
        }

        /// <summary>Defines common results of the ModAcc measurement for a single component carrier.</summary>
        public struct ModAccComponentCarrierResults
        {
            public int PeakCompositeEvmSubcarrierIndex;
            public int PeakCompositeEvmSymbolIndex;
            public double MeanRmsCompositeEvm;
            public double MaxPeakCompositeEvm;
            public double MeanFrequencyError_Hz;
            public int PeakCompositeEvmSlotIndex;
        }

        /// <summary>Defines common results of the ModAcc measurement.</summary>
        public struct ModAccResults
        {
            public ModAccComponentCarrierResults[] ComponentCarrierResults;
        }

        /// <summary>Defines common settings for the ACP measurement.</summary>
        public struct AcpConfiguration
        {
            public int NumberOfUtraOffsets;
            public int NumberOfEutraOffsets;
            public int NumberOfNrOffsets;
            public int NumberOfEndcOffsets;
            public RFmxNRMXAcpMeasurementMethod MeasurementMethod;
            public RFmxNRMXAcpNoiseCompensationEnabled NoiseCompensationEnabled;
            public RFmxNRMXAcpSweepTimeAuto SweepTimeAuto;
            public double SweepTimeInterval_s;
            public RFmxNRMXAcpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxNRMXAcpAveragingType AveragingType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AcpConfiguration GetDefault()
            {
                return new AcpConfiguration
                {
                    NumberOfUtraOffsets = 2,
                    NumberOfEutraOffsets = 0,
                    NumberOfNrOffsets = 1,
                    NumberOfEndcOffsets = 0,
                    MeasurementMethod = RFmxNRMXAcpMeasurementMethod.Normal,
                    NoiseCompensationEnabled = RFmxNRMXAcpNoiseCompensationEnabled.False,
                    SweepTimeAuto = RFmxNRMXAcpSweepTimeAuto.True,
                    SweepTimeInterval_s = 1.0e-3,
                    AveragingEnabled = RFmxNRMXAcpAveragingEnabled.False,
                    AveragingCount = 10,
                    AveragingType = RFmxNRMXAcpAveragingType.Rms,
                };
            }
        }

        /// <summary>Defines common results of the ACP measurement for a single defined offset.</summary>
        public struct AcpOffsetResults
        {
            public double LowerAbsolutePower_dBm;
            public double LowerRelativePower_dB;
            public double UpperAbsolutePower_dBm;
            public double UpperRelativePower_dB;
            public double Frequency_Hz;
            public double IntegrationBandwidth_Hz;
        }

        /// <summary>Defines common results of the ACP measurement for a single component carrier.</summary>
        public struct AcpComponentCarrierResults
        {
            public double AbsolutePower_dBm;
            public double RelativePower_dB;
        }

        /// <summary>Defines common results of the ACP measurement.</summary>
        public struct AcpResults
        {
            public AcpOffsetResults[] OffsetResults;
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }

        /// <summary>Defines common settings for the CHP measurement.</summary>
        public struct ChpConfiguration
        {
            public RFmxNRMXChpSweepTimeAuto SweepTimeAuto;
            public double SweepTimeInterval_s;
            public RFmxNRMXChpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxNRMXChpAveragingType AveragingType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static ChpConfiguration GetDefault()
            {
                return new ChpConfiguration
                {
                    SweepTimeAuto = RFmxNRMXChpSweepTimeAuto.True,
                    SweepTimeInterval_s = 1.0e-3,
                    AveragingEnabled = RFmxNRMXChpAveragingEnabled.False,
                    AveragingCount = 10,
                    AveragingType = RFmxNRMXChpAveragingType.Rms
                };
            }
        }

        /// <summary>Defines common results of the CHP measurement for a single component carrier.</summary>
        public struct ChpComponentCarrierResults
        {
            public double AbsolutePower_dBm;
            public double RelativePower_dB;
        }

        /// <summary>Defines common results of the CHP measurement.</summary>
        public struct ChpResults
        {
            public double TotalAggregatedPower_dBm;
            public ChpComponentCarrierResults[] ComponentCarrierResults;
        }
        #endregion

        #region Instrument Configuration

        /// <summary>Configures common measurement settings for the personality.</summary>
        /// <param name="nr">Specifies the NR signal to configure.</param>
        /// <param name="commonConfig">Specifies the common settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureCommon(RFmxNRMX nr, CommonConfiguration commonConfig, string selectorString = "")
        {
            nr.SetSelectedPorts(selectorString, commonConfig.SelectedPorts);
            nr.ConfigureRF(selectorString, commonConfig.CenterFrequency_Hz, commonConfig.ReferenceLevel_dBm, commonConfig.ExternalAttenuation_dB);
            nr.ConfigureFrequency(selectorString, commonConfig.CenterFrequency_Hz);
            nr.ConfigureExternalAttenuation(selectorString, commonConfig.ExternalAttenuation_dB);
            nr.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalTriggerSource, RFmxNRMXDigitalEdgeTriggerEdge.Rising, commonConfig.TriggerDelay_s, commonConfig.TriggerEnabled);
        }
        #endregion

        #region Measurement Configuration

        /// <summary>Configures common settings related to the NR standard of the measured signal.</summary>
        /// <param name="nr">Specifies the NR signal to configure.</param>
        /// <param name="standardConfig">Specifies the WLAN standard settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureStandard(RFmxNRMX nr, StandardConfiguration standardConfig, string selectorString = "")
        {
            nr.SetComponentCarrierSpacingType(selectorString, RFmxNRMXComponentCarrierSpacingType.Nominal); // nominal is assumed
            nr.SetLinkDirection(selectorString, standardConfig.LinkDirection);
            nr.SetFrequencyRange(selectorString, standardConfig.FrequencyRange);
            nr.SetBand(selectorString, standardConfig.Band);
            nr.SetAutoResourceBlockDetectionEnabled(selectorString, standardConfig.AutoResourceBlockDetectionEnabled);
            nr.ComponentCarrier.SetDownlinkTestModel(selectorString, standardConfig.DownlinkTestModel);
            nr.ComponentCarrier.SetDownlinkTestModelDuplexScheme(selectorString, standardConfig.DownlinkTestModelDuplexScheme);
            nr.ComponentCarrier.SetNumberOfComponentCarriers(selectorString, standardConfig.ComponentCarrierConfigurations.Length);
            for (int i = 0; i < standardConfig.ComponentCarrierConfigurations.Length; i++)
            {
                string carrierString = RFmxNRMX.BuildCarrierString(selectorString, i);
                ComponentCarrierConfiguration componentCarrierConfig = standardConfig.ComponentCarrierConfigurations[i];
                nr.ComponentCarrier.SetBandwidth(carrierString, componentCarrierConfig.Bandwidth_Hz);
                nr.ComponentCarrier.SetCellID(carrierString, componentCarrierConfig.CellId);
                nr.ComponentCarrier.SetPuschModulationType(carrierString, componentCarrierConfig.PuschModulationType);
                nr.ComponentCarrier.SetBandwidthPartSubcarrierSpacing(carrierString, componentCarrierConfig.SubcarrierSpacing_Hz);
                nr.ComponentCarrier.SetPuschResourceBlockOffset(carrierString, componentCarrierConfig.PuschResourceBlockOffset);
                nr.ComponentCarrier.SetPuschNumberOfResourceBlocks(carrierString, componentCarrierConfig.PuschNumberOfResourceBlocks);
                nr.ComponentCarrier.SetPuschTransformPrecodingEnabled(carrierString, componentCarrierConfig.PuschTransformPrecodingEnabled);
                nr.ComponentCarrier.SetPuschSlotAllocation(carrierString, componentCarrierConfig.PuschSlotAllocation);
                nr.ComponentCarrier.SetPuschSymbolAllocation(carrierString, componentCarrierConfig.PuschSymbolAllocation);
                nr.ComponentCarrier.SetPuschDmrsConfigurationType(carrierString, componentCarrierConfig.PuschDmrsConfigurationType);
                nr.ComponentCarrier.SetPuschMappingType(carrierString, componentCarrierConfig.PuschMappingType);
                nr.ComponentCarrier.SetPuschDmrsTypeAPosition(carrierString, componentCarrierConfig.PuschDmrsTypeAPosition);
                nr.ComponentCarrier.SetPuschDmrsDuration(carrierString, componentCarrierConfig.PuschDmrsDuration);
                nr.ComponentCarrier.SetPuschDmrsAdditionalPositions(carrierString, componentCarrierConfig.PuschDmrsAdditionalPositions);
            }
        }

        /// <summary>Configures common settings for the ModAcc measurement and selects the measurement.</summary>
        /// <param name="nr">Specifies the NR signal to configure.</param>
        /// <param name="modAccConfig">Specifies the ModAcc settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureModacc(RFmxNRMX nr, ModAccConfiguration modAccConfig, string selectorString = "")
        {
            nr.SelectMeasurements(selectorString, RFmxNRMXMeasurementTypes.ModAcc, false);
            nr.ModAcc.Configuration.SetMeasurementLengthUnit(selectorString, modAccConfig.MeasurementLengthUnit);
            nr.ModAcc.Configuration.SetMeasurementOffset(selectorString, modAccConfig.MeasurementOffset);
            nr.ModAcc.Configuration.SetMeasurementLength(selectorString, modAccConfig.MeasurementLength);
            nr.ModAcc.Configuration.SetEvmUnit(selectorString, modAccConfig.EvmUnit);
            nr.ModAcc.Configuration.SetAveragingEnabled(selectorString, modAccConfig.AveragingEnabled);
            nr.ModAcc.Configuration.SetAveragingCount(selectorString, modAccConfig.AveragingCount);
        }

        /// <summary>Configures common settings for the ACP measurement and selects the measurement.</summary>
        /// <param name="nr">Specifies the NR signal to configure.</param>
        /// <param name="acpConfig">Specifies the ACP settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureAcp(RFmxNRMX nr, AcpConfiguration acpConfig, string selectorString = "")
        {
            nr.SelectMeasurements(selectorString, RFmxNRMXMeasurementTypes.Acp, false);
            nr.Acp.Configuration.ConfigureNumberOfUtraOffsets(selectorString, acpConfig.NumberOfUtraOffsets);
            nr.Acp.Configuration.ConfigureNumberOfEutraOffsets(selectorString, acpConfig.NumberOfEutraOffsets);
            nr.Acp.Configuration.ConfigureNumberOfNROffsets(selectorString, acpConfig.NumberOfNrOffsets);
            nr.Acp.Configuration.ConfigureNumberOfEndcOffsets(selectorString, acpConfig.NumberOfEndcOffsets);
            nr.Acp.Configuration.ConfigureMeasurementMethod(selectorString, acpConfig.MeasurementMethod);
            nr.Acp.Configuration.ConfigureNoiseCompensationEnabled(selectorString, acpConfig.NoiseCompensationEnabled);
            nr.Acp.Configuration.ConfigureSweepTime(selectorString, acpConfig.SweepTimeAuto, acpConfig.SweepTimeInterval_s);
            nr.Acp.Configuration.ConfigureAveraging(selectorString, acpConfig.AveragingEnabled, acpConfig.AveragingCount, acpConfig.AveragingType);
        }

        /// <summary>Configures common settings for the CHP measurement and selects the measurement.</summary>
        /// <param name="nr">Specifies the NR signal to configure.</param>
        /// <param name="chpConfig">Specifies the CHP settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureChp(RFmxNRMX nr, ChpConfiguration chpConfig, string selectorString = "")
        {
            nr.SelectMeasurements(selectorString, RFmxNRMXMeasurementTypes.Chp, false);
            nr.Chp.Configuration.ConfigureSweepTime(selectorString, chpConfig.SweepTimeAuto, chpConfig.SweepTimeInterval_s);
            nr.Chp.Configuration.ConfigureAveraging(selectorString, chpConfig.AveragingEnabled, chpConfig.AveragingCount, chpConfig.AveragingType);
        }

        /// <summary>Performs actions to initiate acquisition and measurement.<para></para> Enables the specified measurement(s) before optionally 
        /// automatically adjusting the reference level before beginning measurements. Finally, initiates the acquisition and measurement(s).</summary>
        /// <param name="nr">Specifies the NR signal to configure.</param>
        /// <param name="measurements">Specifies one or more previously configured measurements to enable for this acquisition.</param>
        /// <param name="autoLevelConfig">Specifies the configuration for the optional AutoLevel process which will automatically set the analyzer's reference level.</param>
        /// <param name="enableTraces">(Optional) Specifies whether traces should be enabled for the measurement(s).</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        /// <param name="resultName">(Optional) Specifies the name to be associated with measurement results. Provide a unique name, such as "r1" to enable 
        /// fetching of multiple measurement results and traces. See the RFmx help for more documentation of this parameter.</param>
        public static void SelectAndInitiateMeasurements(RFmxNRMX nr, RFmxNRMXMeasurementTypes[] measurements, AutoLevelConfiguration autoLevelConfig,
            bool enableTraces = false, string selectorString = "", string resultName = "")
        {
            // Aggregate the selected measurements into a single value
            // OR of 0 and x equals x
            RFmxNRMXMeasurementTypes selectedMeasurements = 0;
            foreach (RFmxNRMXMeasurementTypes measurement in measurements)
                selectedMeasurements |= measurement;
            nr.SelectMeasurements(selectorString, selectedMeasurements, enableTraces);

            if (autoLevelConfig.Enabled)
                nr.AutoLevel(selectorString, autoLevelConfig.MeasurementInterval_s, out double _);

            // Initiate acquisition and measurement for the selected measurements
            nr.Initiate(selectorString, resultName);
        }
        #endregion

        #region Measurement Results

        /// <summary>Fetches common results from the ModAcc measurement.</summary>
        /// <param name="nr">Specifies the NR signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        public static ModAccResults FetchModAcc(RFmxNRMX nr, string selectorString = "")
        {
            nr.ComponentCarrier.GetNumberOfComponentCarriers(selectorString, out int numComponentCarriers);
            ModAccResults modaccResults = new ModAccResults()
            {
                ComponentCarrierResults = new ModAccComponentCarrierResults[numComponentCarriers]
            };
            for (int i = 0; i < numComponentCarriers; i++)
            {
                ModAccComponentCarrierResults componentCarrierResults;
                string carrierString = RFmxNRMX.BuildCarrierString(selectorString, i);
                nr.ModAcc.Results.GetCompositePeakEvmSubcarrierIndex(carrierString, out componentCarrierResults.PeakCompositeEvmSubcarrierIndex);
                nr.ModAcc.Results.GetCompositePeakEvmSymbolIndex(carrierString, out componentCarrierResults.PeakCompositeEvmSymbolIndex);
                nr.ModAcc.Results.GetCompositeRmsEvmMean(carrierString, out componentCarrierResults.MeanRmsCompositeEvm);
                nr.ModAcc.Results.GetCompositePeakEvmMaximum(carrierString, out componentCarrierResults.MaxPeakCompositeEvm);
                nr.ModAcc.Results.GetComponentCarrierFrequencyErrorMean(carrierString, out componentCarrierResults.MeanFrequencyError_Hz);
                nr.ModAcc.Results.GetCompositePeakEvmSlotIndex(carrierString, out componentCarrierResults.PeakCompositeEvmSlotIndex);
                modaccResults.ComponentCarrierResults[i] = componentCarrierResults;
            }
            return modaccResults;
        }

        /// <summary>Fetches common results from the ACP measurement.</summary>
        /// <param name="nr">Specifies the NR signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        public static AcpResults FetchAcp(RFmxNRMX nr, string selectorString = "")
        {
            double[] lowerRelativePower = null;
            double[] upperRelativePower = null;
            double[] lowerAbsolutePower = null;
            double[] upperAbsolutePower = null;
            nr.Acp.Results.FetchOffsetMeasurementArray(selectorString, 10.0, 
                ref lowerRelativePower, ref upperRelativePower, ref lowerAbsolutePower, ref upperAbsolutePower);
            AcpResults results = new AcpResults()
            {
                OffsetResults = new AcpOffsetResults[lowerRelativePower.Length]
            };
            for (int i = 0; i < lowerRelativePower.Length; i++)
            {
                string offsetString = RFmxNRMX.BuildOffsetString(selectorString, i);
                nr.Acp.Configuration.GetOffsetFrequency(offsetString, out double offsetFrequency);
                nr.Acp.Configuration.GetOffsetIntegrationBandwidth(offsetString, out double offsetIbw);
                results.OffsetResults[i] = new AcpOffsetResults()
                {
                    LowerRelativePower_dB = lowerRelativePower[i],
                    UpperRelativePower_dB = upperRelativePower[i],
                    LowerAbsolutePower_dBm = lowerAbsolutePower[i],
                    UpperAbsolutePower_dBm = upperAbsolutePower[i],
                    Frequency_Hz = offsetFrequency,
                    IntegrationBandwidth_Hz = offsetIbw
                };
            }
            double[] absolutePower = null;
            double[] relativePower = null;
            nr.Acp.Results.ComponentCarrier.FetchMeasurementArray(selectorString, 10.0, ref absolutePower, ref relativePower);
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

        /// <summary>Fetches common results from the CHP measurement.</summary>
        /// <param name="nr">Specifies the NR signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        public static ChpResults FetchChp(RFmxNRMX nr, string selectorString = "")
        {
            ChpResults results;
            nr.Chp.Results.FetchTotalAggregatedPower(selectorString, 10.0, out results.TotalAggregatedPower_dBm);
            double[] absolutePowers = null;
            double[] relativePowers = null;
            nr.Chp.Results.ComponentCarrier.FetchMeasurementArray(selectorString, 10.0, ref absolutePowers, ref relativePowers);
            results.ComponentCarrierResults = new ChpComponentCarrierResults[absolutePowers.Length];
            for (int i = 0; i < absolutePowers.Length; i++)
            {
                results.ComponentCarrierResults[i] = new ChpComponentCarrierResults()
                {
                    AbsolutePower_dBm = absolutePowers[i],
                    RelativePower_dB = relativePowers[i]
                };
            }
            return results;
        }
        #endregion
    }
}
