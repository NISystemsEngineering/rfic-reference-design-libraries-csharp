using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    /// <summary>Defines common types and methods for NI-RFmx SpecAn measurements.</summary>
    public static class RFmxSpecAn
    {
        #region Type Definitions

        /// <summary>Defines common settings for the TxP measurement.</summary>
        public struct TxpConfiguration
        {
            /// <summary>Specifies the acquisition time for the TXP measurement. This value is expressed in seconds. See the RFmx help for more documention of this parameter.</summary>
            public double MeasurementInterval_s;
            /// <summary>Specifies the bandwidth of the resolution bandwidth (RBW) filter used to measure the signal. This value is expressed in Hz.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double Rbw_Hz;
            /// <summary>Specifies the shape of the digital resolution bandwidth (RBW) filter. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXTxpRbwFilterType RbwFilterType;
            /// <summary>Specifies the roll-off factor for the root-raised-cosine (RRC) filter. See the RFmx help for more documention of this parameter.</summary>
            public double RrcAlpha;
            
            /// <summary>Specifies whether to enable averaging for the TXP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXTxpAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set <see cref="AveragingEnabled"/> to True.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies the averaging type for the TXP measurement. The averaged power trace is used for the measurement.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXTxpAveragingType AveragingType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static TxpConfiguration GetDefault()
            {
                return new TxpConfiguration
                {
                    MeasurementInterval_s = 1e-3,
                    Rbw_Hz = 20e6,
                    RbwFilterType = RFmxSpecAnMXTxpRbwFilterType.Flat,
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
            /// <summary>Specifies the frequency range over which the measurement integrates the carrier power. This value is expressed in Hz.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double IntegrationBandwidth_Hz;
            /// <summary>Specifies the center frequency of the carrier, relative to the RF center frequency. This value is expressed in Hz.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double Frequency_Hz;
            /// <summary>Specifies whether to apply the root-raised-cosine (RRC) filter on the acquired carrier channel before measuring the carrier channel power.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpCarrierRrcFilterEnabled RrcFilterEnabled;
            /// <summary>Specifies the roll-off factor for the root-raised-cosine (RRC) filter. See the RFmx help for more documention of this parameter.</summary>
            public double RrcAlpha;
            /// <summary>Specifies whether to consider the carrier power as part of the total carrier power measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpCarrierMode Mode;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AcpComponentCarrierConfiguration GetDefault()
            {
                return new AcpComponentCarrierConfiguration
                {
                    Frequency_Hz = 0.00,
                    IntegrationBandwidth_Hz = 18e6,
                    RrcFilterEnabled = RFmxSpecAnMXAcpCarrierRrcFilterEnabled.True,
                    RrcAlpha = 0.22,
                    Mode = RFmxSpecAnMXAcpCarrierMode.Active
                };
            }
        }

        /// <summary>Defines common settings for the ACP measurement for a single offset channel.</summary>
        public struct AcpOffsetChannelConfiguration
        {
            /// <summary>Specifies the frequency range, over which the measurement integrates the offset channel power. This value is expressed in Hz.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double IntegrationBandwidth_Hz;
            /// <summary>Specifies the center or edge frequency of the offset channel, relative to the center frequency of the closest carrier.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double Frequency_Hz;
            /// <summary>Specifies whether to enable the offset channel for ACP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpOffsetEnabled Enabled;
            /// <summary>Specifies whether the offset channel is present on one side, or on both sides of the carrier. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpOffsetSideband SideBand;
            /// <summary>Specifies the carrier to be used as power reference to measure the offset channel relative power. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpOffsetPowerReferenceCarrier PowerReferenceCarrier;
            /// <summary>Specifies the index of the carrier to be used as the reference carrier. The power measured in this carrier is used as the power reference
            /// for measuring the offset channel relative power, when you set <see cref="PowerReferenceCarrier"/> to Specific.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int PowerReferenceSpecificIndex;
            /// <summary>Specifies the attenuation relative to the external attenuation specified by the External Attenuation property.
            /// This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double RelativeAttenuation_dB;
            /// <summary>Specifies whether to apply the root-raised-cosine (RRC) filter on the acquired offset channel before measuring the offset channel power.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpOffsetRrcFilterEnabled RrcFilterEnabled;
            /// <summary>Specifies the roll-off factor for the root-raised-cosine (RRC) filter See the RFmx help for more documention of this parameter.</summary>
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
            /// <summary>Specifies the units for the absolute power. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpPowerUnits PowerUnits;
            /// <summary>Specifies whether to enable averaging for the ACP measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the number of acquisitions used for averaging when you set the <see cref="AveragingEnabled"/> to True.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies the averaging type for averaging multiple spectrum acquisitions. The averaged spectrum is used for ACP measurement.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpAveragingType AveragingType;
            /// <summary>Specifies the FFT window type to use to reduce spectral leakage. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpFftWindow FftWindow;
            /// <summary>Specifies the factor by which the time-domain waveform is zero-padded before fast Fourier transform (FFT).
            /// See the RFmx help for more documention of this parameter.</summary>
            public double FftPadding;

            /// <summary>Specifies whether the measurement computes the resolution bandwidth (RBW).  See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpRbwAutoBandwidth RbwAuto;
            /// <summary>Specifies the bandwidth of the resolution bandwidth (RBW) filter used to sweep the acquired signal, when you set <see cref="RbwAuto"/> to False.
            /// This value is expressed in Hz. See the RFmx help for more documention of this parameter.</summary>
            public double Rbw_Hz;
            /// <summary>Specifies the shape of the digital resolution bandwidth (RBW) filter. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpRbwFilterType RbwFilterType;

            /// <summary>Specifies whether the measurement computes the sweep time. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAcpSweepTimeAuto SweepTimeAuto;
            /// <summary>Specifies the sweep time when you set the <see cref="SweepTimeAuto"/> property to False. This value is expressed in seconds.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double SweepTimeInterval_s;
            /// <summary>Specifies the array of common ACP configurations for each component carrier in the signal.</summary>
            public AcpComponentCarrierConfiguration[] ComponentCarrierConfiguration;
            /// <summary>Specifies the array of common ACP configurations for each desired ACP offset.</summary>
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
            /// <summary>Specifies the duration of the reference waveform considered for the AMPM measurement. See the RFmx help for more documention of this parameter.</summary>
            public double MeasurementInterval_s;
            /// <summary>Specifies the average power of the signal at the input port of the device under test. This value is expressed in dBm.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double DutAverageInputPower_dBm;
            /// <summary>Specifies the number of acquisitions used for averaging when you set <see cref="AveragingEnabled"/> to True.
            /// See the RFmx help for more documention of this parameter.</summary>
            public int AveragingCount;
            /// <summary>Specifies whether to enable averaging for the AMPM measurement. See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAmpmAveragingEnabled AveragingEnabled;
            /// <summary>Specifies the <see cref="Waveform"/> whose data is the complex baseband equivalent of the RF signal applied at the input port
            /// of the device under test when performing the measurement. See the RFmx help for more documention of this parameter.</summary>
            public Waveform ReferenceWaveform;
            /// <summary>Specifies whether the reference waveform is a modulated signal or a combination of one or more sinusoidal signals.
            /// See the RFmx help for more documention of this parameter.</summary>
            public RFmxSpecAnMXAmpmSignalType SignalType;

            /// <summary>Returns the struct with default values set.</summary>
            /// <returns>The struct with default values set.</returns>
            public static AmpmConfiguration GetDefault()
            {
                return new AmpmConfiguration
                {
                    MeasurementInterval_s = 100e-6,
                    DutAverageInputPower_dBm = -20,
                    AveragingEnabled = RFmxSpecAnMXAmpmAveragingEnabled.False,
                    AveragingCount = 10,
                    ReferenceWaveform = new Waveform(),
                    SignalType = RFmxSpecAnMXAmpmSignalType.Modulated
                };
            }
        }

        /// <summary>Defines common results of the TXP measurement.</summary>
        public struct TxpResults
        {
            /// <summary>Returns the mean power of the signal. This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double AverageMeanPower_dBm;
            /// <summary>Returns the ratio of the peak power of the signal to the mean power. See the RFmx help for more documention of this parameter.</summary>
            public double PeakToAverageRatio_dB;
            /// <summary>Returns the maximum power of the averaged power trace. This value is expressed in dBm.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double MaximumPower_dBm;
            /// <summary>Returns the minimum power of the averaged power trace. This value is expressed in dBm.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double MinimumPower_dBm;
        }

        /// <summary>Defines common results of the ACP measurement for a single defined offset.</summary>
        public struct AcpOffsetResults
        {
            /// <summary>Returns the lower offset channel power. See the RFmx help for more documention of this parameter.</summary>
            public double LowerAbsolutePower_dBm_or_dBmHz;
            /// <summary>Returns the lower offset channel power measured relative to the integrated power of the power reference carrier. This value is expressed in dB.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double LowerRelativePower_dB;
            /// <summary>Returns the upper offset channel power. See the RFmx help for more documention of this parameter.</summary>
            public double UpperAbsolutePower_dBm_or_dBmHz;
            /// <summary>Returns the upper offset channel power measured relative to the integrated power of the power reference carrier.
            /// This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double UpperRelativePower_dB;
            /// <summary>Returns the center or edge frequency, in hertz (Hz), of the offset channel, relative to the center frequency of the closest carrier.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double Frequency_Hz;
            /// <summary>contains the frequency range, in Hz, over which the measurement integrates the power.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double IntegrationBandwidth_Hz;
        }

        /// <summary>Defines common results of the ACP measurement for a single component carrier.</summary>
        public struct AcpComponentCarrierResults
        {
            /// <summary>Returns the measured carrier power. See the RFmx help for more documention of this parameter.</summary>
            public double AbsolutePower_dBm_or_dBmHz;
            /// <summary>Returns the carrier power measured relative to the total carrier power of all active carriers. This value is expressed in dB.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double TotalRelativePower_dB;
            /// <summary>Returns the center frequency of the carrier relative to the Center Frequency of the signal. This value is expressed in Hz.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double Frequency_Hz;
            /// <summary>Returns the frequency range, over which the measurement integrates the carrier power. This value is expressed in Hz.
            /// See the RFmx help for more documention of this parameter.</summary>
            public double IntegrationBandwidth_Hz;
        }

        /// <summary>Defines common results of the ACP measurement.</summary>
        public struct AcpResults
        {
            /// <summary>Returns the total integrated power, in dBm, or the power spectral density, in dBm/Hz depending on the unit selected
            /// in <see cref="AcpConfiguration.PowerUnits"/>. See the RFmx help for more documention of this parameter.</summary>
            public double TotalCarrierPower_dBm_or_dBmHz;
            /// <summary>Returns an array of ACP results for each configured offset.</summary>
            public AcpOffsetResults[] OffsetResults;
            /// <summary>Returns an array of ACP results for each component carrier in the signal.</summary>
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }

        /// <summary>Defines common results of the AMPM measurement.</summary>
        public struct AmpmResults
        {
            /// <summary>Returns the average linear gain of the device under test, computed by rejecting signal samples containing gain compression.
            /// This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double MeanLinearGain_dB;
            /// <summary>Returns the theoretical output power at which the device under test gain drops by 1 dB from its mean linear gain.
            /// This value is expressed in dBm. See the RFmx help for more documention of this parameter.</summary>
            public double OnedBCompressionPoint_dBm;
            /// <summary>Returns the ratio, as a percentage, of l^2 norm of difference between the normalized reference and acquired waveforms,
            /// to the l^2 norm of the normalized reference waveform. See the RFmx help for more documention of this parameter.</summary>
            public double MeanRmsEvm_percent;
            /// <summary>Returns the approximation error of the polynomial approximation of the measured device under test AM-to-AM characteristic.
            /// This value is expressed in dB. See the RFmx help for more documention of this parameter.</summary>
            public double AmToAMResidual_dB;
            /// <summary>Returns the approximation error of the polynomial approximation of the measured AM-to-PM characteristic of the device under test.
            /// This value is expressed in degrees. See the RFmx help for more documention of this parameter.</summary>
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
            RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent idleDurationPresent = ampmConfig.ReferenceWaveform.IdleDurationPresent ? 
                RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.False;
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
                    LowerAbsolutePower_dBm_or_dBmHz = lowerAbsolutePower[i],
                    UpperAbsolutePower_dBm_or_dBmHz = upperAbsolutePower[i],
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

