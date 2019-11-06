using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxSpecAn
    {
        #region Type Definitions
        public struct CommonConfiguration
        {
            public string SelectedPorts;
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string FrequencyReferenceSource;
            public bool EnableTrigger;
            public string DigitalEdgeSource;
            public RFmxSpecAnMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            public bool AutoLevelEnabled;
            public double AutoLevelMeasurementInterval_s;
            public double AutoLevelBandwidth_Hz;
            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    SelectedPorts = "",
                    CenterFrequency_Hz = 1e9,
                    ReferenceLevel_dBm = 0,
                    ExternalAttenuation_dB = 0,
                    FrequencyReferenceSource = RFmxInstrMXConstants.PxiClock,
                    EnableTrigger = true,
                    DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0,
                    DigitalEdgeType = RFmxSpecAnMXDigitalEdgeTriggerEdge.Rising,
                    TriggerDelay_s = 0,
                    AutoLevelEnabled = false,
                    AutoLevelMeasurementInterval_s = 10e-3,
                    AutoLevelBandwidth_Hz = 20e6
                };
            }
        }

        public struct TxpConfiguration
        {
            public double MeasurementInterval_s;
            public double Rbw_Hz;
            public RFmxSpecAnMXTxpRbwFilterType RbwFilterType;
            public double RrcAlpha;

            public RFmxSpecAnMXTxpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxSpecAnMXTxpAveragingType AveragingType;

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

        public struct AcpComponentCarrierConfiguration
        {
            public double IntegrationBandwidth_Hz;
            public double Frequency_Hz;
            public RFmxSpecAnMXAcpCarrierRrcFilterEnabled RrcFilterEnabled;
            public double RrcAlpha;
            public RFmxSpecAnMXAcpCarrierMode Mode;

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

        public struct AmpmConfiguration
        {
            public double MeasurementInterval_s;
            public double DutAverageInputPower_dBm;
            public Waveform ReferenceWaveform;
            public RFmxSpecAnMXAmpmSignalType SignalType;

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

        public struct TxpResults
        {
            public double AverageMeanPower_dBm;
            public double PeakToAverageRatio_dB;
            public double MaximumPower_dBm;
            public double MinimumPower_dBm;
        }

        public struct AcpOffsetResults
        {
            public double LowerAbsolutePower_dBm;
            public double LowerRelativePower_dB;
            public double UpperAbsolutePower_dBm;
            public double UpperRelativePower_dB;
            public double Frequency_Hz;
            public double IntegrationBandwidth_Hz;
        }

        public struct AcpComponentCarrierResults
        {
            public double AbsolutePower_dBm_or_dBmHz;
            public double TotalRelativePower_dB;
            public double Frequency_Hz;
            public double IntegrationBandwidth_Hz;
        }

        public struct AcpResults
        {
            public double TotalCarrierPower_dBm_or_dBmHz;
            public AcpOffsetResults[] OffsetResults;
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }

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
        public static void ConfigureCommon(RFmxInstrMX sessionHandle, RFmxSpecAnMX specAnSignal, CommonConfiguration commonConfig, string selectorString = "")
        {
            sessionHandle.ConfigureFrequencyReference("", commonConfig.FrequencyReferenceSource, 10e6);
            specAnSignal.SetSelectedPorts(selectorString, commonConfig.SelectedPorts);
            specAnSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalEdgeSource, commonConfig.DigitalEdgeType, commonConfig.TriggerDelay_s, commonConfig.EnableTrigger);
            specAnSignal.ConfigureFrequency(selectorString, commonConfig.CenterFrequency_Hz);
            specAnSignal.ConfigureExternalAttenuation(selectorString, commonConfig.ExternalAttenuation_dB);
            if (commonConfig.AutoLevelEnabled) specAnSignal.AutoLevel(selectorString, commonConfig.AutoLevelBandwidth_Hz, commonConfig.AutoLevelMeasurementInterval_s, out _);
            else specAnSignal.ConfigureReferenceLevel(selectorString, commonConfig.ReferenceLevel_dBm);
        }

        public static void ConfigureTxp(RFmxSpecAnMX specAn, TxpConfiguration txpConfig, string selectorString = "")
        {
            specAn.Txp.Configuration.SetMeasurementEnabled(selectorString, true);
            specAn.Txp.Configuration.SetAllTracesEnabled(selectorString, true);
            specAn.Txp.Configuration.SetMeasurementInterval(selectorString, txpConfig.MeasurementInterval_s);
            specAn.Txp.Configuration.ConfigureRbwFilter(selectorString, txpConfig.Rbw_Hz, txpConfig.RbwFilterType, txpConfig.RrcAlpha);
            specAn.Txp.Configuration.ConfigureAveraging(selectorString, txpConfig.AveragingEnabled, txpConfig.AveragingCount, txpConfig.AveragingType);
        }

        public static void ConfigureAcp(RFmxSpecAnMX specAn, AcpConfiguration acpConfig, string selectorString = "")
        {
            specAn.Acp.Configuration.SetMeasurementEnabled(selectorString, true);
            specAn.Acp.Configuration.SetAllTracesEnabled(selectorString, true);
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

        public static void ConfigureAmpm(RFmxSpecAnMX specAn, AmpmConfiguration ampmConfig, string selectorString = "")
        {
            specAn.Ampm.Configuration.SetMeasurementEnabled(selectorString, true);
            specAn.Ampm.Configuration.SetAllTracesEnabled(selectorString, true);
            specAn.Ampm.Configuration.ConfigureMeasurementInterval(selectorString, ampmConfig.MeasurementInterval_s);
            specAn.Ampm.Configuration.ConfigureDutAverageInputPower(selectorString, ampmConfig.DutAverageInputPower_dBm);
            RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent idleDurationPresent = ampmConfig.ReferenceWaveform.IdleDurationPresent ? RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.True : RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.False;
            specAn.Ampm.Configuration.ConfigureReferenceWaveform(selectorString, ampmConfig.ReferenceWaveform.Data, idleDurationPresent, ampmConfig.SignalType);
        }
        #endregion

        #region Measurement Results
        public static TxpResults FetchTxp(RFmxSpecAnMX specAn, string selectorString = "")
        {
            TxpResults txpResults = new TxpResults();
            specAn.Txp.Results.FetchMeasurement(selectorString, 10.0, out txpResults.AverageMeanPower_dBm,
                out txpResults.PeakToAverageRatio_dB, out txpResults.MaximumPower_dBm, out txpResults.MinimumPower_dBm);
            return txpResults;
        }

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

