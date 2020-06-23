using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;
using System;
using static NationalInstruments.ReferenceDesignLibraries.SG;


namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    public class SPECAN_DPD_AMPM_ACP_TXP
    {
        #region Declaring Parameters
        //shared Parameters
        public string resourceName;
        public double centerFrequency;
        public string signalStringSpecan;
        public string resultStringSpecan;

        //Generator Configuration
        SG.InstrumentConfiguration SgInstrConfig;

        //Waveform
        public string filePath;

        //Analyzer Configuration 
        public SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
        public SA.CommonConfiguration saCommonConfig;
        public SA.AutoLevelConfiguration saAutolevelConfig;

        //Methods Configuration 
        public Methods.RFmxDPD.CommonConfiguration CommonConfigurationDpd;
        public Methods.RFmxDPD.MemoryPolynomialConfiguration MemoryPolynomialConfiguration;
        public Methods.RFmxDPD.PreDpdCrestFactorReductionConfiguration preDpdCrestFactorReductionConfig;
        public Methods.RFmxDPD.ApplyDpdCrestFactorReductionConfiguration applyDpdCrestFactorReductionConfig;
        public bool EnableDpd;

        //Measurements Configuration
        public SA.RFmxSpecAn.AmpmConfiguration AmpmConfigurationSpecAn;
        public SA.RFmxSpecAn.AmpmResults AmpmResultsSpecAn;

        public SA.RFmxSpecAn.AcpConfiguration AcpConfigurationSpecAn;
        const int NumberOfOffsets = 3;
       
        public SA.RFmxSpecAn.AcpResults AcpResultsSpecAn;

        public SA.RFmxSpecAn.TxpConfiguration TxpConfigurationSpecAn;
        public SA.RFmxSpecAn.TxpResults TxpResultsSpecAn;
        #endregion
        public SPECAN_DPD_AMPM_ACP_TXP()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {
            //shared Parameters
            centerFrequency = 3.5e9; //Hz
            resourceName="5840";
            filePath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\LTE_FDD_UL_1x20MHz_256QAM_OS4.tdms";
            signalStringSpecan = "specanSig0";
            resultStringSpecan = "specanResult0";

            //Generator Configuiration
            SgInstrConfig = SG.InstrumentConfiguration.GetDefault();
            SgInstrConfig.CarrierFrequency_Hz = centerFrequency;
            SgInstrConfig.DutAverageInputPower_dBm = -10.0;
            SgInstrConfig.ExternalAttenuation_dB = 0;

            //Analyzer Configuration
            saInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            saCommonConfig = SA.CommonConfiguration.GetDefault();
            saCommonConfig.ExternalAttenuation_dB = 0;
            saCommonConfig.CenterFrequency_Hz = centerFrequency;
            saCommonConfig.ReferenceLevel_dBm = 0.0;

            saAutolevelConfig = SA.AutoLevelConfiguration.GetDefault();
            saAutolevelConfig.Enabled = true;

            AmpmConfigurationSpecAn = SA.RFmxSpecAn.AmpmConfiguration.GetDefault();
            
            TxpConfigurationSpecAn = SA.RFmxSpecAn.TxpConfiguration.GetDefault();
            TxpConfigurationSpecAn.RbwFilterType = RFmxSpecAnMXTxpRbwFilterType.None;
            TxpConfigurationSpecAn.RrcAlpha = 0;
            AcpConfigurationSpecAn = SA.RFmxSpecAn.AcpConfiguration.GetDefault();
            AcpConfigurationSpecAn.Rbw_Hz = 10e3;
            AcpConfigurationSpecAn.ComponentCarrierConfiguration[0].RrcFilterEnabled = RFmxSpecAnMXAcpCarrierRrcFilterEnabled.False;
            AcpConfigurationSpecAn.OffsetChannelConfiguration = new SA.RFmxSpecAn.AcpOffsetChannelConfiguration[NumberOfOffsets];

            //Define ACP offset configuration
            for (int i = 0; i < NumberOfOffsets; i++)
            {
               AcpConfigurationSpecAn.OffsetChannelConfiguration[i].Enabled = RFmxSpecAnMXAcpOffsetEnabled.True;
               AcpConfigurationSpecAn.OffsetChannelConfiguration[i].SideBand = RFmxSpecAnMXAcpOffsetSideband.Both;
               AcpConfigurationSpecAn.OffsetChannelConfiguration[i].PowerReferenceCarrier = RFmxSpecAnMXAcpOffsetPowerReferenceCarrier.Closest;
               AcpConfigurationSpecAn.OffsetChannelConfiguration[i].PowerReferenceSpecificIndex = 0;
               AcpConfigurationSpecAn.OffsetChannelConfiguration[i].RelativeAttenuation_dB = 0.00;
               AcpConfigurationSpecAn.OffsetChannelConfiguration[i].RrcFilterEnabled = RFmxSpecAnMXAcpOffsetRrcFilterEnabled.False;
               AcpConfigurationSpecAn.OffsetChannelConfiguration[i].RrcAlpha = 0.220;

                if (i == 0) //For offset 0, frequency offset = 10MHz, IBW = 9MHz
                {
                   AcpConfigurationSpecAn.OffsetChannelConfiguration[i].Frequency_Hz = 20e6;
                   AcpConfigurationSpecAn.OffsetChannelConfiguration[i].IntegrationBandwidth_Hz = 18e6;
                }
                else if (i == 1) //For offset 1, frequency offset = 7.5MHz, IBW = 3.84MHz
                {
                   AcpConfigurationSpecAn.OffsetChannelConfiguration[i].Frequency_Hz = 12.5e6;
                   AcpConfigurationSpecAn.OffsetChannelConfiguration[i].IntegrationBandwidth_Hz = 3.84e6;
                }
                else if (i == 2) //For offset 2, frequency offset = 12.5MHz, IBW = 3.84MHz
                {
                   AcpConfigurationSpecAn.OffsetChannelConfiguration[i].Frequency_Hz = 17.5e6;
                   AcpConfigurationSpecAn.OffsetChannelConfiguration[i].IntegrationBandwidth_Hz = 3.84e6;
                }
            }

            //Methods Configuration
            CommonConfigurationDpd = Methods.RFmxDPD.CommonConfiguration.GetDefault();
            CommonConfigurationDpd.DutAverageInputPower_dBm = SgInstrConfig.DutAverageInputPower_dBm;
            MemoryPolynomialConfiguration = Methods.RFmxDPD.MemoryPolynomialConfiguration.GetDefault();
            MemoryPolynomialConfiguration.NumberOfIterations = 1;
            EnableDpd = true;
            preDpdCrestFactorReductionConfig = Methods.RFmxDPD.PreDpdCrestFactorReductionConfiguration.GetDefault();
            preDpdCrestFactorReductionConfig.Enabled = RFmxSpecAnMXDpdPreDpdCfrEnabled.False;
            applyDpdCrestFactorReductionConfig = Methods.RFmxDPD.ApplyDpdCrestFactorReductionConfiguration.GetDefault();
            applyDpdCrestFactorReductionConfig.Enabled = RFmxSpecAnMXDpdApplyDpdCfrEnabled.False;
        }

        public void Run()
        {
            #region Create Sessions
            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
            RFmxSpecAnMX specAn = instr.GetSpecAnSignalConfiguration(signalStringSpecan);
            #endregion

            #region Configure Generation
            ConfigureInstrument(nIRfsg, SgInstrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);

            // Apply CRF to the waveform if it is enabled
            waveform = Methods.RFmxDPD.ConfigurePreDpdCrestFactorReduction(specAn, waveform, preDpdCrestFactorReductionConfig);

            DownloadWaveform(nIRfsg, waveform);
            ConfigureContinuousGeneration(nIRfsg, waveform);
            nIRfsg.Initiate();
            #endregion

            #region Configure Analyzer
            saAutolevelConfig.MeasurementInterval_s = waveform.BurstLength_s;
            SA.RFmxInstr.ConfigureInstrument(instr, saInstrConfig);
            SA.RFmxSpecAn.ConfigureCommon(specAn, saCommonConfig);

            AmpmConfigurationSpecAn.ReferenceWaveform = waveform;
            AmpmConfigurationSpecAn.DutAverageInputPower_dBm = SgInstrConfig.DutAverageInputPower_dBm;
            SA.RFmxSpecAn.ConfigureAmpm(specAn, AmpmConfigurationSpecAn);

            SA.RFmxSpecAn.ConfigureTxp(specAn, TxpConfigurationSpecAn);
            SA.RFmxSpecAn.ConfigureAcp(specAn, AcpConfigurationSpecAn, "");

            if (EnableDpd)
            {
                Methods.RFmxDPD.ConfigureCommon(specAn, CommonConfigurationDpd, waveform);
                Methods.RFmxDPD.ConfigureMemoryPolynomial(specAn, MemoryPolynomialConfiguration);
                Methods.RFmxDPD.ConfigureApplyDpdCrestFactorReduction(specAn, applyDpdCrestFactorReductionConfig);
            }
            #endregion

            #region Measure
            
            Console.WriteLine("\n--------------------- Results --------------------\n");
            if (EnableDpd)
            {
                specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Dpd, true);
                Methods.RFmxDPD.PerformMemoryPolynomial(specAn, nIRfsg, MemoryPolynomialConfiguration, waveform);
            }
            RFmxSpecAnMXMeasurementTypes[] specanMeasurements = new RFmxSpecAnMXMeasurementTypes[1] { RFmxSpecAnMXMeasurementTypes.Ampm };
            SA.RFmxSpecAn.SelectAndInitiateMeasurements(specAn, specanMeasurements, saAutolevelConfig, waveform.SignalBandwidth_Hz, false, "", resultStringSpecan);
            AmpmResultsSpecAn = SA.RFmxSpecAn.FetchAmpm(specAn, RFmxSpecAnMX.BuildResultString(resultStringSpecan));
            PrintAMPMResults();

            specanMeasurements[0] = RFmxSpecAnMXMeasurementTypes.Txp;
            SA.RFmxSpecAn.SelectAndInitiateMeasurements(specAn, specanMeasurements, saAutolevelConfig, waveform.SignalBandwidth_Hz, false, "", resultStringSpecan);
            TxpResultsSpecAn = SA.RFmxSpecAn.FetchTxp(specAn, RFmxSpecAnMX.BuildResultString(resultStringSpecan));
            PrintTxPResults();

            specanMeasurements[0] = RFmxSpecAnMXMeasurementTypes.Acp;
            SA.RFmxSpecAn.SelectAndInitiateMeasurements(specAn, specanMeasurements, saAutolevelConfig, waveform.SignalBandwidth_Hz, false, "", resultStringSpecan);
            AcpResultsSpecAn = SA.RFmxSpecAn.FetchAcp(specAn, RFmxSpecAnMX.BuildResultString(resultStringSpecan));
            PrintACPResults();
            #endregion

            AbortGeneration(nIRfsg);
            CloseInstrument(nIRfsg);
            specAn.Dispose();
            specAn = null;
            instr.Close();
            instr = null;
        }

        #region Utilities
        private void PrintTxPResults()
        {
            Console.WriteLine("\n----------------------- TXP Results -----------------------\n");
            
                Console.WriteLine("AverageMeanPower_dBm    : {0:0.000}", TxpResultsSpecAn.AverageMeanPower_dBm);
                Console.WriteLine("PeakToAverageRatio_dB    : {0:0.000}", TxpResultsSpecAn.PeakToAverageRatio_dB);
                Console.WriteLine("MaximumPower_dBm   : {0:0.000}", TxpResultsSpecAn.MaximumPower_dBm);
                Console.WriteLine("MinimumPower_dBm   : {0:0.000}", TxpResultsSpecAn.MinimumPower_dBm);
                Console.WriteLine("-------------------------------------------------\n");
            
        }

        private void PrintACPResults()
        {
            Console.WriteLine("\n----------------------- ACP Results -----------------------\n");
            Console.WriteLine("Total Carrier Power (dBm): {0:0.00}\n", AcpResultsSpecAn.TotalCarrierPower_dBm_or_dBmHz);
            Console.WriteLine("\n-----------Offset Channel Measurements----------- \n");

            for (int i = 0; i < AcpResultsSpecAn.OffsetResults.Length; i++)
            {
                Console.WriteLine("Offset  {0}", i);
                Console.WriteLine("Lower Absolute Power (dBm or dBm/Hz)   : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].LowerAbsolutePower_dBm_or_dBmHz);
                Console.WriteLine("Upper Absolute Power (dBm or dBm/Hz)   : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].UpperAbsolutePower_dBm_or_dBmHz);
                Console.WriteLine("Lower Relative Power (dB)              : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].LowerRelativePower_dB);
                Console.WriteLine("Upper Relative Power (dB)              : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].UpperRelativePower_dB);
                Console.WriteLine("-------------------------------------------------\n");
            }
        }


        private void PrintAMPMResults()
        {
            Console.WriteLine("\n----------------------- AMPM Results ----------------------\n");
            Console.WriteLine("Mean Linear Gain (dB):                       {0:0.00}", AmpmResultsSpecAn.MeanLinearGain_dB);
            Console.WriteLine("Mean RMS EVM (%):                            {0:0.00}", AmpmResultsSpecAn.MeanRmsEvm_percent);
            Console.WriteLine("AM to AM Residual (dB):                      {0:0.00}", AmpmResultsSpecAn.AmToAMResidual_dB);
            Console.WriteLine("AM to PM Residual (dB):                      {0:0.00}", AmpmResultsSpecAn.AmToPMResidual_deg);
            Console.WriteLine("1 dB Compression Point (dBm):                {0:0.00}", AmpmResultsSpecAn.OnedBCompressionPoint_dBm);
        }
        #endregion
    }
}
