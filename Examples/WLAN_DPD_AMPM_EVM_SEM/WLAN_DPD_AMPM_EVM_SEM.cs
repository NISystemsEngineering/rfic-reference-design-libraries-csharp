using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.WlanMX;
using NationalInstruments.RFmx.SpecAnMX;
using System;
using System.IO;
using static NationalInstruments.ReferenceDesignLibraries.SA.RFmxWLAN;
using static NationalInstruments.ReferenceDesignLibraries.SG;

namespace NationalInstruments.ReferenceDesignLibraries.Examples
{
    public class WLAN_DPD_AMPM_EVM_SEM
    {
        #region Declaring Parameters
        //shared Parameters
        public string resourceName;
        public double centerFrequency;
        public string signalStringSpecan, signalStringWlan;
        public string resultStringSpecan, resultStringWlan;

        //Generator Configuration
        public SG.InstrumentConfiguration sgInstrConfig;

        //Waveform
        public string filePath;

        //Analyzer Configuration
        public SA.RFmxInstr.InstrumentConfiguration saInstrConfig;
        public SA.CommonConfiguration saCommonConfig;
        public SA.AutoLevelConfiguration saAutolevelConfig;
        public SA.RFmxWLAN.StandardConfiguration wlanStandardConfig;

        //Measurements Configuration
        public SA.RFmxWLAN.OFDMModAccConfiguration modAccConfig;
        public SA.RFmxWLAN.OFDMModAccResults modAccResults;

        public SA.RFmxWLAN.SEMConfiguration semConfig;
        public SA.RFmxWLAN.SEMResults semResults;

        public SA.RFmxSpecAn.AmpmConfiguration ampmConfigurationSpecAn;
        public SA.RFmxSpecAn.AmpmResults ampmResultsSpecAn;

        //Methods Configuration 
        public Methods.RFmxDPD.CommonConfiguration commonConfigurationDpd;
        public Methods.RFmxDPD.MemoryPolynomialConfiguration memoryPolynomialConfiguration;
        public Methods.RFmxDPD.PreDpdCrestFactorReductionConfiguration preDpdCrestFactorReductionConfig;
        public Methods.RFmxDPD.ApplyDpdCrestFactorReductionConfiguration applyDpdCrestFactorReductionConfig;
        public bool EnableDpd;
        #endregion

        public WLAN_DPD_AMPM_EVM_SEM()
        {
            InitializeParameters();
        }

        public void InitializeParameters()
        {
            //shared Parameters
            resourceName = "5840";
            centerFrequency = 5.18e9; //Hz
            filePath = @"C:\Users\Public\Documents\National Instruments\RFIC Test Software\Waveforms\80211ax_80M_MCS11.tdms";
            signalStringSpecan = "specanSig0";
            signalStringWlan = "wlanSig0";
            resultStringSpecan = "specanResult0";
            resultStringWlan = "wlanResult0";

            //Generator Configuration
            sgInstrConfig = InstrumentConfiguration.GetDefault();
            sgInstrConfig.CarrierFrequency_Hz = centerFrequency;
            sgInstrConfig.DutAverageInputPower_dBm = -10.0;
            sgInstrConfig.ExternalAttenuation_dB = 0;

            // Analyzer Configuration
            saInstrConfig = SA.RFmxInstr.InstrumentConfiguration.GetDefault();
            saCommonConfig = SA.CommonConfiguration.GetDefault();
            saCommonConfig.ExternalAttenuation_dB = 0;
            saCommonConfig.CenterFrequency_Hz = centerFrequency;
            saCommonConfig.ReferenceLevel_dBm = 0.0;

            saAutolevelConfig = SA.AutoLevelConfiguration.GetDefault();
            saAutolevelConfig.Enabled = true;

            ampmConfigurationSpecAn = SA.RFmxSpecAn.AmpmConfiguration.GetDefault();
            ampmConfigurationSpecAn.DutAverageInputPower_dBm = sgInstrConfig.DutAverageInputPower_dBm;
            // WLAN Configuration
            wlanStandardConfig = SA.RFmxWLAN.StandardConfiguration.GetDefault();
            wlanStandardConfig.ChannelBandwidth_Hz = 80e6; // Hz
            wlanStandardConfig.Standard = RFmxWlanMXStandard.Standard802_11ax;

            modAccConfig = SA.RFmxWLAN.OFDMModAccConfiguration.GetDefault();

            semConfig = SA.RFmxWLAN.SEMConfiguration.GetDefault();

            //Methods Configuration
            commonConfigurationDpd = Methods.RFmxDPD.CommonConfiguration.GetDefault();
            commonConfigurationDpd.DutAverageInputPower_dBm = sgInstrConfig.DutAverageInputPower_dBm;
            memoryPolynomialConfiguration = Methods.RFmxDPD.MemoryPolynomialConfiguration.GetDefault();
            memoryPolynomialConfiguration.NumberOfIterations = 1;
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
            RFmxWlanMX wlan = instr.GetWlanSignalConfiguration(signalStringWlan);
            #endregion

            #region Configure Generation
            ConfigureInstrument(nIRfsg, sgInstrConfig);
            Waveform waveform = LoadWaveformFromTDMS(filePath);

            // Apply CRF to the waveform if it is enabled
            waveform = Methods.RFmxDPD.ConfigurePreDpdCrestFactorReduction(specAn, waveform, preDpdCrestFactorReductionConfig);

            DownloadWaveform(nIRfsg, waveform);
            ConfigureContinuousGeneration(nIRfsg, waveform);

            var waveformLength_s = waveform.Data.SampleCount / waveform.SampleRate;

            nIRfsg.Initiate();
            #endregion

            #region configure Analyzer
            saAutolevelConfig.MeasurementInterval_s = waveform.BurstLength_s;

            SA.RFmxInstr.ConfigureInstrument(instr, saInstrConfig);
            SA.RFmxSpecAn.ConfigureCommon(specAn, saCommonConfig);
            SA.RFmxWLAN.ConfigureCommon(wlan, saCommonConfig);
            #endregion

            #region Configure SpecAn
            ampmConfigurationSpecAn.ReferenceWaveform = waveform;
            ampmConfigurationSpecAn.DutAverageInputPower_dBm = sgInstrConfig.DutAverageInputPower_dBm;
            SA.RFmxSpecAn.ConfigureAmpm(specAn, ampmConfigurationSpecAn);
            #endregion

            #region Configure WLAN Measurement
            SA.RFmxWLAN.ConfigureStandard(wlan, wlanStandardConfig);
            SA.RFmxWLAN.ConfigureOFDMModAcc(wlan, modAccConfig);
            SA.RFmxWLAN.ConfigureSEM(wlan, semConfig);
            #endregion

            #region Configure and Measure DPD
            if (EnableDpd)
            {
                Methods.RFmxDPD.ConfigureCommon(specAn, commonConfigurationDpd, waveform);
                Methods.RFmxDPD.ConfigureMemoryPolynomial(specAn, memoryPolynomialConfiguration);
                Methods.RFmxDPD.ConfigureApplyDpdCrestFactorReduction(specAn, applyDpdCrestFactorReductionConfig);

                Console.WriteLine("\n------------------------ Perform DPD ----------------------\n");

                specAn.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Dpd, true);
                Methods.RFmxDPD.PerformMemoryPolynomial(specAn, nIRfsg, memoryPolynomialConfiguration, waveform);
                Console.WriteLine("\n------------------------ DPD done --------------------------\n");
            }
            #endregion

            #region Measure SpecAn
            RFmxSpecAnMXMeasurementTypes[] specanMeasurements = new RFmxSpecAnMXMeasurementTypes[1] { RFmxSpecAnMXMeasurementTypes.Ampm };
            SA.RFmxSpecAn.SelectAndInitiateMeasurements(specAn, specanMeasurements, saAutolevelConfig, waveform.SignalBandwidth_Hz, false, "", resultStringSpecan);
            ampmResultsSpecAn = SA.RFmxSpecAn.FetchAmpm(specAn, RFmxSpecAnMX.BuildResultString(resultStringSpecan));
            PrintAMPMResults();
            #endregion

            #region measure and results
            RFmxWlanMXMeasurementTypes[] wlanMeasurements = new RFmxWlanMXMeasurementTypes[1] { RFmxWlanMXMeasurementTypes.OfdmModAcc };
            SA.RFmxWLAN.SelectAndInitiateMeasurements(wlan, wlanMeasurements, saAutolevelConfig, false, "", resultStringWlan);
            modAccResults = SA.RFmxWLAN.FetchOFDMModAcc(wlan, RFmxWlanMX.BuildResultString(resultStringWlan));
            PrintModAccResults();

            wlanMeasurements[0] = RFmxWlanMXMeasurementTypes.Sem;
            SA.RFmxWLAN.SelectAndInitiateMeasurements(wlan, wlanMeasurements, saAutolevelConfig, false, "", resultStringWlan);
            semResults = SA.RFmxWLAN.FetchSEM(wlan, RFmxWlanMX.BuildResultString(resultStringWlan));
            PrintSemResults();
            #endregion

            AbortGeneration(nIRfsg);
            CloseInstrument(nIRfsg);
            wlan.Dispose();
            wlan = null;
            instr.Close();
            instr = null;
        }

        #region Utilities
        private void PrintSemResults()
        {
            Console.WriteLine("\n----------------------- SEM Results -----------------------\n");
            Console.WriteLine("Measurement Status                          :{0}", semResults.measurementStatus);
            Console.WriteLine("Carrier Absolute Power (dBm)                :{0}", semResults.AbsolutePower_dBm);
            Console.WriteLine("\n---------------- Lower Offset Measurements ----------------\n");
            for (int i = 0; i < semResults.LowerOffsetMargin_dB.Length; i++)
            {
                Console.WriteLine("Offset {0}", i);
                Console.WriteLine("Measurement Status              :{0}", semResults.lowerOffsetMeasurementStatus[i]);
                Console.WriteLine("Margin (dB)                     :{0}", semResults.LowerOffsetMargin_dB[i]);
                Console.WriteLine("Margin Frequency (Hz)           :{0}", semResults.LowerOffsetMarginFrequency_Hz[i]);
                Console.WriteLine("Margin Absolute Power (dBm)     :{0}\n", semResults.LowerOffsetMarginAbsolutePower_dBm[i]);
            }
            Console.WriteLine("\n---------------- Upper Offset Measurements ----------------\n");
            for (int i = 0; i < semResults.UpperOffsetMargin_dB.Length; i++)
            {
                Console.WriteLine("Offset {0}", i);
                Console.WriteLine("Measurement Status              :{0}", semResults.upperOffsetMeasurementStatus[i]);
                Console.WriteLine("Margin (dB)                     :{0}", semResults.UpperOffsetMargin_dB[i]);
                Console.WriteLine("Margin Frequency (Hz)           :{0}", semResults.UpperOffsetMarginFrequency_Hz[i]);
                Console.WriteLine("Margin Absolute Power (dBm)     :{0}\n", semResults.UpperOffsetMarginAbsolutePower_dBm[i]);
            }
        }
        private void PrintModAccResults()
        {
            Console.WriteLine("\n---------------------- ModAcc Results ---------------------\n");
            Console.WriteLine("Composite RMS EVM (dB): {0:N}", modAccResults.CompositeRMSEVMMean_dB);
        }
        private void PrintAMPMResults()
        {
            Console.WriteLine("\n----------------------- AMPM Results ----------------------\n");
            Console.WriteLine("Mean Linear Gain (dB):                       {0:0.00}", ampmResultsSpecAn.MeanLinearGain_dB);
            Console.WriteLine("Mean RMS EVM (%):                            {0:0.00}", ampmResultsSpecAn.MeanRmsEvm_percent);
            Console.WriteLine("AM to AM Residual (dB):                      {0:0.00}", ampmResultsSpecAn.AmToAMResidual_dB);
            Console.WriteLine("AM to PM Residual (dB):                      {0:0.00}", ampmResultsSpecAn.AmToPMResidual_deg);
            Console.WriteLine("1 dB Compression Point (dBm):                {0:0.00}", ampmResultsSpecAn.OnedBCompressionPoint_dBm);
        }
        #endregion
    }
}