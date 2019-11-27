using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxSpecAn
    {
        #region Type Definitions

        /// <summary>Defines common settings for the TxP measurement.</summary>
        public struct TxpConfiguration
        {
            public double MeasurementInterval_s;
            public double Rbw_Hz;
            public RFmxSpecAnMXTxpRbwFilterType RbwFilterType;
            public double RrcAlpha;

            public RFmxSpecAnMXTxpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxSpecAnMXTxpAveragingType AveragingType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static TxpConfiguration GetDefault()
            {
                return new TxpConfiguration
                {
                    MeasurementInterval_s = 1e-3,
                    Rbw_Hz = 20e6,
                    RbwFilterType = RFmxSpecAnMXTxpRbwFilterType.None,
                    RrcAlpha = 0.01,
                    AveragingEnabled = RFmxSpecAnMXTxpAveragingEnabled.False,
                    AveragingCount = 10,
                    AveragingType = RFmxSpecAnMXTxpAveragingType.Rms,
                };
            }
        }

        /// <summary>Defines common settings for the ACP measurement for a single component carrier.</summary>
        public struct AcpComponentCarrierConfiguration
        {
            public double IntegrationBandwidth_Hz;
            public double Frequency_Hz;
            public RFmxSpecAnMXAcpCarrierRrcFilterEnabled RrcFilterEnabled;
            public double RrcAlpha;
            public RFmxSpecAnMXAcpCarrierMode Mode;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AcpComponentCarrierConfiguration GetDefault()
            {
                return new AcpComponentCarrierConfiguration
                {
                    Frequency_Hz = 0.00,
                    IntegrationBandwidth_Hz = 18e6,
                    RrcFilterEnabled = RFmxSpecAnMXAcpCarrierRrcFilterEnabled.False,
                    RrcAlpha = 0.22,
                    Mode = RFmxSpecAnMXAcpCarrierMode.Active
                };
            }
        }

        /// <summary>Defines common settings for the ACP measurement for a single offset channel.</summary>
        public struct AcpOffsetChannelConfiguration
        {
            public double IntegrationBandwidth_Hz;
            public double Frequency_Hz;
            public RFmxSpecAnMXAcpOffsetEnabled Enabled;
            public RFmxSpecAnMXAcpOffsetSideband SideBand;
            public RFmxSpecAnMXAcpOffsetPowerReferenceCarrier PowerReferenceCarrier;
            public int PowerReferenceSpecificIndex;
            public double RelativeAttenuation_dB;
            public RFmxSpecAnMXAcpOffsetRrcFilterEnabled RrcFilterEnabled;
            public double RrcAlpha;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AcpOffsetChannelConfiguration GetDefault()
            {
                return new AcpOffsetChannelConfiguration
                {
                    Frequency_Hz = 20e6,
                    IntegrationBandwidth_Hz = 18e6,
                    Enabled = RFmxSpecAnMXAcpOffsetEnabled.True,
                    SideBand = RFmxSpecAnMXAcpOffsetSideband.Both,
                    PowerReferenceCarrier = RFmxSpecAnMXAcpOffsetPowerReferenceCarrier.Closest,
                    PowerReferenceSpecificIndex = 0,
                    RelativeAttenuation_dB = 0,
                    RrcFilterEnabled = RFmxSpecAnMXAcpOffsetRrcFilterEnabled.False,
                    RrcAlpha = 0.22
                };
            }
        }

        /// <summary>Defines common settings for the ACP measurement.</summary>
        public struct AcpConfiguration
        {
            public RFmxSpecAnMXAcpPowerUnits PowerUnits;
            public RFmxSpecAnMXAcpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxSpecAnMXAcpAveragingType AveragingType;
            public RFmxSpecAnMXAcpFftWindow FftWindow;
            public double FftPadding;

            public RFmxSpecAnMXAcpRbwAutoBandwidth RbwAuto;
            public double Rbw_Hz;
            public RFmxSpecAnMXAcpRbwFilterType RbwFilterType;

            public RFmxSpecAnMXAcpSweepTimeAuto SweepTimeAuto;
            public double SweepTimeInterval_s;
            public AcpComponentCarrierConfiguration[] ComponentCarrierConfiguration;
            public AcpOffsetChannelConfiguration[] OffsetChannelConfiguration;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AcpConfiguration GetDefault()
            {
                return new AcpConfiguration
                {
                    PowerUnits = RFmxSpecAnMXAcpPowerUnits.dBm,
                    AveragingEnabled = RFmxSpecAnMXAcpAveragingEnabled.False,
                    AveragingCount = 10,
                    AveragingType = RFmxSpecAnMXAcpAveragingType.Rms,
                    FftWindow = RFmxSpecAnMXAcpFftWindow.FlatTop,
                    FftPadding = -1,
                    RbwAuto = RFmxSpecAnMXAcpRbwAutoBandwidth.True,
                    Rbw_Hz = 10e3,
                    RbwFilterType = RFmxSpecAnMXAcpRbwFilterType.Gaussian,
                    SweepTimeAuto = RFmxSpecAnMXAcpSweepTimeAuto.True,
                    SweepTimeInterval_s = 1e-3,
                    ComponentCarrierConfiguration = new AcpComponentCarrierConfiguration[] { AcpComponentCarrierConfiguration.GetDefault() },
                    OffsetChannelConfiguration = new AcpOffsetChannelConfiguration[] { AcpOffsetChannelConfiguration.GetDefault() }
                };
            }
        }

        /// <summary>Defines common settings for the AMPM measurement.</summary>
        public struct AmpmConfiguration
        {
            public double MeasurementInterval_s;
            public double DutAverageInputPower_dBm;
            public Waveform ReferenceWaveform;
            public RFmxSpecAnMXAmpmSignalType SignalType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AmpmConfiguration GetDefault()
            {
                return new AmpmConfiguration
                {
                    MeasurementInterval_s = 100e-6,
                    DutAverageInputPower_dBm = -20,
                    ReferenceWaveform = new Waveform(),
                    SignalType = RFmxSpecAnMXAmpmSignalType.Modulated
                };
            }
        }

        /// <summary>Defines common results of the TXP measurement.</summary>
        public struct TxpResults
        {
            public double AverageMeanPower_dBm;
            public double PeakToAverageRatio_dB;
            public double MaximumPower_dBm;
            public double MinimumPower_dBm;
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
            public double AbsolutePower_dBm_or_dBmHz;
            public double TotalRelativePower_dB;
            public double Frequency_Hz;
            public double IntegrationBandwidth_Hz;
        }

        /// <summary>Defines common results of the ACP measurement.</summary>
        public struct AcpResults
        {
            public double TotalCarrierPower_dBm_or_dBmHz;
            public AcpOffsetResults[] OffsetResults;
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }

        /// <summary>Defines common results of the AMPM measurement.</summary>
        public struct AmpmResults
        {
            public double MeanLinearGain_dB;
            public double OnedBCompressionPoint_dBm;
            public double MeanRmsEvm_percent;
            public double AmToAMResidual_dB;
            public double AmToPMResidual_deg;
        }
        #endregion

        #region Instrument Configurations

        /// <summary>Configures common measurement settings for the personality.</summary>
        /// <param name="specAnSignal">Specifies the SpecAn signal to configure.</param>
        /// <param name="commonConfig">Specifies the common settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. 
        /// See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureCommon(RFmxSpecAnMX specAnSignal, CommonConfiguration commonConfig, string selectorString = "")
        {
            specAnSignal.SetSelectedPorts(selectorString, commonConfig.SelectedPorts);
            specAnSignal.ConfigureRF(selectorString, commonConfig.CenterFrequency_Hz, commonConfig.ReferenceLevel_dBm, commonConfig.ExternalAttenuation_dB);
            specAnSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalTriggerSource, RFmxSpecAnMXDigitalEdgeTriggerEdge.Rising, commonConfig.TriggerDelay_s, commonConfig.TriggerEnabled);
        }

        /// <summary>Configures common settings for the TxP measurement and selects the measurement.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="txpConfig">Specifies the TxP settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureTxp(RFmxSpecAnMX specAn, TxpConfiguration txpConfig, string selectorString = "")
        {
            specAn.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Txp, false);
            specAn.Txp.Configuration.SetMeasurementInterval(selectorString, txpConfig.MeasurementInterval_s);
            specAn.Txp.Configuration.ConfigureRbwFilter(selectorString, txpConfig.Rbw_Hz, txpConfig.RbwFilterType, txpConfig.RrcAlpha);
            specAn.Txp.Configuration.ConfigureAveraging(selectorString, txpConfig.AveragingEnabled, txpConfig.AveragingCount, txpConfig.AveragingType);
        }

        /// <summary>Configures common settings for the ACP measurement and selects the measurement.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="acpConfig">Specifies the ACP settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureAcp(RFmxSpecAnMX specAn, AcpConfiguration acpConfig, string selectorString = "")
        {
            specAn.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Acp, false);
            specAn.Acp.Configuration.ConfigurePowerUnits(selectorString, acpConfig.PowerUnits);
            specAn.Acp.Configuration.ConfigureAveraging(selectorString, acpConfig.AveragingEnabled, acpConfig.AveragingCount, acpConfig.AveragingType);
            specAn.Acp.Configuration.ConfigureFft(selectorString, acpConfig.FftWindow, acpConfig.FftPadding);
            specAn.Acp.Configuration.ConfigureRbwFilter(selectorString, acpConfig.RbwAuto, acpConfig.Rbw_Hz, acpConfig.RbwFilterType);
            specAn.Acp.Configuration.ConfigureSweepTime(selectorString, acpConfig.SweepTimeAuto, acpConfig.SweepTimeInterval_s);

            specAn.Acp.Configuration.ConfigureNumberOfCarriers(selectorString, acpConfig.ComponentCarrierConfiguration.Length);
            for (int i = 0; i < acpConfig.ComponentCarrierConfiguration.Length; i++)
            {
                string carrierString = RFmxSpecAnMX.BuildCarrierString2(selectorString, i);
                AcpComponentCarrierConfiguration carrierConfiguration = acpConfig.ComponentCarrierConfiguration[i];
                specAn.Acp.Configuration.ConfigureCarrierIntegrationBandwidth(carrierString, carrierConfiguration.IntegrationBandwidth_Hz);
                specAn.Acp.Configuration.ConfigureCarrierFrequency(carrierString, carrierConfiguration.Frequency_Hz);
                specAn.Acp.Configuration.ConfigureCarrierRrcFilter(carrierString, carrierConfiguration.RrcFilterEnabled, carrierConfiguration.RrcAlpha);
                specAn.Acp.Configuration.ConfigureCarrierMode(carrierString, carrierConfiguration.Mode);
            }

            specAn.Acp.Configuration.ConfigureNumberOfOffsets(selectorString, acpConfig.OffsetChannelConfiguration.Length);
            for (int i = 0; i < acpConfig.OffsetChannelConfiguration.Length; i++)
            {
                string offsetString = RFmxSpecAnMX.BuildOffsetString2(selectorString, i);
                AcpOffsetChannelConfiguration offsetConfiguration = acpConfig.OffsetChannelConfiguration[i];
                specAn.Acp.Configuration.ConfigureOffsetIntegrationBandwidth(offsetString, offsetConfiguration.IntegrationBandwidth_Hz);
                specAn.Acp.Configuration.ConfigureOffset(offsetString, offsetConfiguration.Frequency_Hz, offsetConfiguration.SideBand, offsetConfiguration.Enabled);
                specAn.Acp.Configuration.ConfigureOffsetPowerReference(offsetString, offsetConfiguration.PowerReferenceCarrier, offsetConfiguration.PowerReferenceSpecificIndex);
                specAn.Acp.Configuration.ConfigureOffsetRelativeAttenuation(offsetString, offsetConfiguration.RelativeAttenuation_dB);
                specAn.Acp.Configuration.ConfigureOffsetRrcFilter(offsetString, offsetConfiguration.RrcFilterEnabled, offsetConfiguration.RrcAlpha);
            }
        }

        /// <summary>Configures common settings for the AMPM measurement and selects the measurement.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="ampmConfig">Specifies the AMPM settings to apply.</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        public static void ConfigureAmpm(RFmxSpecAnMX specAn, AmpmConfiguration ampmConfig, string selectorString = "")
        {
            specAn.SelectMeasurements(selectorString, RFmxSpecAnMXMeasurementTypes.Ampm, false);
            specAn.Ampm.Configuration.ConfigureMeasurementInterval(selectorString, ampmConfig.MeasurementInterval_s);
            specAn.Ampm.Configuration.ConfigureDutAverageInputPower(selectorString, ampmConfig.DutAverageInputPower_dBm);
            RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent idleDurationPresent = ampmConfig.ReferenceWaveform.IdleDurationPresent ? RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.False;
            specAn.Ampm.Configuration.ConfigureReferenceWaveform(selectorString, ampmConfig.ReferenceWaveform.Data, idleDurationPresent, ampmConfig.SignalType);
        }

        /// <summary>Performs actions to initiate acquisition and measurement.<para></para> Enables the specified measurement(s) before optionally 
        /// automatically adjusting the reference level before beginning measurements. Finally, initiates the acquisition and measurement(s).</summary>
        /// <param name="specAn">Specifies the SpecAn signal to configure.</param>
        /// <param name="measurements">Specifies one or more previously configured measurements to enable for this acquisition.</param>
        /// <param name="autoLevelConfig">Specifies the configuration for the optional AutoLevel process which will automatically set the analyzer's reference level.</param>
        /// <param name="autoLevelBandwidth_Hz">Specifies the bandwidth, in hertz (Hz), of the signal to be analyzed. See the RFmx help for more documentation of this parameter.</param>
        /// <param name="enableTraces">(Optional) Specifies whether traces should be enabled for the measurement(s).</param>
        /// <param name="selectorString">Pass an empty string. The signal name that is passed when creating the signal configuration is used. See the RFmx help for more documention of this parameter.</param>
        /// <param name="resultName">(Optional) Specifies the name to be associated with measurement results. Provide a unique name, such as "r1" to enable 
        /// fetching of multiple measurement results and traces. See the RFmx help for more documentation of this parameter.</param>
        public static void SelectAndInitiateMeasurements(RFmxSpecAnMX specAn, RFmxSpecAnMXMeasurementTypes[] measurements, AutoLevelConfiguration autoLevelConfig = default, 
            double autoLevelBandwidth_Hz = 200e3, bool enableTraces = false, string selectorString = "", string resultName = "")
        {
            // Aggregate the selected measurements into a single value
            // OR of 0 and x equals x
            RFmxSpecAnMXMeasurementTypes selectedMeasurements = 0;
            foreach (RFmxSpecAnMXMeasurementTypes measurement in measurements)
                selectedMeasurements |= measurement;
            specAn.SelectMeasurements(selectorString, selectedMeasurements, enableTraces);

            if (autoLevelConfig.Enabled)
                specAn.AutoLevel(selectorString, autoLevelBandwidth_Hz, autoLevelConfig.MeasurementInterval_s, out double _);

            // Initiate acquisition and measurement for the selected measurements
            specAn.Initiate(selectorString, resultName);
        }
        #endregion

        #region Measurement Results

        /// <summary>Fetches common results from the TxP measurement.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common TxP measurement results.</returns>
        public static TxpResults FetchTxp(RFmxSpecAnMX specAn, string selectorString = "")
        {
            TxpResults txpResults = new TxpResults();
            specAn.Txp.Results.FetchMeasurement(selectorString, 10.0, out txpResults.AverageMeanPower_dBm,
                out txpResults.PeakToAverageRatio_dB, out txpResults.MaximumPower_dBm, out txpResults.MinimumPower_dBm);
            return txpResults;
        }

        /// <summary>Fetches common results from the ACP measurement.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common ACP measurement results.</returns>
        public static AcpResults FetchAcp(RFmxSpecAnMX specAn, string selectorString = "")
        {
            double[] lowerRelativePower = null;
            double[] upperRelativePower = null;
            double[] lowerAbsolutePower = null;
            double[] upperAbsolutePower = null;
            specAn.Acp.Results.FetchOffsetMeasurementArray(selectorString, 10.0,
                ref lowerRelativePower, ref upperRelativePower, ref lowerAbsolutePower, ref upperAbsolutePower);
            AcpResults results = new AcpResults()
            {
                OffsetResults = new AcpOffsetResults[lowerRelativePower.Length]
            };
            for (int i = 0; i < lowerRelativePower.Length; i++)
            {
                string offsetString = RFmxSpecAnMX.BuildOffsetString2(selectorString, i);
                specAn.Acp.Configuration.GetOffsetFrequency(offsetString, out double offsetFrequency);
                specAn.Acp.Configuration.GetOffsetIntegrationBandwidth(offsetString, out double offsetIbw);
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
            specAn.Acp.Configuration.GetNumberOfCarriers(selectorString, out int numCarrierChannels);
            results.ComponentCarrierResults = new AcpComponentCarrierResults[numCarrierChannels];
            for (int i = 0; i < numCarrierChannels; i++)
            {
                string carrierString = RFmxSpecAnMX.BuildCarrierString2(selectorString, i);
                AcpComponentCarrierResults componentCarrierResults;
                specAn.Acp.Results.FetchCarrierMeasurement(carrierString, 10.0, out componentCarrierResults.AbsolutePower_dBm_or_dBmHz,
                    out componentCarrierResults.TotalRelativePower_dB, out componentCarrierResults.Frequency_Hz, out componentCarrierResults.IntegrationBandwidth_Hz);
                results.ComponentCarrierResults[i] = componentCarrierResults;
            }
            specAn.Acp.Results.FetchTotalCarrierPower(selectorString, 10.0, out results.TotalCarrierPower_dBm_or_dBmHz);
            return results;
        }

        /// <summary>Fetches common results from the AMPM measurement.</summary>
        /// <param name="specAn">Specifies the SpecAn signal to fetch results from.</param>
        /// <param name="selectorString">(Optional) Specifies the result name. See the RFmx help for more documentation of this parameter.</param>
        /// <returns>Common AMPM measurement results.</returns>
        public static AmpmResults FetchAmpm(RFmxSpecAnMX specAn, string selectorString = "")
        {
            AmpmResults ampmResults = new AmpmResults();
            specAn.Ampm.Results.FetchDutCharacteristics(selectorString, 10.0, out ampmResults.MeanLinearGain_dB, out ampmResults.OnedBCompressionPoint_dBm, out ampmResults.MeanRmsEvm_percent);
            specAn.Ampm.Results.FetchCurveFitResidual(selectorString, 10.0, out ampmResults.AmToAMResidual_dB, out ampmResults.AmToPMResidual_deg);
            return ampmResults;
        }
        #endregion
    }
}

