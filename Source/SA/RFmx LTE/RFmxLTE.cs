using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.LteMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    /// <summary>Defines common types and methods for NI-RFmx LTE measurements.</summary>
    public static class RFmxLTE
    {
        #region Type Definitions

        /// <summary>Defines common settings for a single component carrier.</summary>
        public struct ComponentCarrierConfiguration
        {
            /// <summary>Specifies the channel bandwidth of the signal being measured. See the RFmx help for more documention of this parameter.</summary>
            public double Bandwidth_Hz;
            /// <summary>Specifies a physical layer cell identity. See the RFmx help for more documention of this parameter.</summary>
            public int CellId;
            /// <summary>Specifies the modulation scheme used in the physical uplink shared channel (PUSCH) of the signal being measured. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXPuschModulationType PuschModulationType;
            /// <summary>Specifies the starting resource block number of a physical uplink shared channel (PUSCH) cluster. See the RFmx help for more documention of this parameter.</summary>
            public int PuschResourceBlockOffset;
            /// <summary>Specifies the number of consecutive resource blocks in a physical uplink shared channel (PUSCH) cluster. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public int PuschNumberOfResourceBlocks;
            /// <summary>Specifies the EUTRA test model type when you set <see cref="StandardConfiguration.LinkDirection"/> to Downlink. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXDownlinkTestModel DownlinkTestModel;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static ComponentCarrierConfiguration GetDefault()
            {
                return new ComponentCarrierConfiguration()
                {
                    Bandwidth_Hz = 10e6,
                    CellId = 0,
                    PuschModulationType = RFmxLteMXPuschModulationType.ModulationTypeQpsk,
                    PuschResourceBlockOffset = 0,
                    PuschNumberOfResourceBlocks = -1,
                    DownlinkTestModel = RFmxLteMXDownlinkTestModel.TM1_1
                };
            }
        }

        /// <summary>Defines common settings related to the standard of the measured LTE signal.</summary>
        public struct StandardConfiguration
        {
            /// <summary>Specifies the link direction of the received signal. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXLinkDirection LinkDirection;
            /// <summary>Specifies the duplexing technique of the signal being measured. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXDuplexScheme DuplexScheme;
            /// <summary>Specifies the configuration of the LTE frame structure in the time division duplex (TDD) mode. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXUplinkDownlinkConfiguration UplinkDownlinkConfiguration;
            /// <summary>Specifies the evolved universal terrestrial radio access (E-UTRA) operating frequency band of a subblock.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int Band;
            /// <summary>Specifies whether the values of the PUSCH Mod Type, PUSCH Num RB Clusters, PUSCH RB Offset, and PUSCH Num RBs properties are auto-detected by the 
            /// measurement or if you specify the values of these properties. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXAutoResourceBlockDetectionEnabled PuschAutoResourceBlockDetectionEnabled;
            /// <summary>Specifies whether you configure the values of the demodulation reference signal (DMRS) parameters, such as UL Group Hopping Enabled, 
            /// UL Sequence Hopping Enabled, Cell ID, PUSCH n_DMRS_1, PUSCH n_DMRS_2, and PUSCH Delta SS properties, or if the values of these properties 
            /// are auto-detected by the measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXAutoDmrsDetectionEnabled AutoDmrsDetectionEnabled;
            /// <summary>Specifies whether to enable autodetection of the cell ID. If the signal being measured does not contain primary and secondary sync signal (PSS/SSS),
            /// autodetection of cell ID is not possible. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXDownlinkAutoCellIDDetectionEnabled DownlinkAutoCellIDDetectionEnabled;
            /// <summary>Specifies individual component carrier configurations.</summary>
            public ComponentCarrierConfiguration[] ComponentCarrierConfigurations;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static StandardConfiguration GetDefault()
            {
                return new StandardConfiguration()
                {
                    LinkDirection = RFmxLteMXLinkDirection.Uplink,
                    DuplexScheme = RFmxLteMXDuplexScheme.Fdd,
                    UplinkDownlinkConfiguration = RFmxLteMXUplinkDownlinkConfiguration.Configuration0,
                    Band = 1,
                    PuschAutoResourceBlockDetectionEnabled = RFmxLteMXAutoResourceBlockDetectionEnabled.True,
                    AutoDmrsDetectionEnabled = RFmxLteMXAutoDmrsDetectionEnabled.False,
                    DownlinkAutoCellIDDetectionEnabled = RFmxLteMXDownlinkAutoCellIDDetectionEnabled.False,
                    ComponentCarrierConfigurations = new ComponentCarrierConfiguration[] { ComponentCarrierConfiguration.GetDefault() }
                };
            }
        }

        /// <summary>Defines common settings for the ModAcc measurement.</summary>
        public struct ModAccConfiguration
        {
            /// <summary>Specifies whether the measurement is performed from the frame or the slot boundary. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXModAccSynchronizationMode SynchronizationMode;
            /// <summary>Specifies the measurement offset to skip from the synchronization boundary. The synchronization boundary is specified by the 
            /// <see cref="SynchronizationMode"/> property. See the RFmx help for more documention of this parameter.</summary>
            public int MeasurementOffset;
            /// <summary>Specifies the number of slots to be measured. This value is expressed in slots. See the RFmx help for more documention of this parameter.</summary>
            public int MeasurementLength;
            /// <summary>Specifies the units of the EVM results. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXModAccEvmUnit EvmUnit;
            /// <summary>Specifies whether to enable averaging for the ModAcc measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXModAccAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set the <see cref="AveragingEnabled"/> property 
            /// to True. See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
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

        /// <summary>Defines common results of the ModAcc measurement for a single component carrier.</summary>
        public struct ModAccComponentCarrierResults
        {
            /// <summary>Returns the subcarrier index where the maximum peak composite EVM for ModAcc occurs. See the RFmx help for more documention of this parameter.</summary>
            public int PeakCompositeEvmSubcarrierIndex;
            /// <summary>Returns the symbol index where the maximum peak composite EVM for ModAcc occurs. See the RFmx help for more documention of this parameter.</summary>
            public int PeakCompositeEvmSymbolIndex;
            /// <summary>Returns the mean value of the RMS EVMs calculated on all the configured channels. See the RFmx help for more documention of this parameter.</summary>
            public double MeanRmsCompositeEvm;
            /// <summary>Returns the maximum value of the peak EVMs calculated on all the configured channels. See the RFmx help for more documention of this parameter.</summary>
            public double MaxPeakCompositeEvm;
            /// <summary>Returns the estimated carrier frequency offset averaged over the configured slots. This value is expressed in Hz. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public double MeanFrequencyError_Hz;
            /// <summary>Returns the slot index where the ModAcc maximum peak composite EVM occurs. See the RFmx help for more documention of this parameter.</summary>
            public int PeakCompositeEvmSlotIndex;
        }

        /// <summary>Defines common results of the ModAcc measurement.</summary>
        public struct ModAccResults
        {
            /// <summary>Returns the ModAcc results for each configured component carrier.</summary>
            public ModAccComponentCarrierResults[] ComponentCarrierResults;
        }

        /// <summary>Defines common settings for the ACP measurement.</summary>
        public struct AcpConfiguration
        {
            /// <summary>Specifies the number of GSM adjacent channel offsets to be configured. See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfGsmOffsets;
            /// <summary>Specifies the number of universal terrestrial radio access (UTRA) adjacent channel offsets to be configured at offset positions.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfUtraOffsets;
            /// <summary>Specifies the number of evolved universal terrestrial radio access (E-UTRA) adjacent channel offsets to be configured at offset positions.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfEutraOffsets;
            /// <summary>Specifies whether the measurement computes the sweep time. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXAcpSweepTimeAuto SweepTimeAuto;
            /// <summary>Specifies the sweep time when you set <see cref="SweepTimeAuto"/> to False. This value is expressed in seconds.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double SweepTimeInterval_s;
            /// <summary>Specifies whether to enable averaging for the ACP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXAcpAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set <see cref="AveragingEnabled"/> to True. See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies the averaging type for averaging multiple spectrum acquisitions. The averaged spectrum is used for ACP measurement. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXAcpAveragingType AveragingType;
            /// <summary>Specifies whether the measurement computes the RBW. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXAcpRbwAutoBandwidth RbwAuto;
            /// <summary>Specifies the bandwidth of the RBW filter, used to sweep the acquired signal, when you set <see cref="RbwAuto"/>
            /// This value is expressed in Hz. See the RFmx help for more documention of this parameter.</summary>
            public double Rbw_Hz;
            /// <summary>Specifies the shape of the RBW filter. See the RFmx help for more documention of this parameter.</summary>
            public RFmxLteMXAcpRbwFilterType RbwFilterType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
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

        /// <summary>Defines common results of the ACP measurement for a single defined offset.</summary>
        public struct AcpOffsetResults
        {
            /// <summary>Returns the lower (negative) offset channel power. For the intra-band noncontiguous type of carrier aggregation, 
            /// if this offset is not applicable, a NaN is returned. This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double LowerAbsolutePower_dBm;
            /// <summary>Returns the power in lower (negative) offset channel relative to the total aggregated power. This value is expressed in dB.
            /// For the intra-band noncontiguous type of carrier aggregation,if this offset is not applicable, a NaN is returned.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double LowerRelativePower_dB;
            /// <summary>Returns the upper (positive) offset channel power. For the intra-band noncontiguous type of carrier aggregation,
            /// if this offset is not applicable, the property returns a NaN. This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double UpperAbsolutePower_dBm;
            /// <summary>Returns the power in upper (positive) offset channel relative to the total aggregated power. This value is expressed in dB.
            /// For the intra-band noncontiguous type of carrier aggregation, if this offset is not applicable, a NaN is returned.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double UpperRelativePower_dB;
            /// <summary>Returns the absolute center frequency of the subblock, which is the center of the subblock integration bandwidth. This value is expressed in Hz. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public double Frequency_Hz;
            /// <summary>Returns the integration bandwidth used in calculating the power of the subblock. This value is expressed in Hz. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public double IntegrationBandwidth_Hz;
        }

        /// <summary>Defines common results of the ACP measurement for a single component carrier.</summary>
        public struct AcpComponentCarrierResults
        {
            /// <summary>Returns the power measured over the integration bandwidth of the carrier. This value is expressed in dBm.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double AbsolutePower_dBm;
            /// <summary>Returns the component carrier power relative to its subblock power. This value is expressed in dB.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double RelativePower_dB;
        }

        /// <summary>Defines common results of the ACP measurement.</summary>
        public struct AcpResults
        {
            /// <summary>Returns an array of results for each configured offset.</summary>
            public AcpOffsetResults[] OffsetResults;
            /// <summary>Returns an array of results for each configured component carrier.</summary>
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }
        #endregion

        #region Instrument Configuration

        /// <summary>Configures common measurement settings for the personality.</summary>
        /// <param name="lte">Specifies the LTE signal to configure.</param>
        /// <param name="commonConfig">Specifies the common settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureCommon(RFmxLteMX lte, CommonConfiguration commonConfig, string selectorString = "")
        {
            lte.SetSelectedPorts(selectorString, commonConfig.SelectedPorts);
            lte.ConfigureRF(selectorString, commonConfig.CenterFrequency_Hz, commonConfig.ReferenceLevel_dBm, commonConfig.ExternalAttenuation_dB);
            lte.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalTriggerSource, RFmxLteMXDigitalEdgeTriggerEdge.Rising, commonConfig.TriggerDelay_s, commonConfig.TriggerEnabled);
        }
        #endregion

        /// <summary>Configures common settings related to the LTE standard of the measured signal.</summary>
        /// <param name="lte">Specifies the LTE signal to configure.</param>
        /// <param name="standardConfig">Speci fies the LTE standard settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        #region Measurement Configuration
        public static void ConfigureStandard(RFmxLteMX lte, StandardConfiguration standardConfig, string selectorString = "")
        {
            lte.ComponentCarrier.SetSpacingType(selectorString, RFmxLteMXComponentCarrierSpacingType.Nominal); // nominal spacing is assumed
            lte.ConfigureLinkDirection(selectorString, standardConfig.LinkDirection);
            lte.ConfigureDuplexScheme(selectorString, standardConfig.DuplexScheme, standardConfig.UplinkDownlinkConfiguration);
            lte.ConfigureBand(selectorString, standardConfig.Band);
            lte.ConfigureNumberOfComponentCarriers(selectorString, standardConfig.ComponentCarrierConfigurations.Length);
            for (int i = 0; i < standardConfig.ComponentCarrierConfigurations.Length; i++)
            {
                string carrierString = RFmxLteMX.BuildCarrierString(selectorString, i);
                ComponentCarrierConfiguration componentCarrierConfig = standardConfig.ComponentCarrierConfigurations[i];
                lte.ComponentCarrier.SetBandwidth(carrierString, componentCarrierConfig.Bandwidth_Hz);
                lte.ComponentCarrier.SetCellId(carrierString, componentCarrierConfig.CellId);
                lte.ComponentCarrier.ConfigurePuschModulationType(carrierString, componentCarrierConfig.PuschModulationType);
                lte.ComponentCarrier.ConfigurePuschResourceBlocks(carrierString, componentCarrierConfig.PuschResourceBlockOffset, componentCarrierConfig.PuschNumberOfResourceBlocks);
                lte.ComponentCarrier.ConfigureDownlinkTestModel(carrierString, componentCarrierConfig.DownlinkTestModel);
            }
            lte.ComponentCarrier.ConfigureAutoResourceBlockDetectionEnabled(selectorString, standardConfig.PuschAutoResourceBlockDetectionEnabled);
            lte.ConfigureAutoDmrsDetectionEnabled(selectorString, standardConfig.AutoDmrsDetectionEnabled);
            lte.ComponentCarrier.ConfigureDownlinkAutoCellIDDetectionEnabled(selectorString, standardConfig.DownlinkAutoCellIDDetectionEnabled);
        }

        /// <summary>Configures common settings for the ACP measurement and selects the measurement.</summary>
        /// <param name="lte">Specifies the LTE signal to configure.</param>
        /// <param name="acpConfig">Specifies the ACP settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureAcp(RFmxLteMX lte, AcpConfiguration acpConfig, string selectorString = "")
        {
            lte.SelectMeasurements(selectorString, RFmxLteMXMeasurementTypes.Acp, false);
            lte.Acp.Configuration.ConfigureNumberOfEutraOffsets(selectorString, acpConfig.NumberOfEutraOffsets);
            lte.Acp.Configuration.ConfigureNumberOfUtraOffsets(selectorString, acpConfig.NumberOfUtraOffsets);
            lte.Acp.Configuration.ConfigureNumberOfGsmOffsets(selectorString, acpConfig.NumberOfGsmOffsets);
            lte.Acp.Configuration.ConfigureRbwFilter(selectorString, acpConfig.RbwAuto, acpConfig.Rbw_Hz, acpConfig.RbwFilterType);
            lte.Acp.Configuration.ConfigureAveraging(selectorString, acpConfig.AveragingEnabled, acpConfig.AveragingCount, acpConfig.AveragingType);
            lte.Acp.Configuration.ConfigureSweepTime(selectorString, acpConfig.SweepTimeAuto, acpConfig.SweepTimeInterval_s);
        }

        /// <summary>Configures common settings for the ModAcc measurement and selects the measurement.</summary>
        /// <param name="lte">Specifies the LTE signal to configure.</param>
        /// <param name="modAccConfig">Specifies the ModAcc settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureModAcc(RFmxLteMX lte, ModAccConfiguration modAccConfig, string selectorString = "")
        {
            lte.SelectMeasurements(selectorString, RFmxLteMXMeasurementTypes.ModAcc, false);
            lte.ModAcc.Configuration.ConfigureAveraging(selectorString, modAccConfig.AveragingEnabled, modAccConfig.AveragingCount);
            lte.ModAcc.Configuration.ConfigureSynchronizationModeAndInterval(selectorString, modAccConfig.SynchronizationMode, modAccConfig.MeasurementOffset, modAccConfig.MeasurementLength);
            lte.ModAcc.Configuration.ConfigureEvmUnit(selectorString, modAccConfig.EvmUnit);
        }

        /// <summary>Performs actions to initiate acquisition and measurement.<para></para> Enables the specified measurement(s) before optionally 
        /// automatically adjusting the reference level before beginning measurements. Finally, initiates the acquisition and measurement(s).</summary>
        /// <param name="lte">Specifies the LTE signal to configure.</param>
        /// <param name="measurements">Specifies one or more previously configured measurements to enable for this acquisition.</param>
        /// <param name="autoLevelConfig">Specifies the configuration for the optional AutoLevel process which will automatically set the analyzer's reference level.</param>
        /// <param name="enableTraces">(Optional) Specifies whether traces should be enabled for the measurement(s).</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        /// <param name="resultName">(Optional) Specifies the name to be associated with measurement results. Provide a unique name, such as "r1" to enable 
        /// fetching of multiple measurement results and traces. See the RFmx help for more documentation of this parameter.</param>
        public static void SelectAndInitiateMeasurements(RFmxLteMX lte, RFmxLteMXMeasurementTypes[] measurements, AutoLevelConfiguration autoLevelConfig = default,
            bool enableTraces = false, string selectorString = "", string resultName = "")
        {
            // Aggregate the selected measurements into a single value
            // OR of 0 and x equals x
            RFmxLteMXMeasurementTypes selectedMeasurements = 0;
            foreach (RFmxLteMXMeasurementTypes measurement in measurements)
                selectedMeasurements |= measurement;
            lte.SelectMeasurements(selectorString, selectedMeasurements, enableTraces);

            if (autoLevelConfig.Enabled)
                lte.AutoLevel(selectorString, autoLevelConfig.MeasurementInterval_s, out double _);

            // Initiate acquisition and measurement for the selected measurements
            lte.Initiate(selectorString, resultName);
        }
        #endregion

        #region Measurement Results

        /// <summary>Fetches common results from the ACP measurement.</summary>
        /// <param name="lte">Specifies the LTE signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common ACP measurement results.</returns>
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
                string offsetString = RFmxLteMX.BuildOffsetString(selectorString,i); // get the signal string
                lte.Acp.Configuration.GetOffsetFrequency(offsetString, out double offsetFrequency);
                lte.Acp.Configuration.GetOffsetIntegrationBandwidth(offsetString, out double offsetIbw);
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

        /// <summary>Fetches common results from the ModAcc measurement.</summary>
        /// <param name="lte">Specifies the LTE signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common ModAcc measurement results.</returns>
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
                    MeanFrequencyError_Hz = meanFrequencyError[i],
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
