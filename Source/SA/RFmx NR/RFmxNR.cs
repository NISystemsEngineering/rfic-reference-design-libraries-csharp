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
            /// <summary>Specifies the channel bandwidth of the signal being measured. This value is expressed in Hz. See the RFmx help for more documention of this parameter.</summary>
            public double Bandwidth_Hz;
            /// <summary>Specifies a physical layer cell identity. See the RFmx help for more documention of this parameter.</summary>
            public int CellId;
            /// <summary>Specifies the modulation scheme used in the physical uplink shared channel (PUSCH) of the signal being measured. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXPuschModulationType PuschModulationType;
            /// <summary>Specifies the subcarrier spacing of the bandwidth part used in the component carrier. See the RFmx help for more documention of this parameter.</summary>
            public double SubcarrierSpacing_Hz;
            /// <summary>Specifies the starting resource block number of a PUSCH cluster. This property is ignored if you set 
            /// <see cref="StandardConfiguration.AutoResourceBlockDetectionEnabled"/> to True. See the RFmx help for more documention of this parameter.</summary>
            public int PuschResourceBlockOffset;
            /// <summary>Specifies the number of consecutive resource blocks in a physical uplink shared channel (PUSCH) cluster. This property is ignored if you set 
            /// <see cref="StandardConfiguration.AutoResourceBlockDetectionEnabled"/> to True. See the RFmx help for more documention of this parameter.</summary>
            public int PuschNumberOfResourceBlocks;
            /// <summary>Specifies whether transform precoding is enabled. Enable transform precoding when analyzing a DFT-s-OFDM waveform.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXPuschTransformPrecodingEnabled PuschTransformPrecodingEnabled;
            /// <summary>Specifies the slot allocation in NR Frame. This defines the indices of the allocated slots. See the RFmx help for more documention of this parameter.</summary>
            public string PuschSlotAllocation;
            /// <summary>Specifies the symbol allocation of each slot allocation. See the RFmx help for more documention of this parameter.</summary>
            public string PuschSymbolAllocation;
            /// <summary>Specifies the configuration type of DMRS. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXPuschDmrsConfigurationType PuschDmrsConfigurationType;
            /// <summary>Specifies the mapping type of DMRS. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXPuschMappingType PuschMappingType;
            /// <summary>Specifies the position of first DMRS symbol in a slot when you set <see cref="PuschDmrsConfigurationType"/> to Type A.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int PuschDmrsTypeAPosition;
            /// <summary>Specifies whether the DMRS is single-symbol or double-symbol. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXPuschDmrsDuration PuschDmrsDuration;
            /// <summary>Specifies the number of additional sets of consecutive DMRS symbols in a slot. See the RFmx help for more documention of this parameter.</summary>
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
            /// <summary>Specifies the link direction of the received signal. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXLinkDirection LinkDirection;
            /// <summary>Specifies whether to use channel bandwidth and subcarrier spacing configuration supported in the frequency range 1 (sub 6 GHz)
            /// or the frequency range 2 (above 24 GHz). See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXFrequencyRange FrequencyRange;
            /// <summary>Specifies the evolved universal terrestrial radio access (E-UTRA) or NR operating frequency band of a subblock.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int Band;
            /// <summary>Specifies whether the values of modulation type, number of resource block clusters, resource block offsets, and number of resource blocks are
            /// auto-detected by the measurement or configured by you. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXAutoResourceBlockDetectionEnabled AutoResourceBlockDetectionEnabled;
            /// <summary>Specifies the NR test model type when you set <see cref="LinkDirection"/> to Downlink. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXDownlinkTestModel DownlinkTestModel;
            /// <summary>Specifies the duplexing technique of the signal being measured when you set <see cref="LinkDirection"/> to Downlink.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXDownlinkTestModelDuplexScheme DownlinkTestModelDuplexScheme;
            /// <summary>Specifies an array of common configurations for each component carrier in the signal.</summary>
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
            /// <summary>Specifies the units in which measurement offset and measurement length are specified. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXModAccMeasurementLengthUnit MeasurementLengthUnit;
            /// <summary>Specifies the measurement offset to skip from the synchronization boundary. See the RFmx help for more documention of this parameter.</summary>
            public double MeasurementOffset;
            /// <summary>Specifies the measurement length in units specified by <see cref="MeasurementLengthUnit"/>. See the RFmx help for more documention of this parameter.</summary>
            public double MeasurementLength;
            /// <summary>Specifies the units of the EVM results. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXModAccEvmUnit EvmUnit;
            /// <summary>Enables averaging for the measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXModAccAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set <see cref="AveragingEnabled"/> to True.
            /// See the RFmx help for more documention of this parameter.</summary>
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
            /// <summary>Returns the subcarrier index where the max peak composite EVM occurs. See the RFmx help for more documention of this parameter.</summary>
            public int PeakCompositeEvmSubcarrierIndex;
            /// <summary>Returns the symbol index where the max peak composite EVM occurs. See the RFmx help for more documention of this parameter.</summary>
            public int PeakCompositeEvmSymbolIndex;
            /// <summary>Returns the slot index where the max peak composite EVM occurs. See the RFmx help for more documention of this parameter.</summary>
            public int PeakCompositeEvmSlotIndex;
            /// <summary>Returns the mean value of RMS EVMs calculated over measurement length. See the RFmx help for more documention of this parameter.</summary>
            public double MeanRmsCompositeEvm;
            /// <summary>Returns the maximum value of peak EVMs calculated over measurement length. See the RFmx help for more documention of this parameter.</summary>
            public double MaxPeakCompositeEvm;
            /// <summary>Returns the estimated carrier frequency offset averaged over measurement length. This value is expressed in Hz.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double MeanFrequencyError_Hz;
        }

        /// <summary>Defines common results of the ModAcc measurement.</summary>
        public struct ModAccResults
        {
            /// <summary>Returns ModAcc results for each configured component carrier in the signal.</summary>
            public ModAccComponentCarrierResults[] ComponentCarrierResults;
        }

        /// <summary>Defines common settings for the ACP measurement.</summary>
        public struct AcpConfiguration
        {
            /// <summary>Specifies the number of universal terrestrial radio access (UTRA) adjacent channel offsets to be configured at offset positions.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfUtraOffsets;
            /// <summary>Specifies the number of evolved universal terrestrial radio access (E-UTRA) adjacent channel offsets to be configured at offset positions.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfEutraOffsets;
            /// <summary>Specifies the number of NR adjacent channel offsets to be configured at offset positions. See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfNrOffsets;
            /// <summary>Specifies the number of ENDC adjacent channel offsets to be configured at offset positions. See the RFmx help for more documention of this parameter.</summary>
            public int NumberOfEndcOffsets;
            /// <summary>Specifies the method for performing the ACP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXAcpMeasurementMethod MeasurementMethod;
            /// <summary>Specifies whether to enable compensation of the channel powers for the inherent noise floor of the signal analyzer.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXAcpNoiseCompensationEnabled NoiseCompensationEnabled;
            /// <summary>Specifies whether the measurement sets the sweep time. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXAcpSweepTimeAuto SweepTimeAuto;
            /// <summary>Specifies the sweep time when you set <see cref="SweepTimeAuto"/> to False. This value is expressed in seconds.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double SweepTimeInterval_s;
            /// <summary>Specifies whether to enable averaging for the ACP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXAcpAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set <see cref="AveragingEnabled"/> to True.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies the averaging type for averaging multiple spectrum acquisitions. The averaged spectrum is used for ACP measurement.
            /// See the RFmx help for more documention of this parameter.</summary>
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
            /// <summary>Returns the lower (negative) offset channel power. This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double LowerAbsolutePower_dBm;
            /// <summary>Returns the power in lower (negative) offset channel relative to the total aggregated power. This value is expressed in dB. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public double LowerRelativePower_dB;
            /// <summary>Returns the upper (positive) offset channel power. This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double UpperAbsolutePower_dBm;
            /// <summary>Returns the power in the upper (positive) offset channel relative to the total aggregated power. This value is expressed in dB.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double UpperRelativePower_dB;
            /// <summary>Returns the offset frequency of an offset channel. This value is expressed in Hz. See the RFmx help for more documention of this parameter.</summary>
            public double Frequency_Hz;
            /// <summary>Returns the integration bandwidth of an offset channel. This value is expressed in Hz. See the RFmx help for more documention of this parameter.</summary>
            public double IntegrationBandwidth_Hz;
        }

        /// <summary>Defines common results of the ACP measurement for a single component carrier.</summary>
        public struct AcpComponentCarrierResults
        {
            /// <summary>Returns the power measured over the integration bandwidth of the component carrier. This value is expressed in dBm.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double AbsolutePower_dBm;
            /// <summary>Returns the component carrier power relative to its subblock power. This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double RelativePower_dB;
        }

        /// <summary>Defines common results of the ACP measurement.</summary>
        public struct AcpResults
        {
            /// <summary>Returns an array of ACP results for the configured offsets.</summary>
            public AcpOffsetResults[] OffsetResults;
            /// <summary>Returns an array of ACP results for the component carriers of the signal.</summary>
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }

        /// <summary>Defines common settings for the CHP measurement.</summary>
        public struct ChpConfiguration
        {
            /// <summary>Specifies whether the measurement sets the sweep time. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXChpSweepTimeAuto SweepTimeAuto;
            /// <summary>Specifies the sweep time when you set <see cref="SweepTimeAuto"/> to False. This value is expressed in seconds.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double SweepTimeInterval_s;
            /// <summary>Specifies whether to enable averaging for the CHP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxNRMXChpAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set <see cref="AveragingEnabled"/> to True.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies the averaging type for averaging multiple spectrum acquisitions. The averaged spectrum is used for CHP measurement.
            /// See the RFmx help for more documention of this parameter.</summary>
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
            /// <summary>Returns the power measured over the integration bandwidth of the component carrier. This value is expressed in dBm.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double AbsolutePower_dBm;
            /// <summary>Returns the component carrier power relative to its subblock power. This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double RelativePower_dB;
        }

        /// <summary>Defines common results of the CHP measurement.</summary>
        public struct ChpResults
        {
            /// <summary>Returns the total power of all the subblocks. This value is expressed in dBm. 
            /// See the RFmx help for more documention of this parameter.</summary>
            public double TotalAggregatedPower_dBm;
            /// <summary>Returns an array of CHP results for the component carriers in the signal.</summary>
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
        /// <param name="standardConfig">Specifies the NR standard settings to apply.</param>
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
        public static void SelectAndInitiateMeasurements(RFmxNRMX nr, RFmxNRMXMeasurementTypes[] measurements, AutoLevelConfiguration autoLevelConfig = default,
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
        /// <returns>Common ModAcc measurement results.</returns>
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
        /// <returns>Common ACP measurement results.</returns>
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
        /// <returns>Common CHP measurement results.</returns>
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
