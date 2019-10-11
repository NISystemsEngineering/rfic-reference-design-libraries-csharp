using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;
using System;
using static NationalInstruments.ReferenceDesignLibraries.SG;


namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    public class SPECAN_AMPM_ACP_TXP
    {
        #region Declaring Parameters
        //shared Parameters
        public string resourceName;
        public double centerFrequency;

        //Generator Configuration
        SG.InstrumentConfiguration SgInstrConfig;
       
        
        //Waveform
        public string filePath;
        
        //Analyzer Configuration 
        public SA.RFmxSpecAn.CommonConfiguration CommonConfigurationSpecAn;
        
        //Measurements Configuration
        public SA.RFmxSpecAn.AmpmConfiguration AmpmConfigurationSpecAn;
        public SA.RFmxSpecAn.AmpmResults AmpmResultsSpecAn;

        public SA.RFmxSpecAn.AcpConfiguration AcpConfigurationSpecAn;
        const int NumberOfOffsets = 3;
       
        public SA.RFmxSpecAn.AcpResults AcpResultsSpecAn;

        public SA.RFmxSpecAn.TxpConfiguration TxpConfigurationSpecAn;
        public SA.RFmxSpecAn.TxpResults TxpResultsSpecAn;
        #endregion
        public SPECAN_AMPM_ACP_TXP()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {

            //shared Parameters
            centerFrequency = 3.5e9; //Hz
            resourceName="VST2";
            filePath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\LTE_FDD_UL_1x20MHz_256QAM_OS4.tdms";
            //Generator Configuiration
            SgInstrConfig = SG.InstrumentConfiguration.GetDefault();
            SgInstrConfig.CarrierFrequency_Hz = centerFrequency;
            SgInstrConfig.DutAverageInputPower_dBm = -10.0;
            SgInstrConfig.ExternalAttenuation_dBm = 0;
           

            //Analyzer Configuration 
            
            CommonConfigurationSpecAn = SA.RFmxSpecAn.CommonConfiguration.GetDefault();
            CommonConfigurationSpecAn.CenterFrequency_Hz = centerFrequency;
            AmpmConfigurationSpecAn = SA.RFmxSpecAn.AmpmConfiguration.GetDefault();
            
            TxpConfigurationSpecAn = SA.RFmxSpecAn.TxpConfiguration.GetDefault();
            TxpConfigurationSpecAn.Rbw_Hz = 20e6;
            AcpConfigurationSpecAn = SA.RFmxSpecAn.AcpConfiguration.GetDefault();
            AcpConfigurationSpecAn.Rbw_Hz = 10e6;
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
        }

        public void Run()
        {

            #region Configure Generation
            NIRfsg nIRfsg = new NIRfsg(resourceName, false, false);
            ConfigureInstrument(nIRfsg, SgInstrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);

            DownloadWaveform(nIRfsg, waveform);
            ConfigureContinuousGeneration(nIRfsg, waveform);
            nIRfsg.Initiate();
            #endregion

            #region Configure Analyzer
            RFmxInstrMX instr = new RFmxInstrMX(resourceName, "");
           

            RFmxSpecAnMX specAn = instr.GetSpecAnSignalConfiguration();
            SA.RFmxSpecAn.ConfigureCommon(instr, specAn, CommonConfigurationSpecAn);
            AmpmConfigurationSpecAn.ReferenceWaveform = waveform;
            AmpmConfigurationSpecAn.DutAverageInputPower_dBm = SgInstrConfig.DutAverageInputPower_dBm;
            SA.RFmxSpecAn.ConfigureAmpm(specAn, AmpmConfigurationSpecAn);

            SA.RFmxSpecAn.ConfigureTxp(specAn, TxpConfigurationSpecAn);
            SA.RFmxSpecAn.ConfigureAcp(specAn, AcpConfigurationSpecAn, "");
            #endregion

            #region Measure
            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Ampm, true);
            specAn.Initiate("", "");
            AmpmResultsSpecAn = SA.RFmxSpecAn.FetchAmpm(specAn, "");
            PrintAMPMResults();

            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Txp, true);
            specAn.Initiate("", "");
            TxpResultsSpecAn=SA.RFmxSpecAn.FetchTxp(specAn, "");
            PrintTxPResults();

            specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Acp, true);
            specAn.Initiate("", "");
            AcpResultsSpecAn = SA.RFmxSpecAn.FetchAcp(specAn,"");
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
                Console.WriteLine("Lower Relative Power (dB)    : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].LowerRelativePower_dB);
                Console.WriteLine("Upper Relative Power (dB)    : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].UpperRelativePower_dB);
                Console.WriteLine("Lower Absolute Power (dBm)   : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].LowerAbsolutePower_dBm);
                Console.WriteLine("Upper Absolute Power (dBm)   : {0:0.000}", AcpResultsSpecAn.OffsetResults[i].UpperAbsolutePower_dBm);
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
